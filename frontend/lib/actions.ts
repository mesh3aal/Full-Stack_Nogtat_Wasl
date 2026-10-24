"use server";

import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import {
  LoginFormSchema,
  SignupFormSchema,
  FormState,
  TokenResponse,
} from "./definitions";
import { createSession, deleteSession } from "./session";

const getApiUrl = () =>
  process.env.API_URL ||
  process.env.NEXT_PUBLIC_API_URL ||
  "https://localhost:5001";

export async function login(
  prevState: FormState,
  formData: FormData,
): Promise<FormState> {
  const data = Object.fromEntries(formData);
  const validatedFields = LoginFormSchema.safeParse(data);

  if (!validatedFields.success) {
    return {
      errors: validatedFields.error.flatten().fieldErrors,
    };
  }

  const { email, password } = validatedFields.data;
  const apiUrl = getApiUrl();

  let tokenData: TokenResponse | null = null;
  let rawCookies: string[] = [];

  try {
    const response = await fetch(`${apiUrl}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email, password }),
      // Don't cache auth requests
      cache: "no-store",
    });

    if (!response.ok) {
      let errorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة";
      const fieldErrors: Record<string, string[]> = {};

      try {
        const errorJson = await response.json();
        if (errorJson.detail) {
          errorMessage = errorJson.detail;
        } else if (errorJson.title) {
          errorMessage = errorJson.title;
        }

        if (errorJson.errors && typeof errorJson.errors === "object") {
          for (const [key, value] of Object.entries(errorJson.errors)) {
            const lowerKey = key.charAt(0).toLowerCase() + key.slice(1);
            fieldErrors[lowerKey] = Array.isArray(value)
              ? (value as string[])
              : [String(value)];
          }
        }
      } catch {
        // Fallback to default message if JSON parsing fails
      }

      return {
        message: errorMessage,
        errors: fieldErrors,
      };
    }

    // Capture Set-Cookie headers from ASP.NET Core response if present
    const setCookieHeader = response.headers.getSetCookie?.() || [];
    rawCookies = setCookieHeader;

    try {
      const responseBody = await response.json();
      tokenData = (responseBody.value || responseBody) as TokenResponse;
    } catch {
      // Body may be empty if cookies are used exclusively
    }
  } catch (error: unknown) {
    // Check if error is Next.js redirect
    if (
      error &&
      typeof error === "object" &&
      "digest" in error &&
      typeof (error as { digest: unknown }).digest === "string" &&
      ((error as { digest: string }).digest.startsWith("NEXT_REDIRECT") ||
        false)
    ) {
      throw error;
    }

    console.error("Login fetch error:", error);
    return {
      message:
        "تعذر الاتصال بخادم ASP.NET Core. يرجى التأكد من تشغيل الخادم على " +
        apiUrl,
    };
  }

  // Extract accessToken either from response JSON body or Set-Cookie headers
  let accessToken = tokenData?.accessToken;
  let refreshToken = tokenData?.refreshToken;

  if (!accessToken && rawCookies.length > 0) {
    for (const cookieStr of rawCookies) {
      const matchAccess = cookieStr.match(/accessToken=([^;]+)/);
      if (matchAccess) accessToken = matchAccess[1];

      const matchRefresh = cookieStr.match(/refreshToken=([^;]+)/);
      if (matchRefresh) refreshToken = matchRefresh[1];
    }
  }

  if (!accessToken) {
    return {
      message: "تم تسجيل الدخول لكن لم يتم استلام رمز المصادقة من الخادم",
    };
  }

  await createSession(accessToken, refreshToken);

  redirect("/");
}

export async function signup(
  prevState: FormState,
  formData: FormData,
): Promise<FormState> {
  const data = Object.fromEntries(formData);
  const validatedFields = SignupFormSchema.safeParse(data);

  if (!validatedFields.success) {
    return {
      errors: validatedFields.error.flatten().fieldErrors,
    };
  }

  const { username, email, password, confirmPassword } = validatedFields.data;
  const apiUrl = getApiUrl();

  try {
    const response = await fetch(`${apiUrl}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username,
        email,
        password,
        confirmPassword,
        userType: 0, // Customer/User role
      }),
      cache: "no-store",
    });

    if (!response.ok) {
      let errorMessage = "تعذر إنشاء الحساب، يرجى مراجعة البيانات المدخلة";
      const fieldErrors: Record<string, string[]> = {};

      try {
        const errorJson = await response.json();
        if (errorJson.detail) {
          errorMessage = errorJson.detail;
        } else if (errorJson.title) {
          errorMessage = errorJson.title;
        }

        if (errorJson.errors && typeof errorJson.errors === "object") {
          for (const [key, value] of Object.entries(errorJson.errors)) {
            const lowerKey = key.charAt(0).toLowerCase() + key.slice(1);
            fieldErrors[lowerKey] = Array.isArray(value)
              ? (value as string[])
              : [String(value)];
          }
        }
      } catch {
        // Fallback
      }

      return {
        message: errorMessage,
        errors: fieldErrors,
      };
    }
  } catch (error: unknown) {
    if (
      error &&
      typeof error === "object" &&
      "digest" in error &&
      typeof (error as { digest: unknown }).digest === "string" &&
      ((error as { digest: string }).digest.startsWith("NEXT_REDIRECT") ||
        false)
    ) {
      throw error;
    }

    console.error("Signup fetch error:", error);
    return {
      message:
        "تعذر الاتصال بخادم ASP.NET Core. يرجى التأكد من تشغيل الخادم على " +
        apiUrl,
    };
  }

  redirect("/login?registered=true");
}

export async function logout(): Promise<void> {
  const cookieStore = await cookies();
  const token = cookieStore.get("accessToken")?.value;
  const apiUrl = getApiUrl();

  if (token) {
    try {
      await fetch(`${apiUrl}/api/auth/logout`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          Cookie: `accessToken=${token}`,
        },
        cache: "no-store",
      });
    } catch (e) {
      console.warn(
        "Backend logout notification failed, continuing local logout:",
        e,
      );
    }
  }

  await deleteSession();
  redirect("/login");
}
