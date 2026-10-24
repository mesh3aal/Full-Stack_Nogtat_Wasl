import { SignupForm } from "@/app/(auth)/register/signup-form";

export default function Register() {
  return (
      <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10 bg-primary">
        <div className="w-full max-w-sm">
          <SignupForm />
        </div>
      </div>
    );
}
