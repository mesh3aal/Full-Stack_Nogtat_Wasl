"use client";

import { useCallback, useEffect, useState } from "react";
import { cn } from "cn";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  type CarouselApi,
} from "@/components/ui/carousel";
import Image from "next/image";

export function Pattern({ ImagesArray }: { ImagesArray: Array<string> }) {
  const [mainApi, setMainApi] = useState<CarouselApi>();
  const [thumbApi, setThumbApi] = useState<CarouselApi>();
  const [selectedIndex, setSelectedIndex] = useState(0);

  // دالة النقر على الصورة المصغرة للانتقال للصورة الرئيسية
  const onThumbClick = useCallback(
    (index: number) => {
      if (!mainApi || !thumbApi) return;
      mainApi.scrollTo(index);
      thumbApi.scrollTo(index);
    },
    [mainApi, thumbApi],
  );

  // تحديث العنصر النشط عند التحريك من الرئيسي
  const onSelect = useCallback(() => {
    if (!mainApi || !thumbApi) return;
    const index = mainApi.selectedScrollSnap();
    setSelectedIndex(index);
    thumbApi.scrollTo(index);
  }, [mainApi, thumbApi]);

  useEffect(() => {
    if (!mainApi) return;

    onSelect();
    mainApi.on("select", onSelect);
    mainApi.on("reInit", onSelect);

    return () => {
      mainApi.off("select", onSelect);
      mainApi.off("reInit", onSelect);
    };
  }, [mainApi, onSelect]);

  if (!ImagesArray || ImagesArray.length === 0) {
    return <div className="text-gray-400">لا توجد صور متاحة</div>;
  }

  return (
    <div className="flex w-full max-w-md flex-col gap-3 p-4">
      {/* الكاروسيل الرئيسي */}
      <Carousel setApi={setMainApi} opts={{ direction: "rtl" }} className="w-full">
        <CarouselContent>
          {ImagesArray.map((item, index) => (
            <CarouselItem key={index}>
              <div className="bg-muted relative aspect-video w-full overflow-hidden rounded-xl">
                <Image
                  src={item}
                  alt={`Slide ${index + 1}`}
                  fill
                  sizes="(max-width: 768px) 100vw, 400px"
                  priority={index === 0}
                  className="object-cover"
                />
              </div>
            </CarouselItem>
          ))}
        </CarouselContent>
      </Carousel>

      {/* كاروسيل الصور المصغرة (Thumbnails) */}
      <Carousel
        setApi={setThumbApi}
        opts={{
          direction: "rtl",
          containScroll: "keepSnaps",
          dragFree: true,
        }}
        className="w-full"
      >
        <CarouselContent className="-ms-2 flex-row">
          {ImagesArray.map((item, index) => (
            <CarouselItem
              key={index}
              className="basis-1/4 cursor-pointer ps-2 sm:basis-1/5"
              onClick={() => onThumbClick(index)}
            >
              <div
                className={cn(
                  "relative aspect-square overflow-hidden rounded-lg border-2 transition-all",
                  index === selectedIndex
                    ? "border-primary opacity-100 scale-105"
                    : "border-transparent opacity-40 hover:opacity-70",
                )}
              >
                <Image
                  src={item}
                  alt={`Thumb ${index + 1}`}
                  fill
                  sizes="100px"
                  className="object-cover"
                />
              </div>
            </CarouselItem>
          ))}
        </CarouselContent>
      </Carousel>
    </div>
  );
}
