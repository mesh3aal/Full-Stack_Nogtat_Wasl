"use client";

import { useActionState } from "react";
import { useSearchParams } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { login } from "@/lib/actions";

export function LoginForm({ ...props }: React.ComponentProps<typeof Card>) {
  const [state, action, isPending] = useActionState(login, undefined);
  const searchParams = useSearchParams();
  const isRegistered = searchParams.get("registered") === "true";

  return (
    <Card className="bg-black text-white border-stone-800" dir="rtl" {...props}>
      <CardHeader>
        <CardTitle className="text-center text-2xl font-bold">
          أهلاً بك من جديد
        </CardTitle>
        <CardDescription className="text-center text-stone-400">
          أدخل معلوماتك بالأسفل لتسجيل الدخول إلى حسابك
        </CardDescription>
      </CardHeader>

      <CardContent>
        {isRegistered && (
          <div
            role="status"
            className="mb-4 p-3 rounded-lg bg-emerald-950/60 border border-emerald-500/30 text-emerald-300 text-sm text-center"
          >
            تم إنشاء حسابك بنجاح! يمكنك الآن تسجيل الدخول.
          </div>
        )}

        {state?.message && (
          <div
            role="alert"
            className="mb-4 p-3 rounded-lg bg-red-950/60 border border-red-500/30 text-red-400 text-sm text-center"
          >
            {state.message}
          </div>
        )}

        <form action={action} noValidate>
          <FieldGroup>
            {/* البريد الإلكتروني */}
            <Field>
              <FieldLabel htmlFor="email" className="text-stone-300">
                البريد الإلكتروني
              </FieldLabel>
              <Input
                id="email"
                name="email"
                type="email"
                placeholder="name@example.com"
                aria-invalid={!!state?.errors?.email}
                className="bg-stone-900 border-stone-700 text-white placeholder:text-stone-500 focus-visible:ring-primary"
                required
              />
              {state?.errors?.email && (
                <p role="alert" className="text-sm text-red-400 mt-1">
                  {state.errors.email[0]}
                </p>
              )}
            </Field>

            {/* كلمة المرور */}
            <Field>
              <FieldLabel htmlFor="password" className="text-stone-300">
                كلمة المرور
              </FieldLabel>
              <Input
                id="password"
                name="password"
                type="password"
                placeholder="••••••••"
                aria-invalid={!!state?.errors?.password}
                className="bg-stone-900 border-stone-700 text-white placeholder:text-stone-500 focus-visible:ring-primary"
                required
              />
              {state?.errors?.password && (
                <p role="alert" className="text-sm text-red-400 mt-1">
                  {state.errors.password[0]}
                </p>
              )}
            </Field>

            {/* زر تسجيل الدخول */}
            <Field className="mt-2">
              <Button
                type="submit"
                disabled={isPending}
                className="w-full bg-primary hover:bg-primary/90 text-primary-foreground font-semibold py-2.5 transition-all duration-200"
              >
                {isPending ? "جاري تسجيل الدخول..." : "تسجيل الدخول"}
              </Button>

              <FieldDescription className="px-6 text-center mt-4 text-stone-400">
                لا تمتلك حساباً؟{" "}
                <Link
                  href="/register"
                  className="text-white underline underline-offset-4 hover:text-stone-200"
                >
                  قم بالتسجيل الآن
                </Link>
              </FieldDescription>
            </Field>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  );
}
