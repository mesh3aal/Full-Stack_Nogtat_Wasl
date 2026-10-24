namespace Persistence_Layer.Persistence.SeedClasses;

public static class DatabaseSeed
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context, UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        Role[] rolesToAdd =
        [
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN",
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Owner",
                NormalizedName = "OWNER",
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                NormalizedName = "USER",
            }
        ];
        foreach (var role in rolesToAdd)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }

        await context.SaveChangesAsync();

        // قاموس لربط الفئة الرئيسية برابط التنقل (Source / href) في واجهة React
        var categorySources = new Dictionary<string, string>
        {
            ["الرياضة واللياقة"] = "/category/sports",
            ["شركات ومؤسسات"] = "/category/companies",
            ["حدائق وأماكن مفتوحة"] = "/category/parks",
            ["مساجد ودور عبادة"] = "/category/mosques",
            ["مرافق حكومية وعامة"] = "/category/government",
            ["نقل وسفر"] = "/category/travel",
            ["تعليم"] = "/category/education",
            ["ترفيه وفنون"] = "/category/entertainment",
            ["خدمات عامة"] = "/category/services",
            ["تسوق وتجزئة"] = "/category/shopping",
            ["خدمات طبية وصحية"] = "/category/hospitals",
            ["فنادق وإقامة"] = "/category/hotels",
            ["مطاعم"] = "/category/restaurants",
            ["مطاعم وكافيهات"] = "/category/restaurants"
        };

        if (!context.Categories.Any())
        {
            // قاموس لربط الفئة الرئيسية بأيقونتها المناسبة من الملف
            var categoryIcons = new Dictionary<string, string>
            {
                ["الرياضة واللياقة"] = "Dumbbell",
                ["شركات ومؤسسات"] = "Building2",
                ["حدائق وأماكن مفتوحة"] = "Trees",
                ["مساجد ودور عبادة"] = "Landmark",
                ["مرافق حكومية وعامة"] = "Landmark",
                ["نقل وسفر"] = "Plane",
                ["تعليم"] = "GraduationCap",
                ["ترفيه وفنون"] = "Palette",
                ["خدمات عامة"] = "Wrench",
                ["تسوق وتجزئة"] = "ShoppingBag",
                ["خدمات طبية وصحية"] = "Stethoscope",
                ["فنادق وإقامة"] = "Hotel",
                ["مطاعم"] = "UtensilsCrossed",
                ["مطاعم وكافيهات"] = "UtensilsCrossed"
            };

            foreach (var placeName in categoryIcons)
            {
                if (context.Categories.Any(x => x.CategoryName == placeName.Key))
                {
                    continue;
                }

                categoryIcons.TryGetValue(placeName.Key, out var icon);
                categorySources.TryGetValue(placeName.Key, out var source);

                var category = new Category
                {
                    CategoryName = placeName.Key,
                    CategoryIcon = icon ?? "Store", // أيقونة افتراضية في حال لم يجد مطابقة
                    Source = source ?? ""
                };

                await context.Categories.AddAsync(category);
            }

            await context.SaveChangesAsync();
        }
        else
        {
            // تحديث الـ Source في حال كانت الفئات موجودة مسبقاً بقيم فارغة
            var existingCategories = await context.Categories.ToListAsync();
            bool hasChanges = false;
            foreach (var cat in existingCategories)
            {
                if (string.IsNullOrEmpty(cat.Source) && categorySources.TryGetValue(cat.CategoryName, out var source))
                {
                    cat.Source = source;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync();
            }
        }

        // sub-categories seed
        string[][] categoriesWithSubcategories = new string[][]
        {
            ["ملاعب كرة قدم", "صالات رياضية (Gym)", "مسابح", "مراكز فنون قتالية", "مضامير جري"],
            ["شركات تقنية", "بنوك ومصارف", "مصانع", "شركات استشارات", "مكاتب إدارية"],
            ["حدائق عامة", "حدائق نباتية", "شواطئ", "مسارات مشي جبلية", "كورنيش"],
            ["مساجد", "كنائس", "معابد", "مصليات", "مقابر"],
            ["بلديات", "مراكز شرطة", "مراكز دفاع مدني", "مكاتب بريد", "مكتبات عامة"],
            ["مطارات", "محطات حافلات", "محطات قطارات", "مكاتب تأجير سيارات", "وكالات سفر"],
            ["مدارس", "جامعات", "مراكز تدريب", "رياض أطفال", "معاهد لغات"],
            ["دور سينما", "معارض فنية", "مدن ملاهي", "مسارح", "متاحف"],
            ["مغاسل سيارات", "مغاسل ملابس", "صالونات حلاقة", "صالونات تجميل", "ورش صيانة"],
            ["مولات تجارية", "سوبر ماركت", "محلات ملابس", "محلات إلكترونيات", "صيدليات"],
            ["مستشفيات", "عيادات", "عيادات أسنان", "مختبرات طبية", "مراكز علاج طبيعي"],
            ["فنادق", "شقق فندقية", "منتجعات", "نُزل", "موتيلات"],
            ["مطاعم وجبات سريعة", "مطاعم عائلية", "كافيهات", "مخابز", "مطاعم شعبية"]
        };

        var subCategoryIcons = new Dictionary<string, string>
{
    // الرياضة واللياقة
    { "ملاعب كرة قدم", "Goal" },
    { "صالات رياضية (Gym)", "Dumbbell" },
    { "مسابح", "Waves" },
    { "مراكز فنون قتالية", "Swords" },
    { "مضامير جري", "Footprints" },

    // شركات ومؤسسات
    { "شركات تقنية", "Laptop" },
    { "بنوك ومصارف", "Landmark" },
    { "مصانع", "Factory" },
    { "شركات استشارات", "BriefcaseBusiness" },
    { "مكاتب إدارية", "Building2" },

    // حدائق وأماكن مفتوحة
    { "حدائق عامة", "Trees" },
    { "حدائق نباتية", "Flower2" },
    { "شواطئ", "Umbrella" },
    { "مسارات مشي جبلية", "Mountain" },
    { "كورنيش", "Waves" },

    // مساجد ودور عبادة
    { "مساجد", "Moon" },
    { "كنائس", "Church" },
    { "معابد", "Landmark" },
    { "مصليات", "Building" },
    { "مقابر", "Flower2" },

    // مرافق حكومية وعامة
    { "بلديات", "Building2" },
    { "مراكز شرطة", "ShieldCheck" },
    { "مراكز دفاع مدني", "Siren" },
    { "مكاتب بريد", "Mail" },
    { "مكتبات عامة", "Library" },

    // نقل وسفر
    { "مطارات", "Plane" },
    { "محطات حافلات", "Bus" },
    { "محطات قطارات", "TrainFront" },
    { "مكاتب تأجير سيارات", "CarFront" },
    { "وكالات سفر", "Luggage" },

    // تعليم
    { "مدارس", "School" },
    { "جامعات", "GraduationCap" },
    { "مراكز تدريب", "Presentation" },
    { "رياض أطفال", "Baby" },
    { "معاهد لغات", "Languages" },

    // ترفيه وفنون
    { "دور سينما", "Clapperboard" },
    { "معارض فنية", "Palette" },
    { "مدن ملاهي", "FerrisWheel" },
    { "مسارح", "Drama" },
    { "متاحف", "Landmark" },

    // خدمات عامة
    { "مغاسل سيارات", "CarFront" },
    { "مغاسل ملابس", "WashingMachine" },
    { "صالونات حلاقة", "Scissors" },
    { "صالونات تجميل", "Sparkles" },
    { "ورش صيانة", "Wrench" },

    // تسوق وتجزئة
    { "مولات تجارية", "Store" },
    { "سوبر ماركت", "ShoppingCart" },
    { "محلات ملابس", "Shirt" },
    { "محلات إلكترونيات", "MonitorSmartphone" },
    { "صيدليات", "Pill" },

    // خدمات طبية وصحية
    { "مستشفيات", "Hospital" },
    { "عيادات", "Stethoscope" },
    { "عيادات أسنان", "Smile" },
    { "مختبرات طبية", "Microscope" },
    { "مراكز علاج طبيعي", "Activity" },

    // فنادق وإقامة
    { "فنادق", "Hotel" },
    { "شقق فندقية", "Building" },
    { "منتجعات", "Palmtree" },
    { "نُزل", "BedDouble" },
    { "موتيلات", "Bed" },

    // مطاعم وكافيهات
    { "مطاعم وجبات سريعة", "Sandwich" },
    { "مطاعم عائلية", "UtensilsCrossed" },
    { "كافيهات", "Coffee" },
    { "مخابز", "Croissant" },
    { "مطاعم شعبية", "CookingPot" },
};

        if (!context.SubCategories.Any())
        {
            for (int i = 0; i < categoriesWithSubcategories.Length; i++)
            {
                string currentCategoryName = i switch
                {
                    0 => "الرياضة واللياقة",
                    1 => "شركات ومؤسسات",
                    2 => "حدائق وأماكن مفتوحة",
                    3 => "مساجد ودور عبادة",
                    4 => "مرافق حكومية وعامة",
                    5 => "نقل وسفر",
                    6 => "تعليم",
                    7 => "ترفيه وفنون",
                    8 => "خدمات عامة",
                    9 => "تسوق وتجزئة",
                    10 => "خدمات طبية وصحية",
                    11 => "فنادق وإقامة",
                    12 => "مطاعم وكافيهات",
                    _ => ""
                };

                var category = context.Categories.Single(x => x.CategoryName == currentCategoryName);

                for (int j = 0; j < categoriesWithSubcategories[i].Length; j++)
                {
                    string subName = categoriesWithSubcategories[i][j];
                    subCategoryIcons.TryGetValue(subName, out var icon);

                    context.SubCategories.Add(new SubCategory
                    {
                        Name = subName,
                        supCategoryIcon = icon ?? "Store", // أيقونة افتراضية في حال لم يجد مطابقة
                        CategoryId = category.CategoryId,
                        Category = category
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}