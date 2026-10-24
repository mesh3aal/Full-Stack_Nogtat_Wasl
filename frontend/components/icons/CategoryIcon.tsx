
import * as LucideIcons from "lucide-react";
import type { LucideIcon } from "lucide-react";

type CategoryIconProps = {
  name?: string | null;
  size?: number;
  className?: string;
};

const iconMap = LucideIcons as unknown as Record<
  string,
  LucideIcon
>;

export default function CategoryIcon({
  name,
  size = 24,
  className,
}: CategoryIconProps) {
  const Icon =
    name && Object.prototype.hasOwnProperty.call(iconMap, name)
      ? iconMap[name]
      : LucideIcons.Store;

  return (
    <Icon
      size={size}
      className={className}
      aria-hidden="true"
    />
  );
}
