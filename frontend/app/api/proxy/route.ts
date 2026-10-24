import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";

const API_URL =
  process.env.API_URL ||
  process.env.NEXT_PUBLIC_API_URL ||
  "https://localhost:5001";

/** Safely build a NextResponse from a backend fetch Response.
 *  Handles 204/205 No-Content (no body allowed by spec). */
async function buildResponse(res: Response): Promise<NextResponse> {
  const noBody = res.status === 204 || res.status === 205;
  if (noBody) {
    return new NextResponse(null, { status: res.status });
  }
  const data = await res.json().catch(() => null);
  return NextResponse.json(data, { status: res.status });
}

function getToken(cookieStore: Awaited<ReturnType<typeof cookies>>) {
  return cookieStore.get("accessToken")?.value;
}

function buildTargetUrl(req: NextRequest): string | null {
  const url = req.nextUrl.searchParams.get("url");
  if (!url) return null;
  const params = new URLSearchParams();
  req.nextUrl.searchParams.forEach((value, key) => {
    if (key !== "url") params.set(key, value);
  });
  const qs = params.toString();
  return `${API_URL}${url}${qs ? `?${qs}` : ""}`;
}

export async function GET(req: NextRequest) {
  const cookieStore = await cookies();
  const token = getToken(cookieStore);
  const targetUrl = buildTargetUrl(req);
  if (!targetUrl) return NextResponse.json({ error: "missing url param" }, { status: 400 });

  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) headers["Authorization"] = `Bearer ${token}`;

  const res = await fetch(targetUrl, { method: "GET", headers, cache: "no-store" });
  return buildResponse(res);
}

export async function POST(req: NextRequest) {
  const cookieStore = await cookies();
  const token = getToken(cookieStore);
  const targetUrl = buildTargetUrl(req);
  if (!targetUrl) return NextResponse.json({ error: "missing url param" }, { status: 400 });

  const contentType = req.headers.get("content-type") ?? "application/json";
  const headers: Record<string, string> = { "Content-Type": contentType };
  if (token) headers["Authorization"] = `Bearer ${token}`;

  let body: BodyInit | undefined;
  if (contentType.includes("multipart/form-data")) {
    body = await req.formData();
    delete headers["Content-Type"];
  } else {
    body = await req.text();
  }

  const res = await fetch(targetUrl, { method: "POST", headers, body, cache: "no-store" });
  return buildResponse(res);
}

export async function PATCH(req: NextRequest) {
  const cookieStore = await cookies();
  const token = getToken(cookieStore);
  const targetUrl = buildTargetUrl(req);
  if (!targetUrl) return NextResponse.json({ error: "missing url param" }, { status: 400 });

  const contentType = req.headers.get("content-type") ?? "application/json";
  const headers: Record<string, string> = { "Content-Type": contentType };
  if (token) headers["Authorization"] = `Bearer ${token}`;

  let body: BodyInit | undefined;
  if (contentType.includes("multipart/form-data")) {
    body = await req.formData();
    delete headers["Content-Type"];
  } else {
    body = await req.text();
  }

  const res = await fetch(targetUrl, { method: "PATCH", headers, body, cache: "no-store" });
  return buildResponse(res);
}

export async function DELETE(req: NextRequest) {
  const cookieStore = await cookies();
  const token = getToken(cookieStore);
  const targetUrl = buildTargetUrl(req);
  if (!targetUrl) return NextResponse.json({ error: "missing url param" }, { status: 400 });

  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) headers["Authorization"] = `Bearer ${token}`;

  const res = await fetch(targetUrl, { method: "DELETE", headers, cache: "no-store" });
  return buildResponse(res);
}
