import { cookies } from "next/headers";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import ReviewsList from "./ReviewsList";

const API_URL = process.env.API_URL || "https://localhost:5001";

export default async function ProfilePage() {
  const session = await getSession();
  if (!session) redirect("/login");

  const cookieStore = await cookies();
  const token = cookieStore.get("accessToken")?.value;

  let initialReviews: any[] = [];
  let initialRemainingCount = 0;

  try {
    const res = await fetch(
      `${API_URL}/api/reviews/myReviews?pageNumber=1&pageSize=10`,
      {
        headers: { Authorization: `Bearer ${token}` },
        cache: "no-store",
      }
    );
    if (res.ok) {
      const data = await res.json();
      initialReviews = data?.reviews ?? [];
      initialRemainingCount = data?.remainingReviewsCount ?? 0;
    }
  } catch {
    // silent — client will show empty state
  }

  return (
    <ReviewsList
      initialReviews={initialReviews}
      initialRemainingCount={initialRemainingCount}
    />
  );
}
