"use client";

import { useActionState } from "react";
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
import { signup } from "@/lib/actions";

export function SignupForm({ ...props }: React.ComponentProps<typeof Card>) {
  const [state, action, isPending] = useActionState(signup, undefined);

  return (
    <Card className="bg-black text-white border-stone-800" dir="rtl" {...props}>
      <CardHeader>
        <CardTitle className="text-center text-2xl font-bold">
          إنشاء حساب جديد
        </CardTitle>
        <CardDescription className="text-center text-stone-400">
          أدخل بياناتك بالأسفل للبدء في استخدام التطبيق
        </CardDescription>
      </CardHeader>

      <CardContent>
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
            {/* اسم المستخدم */}
            <Field>
              <FieldLabel htmlFor="username" className="text-stone-300">
                اسم المستخدم
              </FieldLabel>
              <Input
                id="username"
                name="username"
                type="text"
                placeholder="أحمد محمد"
                aria-invalid={!!state?.errors?.username}
                className="bg-stone-900 border-stone-700 text-white placeholder:text-stone-500 focus-visible:ring-primary"
                required
              />
              {state?.errors?.username && (
                <p role="alert" className="text-sm text-red-400 mt-1">
                  {state.errors.username[0]}
                </p>
              )}
            </Field>

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

            {/* تأكيد كلمة المرور */}
            <Field>
              <FieldLabel htmlFor="confirmPassword" className="text-stone-300">
                تأكيد كلمة المرور
              </FieldLabel>
              <Input
                id="confirmPassword"
                name="confirmPassword"
                type="password"
                placeholder="••••••••"
                aria-invalid={!!state?.errors?.confirmPassword}
                className="bg-stone-900 border-stone-700 text-white placeholder:text-stone-500 focus-visible:ring-primary"
                required
              />
              {state?.errors?.confirmPassword && (
                <p role="alert" className="text-sm text-red-400 mt-1">
                  {state.errors.confirmPassword[0]}
                </p>
              )}
            </Field>

            {/* زر إنشاء الحساب */}
            <Field className="mt-2">
              <Button
                type="submit"
                disabled={isPending}
                className="w-full bg-primary hover:bg-primary/90 text-primary-foreground font-semibold py-2.5 transition-all duration-200"
              >
                {isPending ? "جاري إنشاء الحساب..." : "إنشاء حساب"}
              </Button>

              <FieldDescription className="px-6 text-center mt-4 text-stone-400">
                تمتلك حساباً بالفعل؟{" "}
                <Link
                  href="/login"
                  className="text-white underline underline-offset-4 hover:text-stone-200"
                >
                  تسجيل الدخول
                </Link>
              </FieldDescription>
            </Field>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  );
}
