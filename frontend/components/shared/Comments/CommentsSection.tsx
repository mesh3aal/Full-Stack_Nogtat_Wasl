"use client";

import { useState } from "react";
import Image from "next/image";
import { Rating } from "@/components/reui/rating";
import { api } from "@/lib/api";

/* ─── Types ─── */
interface ReviewUser {
  userName: string;
  userAvatar: string;
}

interface ReactionCount {
  likes: number;
  dislikes: number;
}

export interface Review {
  reviewId: number;
  placeName: string;
  rateValue: number;
  note: string;
  createdAt: string;
  user: ReviewUser;
  reactionType: string;
  reactionCount: ReactionCount;
}

export interface OneToFiveRating {
  fiveStar: number;
  fourStar: number;
  threeStar: number;
  twoStar: number;
  oneStar: number;
}

interface CommentsSectionProps {
  placeId: number;
  initialReviews: Review[];
  oneToFiveRating: OneToFiveRating;
  averageRate: number;
  rateCount: number;
  initialRemainingCount: number;
}

/* ─── Arabic relative time ─── */
function getRelativeTime(dateString: string): string {
  const now = new Date();
  const date = new Date(dateString);
  const diffMs = now.getTime() - date.getTime();
  const diffSeconds = Math.floor(diffMs / 1000);
  const diffMinutes = Math.floor(diffSeconds / 60);
  const diffHours = Math.floor(diffMinutes / 60);
  const diffDays = Math.floor(diffHours / 24);
  const diffMonths = Math.floor(diffDays / 30);
  const diffYears = Math.floor(diffDays / 365);

  if (diffSeconds < 60) return "منذ لحظات";
  if (diffMinutes === 1) return "منذ دقيقة";
  if (diffMinutes <= 10) return `منذ ${diffMinutes} دقائق`;
  if (diffMinutes < 60) return `منذ ${diffMinutes} دقيقة`;
  if (diffHours === 1) return "منذ ساعة";
  if (diffHours <= 10) return `منذ ${diffHours} ساعات`;
  if (diffHours < 24) return `منذ ${diffHours} ساعة`;
  if (diffDays === 1) return "منذ يوم";
  if (diffDays <= 10) return `منذ ${diffDays} أيام`;
  if (diffDays < 30) return `منذ ${diffDays} يوماً`;
  if (diffMonths === 1) return "منذ شهر";
  if (diffMonths <= 10) return `منذ ${diffMonths} أشهر`;
  if (diffMonths < 12) return `منذ ${diffMonths} شهراً`;
  if (diffYears === 1) return "منذ سنة";
  return `منذ ${diffYears} سنوات`;
}

/* ─── Inline SVG icons ─── */
function ThumbUpIcon({ className }: { className?: string }) {
  return (
    <svg
      className={className}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M7 10v12" />
      <path d="M15 5.88 14 10h5.83a2 2 0 0 1 1.92 2.56l-2.33 8A2 2 0 0 1 17.5 22H4a2 2 0 0 1-2-2v-8a2 2 0 0 1 2-2h2.76a2 2 0 0 0 1.79-1.11L12 2h0a3.13 3.13 0 0 1 3 3.88Z" />
    </svg>
  );
}

function ThumbDownIcon({ className }: { className?: string }) {
  return (
    <svg
      className={className}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="M17 14V2" />
      <path d="M9 18.12 10 14H4.17a2 2 0 0 1-1.92-2.56l2.33-8A2 2 0 0 1 6.5 2H20a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2h-2.76a2 2 0 0 0-1.79 1.11L12 22h0a3.13 3.13 0 0 1-3-3.88Z" />
    </svg>
  );
}

function SpinnerIcon({ className }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 24 24" fill="none">
      <circle
        className="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        strokeWidth="4"
      />
      <path
        className="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>
  );
}

