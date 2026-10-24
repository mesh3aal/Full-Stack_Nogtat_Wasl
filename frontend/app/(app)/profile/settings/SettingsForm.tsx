"use client";

import { useState, useRef } from "react";
import Image from "next/image";
import { authApi } from "@/lib/authApi";

interface ProfileInfo { username?: string; email?: string; profileImage?: string; }

function UserAvatar({ src, name, size = 80 }: { src?: string; name?: string; size?: number }) {
  const [err, setErr] = useState(false);
  if (!src || err) return (
    <div className="rounded-full bg-gradient-to-br from-stone-300 to-stone-500 flex items-center justify-center font-bold text-white select-none shrink-0"
      style={{ width: size, height: size, fontSize: size * 0.4 }}>
      {name?.[0]?.toUpperCase() ?? "؟"}
    </div>
  );
  return (
    <div className="relative rounded-full overflow-hidden shrink-0" style={{ width: size, height: size }}>
      <Image src={src} alt={name ?? "avatar"} fill sizes={`${size}px`} className="object-cover" onError={() => setErr(true)} />
    </div>
  );
}

function Toast({ message, type, onClose }: { message: string; type: "success" | "error"; onClose: () => void }) {
  useState(() => { const t = setTimeout(onClose, 4000); return () => clearTimeout(t); });
  return (
    <div className={`fixed top-6 left-1/2 -translate-x-1/2 z-50 flex items-center gap-3 px-5 py-3 rounded-2xl shadow-xl text-sm font-medium ${type === "success" ? "bg-green-600 text-white" : "bg-red-500 text-white"}`}>
      {type === "success" ? "✓" : "!"} {message}
    </div>
  );
}

function Card({ title, desc, icon, children }: { title: string; desc: string; icon: React.ReactNode; children: React.ReactNode }) {
  return (
    <div className="bg-white rounded-2xl border border-stone-200/60 overflow-hidden">
      <div className="px-6 py-4 border-b border-stone-100 flex items-center gap-3">
        <div className="w-9 h-9 rounded-xl bg-stone-900 flex items-center justify-center text-white shrink-0">{icon}</div>
        <div><h2 className="font-bold text-stone-900 text-base">{title}</h2><p className="text-xs text-stone-500">{desc}</p></div>
      </div>
      <div className="p-6">{children}</div>
    </div>
  );
}

