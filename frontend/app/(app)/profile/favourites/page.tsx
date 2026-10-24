import { cookies } from "next/headers";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import FavouritesList from "./FavouritesList";

const API_URL = process.env.API_URL || "https://localhost:5001";

export default async function FavouritesPage() {
  const session = await getSession();
  if (!session) redirect("/login");

  const cookieStore = await cookies();
  const token = cookieStore.get("accessToken")?.value;

  let initialFavourites: any[] = [];

  try {
    const res = await fetch(
      `${API_URL}/api/customer/favourites?pageNumber=1&pageSize=12`,
      {
        headers: { Authorization: `Bearer ${token}` },
        cache: "no-store",
      }
    );
    if (res.ok) {
      const data = await res.json();
      initialFavourites = Array.isArray(data) ? data : [];
    }
  } catch {
    // silent
  }

  return <FavouritesList initialFavourites={initialFavourites} />;
}
