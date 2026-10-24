import { getSession } from "@/lib/session";
import Link from "next/link";

export default async function HeroSection() {
  const session = await getSession();
  return (
    <>
      <div className="w-full bg-[url(/image-408.jpg)] bg-cover bg-center mb-24">
        <div className="w-full flex flex-col justify-center items-center  gap-y-14 py-30 bg-black/50 text-white">
          {!session ? (
            <>
              <p className="text-center text-xl/10">
                صاحب محل وتبي تقييم الناس لإداء محلك ؟ مستخدم وتبي تقيم محلات
                انت زرتها ؟ <br /> الحل عندنا سجل وقيم المحلات اللي زرتها بنفسك.
                او سجل محلاتك وتابع تقييم الناس لك.
              </p>
              <h2 className="text-5xl">سجل الأن وقيم المحلات اللي حولك</h2>
              <div className="flex gap-x-10">
                <Link
                  href="/login"
                  className="text-2xl text-white hover:bg-gray-950 bg-black py-2.5 px-14 rounded-xl"
                >
                  سجل
                </Link>
                <Link
                  href="/category"
                  className="text-2xl font-medium text-black bg-primary hover:bg-olive-400 py-2.5 px-14 rounded-xl"
                >
                  تصفح
                </Link>
              </div>
            </>
          ) : (
            <>
              <p className="text-center text-xl/10">
                صاحب محل وتبي تقييم الناس لإداء محلك ؟ مستخدم وتبي تقيم محلات
                انت زرتها ؟ <br /> الحل عندنا سجل وقيم المحلات اللي زرتها بنفسك.
                او سجل محلاتك وتابع تقييم الناس لك.
              </p>
              <h2 className="text-5xl">تصفح الان المحلات اللي حولك</h2>
              <div className="flex gap-x-10">
                <Link
                  href="/category"
                  className="text-2xl text-white hover:bg-gray-950 bg-black py-2.5 px-14 rounded-xl"
                >
                  تصفح
                </Link>
              </div>
            </>
          )}
        </div>
      </div>
    </>
  );
}
