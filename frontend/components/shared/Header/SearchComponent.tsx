"use client";
import { api } from "@/lib/api";
import { Search } from "lucide-react";
import Link from "next/link";
import { useEffect, useState } from "react";

interface places {
  places: Array<dataResponse>;
}

interface dataResponse {
  id: number;
  name: string;
  description: string | null;
  email: string | null;
  categoryName: string;
  subCategoryName: string;
  state: string;
  createdAt: Date;
  openingTime: string;
  closingTime: string;
  unavailableAt: Array<string>;
  nearestLandMark: string;
  latitude: number | string;
  longitude: number | string;
  districtName: string;
  directorateName: string;
  imageUrls: Array<string>;
  contacts: Array<string>;
  averageRate: number;
  rateCount: number;
}

export default function SearchComponent() {
  const [data, setData] = useState<dataResponse[]>([]);
  const [search, setSearch] = useState<string>("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const dataResponse = await api.get<places>(
          "/api/customer/places/getall",
          {
            params: {
              pageNumber: 1,
              pageSize: 40,
            },
          },
        );
        setData(dataResponse.data.places);
      } catch (error) {
        console.error("يوجد خطاء: ", error);
      }
    };
    fetchData();
  }, []);

  const filteredPlaces =
    search.trim() === ""
      ? []
      : data.filter((place) =>
          place.name.toLowerCase().includes(search.toLowerCase()),
        );

  return (
    <div className="relative flex items-center">
      <Search
        size={20}
        color="#b6b09f"
        strokeWidth={2.5}
        className="absolute inset-s-2 z-10"
      />
      <input
        onChange={(e) => setSearch(e.target.value)}
        value={search}
        className="ps-8 py-1.5 w-3xs bg-base-light rounded-lg
           relative border border-primary focus:border-accent-black/60 
           focus:outline-none transition-colors focus:placeholder:text-accent-black/50 
           placeholder:text-primary placeholder:font-medium shadow-md"
        type="search"
        name="search"
        id="search"
        placeholder="ابحث عن محل للتقيم"
      />

      {search.trim() !== "" && (
        <div className="absolute top-full mt-1 w-3xs bg-white border border-primary/20 rounded-lg shadow-lg max-h-60 overflow-y-auto z-50">
          {filteredPlaces.length > 0 ? (
            filteredPlaces.map((place) => (
              <Link
                key={place.id}
                href={`/places/${place.name}`}
                onClick={() => setSearch("")}
                className="block px-4 py-2 hover:bg-base-light text-sm border-b border-gray-100 last:border-none"
              >
                <div className="font-semibold text-primary">{place.name}</div>
                <div className="text-xs text-gray-500">
                  {place.categoryName} - {place.districtName}
                </div>
              </Link>
            ))
          ) : (
            <div className="px-4 py-3 text-sm text-gray-500 text-center">
              لا توجد نتائج مطابقة
            </div>
          )}
        </div>
      )}
    </div>
  );
}
