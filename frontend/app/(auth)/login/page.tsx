import { Suspense } from "react";
import { LoginForm } from "@/app/(auth)/login/login-form";

export default function Login() {
  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10 bg-primary">
      <div className="w-full max-w-sm">
        <Suspense fallback={<div className="text-white text-center">جاري التحميل...</div>}>
          <LoginForm />
        </Suspense>
      </div>
    </div>
  );
}
