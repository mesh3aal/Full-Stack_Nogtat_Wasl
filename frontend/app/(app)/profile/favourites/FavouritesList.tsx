"use client";

import { useState, useCallback } from "react";
import Image from "next/image";
import Link from "next/link";
import { authApi } from "@/lib/authApi";

export interface Favourite {
  placeId: number; placeName: string; mainCategoryName: string;
  subcategoryName: string; image?: string; averageRating: string;
  directorateName: string; districtName: string; favouritedAt: string;
}

function getRelativeTime(ds: string): string {
  const d = Math.floor((Date.now() - new Date(ds).getTime()) / 86400000);
  if (d === 0) return "اليوم";
  if (d === 1) return "منذ يوم";
  if (d < 30) return `منذ ${d} أيام`;
  const mo = Math.floor(d / 30);
  if (mo < 12) return `منذ ${mo} شهر`;
  return `منذ ${Math.floor(d / 365)} سنة`;
}

function PlaceImage({ src, name }: { src?: string; name: string }) {
  const [err, setErr] = useState(false);
  if (!src || err) return (
    <div className="w-full h-full bg-gradient-to-br from-stone-200 to-stone-300 flex items-center justify-center">
      <svg className="w-8 h-8 text-stone-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/>
        <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z"/>
      </svg>
    </div>
  );
  return <Image src={src} alt={name} fill sizes="280px" className="object-cover" onError={() => setErr(true)} />;
}

function DeleteModal({ fav, onClose, onDeleted }: { fav: Favourite; onClose: () => void; onDeleted: () => void }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const del = async () => {
    setLoading(true); setError("");
    try { await authApi.delete(`/api/customer/favourites/${fav.placeId}`); onDeleted(); }
    catch (err: any) { setError(err?.message ?? "حدث خطأ"); setLoading(false); }
  };
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm" onClick={onClose}>
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6" onClick={e => e.stopPropagation()}>
        <div className="flex flex-col items-center text-center gap-3 mb-6">
          <div className="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
            <svg className="w-6 h-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12Z"/></svg>
          </div>
          <h3 className="text-lg font-bold">إزالة من المفضلة</h3>
          <p className="text-sm text-stone-500">هل تريد إزالة <span className="font-semibold text-stone-700">{fav.placeName}</span>؟</p>
        </div>
        {error && <p className="text-sm text-red-500 bg-red-50 rounded-lg px-3 py-2 mb-4">{error}</p>}
        <div className="flex gap-3">
          <button onClick={del} disabled={loading} className="flex-1 py-2.5 bg-red-500 text-white rounded-xl text-sm font-medium hover:bg-red-600 disabled:opacity-50">
            {loading ? "جاري الإزالة..." : "إزالة"}
          </button>
          <button onClick={onClose} className="flex-1 py-2.5 bg-stone-100 text-stone-700 rounded-xl text-sm font-medium hover:bg-stone-200">إلغاء</button>
        </div>
      </div>
    </div>
  );
}

