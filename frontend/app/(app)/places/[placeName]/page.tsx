import { Pattern } from "@/components/examples/c-carousel-9";
import { Rating } from "@/components/reui/rating";
import {
  CommentsSection,
  type Review,
  type OneToFiveRating,
} from "@/components/shared/Comments/CommentsSection";
import { api } from "@/lib/api";

interface PageProps {
  params: Promise<{ placeName: string }>;
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

interface DetailApiResponse {
  placeDetail: dataResponse;
  oneToFiveRating: OneToFiveRating;
  rates: {
    reviews: Review[];
    remainingReviewsCount: number;
  };
}

enum WeekDays {
  الاحد = "الأحد",
  الاثنين = "الاثنين",
  الثلثاء = "الثلثاء",
  الأربعاء = "الأربعاء",
  الخميس = "الخميس",
  الجمعة = "الجمعة",
  السبت = "السبت",
}

const PlaceDetails = async ({ params }: PageProps) => {
  const PagePlaceName = (await params).placeName;

  const PlaceInfoResponse = api.get<dataResponse>(
    `/api/customer/places/redirect/${PagePlaceName}`,
  );
  const PlaceInfo = (await PlaceInfoResponse).data;

  // جلب بيانات التعليقات والتقييمات مع معالجة الأخطاء لضمان استقرار الصفحة
  let detailData: DetailApiResponse | null = null;
  try {
    const detailResponse = await api.get<DetailApiResponse>(
      `/api/customer/places/${PlaceInfo.id}/Detail?pageNumber=1&pageSize=10`,
    );
    detailData = detailResponse.data;
  } catch (error) {
    console.error("Failed to fetch place details/reviews:", error);
  }

  const formatTimeString = (timeStr: string) => {
    if (!timeStr) return "";
    const hours = parseInt(timeStr.split(":")[0], 10); // استخراج ساعات فقط وتحويلها لرقم
    const adjustedHour = hours > 12 ? hours - 12 : hours === 0 ? 12 : hours;
    const suffix = hours >= 12 ? "م" : "ص";
    return `${adjustedHour}${suffix}`;
  };

  const openingFormatted = formatTimeString(PlaceInfo.openingTime);
  const closingFormatted = formatTimeString(PlaceInfo.closingTime);

  const workdays = () => {
    switch (PlaceInfo.unavailableAt[0]) {
      case WeekDays.الاحد:
        return `من الاثنين وحتى السبت ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.الاثنين:
        return `من الثلثاء وحتى الأحد ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.الثلثاء:
        return `من الأربعاء وحتى الاثنين ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.الأربعاء:
        return `من الخميس وحتى الثلثاء ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.الخميس:
        return `من الجمعه وحتى الأربعاء ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.الجمعة:
        return `من السبت وحتى الخميس ${openingFormatted} - ${closingFormatted}`;
      case WeekDays.السبت:
        return `من الأحد وحتى الجمعة ${openingFormatted} - ${closingFormatted}`;
      default:
        return `طوال ايام الاسبوع ${openingFormatted} - ${closingFormatted}`;
    }
  };

  return (
    <>
      <section
        dir="rtl"
        className="mx-auto py-4 container grid grid-cols-12 transition-all duration-75"
      >
        <div className="col-span-4">
          <Pattern ImagesArray={PlaceInfo.imageUrls} />
        </div>
        <div className="col-span-8 ps-3 flex-wrap justify-around gap-y-2">
          <div className="nameAndDescription mb-12 mt-6">
            <h1 className="text-4xl font-semibold mb-3.5">{PlaceInfo.name}</h1>
            <p className="text-gray-500 font-light text-base">
              {PlaceInfo.description}
            </p>
          </div>

          <div className="info">
            <h2 className="text-base">
              <span className="font-semibold text-green-500">مفتوح</span> حتى
              {closingFormatted}
            </h2>
            <h2 className="text-base my-2">{workdays()}</h2>
            <h2 className="text-base">
              <span className="text-red-500 font-bold"> غير متاح </span>
              {PlaceInfo.unavailableAt[0]}
            </h2>
          </div>
        </div>
      </section>
      <hr className="border-t-3" />
      <CommentsSection
        placeId={PlaceInfo.id}
        initialReviews={detailData?.rates?.reviews ?? []}
        oneToFiveRating={
          detailData?.oneToFiveRating ?? {
            fiveStar: 0,
            fourStar: 0,
            threeStar: 0,
            twoStar: 0,
            oneStar: 0,
          }
        }
        averageRate={Number(PlaceInfo.averageRate)}
        rateCount={Number(PlaceInfo.rateCount)}
        initialRemainingCount={detailData?.rates?.remainingReviewsCount ?? 0}
      />
    </>
  );
};

export default PlaceDetails;
