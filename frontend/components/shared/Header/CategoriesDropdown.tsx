"use client";

import { useState, useRef, useEffect, useCallback } from "react";
import Link from "next/link";
import { ChevronDown, ArrowLeft, LayoutGrid } from "lucide-react";
import { api } from "@/lib/api";
import CategoryIcon from "@/components/icons/CategoryIcon";

interface SubCategory {
  subCategoryId: number;
  subCategoryName: string;
  subCategoryIcon?: string;
}

interface Category {
  id: number;
  categoryName: string;
  categoryIcon?: string;
  source: string;
  subCategory: Array<SubCategory>;
}

interface CategoriesResponse {
  categories: Array<Category>;
  totalCount?: number;
}

export default function CategoriesDropdown() {
  const [Categories, setCategories] = useState<Category[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const timeoutRef = useRef<NodeJS.Timeout | null>(null);

  const closeDropdown = useCallback(() => {
    setIsOpen(false);
  }, []);

  useEffect(() => {
    async function fetch() {
      try {
        const res = await api.get<CategoriesResponse>("/api/admin/categories");
        setCategories(res.data.categories ?? []);
      } catch (error: unknown) {
        console.log(`حدث خطاء اثناء الطلب: ${error}`);
      }
    }
    fetch();
  }, []);

  // Close on outside click
  useEffect(() => {
    function handleClickOutside(event: MouseEvent | TouchEvent) {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        closeDropdown();
      }
    }

    if (isOpen) {
      document.addEventListener("mousedown", handleClickOutside);
      document.addEventListener("touchstart", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("touchstart", handleClickOutside);
    };
  }, [isOpen, closeDropdown]);

  // Optional gentle hover behavior with delay
  const handleMouseEnter = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current);
    timeoutRef.current = setTimeout(() => {
      setIsOpen(true);
    }, 120);
  };

  const handleMouseLeave = () => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current);
    timeoutRef.current = setTimeout(() => {
      setIsOpen(false);
    }, 200);
  };

  return (
    <div
      ref={dropdownRef}
      className="relative inline-block"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      {/* Trigger Button */}
      <button
        type="button"
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex items-center gap-1.5 py-1 px-2.5 rounded-lg text-md font-medium text-stone-900 hover:text-stone-700 hover:bg-black/5 transition-all duration-150 cursor-pointer select-none focus:outline-none focus-visible:ring-2 focus-visible:ring-primary"
        aria-expanded={isOpen}
        aria-haspopup="true"
        aria-label="قائمة الفئات"
      >
        <span>الفئات</span>
        <ChevronDown
          size={18}
          className={`transition-transform duration-200 ease-in-out ${
            isOpen ? "rotate-180 text-black" : "text-stone-600"
          }`}
        />
      </button>

      {/* Dropdown Menu */}
      <div
        className={`absolute top-full start-0 mt-2 w-[calc(100vw-2rem)] sm:w-[460px] max-w-[480px] z-50 bg-[#fdfbf7] border border-[#d5cfc2] rounded-2xl shadow-2xl shadow-stone-900/15 overflow-hidden transition-all duration-200 ease-out origin-top before:absolute before:-top-2 before:inset-x-0 before:h-2 ${
          isOpen
            ? "opacity-100 scale-100 translate-y-0 pointer-events-auto"
            : "opacity-0 scale-95 -translate-y-2 pointer-events-none hidden"
        }`}
        role="menu"
      >
        {/* Header inside dropdown */}
        <div className="flex items-center justify-between px-4 py-3 bg-[#f5efe4] border-b border-[#e2dcd0]">
          <div className="flex items-center gap-2 text-stone-800">
            <LayoutGrid size={18} />
            <span className="font-bold text-sm">استكشف الفئات</span>
          </div>
          <span className="text-xs text-stone-600 font-medium bg-[#eae4d5] px-2.5 py-0.5 rounded-full">
            {Categories.length} تصنيفات
          </span>
        </div>

        {/* Categories Grid (2 Columns) */}
        <div className="p-2.5 max-h-95 overflow-y-auto overscroll-contain grid grid-cols-2 gap-1.5 [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-track]:bg-transparent [&::-webkit-scrollbar-thumb]:bg-stone-300 [&::-webkit-scrollbar-thumb]:rounded-full hover:[&::-webkit-scrollbar-thumb]:bg-stone-400">
          {Categories &&
            Categories.map((category) => {
              const Icon = category.categoryIcon;
              return (
                <Link
                  key={category.id}
                  href={category.source}
                  onClick={closeDropdown}
                  role="menuitem"
                  className="group flex items-center gap-2.5 p-2 rounded-xl transition-all duration-150 hover:bg-[#eae4d5]/70 active:scale-[0.98]"
                >
                  <div className="flex items-center justify-center size-9 rounded-lg bg-[#eae4d5] text-stone-800 group-hover:bg-accent-black group-hover:text-white transition-colors duration-150 shrink-0">
                    <CategoryIcon name={Icon} size={24} />
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="text-sm font-semibold text-stone-900 group-hover:text-black truncate leading-tight">
                      {category.categoryName}
                    </p>
                  </div>
                </Link>
              );
            })}
        </div>

        {/* Dropdown Footer */}
        <div className="p-2 bg-[#f8f5ee] border-t border-[#e2dcd0]">
          <Link
            href="/category"
            onClick={closeDropdown}
            className="group flex items-center justify-center gap-2 py-2 px-3 text-xs font-semibold text-stone-700 hover:text-black hover:bg-[#eae4d5]/60 rounded-lg transition-colors"
          >
            <span>عرض كافة التصنيفات وفئات التصنيفات</span>
            <ArrowLeft
              size={14}
              className="transition-transform duration-150 group-hover:-translate-x-1"
            />
          </Link>
        </div>
      </div>
    </div>
  );
}
