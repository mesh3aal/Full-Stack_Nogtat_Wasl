import { Rating } from "@/components/reui/rating";
import { api } from "@/lib/api";
import { ImageIcon } from "lucide-react";
import Image from "next/image";
import Link from "next/link";

export default async function recommended() {
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
  const dataResponse = await api.get<places>("/api/customer/places/Filter", {
    params: {
      orderBy: 1,
      pageNumber: 1,
      pageSize: 12,
    },
  });
  const data = dataResponse.data;
  return (
    <div className="container mx-auto h-dvh my-4">
      <div className="grid grid-col gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 py-6">
        {data.places.map((item) => {
          const imageUrl = item.imageUrls[0];
          return (
            <div
              key={item.id}
              className="flex flex-col gap-y-4 justify-between items-center p-6 bg-primary text-primary-foreground rounded-md text-center"
            >
              {imageUrl ? (
                <Image
                  src={imageUrl}
                  alt={item.name || "place image"}
                  height={500}
                  width={500}
                  className="w-fit size-64 object-cover rounded-md"
                />
              ) : (
                <div className="size-64 flex flex-col items-center justify-center bg-primary-foreground/10 border border-dashed border-primary-foreground/20 rounded-md text-sm gap-2">
                  <ImageIcon className="w-10 h-10 opacity-50" />
                  <span className="opacity-70">لا توجد صورة</span>
                </div>
              )}

              <p className="w-full">{item.name}</p>
              <div className="flex justify-center gap-x-2 items-center">
                <Rating rating={Number(item.averageRate)} showValue={true} />
                <p className="font-light text-sm">({item.rateCount} مراجعة)</p>
              </div>
              <Link
                href={"/places/" + item.name}
                className="bg-accent-black text-white my-4 px-2.5 py-1 rounded-md hover:bg-gray-900"
              >
                تفاصيل المحل
              </Link>
            </div>
          );
        })}
      </div>
    </div>
  );
}
