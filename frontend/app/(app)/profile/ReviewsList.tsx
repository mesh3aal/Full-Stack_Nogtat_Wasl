"use client";

import { useState, useCallback } from "react";
import Image from "next/image";
import { authApi } from "@/lib/authApi";

interface ReviewUser { userName: string; userAvatar: string; }
interface ReactionCount { likes: number; dislikes: number; }
export interface Review {
  reviewId: number; placeName: string; rateValue: number;
  note: string; createdAt: string; user: ReviewUser; reactionCount: ReactionCount;
}

function getRelativeTime(ds: string): string {
  const diff = Math.floor((Date.now() - new Date(ds).getTime()) / 1000);
  const m = Math.floor(diff / 60), h = Math.floor(m / 60), d = Math.floor(h / 24);
  if (diff < 60) return "منذ لحظات";
  if (m < 60) return `منذ ${m} دقيقة`;
  if (h < 24) return `منذ ${h} ساعة`;
  if (d === 1) return "منذ يوم";
  if (d < 30) return `منذ ${d} أيام`;
  const mo = Math.floor(d / 30);
  if (mo < 12) return `منذ ${mo} شهر`;
  return `منذ ${Math.floor(d / 365)} سنة`;
}

function StarRating({ value, onChange }: { value: number; onChange?: (v: number) => void }) {
  const [hovered, setHovered] = useState(0);
  return (
    <div className="flex gap-0.5">
      {[1,2,3,4,5].map(s => (
        <button key={s} type="button"
          onClick={() => onChange?.(s)}
          onMouseEnter={() => onChange && setHovered(s)}
          onMouseLeave={() => onChange && setHovered(0)}
          className={`text-xl transition-transform ${onChange ? "cursor-pointer hover:scale-110" : "cursor-default"}`}>
          <span className={(hovered || value) >= s ? "text-amber-400" : "text-stone-300"}>★</span>
        </button>
      ))}
    </div>
  );
}

function UserAvatar({ src, name }: { src?: string; name: string }) {
  const [err, setErr] = useState(false);
  if (!src || err) return (
    <div className="w-10 h-10 rounded-full bg-stone-200 flex items-center justify-center font-bold text-sm text-stone-600 shrink-0">{name?.[0]?.toUpperCase() ?? "؟"}</div>
  );
  return (
    <div className="relative w-10 h-10 rounded-full overflow-hidden shrink-0 bg-stone-100">
      <Image src={src} alt={name} fill sizes="40px" className="object-cover" onError={() => setErr(true)} />
    </div>
  );
}

function EditModal({ review, onClose, onSaved }: { review: Review; onClose: () => void; onSaved: (u: Review) => void }) {
  const [rate, setRate] = useState(review.rateValue);
  const [note, setNote] = useState(review.note || "");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault(); setLoading(true); setError("");
    try {
      await authApi.patch(`/api/reviews/${review.reviewId}`, { rateValue: rate, note });
      onSaved({ ...review, rateValue: rate, note });
    } catch (err: any) { setError(err?.message ?? "حدث خطأ، حاول مرة أخرى"); }
    finally { setLoading(false); }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md p-6" onClick={e => e.stopPropagation()}>
        <div className="flex justify-between items-center mb-5">
          <h3 className="text-lg font-bold text-stone-900">تعديل التعليق</h3>
          <button onClick={onClose} className="text-stone-400 hover:text-stone-600">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12"/></svg>
          </button>
        </div>
        <p className="text-sm text-stone-500 mb-4">تعليقك على: <span className="font-semibold text-stone-700">{review.placeName}</span></p>
        <form onSubmit={submit} className="flex flex-col gap-4">
          <div>
            <label className="text-sm font-medium text-stone-700 mb-2 block">التقييم</label>
            <StarRating value={rate} onChange={setRate} />
          </div>
          <div>
            <label className="text-sm font-medium text-stone-700 mb-2 block">التعليق</label>
            <textarea value={note} onChange={e => setNote(e.target.value)} maxLength={1000} rows={4}
              className="w-full border border-stone-200 rounded-xl px-4 py-3 text-sm resize-none focus:outline-none focus:ring-2 focus:ring-stone-400"
              placeholder="اكتب تعليقك هنا..." />
            <p className="text-xs text-stone-400 mt-1 text-left">{note.length}/1000</p>
          </div>
          {error && <p className="text-sm text-red-500 bg-red-50 rounded-lg px-3 py-2">{error}</p>}
          <div className="flex gap-3">
            <button type="submit" disabled={loading} className="flex-1 py-2.5 bg-stone-900 text-white rounded-xl text-sm font-medium hover:bg-stone-700 disabled:opacity-50">
              {loading ? "جاري الحفظ..." : "حفظ التعديلات"}
            </button>
            <button type="button" onClick={onClose} className="flex-1 py-2.5 bg-stone-100 text-stone-700 rounded-xl text-sm font-medium hover:bg-stone-200">إلغاء</button>
          </div>
        </form>
      </div>
    </div>
  );
}

function DeleteModal({ reviewId, placeName, onClose, onDeleted }: { reviewId: number; placeName: string; onClose: () => void; onDeleted: () => void }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const del = async () => {
    setLoading(true); setError("");
    try { await authApi.delete(`/api/reviews/${reviewId}`); onDeleted(); }
    catch (err: any) { setError(err?.message ?? "حدث خطأ"); setLoading(false); }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6" onClick={e => e.stopPropagation()}>
        <div className="flex flex-col items-center text-center gap-3 mb-6">
          <div className="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
            <svg className="w-6 h-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z"/></svg>
          </div>
          <h3 className="text-lg font-bold">حذف التعليق</h3>
          <p className="text-sm text-stone-500">هل أنت متأكد من حذف تعليقك على <span className="font-semibold text-stone-700">{placeName}</span>؟</p>
        </div>
        {error && <p className="text-sm text-red-500 bg-red-50 rounded-lg px-3 py-2 mb-4">{error}</p>}
        <div className="flex gap-3">
          <button onClick={del} disabled={loading} className="flex-1 py-2.5 bg-red-500 text-white rounded-xl text-sm font-medium hover:bg-red-600 disabled:opacity-50">
            {loading ? "جاري الحذف..." : "حذف"}
          </button>
          <button onClick={onClose} className="flex-1 py-2.5 bg-stone-100 text-stone-700 rounded-xl text-sm font-medium hover:bg-stone-200">إلغاء</button>
        </div>
      </div>
    </div>
  );
}

