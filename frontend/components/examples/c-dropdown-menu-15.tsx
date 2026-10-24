import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { HugeiconsIcon } from "@hugeicons/react";
import {
  UserIcon,
  Heart,
  Settings01Icon,
  LogoutSquare01Icon,
} from "@hugeicons/core-free-icons";
import { SessionUser } from "@/lib/definitions";
import Link from "next/link";
import { logout } from "@/lib/actions";

export function UserDropDownMenu({ session }: { session: SessionUser }) {
  return (
    <DropdownMenu modal={false}>
      <DropdownMenuTrigger>
        <Avatar className="size-9">
          <AvatarImage src={session.avatar} alt={session.userName} />
          <AvatarFallback>{session.userName.charAt(0)}</AvatarFallback>
        </Avatar>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" sideOffset={12} className="w-56">
        <DropdownMenuGroup>
          <DropdownMenuLabel>
            <div className="flex flex-col">
              <span className="text-foreground text-sm">
                {session.userName}
              </span>
              <span className="text-muted-foreground text-xs">
                {session.email}
              </span>
            </div>
          </DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuGroup>
            <Link href="/profile/">
              <DropdownMenuItem>
                <HugeiconsIcon
                  icon={UserIcon}
                  strokeWidth={2}
                  className="size-4 opacity-60"
                  aria-hidden="true"
                />
                الملف الشخصي
              </DropdownMenuItem>
            </Link>
            <Link href="/profile/favourites">
              <DropdownMenuItem>
                <HugeiconsIcon
                  icon={Heart}
                  strokeWidth={2}
                  className="size-4 opacity-60"
                  aria-hidden="true"
                />
                المفضلة
              </DropdownMenuItem>
            </Link>
            <Link href="/profile/settings">
              <DropdownMenuItem>
                <HugeiconsIcon
                  icon={Settings01Icon}
                  strokeWidth={2}
                  className="size-4 opacity-60"
                  aria-hidden="true"
                />
                إعدادات الحساب
              </DropdownMenuItem>
            </Link>
          </DropdownMenuGroup>
          <DropdownMenuSeparator />
          <DropdownMenuItem className="w-full" variant="destructive">
            <form className="w-full" action={logout}>
              <button type="submit" className="flex gap-x-1.5 w-full">
                <HugeiconsIcon
                  icon={LogoutSquare01Icon}
                  strokeWidth={2}
                  aria-hidden="true"
                />
                تسجيل خروج
              </button>
            </form>
          </DropdownMenuItem>
        </DropdownMenuGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