/* ─── User Avatar with Fallback ─── */
function UserAvatar({ src, name }: { src?: string; name: string }) {
  const [hasError, setHasError] = useState(false);
  const initial = name ? name.trim().charAt(0).toUpperCase() : "؟";

  if (!src || hasError) {
    return (
      <div className="w-10 h-10 rounded-full bg-muted flex items-center justify-center font-bold text-sm text-foreground/80 shrink-0 ring-2 ring-border/50 select-none">
        {initial}
      </div>
    );
  }

  return (
    <div className="relative w-10 h-10 rounded-full overflow-hidden ring-2 ring-border/50 shrink-0 bg-muted">
      <Image
        src={src}
        alt={name}
        fill
        sizes="40px"
        className="object-cover"
        onError={() => setHasError(true)}
      />
    </div>
  );
}

/* ─── Component ─── */
export function CommentsSection({
  placeId,
  initialReviews,
  oneToFiveRating,
  averageRate,
  rateCount,
  initialRemainingCount,
}: CommentsSectionProps) {
  const [reviews, setReviews] = useState<Review[]>(initialReviews);
  const [remainingCount, setRemainingCount] =
    useState(initialRemainingCount);
  const [pageNumber, setPageNumber] = useState(1);
  const [isLoading, setIsLoading] = useState(false);

  const totalRatings =
    (oneToFiveRating?.fiveStar ?? 0) +
    (oneToFiveRating?.fourStar ?? 0) +
    (oneToFiveRating?.threeStar ?? 0) +
    (oneToFiveRating?.twoStar ?? 0) +
    (oneToFiveRating?.oneStar ?? 0);

  const ratingBars = [
    { stars: 5, count: oneToFiveRating?.fiveStar ?? 0 },
    { stars: 4, count: oneToFiveRating?.fourStar ?? 0 },
    { stars: 3, count: oneToFiveRating?.threeStar ?? 0 },
    { stars: 2, count: oneToFiveRating?.twoStar ?? 0 },
    { stars: 1, count: oneToFiveRating?.oneStar ?? 0 },
  ];

  const loadMore = async () => {
    if (isLoading) return;
    setIsLoading(true);
    try {
      const nextPage = pageNumber + 1;
      const response = await api.get(
        `/api/customer/places/${placeId}/Detail?pageNumber=${nextPage}&pageSize=10`,
      );
      const data = response.data;
      if (data?.rates?.reviews) {
        setReviews((prev) => {
          const existingIds = new Set(prev.map((r) => r.reviewId));
          const newReviews = data.rates.reviews.filter(
            (r: Review) => !existingIds.has(r.reviewId),
          );
          return [...prev, ...newReviews];
        });
        setRemainingCount(data.rates.remainingReviewsCount ?? 0);
        setPageNumber(nextPage);
      }
    } catch (error) {
      console.error("Failed to load more reviews:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <section dir="rtl" className="mx-auto container py-8 px-4">
      {/* ── Section Title ── */}
      <h2 className="text-2xl font-bold mb-8">التقييمات والمراجعات</h2>

      {/* ── Rating Summary ── */}
      <div className="flex flex-col sm:flex-row gap-8 items-start mb-8">
        {/* Average */}
        <div className="flex flex-col items-center gap-2 min-w-[130px] py-5 px-7 rounded-2xl bg-muted/50 border border-border/40">
          <span className="text-5xl font-bold tracking-tight">
            {Number(averageRate).toFixed(1)}
          </span>
          <Rating rating={Number(averageRate)} />
          <span className="text-muted-foreground text-sm">
            {rateCount} تقييم
          </span>
        </div>

        {/* Distribution bars */}
        <div className="flex-1 flex flex-col gap-2.5 w-full max-w-md pt-2">
          {ratingBars.map(({ stars, count }) => (
            <div key={stars} className="flex items-center gap-3">
              <div className="flex items-center gap-1 w-8 shrink-0">
                <span className="text-sm font-medium text-muted-foreground">
                  {stars}
                </span>
                <span className="text-yellow-400 text-xs">★</span>
              </div>
              <div className="flex-1 h-2.5 bg-muted rounded-full overflow-hidden">
                <div
                  className="h-full bg-yellow-400 rounded-full transition-all duration-700 ease-out"
                  style={{
                    width:
                      totalRatings > 0
                        ? `${(count / totalRatings) * 100}%`
                        : "0%",
                  }}
                />
              </div>
              <span className="text-sm w-6 text-muted-foreground text-center font-medium shrink-0">
                {count}
              </span>
            </div>
          ))}
        </div>
      </div>

      {/* ── Separator ── */}
      <div className="border-t border-border mb-8" />

      {/* ── Reviews List ── */}
      {reviews.length > 0 ? (
        <div className="flex flex-col gap-5">
          {reviews.map((review) => {
            const isLiked = review.reactionType === "Like";
            const isDisliked = review.reactionType === "Dislike";

            return (
              <article
                key={review.reviewId}
                className="group p-5 rounded-xl border border-border/50 bg-card hover:border-border hover:shadow-[0_2px_12px_rgba(0,0,0,0.04)] transition-all duration-300"
              >
                {/* User info */}
                <div className="flex items-center gap-3 mb-3">
                  <UserAvatar
                    src={review.user?.userAvatar}
                    name={review.user?.userName ?? "مستخدم"}
                  />
                  <div className="flex-1 min-w-0">
                    <h4 className="font-semibold text-sm truncate">
                      {review.user?.userName}
                    </h4>
                    <div className="flex items-center gap-2 mt-0.5">
                      <Rating rating={review.rateValue} size="sm" />
                      <span className="text-[11px] text-muted-foreground">
                        · {getRelativeTime(review.createdAt)}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Comment text */}
                {review.note && (
                  <p className="text-sm leading-relaxed text-foreground/80 mb-3 ps-[52px]">
                    {review.note}
                  </p>
                )}

                {/* Reactions */}
                <div className="flex items-center gap-5 ps-[52px]">
                  <button
                    type="button"
                    className={`flex items-center gap-1.5 transition-colors duration-200 text-xs group/like ${
                      isLiked
                        ? "text-green-600 font-semibold"
                        : "text-muted-foreground hover:text-green-600"
                    }`}
                  >
                    <ThumbUpIcon className="w-4 h-4 transition-transform group-hover/like:scale-110" />
                    <span>{review.reactionCount?.likes ?? 0}</span>
                  </button>
                  <button
                    type="button"
                    className={`flex items-center gap-1.5 transition-colors duration-200 text-xs group/dislike ${
                      isDisliked
                        ? "text-red-500 font-semibold"
                        : "text-muted-foreground hover:text-red-500"
                    }`}
                  >
                    <ThumbDownIcon className="w-4 h-4 transition-transform group-hover/dislike:scale-110" />
                    <span>{review.reactionCount?.dislikes ?? 0}</span>
                  </button>
                </div>
              </article>
            );
          })}
        </div>
      ) : (
        <div className="text-center py-16">
          <div className="text-4xl mb-3">💬</div>
          <p className="text-muted-foreground text-sm">
            لا توجد تعليقات حتى الآن
          </p>
        </div>
      )}

      {/* ── Load More ── */}
      {remainingCount > 0 && (
        <div className="flex justify-center mt-8">
          <button
            type="button"
            onClick={loadMore}
            disabled={isLoading}
            className="px-8 py-2.5 rounded-xl border border-border text-sm font-medium text-foreground/80 hover:bg-muted hover:text-foreground active:scale-[0.98] transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
          >
            {isLoading ? (
              <span className="flex items-center gap-2">
                <SpinnerIcon className="animate-spin w-4 h-4" />
                جاري التحميل...
              </span>
            ) : (
              `تحميل المزيد (${remainingCount})`
            )}
          </button>
        </div>
      )}
    </section>
  );
}
