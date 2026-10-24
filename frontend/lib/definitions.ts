import { z } from "zod";

export const LoginFormSchema = z.object({
  email: z
    .email({ message: "الرجاء إدخال بريد إلكتروني صحيح" })
    .min(1, { message: "البريد الإلكتروني مطلوب" })
    .trim(),
  password: z
    .string()
    .min(1, { message: "كلمة المرور مطلوبة" })
    .trim(),
});

export const SignupFormSchema = z
  .object({
    username: z
      .string()
      .min(2, { message: "لا يمكن لاسم المستخدم أن يكون أقل من حرفين" })
      .max(50, { message: "اسم المستخدم لا يمكن أن يتجاوز 50 حرفاً" })
      .trim(),
    email: z
      .email({ message: "الرجاء إدخال بريد إلكتروني صحيح" }).min(1, { message: "البريد الإلكتروني مطلوب" })
      .trim(),
    password: z
      .string()
      .min(6, { message: "يجب أن تكون كلمة المرور 6 أحرف على الأقل" })
      .max(100, { message: "كلمة المرور لا يمكن أن تتجاوز 100 حرفاً" })
      .trim(),
    confirmPassword: z
      .string()
      .min(1, { message: "تأكيد كلمة المرور مطلوب" })
      .trim(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "كلمة المرور وتأكيد كلمة المرور غير متطابقين",
    path: ["confirmPassword"],
  });

export type FormState = | {
  errors?: {
    username?: string[];
    email?: string[];
    password?: string[];
    confirmPassword?: string[];
    root?: string[];
    [key: string]: string[] | undefined;
  };
  message?: string;
  success?: boolean;
}
  | undefined;

export interface SessionPayload {
  userId: string;
  userName?: string;
  email?: string;
  role?: string;
  avatar?: string;
  expiresAt?: Date;
}

export interface SessionUser {
  userId: string;
  userName: string;
  email: string;
  role: string;
  avatar?: string;
}

export interface TokenResponse {
  roleName: string;
  accessToken: string;
  accessTokenExpiresInSec: number;
  refreshToken: string;
  refreshTokenExpiresDateTime: string;
}