export default function SettingsForm({ initialProfile }: { initialProfile: ProfileInfo | null }) {
  const [profile, setProfile] = useState<ProfileInfo | null>(initialProfile);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [showDeactivate, setShowDeactivate] = useState(false);
  const fileRef = useRef<HTMLInputElement>(null);

  const [username, setUsername] = useState(initialProfile?.username ?? "");
  const [email, setEmail] = useState(initialProfile?.email ?? "");
  const [preview, setPreview] = useState<string | null>(null);
  const [file, setFile] = useState<File | null>(null);
  const [profileLoading, setProfileLoading] = useState(false);

  const [oldPw, setOldPw] = useState(""); const [newPw, setNewPw] = useState(""); const [confPw, setConfPw] = useState("");
  const [showOld, setShowOld] = useState(false); const [showNew, setShowNew] = useState(false); const [showConf, setShowConf] = useState(false);
  const [pwLoading, setPwLoading] = useState(false);

  const [reactEmail, setReactEmail] = useState(""); const [reactLoading, setReactLoading] = useState(false);

  const toast$ = (msg: string, type: "success" | "error") => setToast({ message: msg, type });

  const handleFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0]; if (!f) return;
    setFile(f); setPreview(URL.createObjectURL(f));
  };

  const submitProfile = async (e: React.FormEvent) => {
    e.preventDefault(); setProfileLoading(true);
    try {
      const form = new FormData();
      if (username !== profile?.username) form.append("username", username);
      if (email !== profile?.email) form.append("email", email);
      if (file) form.append("profileImage", file);
      const p = new URLSearchParams({ url: "/api/customer/ChangeProfile" });
      const res = await fetch(`/api/proxy?${p}`, { method: "PATCH", body: form });
      if (!res.ok) { const d = await res.json().catch(() => null); throw new Error(d?.detail ?? d?.title ?? "خطأ"); }
      setProfile(prev => ({ ...prev, username, email, profileImage: preview ?? prev?.profileImage }));
      setFile(null); toast$("تم تحديث الملف الشخصي بنجاح", "success");
    } catch (err: any) { toast$(err?.message ?? "حدث خطأ", "error"); }
    finally { setProfileLoading(false); }
  };

  const submitPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (newPw !== confPw) { toast$("كلمتا المرور غير متطابقتين", "error"); return; }
    setPwLoading(true);
    try {
      await authApi.post("/api/customer/changePassword", { oldPassword: oldPw, newPassword: newPw, confirmPassword: confPw });
      setOldPw(""); setNewPw(""); setConfPw("");
      toast$("تم تغيير كلمة المرور بنجاح", "success");
    } catch (err: any) { toast$(err?.message ?? "حدث خطأ", "error"); }
    finally { setPwLoading(false); }
  };

  const submitReactivate = async (e: React.FormEvent) => {
    e.preventDefault(); setReactLoading(true);
    try {
      await authApi.post("/api/customer/reactivate", undefined, { email: reactEmail });
      setReactEmail(""); toast$("تم إرسال طلب إعادة التفعيل، تحقق من بريدك", "success");
    } catch (err: any) { toast$(err?.message ?? "حدث خطأ", "error"); }
    finally { setReactLoading(false); }
  };

  const handleDeactivate = async () => {
    setShowDeactivate(false);
    try { await authApi.post("/api/customer/deactivate"); toast$("تم تعطيل حسابك، تحقق من بريدك الإلكتروني", "success"); }
    catch (err: any) { toast$(err?.message ?? "حدث خطأ", "error"); }
  };

  const Eye = ({ show }: { show: boolean }) => (
    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      {show
        ? <path strokeLinecap="round" strokeLinejoin="round" d="M3.98 8.223A10.477 10.477 0 0 0 1.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.451 10.451 0 0 1 12 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 0 1-4.293 5.774M6.228 6.228 3 3m3.228 3.228 3.65 3.65m7.894 7.894L21 21m-3.228-3.228-3.65-3.65m0 0a3 3 0 1 0-4.243-4.243m4.242 4.242L9.88 9.88" />
        : <><path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z"/><path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/></>
      }
    </svg>
  );

  const inp = "w-full border border-stone-200 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-stone-400 transition-shadow";
  const btn = "px-6 py-2.5 bg-stone-900 text-white rounded-xl text-sm font-medium hover:bg-stone-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed";

  return (
    <>
      {toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
      {showDeactivate && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 backdrop-blur-sm" onClick={() => setShowDeactivate(false)}>
          <div className="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6" onClick={e => e.stopPropagation()}>
            <div className="flex flex-col items-center text-center gap-3 mb-6">
              <div className="w-12 h-12 rounded-full bg-amber-100 flex items-center justify-center">
                <svg className="w-6 h-6 text-amber-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z"/></svg>
              </div>
              <h3 className="text-lg font-bold">تعطيل الحساب</h3>
              <p className="text-sm text-stone-500">هل أنت متأكد؟ لن تتمكن من الدخول حتى تعيد التفعيل.</p>
            </div>
            <div className="flex gap-3">
              <button onClick={handleDeactivate} className="flex-1 py-2.5 bg-amber-500 text-white rounded-xl text-sm font-medium hover:bg-amber-600">تعطيل</button>
              <button onClick={() => setShowDeactivate(false)} className="flex-1 py-2.5 bg-stone-100 text-stone-700 rounded-xl text-sm font-medium hover:bg-stone-200">إلغاء</button>
            </div>
          </div>
        </div>
      )}

      <div className="flex flex-col gap-6">
        <div>
          <h1 className="text-2xl font-bold text-stone-900">إعدادات الحساب</h1>
          <p className="text-sm text-stone-500 mt-0.5">إدارة معلوماتك الشخصية وأمان حسابك</p>
        </div>

        <Card title="الملف الشخصي" desc="تعديل معلوماتك وصورتك الشخصية"
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z"/></svg>}>
          <form onSubmit={submitProfile} className="flex flex-col gap-5">
            <div className="flex items-center gap-5">
              <div className="relative">
                <UserAvatar src={preview ?? profile?.profileImage} name={profile?.username} size={80} />
                <button type="button" onClick={() => fileRef.current?.click()}
                  className="absolute -bottom-1 -right-1 w-7 h-7 rounded-full bg-stone-900 text-white flex items-center justify-center hover:bg-stone-700 shadow-md">
                  <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}><path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Z"/></svg>
                </button>
              </div>
              <input ref={fileRef} type="file" accept=".jpg,.jpeg,.png" className="hidden" onChange={handleFile} />
              <div>
                <button type="button" onClick={() => fileRef.current?.click()} className="text-sm font-medium text-stone-700 hover:text-stone-900 underline underline-offset-2">تغيير الصورة</button>
                <p className="text-xs text-stone-400 mt-1">JPG أو PNG. الحد الأقصى 1 ميغابايت</p>
              </div>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-stone-700 block mb-1.5">اسم المستخدم</label>
                <input type="text" value={username} onChange={e => setUsername(e.target.value)} className={inp} placeholder="اسم المستخدم" />
              </div>
              <div>
                <label className="text-sm font-medium text-stone-700 block mb-1.5">البريد الإلكتروني</label>
                <input type="email" value={email} onChange={e => setEmail(e.target.value)} className={inp} placeholder="البريد الإلكتروني" />
                {email !== profile?.email && email && <p className="text-xs text-amber-600 mt-1">سيتم إرسال رابط تأكيد إلى بريدك الجديد</p>}
              </div>
            </div>
            <div className="flex justify-end">
              <button type="submit" disabled={profileLoading} className={btn}>{profileLoading ? "جاري الحفظ..." : "حفظ التغييرات"}</button>
            </div>
          </form>
        </Card>

        <Card title="تغيير كلمة المرور" desc="حافظ على أمان حسابك بكلمة مرور قوية"
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 1 0-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H6.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z"/></svg>}>
          <form onSubmit={submitPassword} className="flex flex-col gap-4">
            <div>
              <label className="text-sm font-medium text-stone-700 block mb-1.5">كلمة المرور الحالية</label>
              <div className="relative">
                <input type={showOld ? "text" : "password"} value={oldPw} onChange={e => setOldPw(e.target.value)} required className={`${inp} pl-10`} placeholder="••••••••" />
                <button type="button" onClick={() => setShowOld(!showOld)} className="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400 hover:text-stone-600"><Eye show={showOld} /></button>
              </div>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium text-stone-700 block mb-1.5">كلمة المرور الجديدة</label>
                <div className="relative">
                  <input type={showNew ? "text" : "password"} value={newPw} onChange={e => setNewPw(e.target.value)} required minLength={6} className={`${inp} pl-10`} placeholder="••••••••" />
                  <button type="button" onClick={() => setShowNew(!showNew)} className="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400 hover:text-stone-600"><Eye show={showNew} /></button>
                </div>
              </div>
              <div>
                <label className="text-sm font-medium text-stone-700 block mb-1.5">تأكيد كلمة المرور</label>
                <div className="relative">
                  <input type={showConf ? "text" : "password"} value={confPw} onChange={e => setConfPw(e.target.value)} required className={`${inp} pl-10`} placeholder="••••••••" />
                  <button type="button" onClick={() => setShowConf(!showConf)} className="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400 hover:text-stone-600"><Eye show={showConf} /></button>
                </div>
                {confPw && newPw !== confPw && <p className="text-xs text-red-500 mt-1">كلمتا المرور غير متطابقتين</p>}
              </div>
            </div>
            <div className="flex justify-end">
              <button type="submit" disabled={pwLoading || (!!confPw && newPw !== confPw)} className={btn}>{pwLoading ? "جاري التغيير..." : "تغيير كلمة المرور"}</button>
            </div>
          </form>
        </Card>

        <Card title="إعادة تفعيل الحساب" desc="إذا كان حسابك معطلاً يمكنك إعادة تفعيله من هنا"
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99"/></svg>}>
          <form onSubmit={submitReactivate} className="flex flex-col gap-4">
            <p className="text-sm text-stone-500">أدخل البريد الإلكتروني المرتبط بالحساب المعطل:</p>
            <div className="flex gap-3">
              <input type="email" value={reactEmail} onChange={e => setReactEmail(e.target.value)} required placeholder="البريد الإلكتروني" className={`flex-1 ${inp}`} />
              <button type="submit" disabled={reactLoading} className="px-6 py-2.5 bg-green-600 text-white rounded-xl text-sm font-medium hover:bg-green-700 disabled:opacity-50 shrink-0">
                {reactLoading ? "جاري الإرسال..." : "إعادة تفعيل"}
              </button>
            </div>
          </form>
        </Card>

        <div className="bg-white rounded-2xl border border-red-200/70 overflow-hidden">
          <div className="px-6 py-4 border-b border-red-100 flex items-center gap-3">
            <div className="w-9 h-9 rounded-xl bg-red-500 flex items-center justify-center text-white shrink-0">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126ZM12 15.75h.007v.008H12v-.008Z"/></svg>
            </div>
            <div><h2 className="font-bold text-red-700 text-base">منطقة الخطر</h2><p className="text-xs text-red-400">الإجراءات التالية لا يمكن التراجع عنها بسهولة</p></div>
          </div>
          <div className="p-6 flex items-center justify-between gap-4">
            <div>
              <h3 className="font-semibold text-stone-800 text-sm">تعطيل الحساب</h3>
              <p className="text-xs text-stone-500 mt-1">سيتم تعطيل حسابك مؤقتاً. يمكنك إعادة تفعيله في أي وقت.</p>
            </div>
            <button onClick={() => setShowDeactivate(true)} className="shrink-0 px-5 py-2.5 border border-red-300 text-red-600 rounded-xl text-sm font-medium hover:bg-red-50 transition-colors">
              تعطيل الحساب
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