export default function FavouritesList({ initialFavourites }: { initialFavourites: Favourite[] }) {
  const [favs, setFavs] = useState<Favourite[]>(initialFavourites);
  const [hasMore, setHasMore] = useState(initialFavourites.length === 12);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [searchInput, setSearchInput] = useState("");
  const [loadingMore, setLoadingMore] = useState(false);
  const [deleting, setDeleting] = useState<Favourite | null>(null);

  const doSearch = (e: React.FormEvent) => { e.preventDefault(); setSearch(searchInput); };

  const loadMore = useCallback(async () => {
    setLoadingMore(true);
    try {
      const params: Record<string, string | number | undefined> = { pageNumber: page + 1, pageSize: 12 };
      if (search) params.searchString = search;
      const data = await authApi.get<Favourite[]>("/api/customer/favourites", params);
      const list = Array.isArray(data) ? data : [];
      setFavs(p => [...p, ...list]);
      setHasMore(list.length === 12);
      setPage(p => p + 1);
    } catch { /* silent */ }
    finally { setLoadingMore(false); }
  }, [page, search]);

  return (
    <>
      {deleting && <DeleteModal fav={deleting} onClose={() => setDeleting(null)} onDeleted={() => { setFavs(p => p.filter(f => f.placeId !== deleting!.placeId)); setDeleting(null); }} />}

      <div>
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-bold text-stone-900">المفضلة</h1>
            <p className="text-sm text-stone-500 mt-0.5">الأماكن التي أضفتها إلى مفضلتك</p>
          </div>
          <form onSubmit={doSearch} className="flex gap-2">
            <div className="relative">
              <input type="text" value={searchInput} onChange={e => setSearchInput(e.target.value)}
                placeholder="ابحث في المفضلة..."
                className="pr-10 pl-4 py-2 text-sm border border-stone-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-stone-400 bg-white w-52" />
              <svg className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-stone-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z"/></svg>
            </div>
            <button type="submit" className="px-4 py-2 bg-stone-900 text-white text-sm rounded-xl hover:bg-stone-700 transition-colors">بحث</button>
          </form>
        </div>

        {favs.length === 0 && (
          <div className="bg-white rounded-2xl border border-stone-200/60 p-16 text-center">
            <div className="text-5xl mb-4">🤍</div>
            <h3 className="text-lg font-semibold text-stone-700 mb-2">لا توجد مفضلات بعد</h3>
            <p className="text-sm text-stone-500 mb-6">{search ? "لا توجد نتائج لهذا البحث" : "استكشف الأماكن وأضفها إلى مفضلتك"}</p>
            {!search && <Link href="/" className="inline-flex items-center gap-2 px-6 py-2.5 bg-stone-900 text-white rounded-xl text-sm font-medium hover:bg-stone-700">استكشف الأماكن</Link>}
          </div>
        )}

        {favs.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
            {favs.map(fav => (
              <div key={fav.placeId} className="group bg-white rounded-2xl border border-stone-200/60 overflow-hidden hover:shadow-lg hover:border-stone-300 transition-all duration-300">
                <div className="relative h-40 overflow-hidden bg-stone-100">
                  <PlaceImage src={fav.image} name={fav.placeName} />
                  <div className="absolute top-3 left-3 flex items-center gap-1 bg-black/60 text-white text-xs font-semibold px-2.5 py-1 rounded-full">
                    <span className="text-amber-400">★</span><span>{Number(fav.averageRating).toFixed(1)}</span>
                  </div>
                  <button onClick={() => setDeleting(fav)}
                    className="absolute top-3 right-3 w-8 h-8 flex items-center justify-center bg-white/90 hover:bg-red-50 text-stone-400 hover:text-red-500 rounded-full shadow-sm transition-all opacity-0 group-hover:opacity-100">
                    <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24"><path d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12Z"/></svg>
                  </button>
                </div>
                <div className="p-4">
                  <Link href={`/places/${fav.placeName}`}>
                    <h3 className="font-bold text-stone-900 text-base leading-tight mb-1 line-clamp-1 hover:underline">{fav.placeName}</h3>
                  </Link>
                  <div className="flex items-center gap-1.5 mb-2">
                    <span className="text-xs text-stone-400 bg-stone-100 rounded-md px-2 py-0.5">{fav.mainCategoryName}</span>
                    <span className="text-stone-300 text-xs">·</span>
                    <span className="text-xs text-stone-400">{fav.subcategoryName}</span>
                  </div>
                  <div className="flex items-center gap-1 text-xs text-stone-500 mb-3">
                    <svg className="w-3.5 h-3.5 text-stone-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z"/></svg>
                    <span>{fav.districtName}، {fav.directorateName}</span>
                  </div>
                  <div className="flex items-center justify-between border-t border-stone-100 pt-3">
                    <span className="text-xs text-stone-400">{getRelativeTime(fav.favouritedAt)}</span>
                    <button onClick={() => setDeleting(fav)} className="flex items-center gap-1 text-xs text-red-400 hover:text-red-600 font-medium transition-colors">
                      <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"/></svg>
                      إزالة
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {hasMore && (
          <div className="flex justify-center mt-6">
            <button onClick={loadMore} disabled={loadingMore}
              className="px-8 py-2.5 border border-stone-300 rounded-xl text-sm font-medium text-stone-700 hover:bg-stone-100 transition-all disabled:opacity-50">
              {loadingMore ? "جاري التحميل..." : "تحميل المزيد"}
            </button>
          </div>
        )}
      </div>
    </>
  );
}