export default function ReviewsList({
  initialReviews,
  initialRemainingCount,
}: {
  initialReviews: Review[];
  initialRemainingCount: number;
}) {
  const [reviews, setReviews] = useState<Review[]>(initialReviews);
  const [remainingCount, setRemainingCount] = useState(initialRemainingCount);
  const [pageNumber, setPageNumber] = useState(1);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [editing, setEditing] = useState<Review | null>(null);
  const [deleting, setDeleting] = useState<{ id: number; name: string } | null>(null);

  const loadMore = useCallback(async () => {
    setIsLoadingMore(true);
    try {
      const data = await authApi.get<{ reviews: Review[]; remainingReviewsCount: number }>(
        "/api/reviews/myReviews", { pageNumber: pageNumber + 1, pageSize: 10 }
      );
      setReviews(prev => [...prev, ...(data?.reviews ?? [])]);
      setRemainingCount(data?.remainingReviewsCount ?? 0);
      setPageNumber(p => p + 1);
    } catch { /* silent */ }
    finally { setIsLoadingMore(false); }
  }, [pageNumber]);

  return (
    <>
      {editing && <EditModal review={editing} onClose={() => setEditing(null)} onSaved={u => { setReviews(p => p.map(r => r.reviewId === u.reviewId ? u : r)); setEditing(null); }} />}
      {deleting && <DeleteModal reviewId={deleting.id} placeName={deleting.name} onClose={() => setDeleting(null)} onDeleted={() => { setReviews(p => p.filter(r => r.reviewId !== deleting!.id)); setDeleting(null); }} />}

      <div>
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-bold text-stone-900">تعليقاتي</h1>
            <p className="text-sm text-stone-500 mt-0.5">جميع التقييمات والتعليقات التي أضفتها</p>
          </div>
          {reviews.length > 0 && (
            <span className="px-3 py-1 bg-stone-900 text-white rounded-full text-sm font-medium">
              {reviews.length + remainingCount} تعليق
            </span>
          )}
        </div>

        {reviews.length === 0 && (
          <div className="bg-white rounded-2xl border border-stone-200/60 p-16 text-center">
            <div className="text-5xl mb-4">💬</div>
            <h3 className="text-lg font-semibold text-stone-700 mb-2">لا توجد تعليقات بعد</h3>
            <p className="text-sm text-stone-500">قم بزيارة الأماكن وأضف تقييماتك لتظهر هنا</p>
          </div>
        )}

        {reviews.length > 0 && (
          <div className="flex flex-col gap-4">
            {reviews.map(review => (
              <article key={review.reviewId}
                className="bg-white rounded-2xl border border-stone-200/60 p-5 hover:shadow-md hover:border-stone-300 transition-all duration-200 group">
                <div className="flex items-start gap-3">
                  <UserAvatar src={review.user?.userAvatar} name={review.user?.userName ?? "م"} />
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <h4 className="font-semibold text-stone-900 text-sm">{review.user?.userName}</h4>
                        <div className="flex items-center gap-2 mt-1">
                          <StarRating value={review.rateValue} />
                          <span className="text-xs text-stone-400">· {getRelativeTime(review.createdAt)}</span>
                        </div>
                      </div>
                      <div className="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity shrink-0">
                        <button onClick={() => setEditing(review)}
                          className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-stone-600 bg-stone-100 hover:bg-stone-200 rounded-lg transition-colors">
                          <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Z"/></svg>
                          تعديل
                        </button>
                        <button onClick={() => setDeleting({ id: review.reviewId, name: review.placeName })}
                          className="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-red-600 bg-red-50 hover:bg-red-100 rounded-lg transition-colors">
                          <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"/></svg>
                          حذف
                        </button>
                      </div>
                    </div>
                    <div className="mt-2">
                      <span className="inline-flex items-center gap-1 text-xs text-stone-500 bg-stone-50 border border-stone-200 rounded-lg px-2.5 py-1">
                        <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z"/></svg>
                        {review.placeName}
                      </span>
                    </div>
                    {review.note && <p className="text-sm text-stone-600 leading-relaxed mt-3">{review.note}</p>}
                    <div className="flex gap-4 mt-3 pt-3 border-t border-stone-100">
                      <span className="flex items-center gap-1 text-xs text-stone-400">👍 {review.reactionCount?.likes ?? 0}</span>
                      <span className="flex items-center gap-1 text-xs text-stone-400">👎 {review.reactionCount?.dislikes ?? 0}</span>
                    </div>
                  </div>
                </div>
              </article>
            ))}
          </div>
        )}

        {remainingCount > 0 && (
          <div className="flex justify-center mt-6">
            <button onClick={loadMore} disabled={isLoadingMore}
              className="px-8 py-2.5 border border-stone-300 rounded-xl text-sm font-medium text-stone-700 hover:bg-stone-100 transition-all disabled:opacity-50">
              {isLoadingMore ? "جاري التحميل..." : `تحميل المزيد (${remainingCount})`}
            </button>
          </div>
        )}
      </div>
    </>
  );
}
