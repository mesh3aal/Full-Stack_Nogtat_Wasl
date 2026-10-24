import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { SessionUser } from "./definitions";
import { decrypt } from "./jwt";

export { decrypt };

export async function createSession(
  accessToken: string,
  refreshToken?: string,
  expiresAt?: Date
) {
  const cookieStore = await cookies();

  // 30 minutes expiration for accessToken by default
  const accessExpires =
    expiresAt || new Date(Date.now() + 30 * 60 * 1000);

  cookieStore.set("accessToken", accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    path: "/",
    expires: accessExpires,
  });

  if (refreshToken) {
    const refreshExpires = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000); // 7 days
    cookieStore.set("refreshToken", refreshToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires: refreshExpires,
    });
  }
}

export async function deleteSession() {
  const cookieStore = await cookies();
  cookieStore.delete("accessToken");
  cookieStore.delete("refreshToken");
}

export async function getSession(): Promise<SessionUser | null> {
  const cookieStore = await cookies();
  const token = cookieStore.get("accessToken")?.value;
  if (!token) return null;

  const payload = await decrypt(token);
  if (!payload) return null;

  // ASP.NET Core maps claims either in URI format or shortened names
  const userId = (payload.nameid ||
    payload.sub ||
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]) as string;

  const userName = (payload.unique_name ||
    payload.name ||
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
    "") as string;

  const email = (payload.email ||
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ||
    "") as string;

  const role = (payload.role ||
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
    "") as string;

  const avatar = (payload.avatar as string) || undefined;

  if (!userId) return null;

  return {
    userId,
    userName,
    email,
    role,
    avatar,
  };
}

export async function verifySession(): Promise<SessionUser> {
  const session = await getSession();
  if (!session) {
    redirect("/login");
  }
  return session;
}
