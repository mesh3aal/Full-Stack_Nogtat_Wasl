import { cookies } from "next/headers";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import SettingsForm from "./SettingsForm";

const API_URL = process.env.API_URL || "https://localhost:5001";

export default async function SettingsPage() {
  const session = await getSession();
  if (!session) redirect("/login");

  const cookieStore = await cookies();
  const token = cookieStore.get("accessToken")?.value;

  let profileInfo: { username?: string; email?: string; profileImage?: string } | null = null;

  try {
    const res = await fetch(`${API_URL}/api/customer/GetProfileInfo`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    });
    if (res.ok) {
      profileInfo = await res.json();
    }
  } catch {
    // silent
  }

  return <SettingsForm initialProfile={profileInfo} />;
}
