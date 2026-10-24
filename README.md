# 📍 نقطة وصل | Nogtat Wasl
### *منصة تقييم وتصنيف الأماكن والأنشطة المحلية المتكاملة*
#### *Full-Stack Local Places Ranking & Evaluation Platform*

<div align="center">

![Next.js 16](https://img.shields.io/badge/Next.js_16-black?style=for-the-badge&logo=next.js&logoColor=white)
![ASP.NET Core 10](https://img.shields.io/badge/ASP.NET_Core_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Tailwind CSS v4](https://img.shields.io/badge/Tailwind_CSS_v4-06B6D4?style=for-the-badge&logo=tailwindcss&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![jose JWT](https://img.shields.io/badge/jose_JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=for-the-badge&logo=typescript&logoColor=white)
![C#](https://img.shields.io/badge/C%23_14-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

<br/>

**«نقطة وصل»** هي منصة متكاملة (Full-Stack) متطورة لتقييم واكتشاف الأماكن والأنشطة التجارية المحلية بدقة وشفافية، مبنية وفق أحدث المعايير البرمجية مع التركيز على الأداء الفائق وتجربة مستخدم عصرية وأعلى مستويات الأمان السيبراني.

[🎥 استعراض الفيديو (IFrame)](#-استعراض-المنصة-video-demo) • [🛡️ المعمارية الأمنية الفائقة](#-المعمارية-الأمنية-الفائقة-high-security-architecture) • [🖼️ معرض الصور الأصلية](#-معرض-شاشات-المنصة-screenshots-gallery) • [⚙️ التقنيات المستخدمة](#-التقنيات-المستخدمة-tech-stack) • [🚀 التشغيل والتثبيت](#-دليل-التشغيل-والتثبيت-getting-started)

</div>

---

## 🎥 استعراض المنصة (Video Demo)

استعراض تفاعلي كامل لواجهات المنصة وتجربة المستخدم ونظام المصادقة وإدارة الأماكن:

<div align="center">

<!-- تضمين مشغل اليوتيوب المباشر عبر IFrame -->
<iframe 
  width="100%" 
  height="480" 
  src="https://www.youtube.com/embed/lQR5xYPMab8?si=NogtatWaslDemo" 
  title="استعراض منصة نقطة وصل - Nogtat Wasl Demo" 
  frameborder="0" 
  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" 
  referrerpolicy="strict-origin-when-cross-origin" 
  allowfullscreen
  style="border-radius: 14px; max-width: 900px; box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);">
</iframe>

<br/><br/>

[![مشاهدة فيديو استعراض نقطة وصل](https://img.youtube.com/vi/lQR5xYPMab8/maxresdefault.jpg)](https://youtu.be/lQR5xYPMab8)

👉 **[اضغط هنا لمشاهدة الفيديو مباشرة على YouTube بدقة فائقة](https://youtu.be/lQR5xYPMab8)** 👈

</div>

---

## 🛡️ المعمارية الأمنية الفائقة (High-Security Architecture)

تم تصميم النظام الأمني في **نقطة وصل** ليحقق أعلى معايير الحماية المتوافقة مع بنية الويب الحديثة، من خلال الدمج الذكي بين **ASP.NET Core 10 Identity** و **Next.js 16** بمكتبة **`jose`**:

```mermaid
sequenceDiagram
    autonumber
    actor User as 👤 العميل / المستخدم
    participant NextJS as ⚡ Next.js 16 (BFF / Server Action)
    participant DotNet as 🛡️ ASP.NET Core 10 WebAPI
    participant DB as 🗄️ PostgreSQL

    User->>NextJS: تسجيل الدخول (Username / Password)
    NextJS->>DotNet: إرسال بيانات الاعتماد عبر API داخلي آمن
    DotNet->>DB: التحقق عبر ASP.NET Core Identity & Hasher
    DB-->>DotNet: بيانات الحساب والأدوار (Roles & Claims)
    DotNet->>DotNet: توليد وتوقيع رمز JWT مشفر ومحدد الصلاحية
    DotNet-->>NextJS: إرجاع JWT Token
    NextJS->>NextJS: فك وتدقيق الـ JWT عبر مكتبة jose على السيرفر
    NextJS->>User: تخزين التوكن في كوكي HttpOnly + Secure + SameSite=Lax
    Note over User,NextJS: 🔒 التوكن محمي تماماً ولا يمكن لأي كود JavaScript في المتصفح الوصول إليه (الحماية التامة من XSS)
    User->>NextJS: طلب صفحة محمية (Dashboard / Profile)
    NextJS->>NextJS: Middleware / Proxy يتحقق من التوكن عبر jose
    NextJS->>DotNet: طلب البيانات مع تمرير Authorization: Bearer Header
    DotNet-->>NextJS: البيانات المعتمدة
    NextJS-->>User: الصفحة مع معالجة SSR السريعة
```

### 🔑 ركائز الأمان المطبقة:
1. **إصدار التوكن والتحقق من الهوية (Identity & JWT Issuance):**
   - خادم **ASP.NET Core 10** يتولى إدارة المستخدمين وتشفير كلمات المرور عبر **ASP.NET Core Identity**.
   - إصدار رموز **JWT (JSON Web Token)** موقعة رقمياً ومحمية بمفتاح سري مشفر، محملة ببيانات الهوية والأدوار (`Admin`, `Owner`, `Customer`).
2. **عزل التوكن عن المتصفح (HttpOnly Cookie Protection):**
   - يقوم خادم **Next.js 16** (عبر الـ Server Actions) باستقبال التوكن وتخزينه كـ **`HttpOnly` Cookie** مع تفعيل خواص `Secure` و `SameSite=Lax`.
   - هذا النهج يمنع نهائياً هجمات **XSS (Cross-Site Scripting)**، حيث يستحيل لأي سكربت خبيث قراءة رمز التوكن أو سرقته من المتصفح.
3. **التحقق والتفكيك عبر مكتبة `jose`:**
   - استخدام مكتبة **`jose`** خفيفة الوزن والمتوافقة مع Edge Runtime للتحقق اللحظي من توقيع وصلاحية التوكن وقراءة الـ Claims على مستوى خادم Next.js و Middleware دون أي تأخير في الأداء.
4. **حماية مسارات الـ Proxy و الـ Middleware:**
   - فحص وتوجيه تلقائي للمسارات العامة والمحمية قبل تحميل أي مكونات حساسة.

---

## 🖼️ معرض شاشات المنصة (Screenshots Gallery)

> *جميع الصور التالية مأخوذة مباشرة من مجلد `gallery/` المحلي بأعلى دقة ووضوح ودون أي ضغط خارجي.*

<div align="center">

### 🌟 الصفحة الرئيسية واستكشاف الأماكن المميزة
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-22%20190529.png" width="49%" alt="الصفحة الرئيسية - 1" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-22%20192317.png" width="49%" alt="الصفحة الرئيسية - 2" />
</p>

### 🔍 التصنيفات والأماكن الأعلى تقييماً
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20163319.png" width="49%" alt="تصنيفات الأماكن" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20163330.png" width="49%" alt="تصفية ونتائج البحث" />
</p>

### 🏬 تفاصيل المكان ومعرض الصور التفاعلي
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20170551.png" width="49%" alt="تفاصيل المكان" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20213902.png" width="49%" alt="معرض صور المتجر" />
</p>

### ⭐ نظام التقييمات والمراجعات التفاعلية
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20223956.png" width="49%" alt="قسم التقييمات والمراجعات" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224532.png" width="49%" alt="إضافة تقييم جديد" />
</p>

### 🔐 بوابات تسجيل الدخول وإنشاء الحسابات
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224550.png" width="49%" alt="شاشة تسجيل الدخول" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224559.png" width="49%" alt="شاشة إنشاء الحساب" />
</p>

### 👤 الحساب الشخصي وإدارة الملف التعريفي
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224608.png" width="49%" alt="الملف الشخصي" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224617.png" width="49%" alt="تعديل بيانات الحساب" />
</p>

### 📊 لوحة تحكم أصحاب الأعمال (Owner Dashboard)
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224636.png" width="49%" alt="لوحة التحكم" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224654.png" width="49%" alt="إدارة الأماكن والخدمات" />
</p>

### 🛠️ إضافة وإدارة الفروع والتفاصيل
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224659.png" width="49%" alt="إضافة مكان جديد" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224724.png" width="49%" alt="تعديل تفاصيل الموقع" />
</p>

### 📱 تجربة المستخدم والتجاوب الكامل (Responsive Mobile UI)
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224730.png" width="49%" alt="واجهة الهاتف - 1" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224734.png" width="49%" alt="واجهة الهاتف - 2" />
</p>

### ⚙️ إدارة الإعدادات والتحكم الموسع
<p align="center">
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224745.png" width="49%" alt="إعدادات المنصة" />
  <img src="gallery/%D9%84%D9%82%D8%B7%D8%A9%20%D8%B4%D8%A7%D8%B4%D8%A9%202026-10-24%20224751.png" width="49%" alt="قائمة المراجعات وإدارتها" />
</p>

</div>

---

## ⚙️ التقنيات المستخدمة (Tech Stack)

### 🎨 الواجهة الأمامية (Frontend)
- **Framework:** [Next.js 16](https://nextjs.org/) (App Router, Server Actions, Server Components & React 19).
- **Styling:** [Tailwind CSS v4](https://tailwindcss.com/) مع دعم الوضع الليلي والمتجاوب بالكامل وتصميم عربي RTL أنيق.
- **Components:** [shadcn/ui](https://ui.shadcn.com/) المبنية على Radix و Base-UI.
- **Icons:** Lucide Icons & HugeIcons.
- **State & Data Fetching:** [TanStack React Query v5](https://tanstack.com/query/v5) و Server Actions المدمجة.
- **Validation & Forms:** React Hook Form + Zod.
- **Security & JWT:** [jose](https://github.com/panva/jose) للتحقق وفك التشفير الآمن للتوكنات في الخادم.

### 🛡️ الواجهة الخلفية (Backend)
- **Platform:** [ASP.NET Core 10](https://dotnet.microsoft.com/) WebAPI.
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, WebAPI).
- **Security & Identity:** ASP.NET Core Identity + JWT Bearer Authentication.
- **ORM & Database:** Entity Framework Core 10 مع قاعدة بيانات **PostgreSQL** (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- **API Documentation:** OpenAPI / Scalar Interactive API Documentation.
- **Containerization:** Docker & Docker Compose.

---

## ✨ المميزات الرئيسية للمنصة (Key Features)

- 🔍 **استكشاف وتصفح ذكي:** استعراض الأماكن حسب التصنيف، المدينة، الأعلى تقييماً، والأحدث إضافة.
- ⭐ **نظام مراجعات وتقييمات حقيقي:** كتابة تقييمات تفاعلية، إضافة صور للتجربة، واحتساب متوسط التقييم بدقة.
- 🏢 **لوحة مخصصة لأصحاب الأنشطة التجارية (Store Owners):**
  - إضافة الأماكن وتحديث بيانات التواصل والموقع الجغرافي وساعات العمل.
  - رفع وإدارة معارض الصور التفاعلية.
  - مراقبة آراء الزبائن والرد على الاستفسارات.
- 👮 **إدارة وصلاحيات متقدمة (Role-Based Access Control):**
  - دعم الأدوار: مستخدم عادي (`Customer`)، صاحب عمل (`Owner`)، ومدير نظام (`Admin`).
- ⚡ **أداء خيالي وسرعة استجابة:** بفضل معمارية Next.js 16 Server Components والتوليد المتزامن للواجهات.
- 📱 **واجهة مستخدم متجاوبة وشاملة:** تجربة سلسة وتفاعلية على الشاشات الكبيرة والمكتبية والهواتف الذكية.

---

## 📂 هيكلية المشروع (Project Structure)

```text
Local-Places-Ranking-and-Evaluation-API/
├── Application/                 # طبقة منطق الأعمال والوسائط (CQRS / Services / Interfaces)
├── Core/                        # طبقة النطاق والكيانات الأساسية (Entities / Domain Models)
├── infrastructural/            # طبقة البنية التحتية والوصول للبيانات (EF Core DbContext / Repositories)
├── WebAPI/                      # طبقة الواجهات البرمجية وتكوين الخدمات (Controllers / Endpoints)
│   ├── Controllers/             # وحدات التحكم (Admin, Owner, Customer, Reviews...)
│   ├── ApiDependencyInjection.cs# ربط خدمات الـ JWT و Identity و Database
│   └── Program.cs               # نقطة انطلاق ASP.NET Core 10
├── frontend/                    # تطبيق الواجهة الأمامية (Next.js 16)
│   ├── app/                     # مسارات App Router (الرئيسية، الأماكن، التقييمات، المصادقة)
│   │   ├── (app)/               # الصفحات العامة وتفاصيل الأماكن
│   │   ├── (auth)/              # تسجيل الدخول وإنشاء الحساب
│   │   └── api/                 # مسارات الـ API Proxy الداخلية
│   ├── lib/                     # مكتبات الأمان، إدارة الجلسات (jwt.ts, session.ts, actions.ts)
│   ├── components/              # مكونات الواجهة التفاعلية و shadcn/ui
│   └── proxy.ts                 # معالج الوسيط والتحقق من التوكن
├── gallery/                     # مجلد لقطات الشاشة الأصلية بجودة فائقة
├── compose.yaml                 # إعدادات Docker Compose لقواعد البيانات
└── README.md                    # التوثيق الشامل للمشروع
```

---

## 🚀 دليل التشغيل والتثبيت (Getting Started)

### 1. المتطلبات المسبقة (Prerequisites)
- تثبيت [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- تثبيت [Node.js 20+](https://nodejs.org/) و [pnpm](https://pnpm.io/).
- تثبيت خادم [PostgreSQL](https://www.postgresql.org/) أو تشغيل [Docker Desktop](https://www.docker.com/).

### 2. إعداد قاعدة البيانات والواجهة الخلفية (Backend)
```bash
# الانتقال لمجلد المشروع الرئيسي
cd Local-Places-Ranking-and-Evaluation-API

# تشغيل حاوية قاعدة البيانات (اختياري عبر Docker)
docker compose up -d

# الانتقال إلى مشروع الـ API
cd WebAPI

# تحديث قاعدة البيانات بتطبيق المايجريشن
dotnet ef database update --project ../infrastructural

# تشغيل خادم الـ API
dotnet run
```
*سيعمل خادم الـ API تلقائياً على `https://localhost:7000` أو `http://localhost:5000` مع توثيق Scalar API على مسار `/scalar/v1`.*

### 3. إعداد الواجهة الأمامية (Frontend)
```bash
# الانتقال إلى مجلد الواجهة الأمامية
cd ../frontend

# تثبيت الحزم والاعتماديات
pnpm install

# إعداد المتغيرات البيئية (.env.local)
cp .env.example .env.local
```

تأكد من ضبط المتغيرات البيئية في ملف `.env.local`:
```env
NEXT_PUBLIC_API_URL=https://localhost:7000/api
ASP_NET_JWT_SECRET=YourSuperStrongSecureKeyMatchingAspNetCoreSettings32Chars
ASP_NET_JWT_ISSUER=YourIssuerName
ASP_NET_JWT_AUDIENCE=YourAudienceName
```

ثم ابدأ خادم التطوير:
```bash
pnpm dev
```
*افتح المتصفح وتوجه إلى [http://localhost:3000](http://localhost:3000) لتجربة المنصة.*

---

## 👨‍💻 المطور (Author)

تم بناء وتطوير المشروع بواسطة:
- **مشعل جمال (Meshaal Jamal)**
- GitHub: [@mesh3aal](https://github.com/mesh3aal)
- البريد الإلكتروني: [`meshaal.j@hotmail.com`](mailto:meshaal.j@hotmail.com)

---

<div align="center">
⭐ إذا نال المشروع إعجابك، لا تنسَ ترك نجمة على المستودع لدعم استمرار التطوير! ⭐
</div>
