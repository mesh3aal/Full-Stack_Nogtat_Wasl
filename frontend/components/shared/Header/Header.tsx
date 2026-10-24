import Link from "next/link";
import Image from "next/image";
import CategoriesDropdown from "./CategoriesDropdown";
import SearchComponent from "./SearchComponent";
import { getSession } from "@/lib/session";
import { UserDropDownMenu } from "@/components/examples/c-dropdown-menu-15";

export default async function Header() {
  const session = await getSession();

  return (
    <header className="py-4 border-b border-primary shadow-md w-full bg-surface-warm">
      <div className="flex justify-between items-center container mx-auto px-4 sm:px-6 lg:px-8">
        {session ? (
          <div className="flex items-center gap-x-3">
            <UserDropDownMenu session={session} />
          </div>
        ) : (
          <Link
            href="/login"
            className="text-sm px-6 py-2.5 rounded-md bg-accent-black text-accent-foreground hover:bg-accent-black/80"
          >
            تسجيل
          </Link>
        )}

        <nav className="flex items-center gap-x-6 text-nd">
          <Link
            className="py-1 px-2.5 rounded-lg text-md font-medium text-stone-900 hover:text-stone-700 hover:bg-black/5 transition-all duration-150 cursor-pointer select-none focus:outline-none focus-visible:ring-2 focus-visible:ring-primary"
            href="/"
          >
            الرئيسية
          </Link>
          <CategoriesDropdown />
        </nav>

        <div className="px-2 flex justify-center items-center gap-x-4">
          <SearchComponent />
          <Link href="/">
            <Image src="/logo.svg" width={35} height={35} alt="logo" />
          </Link>
        </div>
      </div>
    </header>
  );
}
