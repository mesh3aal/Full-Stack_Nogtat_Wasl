"use client";
import { Card, CardContent } from "@/components/ui/card";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselPrevious,
  CarouselNext,
} from "@/components/ui/carousel";
import { api } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import Image from "next/image";
import Link from "next/link";

interface DataResponse {
  id: number;
  name: string;
  description: string | null;
  email: string | null;
  categoryName: string;
  subCategoryName: string;
  state: string;
  createdAt: string;
  openingTime: string;
  closingTime: string;
  unavailableAt: Array<string>;
  nearestLandMark: string;
  latitude: number | string;
  longitude: number | string;
  districtName: string;
  directorateName: string | null;
  imageUrls: Array<string>;
  contacts: Array<string>;
  averageRate: string;
  rateCount: string;
}

interface PlacesApiResponse {
  places: Array<DataResponse>;
}
interface Params {
  orderBy: number;
  pageNumber: number;
  pageSize: number;
}

interface CarouselSectionProps {
  url: string;
  queryKey: string[];
  params: Params;
}

export default function CarouselSection({
  url,
  queryKey,
  params,
}: CarouselSectionProps) {
  async function fetchPlaces() {
    const response = await api.get<PlacesApiResponse>(url, {
      params: params,
    });
    return response.data;
  }

  const { data, isPending, error } = useQuery<PlacesApiResponse>({
    queryKey: queryKey,
    queryFn: fetchPlaces,
  });

  if (isPending) return <div className="text-center py-8">جاري التحميل...</div>;
  if (error)
    return (
      <div className="text-center py-8 text-red-500">
        حدث خطأ أثناء جلب البيانات
      </div>
    );

  return (
    <>
      <Carousel
        dir={"ar"}
        opts={{
          align: "start",
          direction: "rtl",
        }}
        className="w-full relative px-10 sm:px-14"
      >
        <CarouselContent>
          {data.places.map((item) => (
            <CarouselItem
              key={item.id}
              className="basis-full sm:basis-1/2 lg:basis-1/3"
            >
              <div className="p-1">
                <Card className="bg-primary">
                  <CardContent className="flex flex-col aspect-square items-center justify-between p-1 text-center">
                    <Image
                      className="size-56"
                      src={item.imageUrls[0]}
                      height={500}
                      width={500}
                      alt="image"
                    />
                    <div className="flex flex-col justify-center items-center">
                      <span className="text-lg font-semibold truncate w-full">
                        {item.name}
                      </span>
                      <span className="text-sm text-gray-500 mt-2">
                        ⭐ {item.averageRate} ({item.rateCount})
                      </span>
                      <Link
                        href={"/places/" + item.name}
                        className="w-fit bg-accent-black text-white my-4 px-2.5 py-1 rounded-md hover:bg-gray-900"
                      >
                        تفاصيل المحل
                      </Link>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </CarouselItem>
          ))}
        </CarouselContent>
        <CarouselPrevious />
        <CarouselNext />
      </Carousel>
      <hr className="bg-primary h-0.5 my-8" />
    </>
  );
}
