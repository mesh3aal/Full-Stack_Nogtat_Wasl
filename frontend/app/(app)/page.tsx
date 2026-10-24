import HeroSection from "@/components/shared/Hero Section/HeroSection";
import CarouselSection from "@/components/shared/CarouselSection/CarouselSection";
import Link from "next/link";

export default function Home() {
  return (
    <>
      <HeroSection />
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between mb-6">
          <p className="text-3xl">موصى به</p>
          <Link
            href="/recommended"
            className="px-6 py-1 bg-accent-black text-white text-base rounded-lg"
          >
            عرض المزيد
          </Link>
        </div>
        <CarouselSection
          url="/api/customer/places/Filter"
          queryKey={["Recommended"]}
          params={{ orderBy: 1, pageNumber: 1, pageSize: 5 }}
        />
        <div className="flex justify-between mb-6">
          <p className="text-3xl">الأكثر تقيماً</p>
          <Link
            href="/highestrating"
            className="px-6 py-1 bg-accent-black text-white text-base rounded-lg"
          >
            عرض المزيد
          </Link>
        </div>
        <CarouselSection
          url="/api/customer/places/Filter"
          queryKey={["Recommended"]}
          params={{ orderBy: 0, pageNumber: 1, pageSize: 5 }}
        />
        <div className="flex justify-between mb-6">
          <p className="text-3xl">تمت إضافته حديثاً</p>
          <Link
            href="/newest"
            className="px-6 py-1 bg-accent-black text-white text-base rounded-lg"
          >
            عرض المزيد
          </Link>
        </div>
        <CarouselSection
          url="/api/customer/places/Filter"
          queryKey={["Recommended"]}
          params={{ orderBy: 2, pageNumber: 1, pageSize: 5 }}
        />
      </div>
    </>
  );
}
