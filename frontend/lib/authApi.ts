/**
 * Authenticated API client that routes calls through the Next.js proxy,
 * which injects the httpOnly accessToken cookie as an Authorization header.
 *
 * Usage (same signature as axios):
 *   import { authApi } from "@/lib/authApi";
 *   const data = await authApi.get("/api/reviews/myReviews", { pageNumber: 1 });
 */

const proxyBase = "/api/proxy";

async function request<T = unknown>(
  method: "GET" | "POST" | "PATCH" | "DELETE",
  apiPath: string,
  queryParams?: Record<string, string | number | undefined>,
  body?: unknown,
  isFormData = false,
): Promise<T> {
  const proxy = new URLSearchParams({ url: apiPath });

  if (queryParams) {
    for (const [key, value] of Object.entries(queryParams)) {
      if (value !== undefined) proxy.set(key, String(value));
    }
  }

  const fetchOptions: RequestInit = {
    method,
    cache: "no-store",
  };

  if (body) {
    if (isFormData) {
      fetchOptions.body = body as FormData;
      // Don't set Content-Type so browser sets boundary for multipart
    } else {
      fetchOptions.headers = { "Content-Type": "application/json" };
      fetchOptions.body = JSON.stringify(body);
    }
  }

  const res = await fetch(`${proxyBase}?${proxy}`, fetchOptions);

  if (!res.ok) {
    const errorData = await res.json().catch(() => null);
    const message =
      errorData?.detail ??
      errorData?.title ??
      `HTTP ${res.status}`;
    throw Object.assign(new Error(message), { status: res.status, data: errorData });
  }

  // 204 / 205 — no body
  if (res.status === 204 || res.status === 205) {
    return null as unknown as T;
  }

  return res.json() as Promise<T>;
}

export const authApi = {
  get: <T = unknown>(path: string, params?: Record<string, string | number | undefined>) =>
    request<T>("GET", path, params),

  post: <T = unknown>(path: string, body?: unknown, params?: Record<string, string | number | undefined>) =>
    request<T>("POST", path, params, body),

  patch: <T = unknown>(path: string, body?: unknown, params?: Record<string, string | number | undefined>, isFormData = false) =>
    request<T>("PATCH", path, params, body, isFormData),

  delete: <T = unknown>(path: string, params?: Record<string, string | number | undefined>) =>
    request<T>("DELETE", path, params),
};
