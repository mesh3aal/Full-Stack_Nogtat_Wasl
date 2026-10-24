namespace Persistence_Layer.Persistence.SeedClasses;

public static class PlacesSeed
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context, UserManager<User> userManager)
    {
        if (!context.Places.Any())
        {
            var owner = userManager.Users.Where(x => x.Role.Name!.ToLower() == "user");
            var user = userManager.Users.Where(x => x.Role.Name!.ToLower() == "owner");
            var request = new Request
            {
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                State = RequestStates.Accepted,
                UserId = owner.First().Id,
            };
            var location = new Location
            {
                Latitude = 5.051651654,
                Longitude = 14.1561651,
                DirectorateId = 1,
                DistrictId = 4,
                NearestLandmark = "جامعة حضرموت المساكن",
            };
            var newPlace = new Place
            {
                PlaceName = "بيتزا هاوس",
                Description = "مطعم لتقديم مختلف انواع البيتزا في المكلا",
                OpeningTime = new TimeOnly(8, 00),
                ClosingTime = new TimeOnly(23, 00),
                Email = "qwerty@gmail.com",
                CreatedAt = DateTime.UtcNow.AddYears(-5).AddMonths(-5).AddDays(-27),
                UnavailableAt = [WeekDays.الجمعة],
                Request = request,
                SubCategoryId = 62,
                UserId = owner.First().Id,
                Location = location,
                Rates =
                {
                    new Review
                    {
                        CreatedAt = DateTime.UtcNow.AddHours(-1),
                        User = user.First(), UserId = user.First().Id,
                        Note = "المكان رائع ويستحق الزيارة المتكررة.", RateValue = 5
                    },
                    new Review
                    {
                        CreatedAt = DateTime.UtcNow.AddHours(-2),
                        User = user.First(),
                        UserId = user.ElementAt(2).Id,
                        Note = "جيد يحتاج إلى مراعاة الاسعار و فتح فرع جديد بسبب الزحمة الزادئة", RateValue = 3
                    },
                    new Review
                    {
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        User = user.First(), UserId = user.ElementAt(3).Id,
                        Note = "ممتاز، ولكن ليس الافضل", RateValue = 4
                    }
                },
                CommericalRegisteration = new CommericalRegisteration()
                {
                    CrNumber = "a11b011wecc56t",
                    EntityName = "بيتزا هاوس",
                    ExpirationDate = DateTime.UtcNow.AddYears(5).AddDays(45),
                    ImagePath = "",
                    User = owner.First(),
                    UserId = owner.First().Id,
                },
                Images = new List<PlaceImage>()
                {
                    new()
                    {
                        ImageUrl = "https://localhost:5001/images/places/Pizza'sHouse/unnamed.png",
                    },
                    new()
                    {
                        ImageUrl = "https://localhost:5001/images/places/Pizza'sHouse/pizza.png",
                    }
                },
                Phones = { new PhoneNumber() { Number = "000000000" } },
            };
            context.Places.Add(newPlace);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedFakeData(ApplicationDbContext context, UserManager<User> userManager)
    {
        if (await context.Places.CountAsync() <= 1)
        {
            var ownersList = userManager.Users.Where(x => x.Role.Name!.ToLower() == "user").ToList();
            var usersList = userManager.Users.Where(x => x.Role.Name!.ToLower() == "owner").ToList();

            var defaultOwner = ownersList.FirstOrDefault();
            var defaultUser1 = usersList.ElementAtOrDefault(0);
            var defaultUser2 = usersList.ElementAtOrDefault(1) ?? defaultUser1;
            var defaultUser3 = usersList.ElementAtOrDefault(2) ?? defaultUser1;

            if (defaultOwner == null || defaultUser1 == null)
                return;

            var directorates = await context.Directorates.ToListAsync();
            var districts = await context.Districts.ToListAsync();

            var getGeoIds = new Func<string, (int dirId, int distId)>((address) =>
            {
                int dirId = directorates.FirstOrDefault()?.Id ?? 1;
                int distId = districts.FirstOrDefault()?.Id ?? 1;

                if (address.Contains("المكلا"))
                {
                    var dir = directorates.FirstOrDefault(d => d.Name == "المكلا");
                    if (dir != null)
                    {
                        dirId = dir.Id;
                        var dist = districts.FirstOrDefault(d => d.DirectorateId == dir.Id);
                        if (dist != null) distId = dist.Id;
                    }
                }
                if (address.Contains("الديس"))
                {
                    var dir = directorates.FirstOrDefault(d => d.Name == "المكلا");
                    if (dir != null)
                    {
                        dirId = dir.Id;
                        var dist = districts.FirstOrDefault(d => d.Name == "الديس" && d.DirectorateId == dir.Id);
                        if (dist != null) distId = dist.Id;
                    }
                }
                else if (address.Contains("فوة") || address.Contains("فوه"))
                {
                    var dir = directorates.FirstOrDefault(d => d.Name == "المكلا");
                    if (dir != null)
                    {
                        dirId = dir.Id;
                        var dist = districts.FirstOrDefault(d => (d.Name == "فوه" || d.Name == "فوه القديمة") && d.DirectorateId == dir.Id);
                        if (dist != null) distId = dist.Id;
                    }
                }
                else if (address.Contains("الشحر"))
                {
                    var dir = directorates.FirstOrDefault(d => d.Name == "الشحر");
                    if (dir != null)
                    {
                        dirId = dir.Id;
                        var dist = districts.FirstOrDefault(d => d.DirectorateId == dir.Id);
                        if (dist != null) distId = dist.Id;
                    }
                }
                else if (address.Contains("قصيعر") || address.Contains("الريدة"))
                {
                    var dir = directorates.FirstOrDefault(d => d.Name == "الريدة وقصيعر");
                    if (dir != null)
                    {
                        dirId = dir.Id;
                        var dist = districts.FirstOrDefault(d => d.DirectorateId == dir.Id);
                        if (dist != null) distId = dist.Id;
                    }
                }

                return (dirId, distId);
            });

            var subCategories = await context.SubCategories.ToListAsync();
            var getSubCategoryId = new Func<string, int>((catName) =>
            {
                if (string.IsNullOrWhiteSpace(catName)) return 62;
                var sub = subCategories.FirstOrDefault(s => s.Name == catName);
                if (sub != null) return sub.SubCategoryId;

                if (catName.Contains("إلكترون") || catName.Contains("كمبيوتر") || catName.Contains("حاسب") || catName.Contains("هواتف") || catName.Contains("اتصالات") || catName.Contains("PC"))
                    return subCategories.FirstOrDefault(s => s.Name == "محلات إلكترونيات" || s.Name == "شركات تقنية")?.SubCategoryId ?? 62;

                if (catName.Contains("صيانة") || catName.Contains("ورشة") || catName.Contains("سيارات"))
                    return subCategories.FirstOrDefault(s => s.Name == "ورش صيانة")?.SubCategoryId ?? 62;

                if (catName.Contains("سوبر") || catName.Contains("ماركت") || catName.Contains("تموينات") || catName.Contains("بقالة"))
                    return subCategories.FirstOrDefault(s => s.Name == "سوبر ماركت")?.SubCategoryId ?? 62;

                if (catName.Contains("كافيه") || catName.Contains("مقهى") || catName.Contains("قهوة") || catName.Contains("محمصة"))
                    return subCategories.FirstOrDefault(s => s.Name == "كافيهات")?.SubCategoryId ?? 62;

                if (catName.Contains("مخبز") || catName.Contains("بيكري"))
                    return subCategories.FirstOrDefault(s => s.Name == "مخابز")?.SubCategoryId ?? 62;

                if (catName.Contains("مطعم"))
                    return subCategories.FirstOrDefault(s => s.Name == "مطاعم عائلية" || s.Name == "مطاعم شعبية" || s.Name == "مطاعم وجبات سريعة")?.SubCategoryId ?? 62;

                return subCategories.FirstOrDefault()?.SubCategoryId ?? 62;
            });

            var allReviewers = usersList.Concat(ownersList).ToList();
            if (!allReviewers.Any())
            {
                allReviewers = await context.Users.ToListAsync();
            }
            var reviewUserIndex = 0;

            var getNotesForRate = new Func<sbyte, string>(rate => rate switch
            {
                5 => "مكان ممتاز وخدمة راقية جداً وجودة عالية، أنصح بالتعامل معه.",
                4 => "تجربة جيدة جداً والخدمة سريعة والأسعار مناسبة.",
                3 => "المكان جيد والخدمة مقبولة ولكن يحتاج إلى بعض التحسينات.",
                2 => "الخدمة متوسطة وكان هناك تأخير في الاستجابة.",
                _ => "يحتاج إلى تطوير مستوى الخدمة والاهتمام بالعملاء."
            });

            var createReviewsForScore = new Func<double, List<Review>>((score) =>
            {
                List<sbyte> values = score switch
                {
                    >= 4.95 => [5, 5, 5, 5, 5],
                    >= 4.85 => [5, 5, 5, 5, 5, 5, 5, 5, 5, 4], // 49 / 10 = 4.9
                    >= 4.75 => [5, 5, 5, 5, 4],                 // 24 / 5  = 4.8
                    >= 4.65 => [5, 5, 5, 5, 5, 5, 5, 4, 4, 4], // 47 / 10 = 4.7
                    >= 4.55 => [5, 5, 5, 4, 4],                 // 23 / 5  = 4.6
                    >= 4.45 => [5, 5, 5, 4, 4, 4],              // 27 / 6  = 4.5
                    >= 4.35 => [5, 5, 5, 4, 3],                 // 22 / 5  = 4.4
                    >= 4.25 => [5, 5, 5, 5, 4, 4, 4, 4, 4, 3], // 43 / 10 = 4.3
                    >= 4.15 => [5, 5, 4, 4, 3],                 // 21 / 5  = 4.2
                    >= 3.85 => [5, 4, 4, 4, 4, 4, 4, 4, 4, 2], // 39 / 10 = 3.9
                    >= 2.45 and <= 2.55 => [3, 3, 2, 2],         // 10 / 4  = 2.5
                    _ => [(sbyte)Math.Clamp((int)Math.Round(score), 1, 5)]
                };

                var list = new List<Review>();
                for (int i = 0; i < values.Count; i++)
                {
                    var reviewer = allReviewers.Any() ? allReviewers[(reviewUserIndex++) % allReviewers.Count] : defaultUser1;
                    list.Add(new Review
                    {
                        RateValue = values[i],
                        Note = getNotesForRate(values[i]),
                        CreatedAt = DateTime.UtcNow.AddDays(-((i + 1) * 3)),
                        UserId = reviewer.Id,
                        User = reviewer
                    });
                }
                return list;
            });

            var placesToAdd = new List<Place>
{
    // 1. بارك للحاسب الألي - COMPUTER PARK (Rating: 4.8)
    new Place
    {
        PlaceName = "بارك للحاسب الألي - COMPUTER PARK",
        Description = "بيع أجهزة الكمبيوتر واللاب توب والطابعات والاحبار بجميع أنواعها والأجهزة الالكترونية بشروره\nمحل . صيانة وبرجمة جميع الأجهزة الالكترونية .\nبيع أحبار . تعبئة احبار بيع و صيانة وإصلاح طابعات والكترونيات",
        OpeningTime = new TimeOnly(8, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [WeekDays.الجمعة],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر أجهزة إلكترونية"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 17.4857399,
            Longitude = 47.1154311,
            DirectorateId = getGeoIds("شارع البلديه، الفيصلية، شرورة 68341، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("شارع البلديه، الفيصلية، شرورة 68341، المملكة العربية السعودية").distId,
            NearestLandmark = "شارع البلديه، الفيصلية، شرورة 68341، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.8),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-01",
            EntityName = "بارك للحاسب الألي - COMPUTER PARK",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLfsIogywm0MRAjnlEJzeYJXp0KCfuwzD78fuBDcZgyGUXAUkib4BUZVX-F_InzEc0If0nD9X1B-FCDgHOWRXR64e4bCEq9UlFd_kCUCX6soN81z6AhiKd2pucaqHt_LPYcc_6Wn9hBuM985=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SEro2jwE9cHxONXjsbZhNCXpHuPoZ416E3L422D1rumw2XVomHKWYo9QDp7X7j_8OnJ9rz1ZscTR9AFTAlAnRGeJJAga8RlCIPqEwVMT-EVTwY_NIbSnc1gBRoDayJgjPI3KvqZaGEE4E4=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SiE8CuXJwOlOLsuvEau6mC5u5UwkSC2xTXBmXG1ly_dKJzdUgR6zSytPK2aE9k7ecek3Y4tKYZe7_ReXa_5JrPFCae20gP3hIgwvzAq_Q71rqWF-zs3Rn_iJQUyzt_5_QtYzWe=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0553590955" },
        }
    },

    // 2. شركة بن دلباح للسيارات مركز صيانة تويوتا & لكزس (Rating: 3.9)
    new Place
    {
        PlaceName = "شركة بن دلباح للسيارات مركز صيانة تويوتا & لكزس",
        Description = "صيانة سيارات تويوتا ولكزس",
        OpeningTime = new TimeOnly(9, 0),
        ClosingTime = new TimeOnly(21, 45),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("مركز صيانة سيارات"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 17.4987754,
            Longitude = 47.1022911,
            DirectorateId = getGeoIds("الملك عبدالعزيز، الفهد، شرورة 55164، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("الملك عبدالعزيز، الفهد، شرورة 55164، المملكة العربية السعودية").distId,
            NearestLandmark = "الملك عبدالعزيز، الفهد، شرورة 55164، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(3.9),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-02",
            EntityName = "شركة بن دلباح للسيارات مركز صيانة تويوتا & لكزس",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLdZ-iwN-HhqbkXJXcvTgSBbT42EGJgaTcx0Ql5SY64w97Daa_Ka2Eev4oinZoz1R9TFDuv8LnRUUw7PdVBHQT3pECE6zQLYCn94x_wBC-5TCe-0zukSvAJbi_MYMS95yAZ43mnNvA=w425-h240-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SXaoWmlJgtXfC1peVF9c6kq66p-a8IrhsFUk00Lf09MSyy9RGekzZByaKJXyILPzMyR9S4QB1rj0FXi014slTmTq8_6EfXHdVoiOiNzJs5RzfxHFearHirp2I8EJBqZd-n0jPVLA=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SzoCZeN5Ieo9BLSKQAAukZoWRQooKRsoyyV6cK2K3cn9v0pbuAETUFwpmPIX5cVUwRUhUD997-rkwCPK0Bw2tECAFbrvJove1yk7oQWKwxM0_f5MJGjbBf3Eon5OfqPS3KHm92lgQx0R9D=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0559046225" },
        }
    },

    // 3. ابارت كافيه (Rating: 4.6)
    new Place
    {
        PlaceName = "ابارت كافيه",
        Description = "الجزء المفضل من يومك ✨🧡\nمقرنا في بوليفارد الجامعة تشرفونا",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("مقهى"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.1406637,
            Longitude = 47.2757605,
            DirectorateId = getGeoIds("OKSE8545، 8545 المنذر بن حسان، 4523، حي الجامعة،, الخرج 16272، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("OKSE8545، 8545 المنذر بن حسان، 4523، حي الجامعة،, الخرج 16272، المملكة العربية السعودية").distId,
            NearestLandmark = "OKSE8545، 8545 المنذر بن حسان، 4523، حي الجامعة،, الخرج 16272، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.6),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-03",
            EntityName = "ابارت كافيه",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLedJDAod1EsnMaBaiWaN2PtOWO9cDIAEfsJYUmCqQcBru8zUHs0BZ2yG_lvnviHM5tD8E4tQaeoZHlpgy7f3BwfIfASZSi9UovlyhqHdqmEnm1W6tYcj4eP2S9KqtmRlSGeSi2l=w408-h447-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9R6uLsCMDKCE05j31ItZT3s9WefOgHGlN8-FCRaQXkTCmgIcqjjGbh_QoGqlJs6P9jw6V7qrPfPzR8hUaniTQwdrToXVZV7IYp4TeN2GM7ZxrVtzU9E2-MTslV9Qj6ZnDl7SRb3=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Sxxuix0OEvwXOzjAV4CqV_6vkGJ8SueLO2zg7hIMwSrhJM_G0whnkWoLaPlAtAxXpbLKHwNmTGGfU4wiNskuR2KaYm8dzVadXlpXwdwJEly8m7vTSAjVKa1hWUYTPH_NhC5HXWPg=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0550508361" },
        }
    },

    // 4. كيو (Rating: 4.5)
    new Place
    {
        PlaceName = "كيو",
        Description = "كيو سوبرماركت",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("سوبرماركت"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.7428374,
            Longitude = 46.8256163,
            DirectorateId = getGeoIds("شارع حسان بن ثابت، النسيم الغربي، الرياض 14728، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("شارع حسان بن ثابت، النسيم الغربي، الرياض 14728، المملكة العربية السعودية").distId,
            NearestLandmark = "شارع حسان بن ثابت، النسيم الغربي، الرياض 14728، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.5),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-04",
            EntityName = "كيو",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLevelKlak0AfQLiU9orykSXml_V8a9YG7gZAxc_EnvZGYa5c0xYc-7iUA2Klwt3Xb7u_i_SoRmy88U5tKEPZuUpL6ehqZhIFOf-lR-CRXezU693ogLIP7T3__yaJxzZzsl3Kzl2VjXZzXtX=w408-h544-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RAi4hB1H-yidj078hitOLRmUjVjHWQBBTgIdfgZwlV8k1ImPrkMNf2xiBLOD1HPcQ8_WHV_FdbK_MUSDjxMoyCXbjStEvz22u05YL67LRE6kaJ1JtptRKEIlx0XJRWMOFCbf-nSIe7LebI=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QoKVaoS8Tynps2vPguuU3so1jLvZGtl5p8u4NG-MiI9NgnCmg4FaA9KV7Lk-JGo4yEBIvhP6KSAmi4kaT5eHQxOcD1ZjjyTRGe6v6zlBNwkB4S9tNKnOglcTAgOGaWLRSiqJZsAMejumY1=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>()
    },

    // 5. تموينات ثمار التفاحة (Rating: 4.9)
    new Place
    {
        PlaceName = "تموينات ثمار التفاحة",
        Description = "تموينات وسوبرماركت ثمار التفاحة",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("سوبرماركت"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 17.5035895,
            Longitude = 47.0866061,
            DirectorateId = getGeoIds("4159 الملك سلمان بن عبدالعزيز، مخطط ا، NSJA7149، 7149، شرورة 68341، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("4159 الملك سلمان بن عبدالعزيز، مخطط ا، NSJA7149، 7149، شرورة 68341، المملكة العربية السعودية").distId,
            NearestLandmark = "4159 الملك سلمان بن عبدالعزيز، مخطط ا، NSJA7149، 7149، شرورة 68341، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.9),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-05",
            EntityName = "تموينات ثمار التفاحة",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLdeqm9cPabsUqyTwX_tajd3oYrAzya-uJak3xv-ePJ0rhYa4jY2U-tjgB5zGp3gCA092r6W3eyNO82Ftk62-rXQBD8zcfvZxgLAYTs-kRjpad27Xx3ksyaTcAp2R6MQL97qS3RurKWw0l-2=w408-h724-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9R8_8Fw1yeCbTGA8MDFzk7z_GB0Axcf_BLASzohhzN7jA8Nd5u4SUEIRgi8jOT-Wc_3hhImuzsdVIvKcN2xvAsoMHbpOpb9I2JnAHjokzVpvNVsH1nntMaBHRBcJNIfflceJn4XcSyvGesF=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RfoCkvLSiLYRug1rkoXVKeoWEidDS7J9q6FOkYrGHEil90hApXmdyYM8Lvo-OgSYfl-KO6R1t61tSi0lspYplevUeKTzyT6tWL_xMmeZQlvF0vLcjHc3qAaaPaZVOU2_N1xeCePXuuIXYP=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0535959684" },
        }
    },

    // 6. وِصال كافيه | قهوة مختصة وأكثر (Rating: 4.4)
    new Place
    {
        PlaceName = "وِصال كافيه | قهوة مختصة وأكثر",
        Description = "قهوة مختصة وأكثر",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("كافيه"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.477113,
            Longitude = 45.5798797,
            DirectorateId = getGeoIds("طريق الملك عبدالعزيز، النخيل، السليل 18651، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("طريق الملك عبدالعزيز، النخيل، السليل 18651، المملكة العربية السعودية").distId,
            NearestLandmark = "طريق الملك عبدالعزيز، النخيل، السليل 18651، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.4),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-06",
            EntityName = "وِصال كافيه | قهوة مختصة وأكثر",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLfskHk09e7XSCUUDHAWNS2hokqwuxqQRuoJXcvKwdMy8atk_X3s9dq2oEBw4s66ynVC5dPtuEfkurOpKSvXLqkVB7sqkfmgjz8B9xIbmQnsLlcXCoyyyPr62PZRVdRQvl7H8j_jLg=w408-h271-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TJquHSvkKXhoWGWYN-mBdAG5PgVbbYkfc1UU6IOll-graHN6gYBTD6A4LOynUi9-LyXsF6lapFiwnQIk7lFNCnpOkuQQKEFvk6h7nxI-p5WWsKCo4DfQJ_HwQxt8zcGuLvsr61FQ=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SGTC88RCmeUKQLFsZicQaN_RHPjUMCI-BRDOFUDK3nstFDLdbcY_3swd0aV8IKXUE6-0DwvSI0VqMmuY2SIDduFA8ZFPzuQKztAqxQRxit7SCj36oQaJvhDUonfb1wK0X8MOTvBfjdFnRX=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0532502711" },
        }
    },

    // 7. مطعم ريف سبأ (Rating: 4.4)
    new Place
    {
        PlaceName = "مطعم ريف سبأ",
        Description = "مطعم ريف سبأ للوجبات والأكلات الشعبية",
        OpeningTime = new TimeOnly(6, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("مطعم"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 21.3372232,
            Longitude = 46.0730305,
            DirectorateId = getGeoIds("83PF+V6، العجلية 18344، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("83PF+V6، العجلية 18344، المملكة العربية السعودية").distId,
            NearestLandmark = "83PF+V6، العجلية 18344، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.4),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-07",
            EntityName = "مطعم ريف سبأ",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLcCp8HbcbW3ds-g0sLI8ovxj0172efh3pcZCse0-4PbNkU4Es8LcKVrJzbvyUk2clw07tji57QPyPK0m1sUpta6yj0T8xDRicT64PvZ-MrzZLjnzLTO_nubfga9so_Q6S5zTGXsMA=w408-h306-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SurM5Tm2rEyAo3nTbtU0kGgdF9TsPSzZjAwUsX-zcvKnNqncYUUs6WQZ7Tbp3QfVPBIYaciagAY3ZwWkopFimZevB_P9F1Rtva6QgY-dFSJfpxxy8RvNrwwygniyJhsI-88oHnjA=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SKolimHmQq4UvNVU4TxEKOdTVR2AmfgfJFxws3Ww0h2CC2GoouzyxZ1JPRg3yxJcTVPy9OkYGtCcDo_qjX0o2CYxmWsNILbW-LmZShirE48BLMUPvXh6549DOqOlTuwUtjjM5J=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>()
    },

    // 8. DARB | Coffee (Rating: 4.5)
    new Place
    {
        PlaceName = "DARB | Coffee",
        Description = "درب قهوة وبيكري",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("كافيه"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.476552,
            Longitude = 45.5789618,
            DirectorateId = getGeoIds("الملك عبدالعزيز، السليل 11913، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("الملك عبدالعزيز، السليل 11913، المملكة العربية السعودية").distId,
            NearestLandmark = "الملك عبدالعزيز، السليل 11913، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.5),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-08",
            EntityName = "DARB | Coffee",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLfkYtNYDvlFd1cGWI54X1yGh4Jwyh0-jnZ8B3VMuqEymvBN6elqvQmrsytXSYLCB4ipQFwAHfTcosCrsAstoR44JWOD4Jn67ucqLF5wvyZqlkW1cokYsf24w7HjWpZFax9J1mz1XNmpn8t3=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SH-wlvCXymBJ3PHPtG0owgQzKyy9AhfUxoB9c0CuugTieAKMNhOzT2vSf2b3NkyFEgbnMZsS1DlPhgWF4u5Iu5_18t0-Nwp0gYSzrJ2L2FVoqqh68IuhfX45duRFjLOW8H0z92uoxqk7qL=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QpMXAdX2krGUCOLGz9VccQ_gCHErzUNfxm1M2xpZyUauv6jWbco9EKedN9o5BoYVb-DENhJUiZ3k3ZNzjvE8DbIio5z5GCgOW2o9mUMZJTQG4TOHcUM-a9hHtpl9kc4b6XBekX3f1AnubW=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0559449908" },
        }
    },

    // 9. الفارس للحاسب والاتصالات وتقنية المعلومات (Rating: 4.6)
    new Place
    {
        PlaceName = "الفارس للحاسب والاتصالات وتقنية المعلومات",
        Description = "صيانة وبيع اجهزة العاب PC\nبيع اجهزة كمبيوتر وصيانة وتركيب البرامج -بيع وصيانة طابعات واحبار-بطاقات الايتونز وقوقل بلاي\nبيع وتفعيل شرائح STC\nاكسسوارات اجهزة ذكية",
        OpeningTime = new TimeOnly(9, 30),
        ClosingTime = new TimeOnly(23, 30),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("خدمة إصلاح أجهزة كمبيوتر"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.1588467,
            Longitude = 47.3261189,
            DirectorateId = getGeoIds("7962 طريق الملك فهد، المنتزة، 2784، الخرج 16441، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("7962 طريق الملك فهد، المنتزة، 2784، الخرج 16441، المملكة العربية السعودية").distId,
            NearestLandmark = "7962 طريق الملك فهد، المنتزة، 2784، الخرج 16441، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.6),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-09",
            EntityName = "الفارس للحاسب والاتصالات وتقنية المعلومات",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLdJ0mXCe6R1e_32hYLVbRtR8DivG6WWCUeO-sLvj33BiV1ibc061hzl-rypIV05UrHfUcfzBdsXBJhbR0k11wmOYyFPRgKkXGitJDEkGvHxSSTylYLVtB-HuJkP5jtt02RUgQWy=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RBCATvnEt3y20PsJH5bXJ1gxQym5XNynarGhCHoi53sunerAfkUUmAEVSp2zqOiivXq978torw0dvb3jSNxyyv8JRGZfKS6w3a11VDUNIscY-TmMXstbvmvptXQx19ntPFfz3I=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TkA2W3piXpMcf5ttM3Kvfvk6UsQUrmf3_l5WGIWWfW0iCBCAsQjMDPzxSprBuXVIkleTcMGY91h-T8iYXnGYEIJVgh3a43zdTsRsuY-56aWFxZJUB9zDy6y8e4PnTJYq9hq2fg=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0592068299" },
        }
    },

    // 10. محل الفاران للاتصالات والالعاب (Rating: 5.0)
    new Place
    {
        PlaceName = "محل الفاران للاتصالات والالعاب",
        Description = "محل الفاران للاتصالات والالعاب واكسسوارات الأجهزة",
        OpeningTime = new TimeOnly(8, 0),
        ClosingTime = new TimeOnly(23, 30),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر هواتف جوالة"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4625003,
            Longitude = 45.5768754,
            DirectorateId = getGeoIds("7838 كريم، حي النخيل، OPMA3394، 3394، السليل 18651، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("7838 كريم، حي النخيل، OPMA3394، 3394، السليل 18651، المملكة العربية السعودية").distId,
            NearestLandmark = "7838 كريم، حي النخيل، OPMA3394، 3394، السليل 18651، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(5.0),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-10",
            EntityName = "محل الفاران للاتصالات والالعاب",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLc1_vaVriNZLIBj1lZ9Ex1yr-awiHo2I0M6UBaOHzu8i7b0nLcT1drLjriBvwYaiA5r5FBg0e3rTmWYEsqLDhuaEw807P-fDXiyFYOHGLMkvFzXsMzDvdwPU2sg4HoGI_2r1XY=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TXvBNoLDo3QJPgBYT3ZOxiezraNRl3mcfsME6bfIbpdZKF7_ok-UciitqzIuLEftuysr-GjSFbJDeY5N0_2GRxwftnZBSA1_rF-7ZAzHAiRdo7QAy4PsRfl8YBVh7982hF_2E=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SLOuQA1ddKIid2TZcI34nMJ7eVl5U-LaMJm7B5hpuW3wZvrVKLHSV-yfJJNMYi7tgBJjErzKUE0y6Ey9xtOQIL7tZWekrHKHt3aFOzwoysnxM1ybAh9EOCZ-mczwBPCYfr4dD6=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0500787215" },
        }
    },

    // 11. ورشة ميكانيكا (Rating: 4.7)
    new Place
    {
        PlaceName = "ورشة ميكانيكا",
        Description = "ورشة ميكانيكا وصيانة وإصلاح سيارات",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("ورشة إصلاح سيارات"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.3852528,
            Longitude = 45.2337424,
            DirectorateId = getGeoIds("OCPA2152، 2152 طريق واد الدواسر العام، 6026، الفائزية 18364، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("OCPA2152، 2152 طريق واد الدواسر العام، 6026، الفائزية 18364، المملكة العربية السعودية").distId,
            NearestLandmark = "OCPA2152، 2152 طريق واد الدواسر العام، 6026، الفائزية 18364، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.7),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-11",
            EntityName = "ورشة ميكانيكا",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLdZpeQ1gnfcgB4SC0xoZ9C2730tk6rLELMkrGzUfZ3OIyAfCOVozSwg4PmVfVudvhswBULlajZk67p2E-knXzZPPT8pk-rAiWFcqOkVa_wgM8ktIdoQ7j-redp9wAirOuFtCPohrnnlRuk=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TtFgvwglRGd2DAt6X3zA6fbVhZBaYBrw3ihLgsyY6e2nvrpHEodaalrPv2sSEyXOGdHdrcXtcm243ppPA27wFJohAiqhYcphHS9RKxnP9G-uhTE5IpGmBKJOUF5mEwY1_RJUULOMQYZ3E=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QTDNc__aIyJOjTiIkpFdx8tEHRPdb6gn6G8gPMXcLFEM-YzFfM0PQ4lYLlJGuPKKsGHj4GDUy9IdxGxBC-3JS2wU7BNqKGNfmsnegWt0iFI4JKKEkrsiREJTPf5Gqi1J5pt0FrXyroTAlz=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0554185406" },
        }
    },

    // 12. الدانة للاتصالات (Rating: 4.8)
    new Place
    {
        PlaceName = "الدانة للاتصالات",
        Description = "نقدم كل جديد في عالم الإتصالات والجوالات بالتقسيطات المريحة لعملائنا , أسرع خدمة وتوصيل",
        OpeningTime = new TimeOnly(9, 0),
        ClosingTime = new TimeOnly(23, 30),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر هواتف جوالة"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4623732,
            Longitude = 45.5765897,
            DirectorateId = getGeoIds("أبوبكر الصديق، الديرة، السليل 18211، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("أبوبكر الصديق، الديرة، السليل 18211، المملكة العربية السعودية").distId,
            NearestLandmark = "أبوبكر الصديق، الديرة، السليل 18211، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.8),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-12",
            EntityName = "الدانة للاتصالات",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLfIzVjQ-9gaCB0zZehlB3CRO09RtuHCn_fK10cAhFx0NJVNcKGBuChA2G3m5bjMkGqg5TjhHKzz4-9pw2naFFvASqmA7VBli8Ccf2CoACLdsFCYCatiE4Wxn0PCDgpxu--UvNQjPQ=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RzMk4sfGX12VONFYsmY8NkzKxh1LlmY3RzRki3rjDWmfWf-OZiWMpGrVF_zT0ea3VmCenRIx0muyrM14-PZTsJ9H9JG9baQhRKnvi58RobxmqgDbKvHWuIExHB_L6QzPasrJc1nw=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SJjPEMB2zMzyjRMF55bmv5aeAC2JAHA_ubls7qHt7rRv70_Cm5wnk3-jexD6vAfdjvabYOCvR9zUOsmnXpQYESOv0PZDbCuMOHrxgpmpiQ43qeln6fDWyy51B27-b-GFg35Kpb=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0504112113" },
        }
    },

    // 13. خلية HIVE (Rating: 4.6)
    new Place
    {
        PlaceName = "خلية HIVE",
        Description = "مخبز وحلويات خلية HIVE",
        OpeningTime = new TimeOnly(16, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("مخبز"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4539413,
            Longitude = 45.5804281,
            DirectorateId = getGeoIds("الشارع الأصفر، الديرة، السليل 00000، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("الشارع الأصفر، الديرة، السليل 00000، المملكة العربية السعودية").distId,
            NearestLandmark = "الشارع الأصفر، الديرة، السليل 00000، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.6),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-13",
            EntityName = "خلية HIVE",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLdz5asIKHT93bUbtJW3_zFwzlLPCSFlUlSAcwKNDdub_H8Ph_xnstvpd2yb1QdilaZWmQaanRQ3W23dmb4-dtdrXkDG22G4wxA4XtFCjJPWH7y3YRfhTON2OUiwRir5azg3y4c7lg2lgyQ9=w408-h544-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QfTRXk1FPoacFcOB9slq45LEnPnugo77xS_GKhcLXk6afFWy5ZpeZpQFmnPe-qw1ItHq5ZOosKYaz48dtnRXtttASlknBqH16CN6jxUfYIvz3hYQxLX4vgXWj7YrWLTYP-sj-MmrN8Bvl-=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9R5Tx0IcW7kchYwAyMb0VBLjaAWuXcIcYLrVqjniJ6imtOjDGT0_dGaRLDLolx5y-FysC8p8p0RwftISGeJ-nUj9cD1-LSSJW6Q1DYMaROon7c7dhGGEbq027Dvkw_4NQG-Swix-vK3VgA=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0553145174" },
        }
    },

    // 14. لابتوب للحاسب الالي (Rating: 2.5)
    new Place
    {
        PlaceName = "لابتوب للحاسب الالي",
        Description = "بيع وصيانة الكمبيوتر وملحقاته",
        OpeningTime = new TimeOnly(9, 0),
        ClosingTime = new TimeOnly(23, 0),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("دعم الكمبيوتر والخدمات"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4626257,
            Longitude = 45.5784536,
            DirectorateId = getGeoIds("ابوبكر، النخيل، السليل 18651، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("ابوبكر، النخيل، السليل 18651، المملكة العربية السعودية").distId,
            NearestLandmark = "ابوبكر، النخيل، السليل 18651، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(2.5),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-14",
            EntityName = "لابتوب للحاسب الالي",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLerQSszmCmNOMBhuc9KLWBIPyu7oua4v8CkGnIdHFhngtAw-LHlY-UgJOioLbaqw2RAfUNkYu63AWjW5nitAfz8pimqT6YCWAahqE-RWkHnRWZXiJmaJSrIqGDKnLPHfRvZTSWnRv5N2iIN=w408-h329-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QUAugBq2abjkLxfSMvQiKBNaTH3FNu7jLWtu-7bTXtnKA19KirmRAdBJjUsCGHh_HEK_uFFraF94-8YuaIzdFdghfn3IpdSy9iU6uudVq6r1oMaomAZvgQaM-nlozPnVMDfOBXlSsT804w=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9S9o20VA5yQDXLp7VLDKdBO8mP0nf71hCD1OTdd4IM53ReQZ-lEP64zP-LwruofoeoxeLe9D7-iIl6BVm4e3DC4lLNPzMfebIj3CD8VXf_FrRn2mkhPV1p_aDQPKZ8DrE-ylo4K871LlWKn=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>()
    },

    // 15. ماڤ كافية (Rating: 4.4)
    new Place
    {
        PlaceName = "ماڤ كافية",
        Description = "ماڤ كل يوم معاك ..",
        OpeningTime = new TimeOnly(6, 0),
        ClosingTime = new TimeOnly(1, 0),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("مقهى"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.1585488,
            Longitude = 47.3014518,
            DirectorateId = getGeoIds("طريق عثمان بن عفان، البرج، السيح، الخرج 16278، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("طريق عثمان بن عفان، البرج، السيح، الخرج 16278، المملكة العربية السعودية").distId,
            NearestLandmark = "طريق عثمان بن عفان، البرج، السيح، الخرج 16278، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.4),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-15",
            EntityName = "ماڤ كافية",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLcg6FzFR9gMDGRbmGtYGPFoNZZX0vSIubjrHwGTbS_ZWHFrNwHESB0PXY12Npjn4IPgOnH98VXO-OWYIJwIIuqSW4ilYZLooBYwHQ3bMOnflEJAvwjypCno4NUiDy_FcQ9aW2-y=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SITgqCcBxFGMlyKgdLhFf6UujGV02Bs2QWVvoQ_QWTzY3-OHP695C60LT265Jm0wlOyuxrCnLnvqp1BQB9YeYnG7nhdygBp6teKQARYFO66LwAj4DhS02qAS6A-RxQ4tflJyyc=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QmmO2_BLcxMey4vuzjb8-8h5iuq8qA_erUoO54npcsG4unm4-ZKygj3KaV614UykX7vLDawjiH6R6fnJq1crqloV95dP8Cn4HOJ8VeOS9yNX9ZmPS76ofq9C7SYPrp9rTMNd3O=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0573739097" },
        }
    },

    // 16. أسواق هلا مارت (Rating: 5.0)
    new Place
    {
        PlaceName = "أسواق هلا مارت",
        Description = "أسواق وسوبرماركت هلا مارت",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [WeekDays.الاحد],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("سوبرماركت"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4567235,
            Longitude = 45.5803084,
            DirectorateId = getGeoIds("FH4J+M4M، طريق الملك خالد، الديرة،، السليل 18621،، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("FH4J+M4M، طريق الملك خالد، الديرة،، السليل 18621،، المملكة العربية السعودية").distId,
            NearestLandmark = "FH4J+M4M، طريق الملك خالد، الديرة،، السليل 18621،، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(5.0),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-16",
            EntityName = "أسواق هلا مارت",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLcrf7RJbs-hJpA22v4MfOeRnSrEFMBisWJIwHKeLio8G2PUqFyU4bQA3oUgtD6eSIPHqWwzNOkpxQJ1YgT65EsSR5uT4xtft30zjOIOp3o9mY1YeJHNIIscXDTtc2YdB9irJg8=w426-h240-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QOhbBynMcfE3wXoGFLfDqQi0fFmHB5CZ8J5aL4Xqqq7f2X2G0ajp0ZQEJQL1Zq-7FJaTdkSZvYUC67TwalQKai3mDjx_fioPZUefUf6Uwoire5d_vgcwSjfMWVIk3ddmPIthk=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Q9faVqlf5vsik7aZ7R4EN8YXZfk7sPKX1gs6nxyI4oE_Nw0DaH35inAczr8tfMn4glPeO8nYpkZ_3mCEKXXx1DFGL9Jndrg_X4YqA1eu5DN7-tS8y633XeJyOSM5XY-V6ZnhDvvw=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>()
    },

    // 17. تموينات اشرقت (Rating: 4.7)
    new Place
    {
        PlaceName = "تموينات اشرقت",
        Description = "تموينات وبقالة اشرقت",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("سوبرماركت"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4695339,
            Longitude = 45.5644182,
            DirectorateId = getGeoIds("FH97+RQ7، 10، السليل 18311، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("FH97+RQ7، 10، السليل 18311، المملكة العربية السعودية").distId,
            NearestLandmark = "FH97+RQ7، 10، السليل 18311، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.7),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-17",
            EntityName = "تموينات اشرقت",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLeEmFzZiJDv-wLoIPKdxGaSc-BNafAicAhr-pBNpZnwFRUXNYEqoV0gs8hLGYcBdRkuCHDWvQmbzuRWrYwrOg-5CrD8zTEySHkU5wD3-ShvZU-P8sBZTMNSwK333qUZEptATQfUE-36P65e=w408-h306-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9S6hqtFoVuHWWAShf1cnZxEDAF475ClJvBUUHPQssXplJqUKpKz3eYRBl0qLOkoqPkT3NhuxKLPsyzSp-1c7T2_21_fha_a18HHJxfmY2Ig8EiYDS5B_cOy6Cx7aaIty5kPXPMp6kAA6XJc=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RHy3_ZQL1tEwU7Y36bIMtMmC1vqjEYgxpNHrX7w-PjDRptTfXQkzabwUFScaem-9tn2FU1OMUG_i4kD7QlFbh-vaSa16BVbyt8XnrOOJe8Ld5V5e7IOBUYyJRRELuojYqprKhEnHrPjXa8=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0507792117" },
        }
    },

    // 18. اسواق سناء (Rating: 5.0)
    new Place
    {
        PlaceName = "اسواق سناء",
        Description = "متجر بقالة ومواد غذائية اسواق سناء",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر بقالة"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 29.8565187,
            Longitude = 43.1374665,
            DirectorateId = getGeoIds("SRJB2415, 8036 طلعه التمياط 61, 2415, التمياط 76714، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("SRJB2415, 8036 طلعه التمياط 61, 2415, التمياط 76714، المملكة العربية السعودية").distId,
            NearestLandmark = "SRJB2415, 8036 طلعه التمياط 61, 2415, التمياط 76714، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(5.0),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-18",
            EntityName = "اسواق سناء",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLet4AM2CcgJdYZfQzzBtTXw3TFy-AKHRWOaFqbS9qMOroL_TcWeAmMVJI7sb6ZM9u4_MtzVnLCTXc-UiXJ8Syr0KRLyRDfZUbhx0yiynpN7D0-jVJtKMfpMLp2sxShr2MLw02HHyQwC17M=w408-h544-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9RCjC6u_GGsUI7AmAPIqRSsmgJ12WN2GwbrasTS-kPI8lSmXIqamFrRWBFbO7nseX3FOA9DTCwZEDU1IDe9ViZrux5sian74wRYEy9YuoYyCvABAzbcFDKeURdoC_fhFSVT7eJRC5_wUW8=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9SsQD9C0n7qFg8a5_9IYR2xz4JPuQY2id1Glbt3ByckcD5S4UMSYpUqKJ1spTRD5o2-9QYNWAnDVkBsSGPCmyXDxtz_ntvn14a158u7TVFDA7QeVKPYeja1u-xio8899Lb8TtXh3LNfBt0=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0536621893" },
        }
    },

    // 19. محمصة كيو (Rating: 4.5)
    new Place
    {
        PlaceName = "محمصة كيو",
        Description = "محمصة كيو Q هي محمصة قهوة بالرياض ووجهة مميزة لعشاق القهوة الباحثين عن الجودة والمذاق الفريد. نقدم تجربة متكاملة في عالم القهوة المختصة من خلال اختيار أفضل محاصيل البن بعناية من مختلف أنحاء العالم، مع الحفاظ على مستوى جودة ثابت يضمن لك نفس الطعم المميز في كل مرة.\n\nنُعد من بين أفضل محامص القهوة في السعودية، حيث نتميز بتقديم منتجات محمصة قهوة مختصة بأسعار تنافسية تناسب مختلف العملاء دون التأثير على الجودة. نوفر تشكيلة متنوعة تشمل البن المحمص الطازج، أظرف القهوة العملية، بالإضافة إلى مستلزمات التحضير مثل فلاتر القهوة والقمع، لتجد كل ما تحتاجه في مكان واحد.\n\nإذا كنت تبحث عن محمصة قهوة تجمع بين الجودة المستمرة، الأسعار المناسبة، والتنوع، فإن محمصة كيو هي خيارك الأمثل داخل الرياض",
        OpeningTime = new TimeOnly(9, 0),
        ClosingTime = new TimeOnly(23, 30),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("محمصة قهوة"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.6047314,
            Longitude = 46.7085396,
            DirectorateId = getGeoIds("المزروعية، الرياض 12671، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("المزروعية، الرياض 12671، المملكة العربية السعودية").distId,
            NearestLandmark = "المزروعية، الرياض 12671، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.5),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-19",
            EntityName = "محمصة كيو",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLcgww4d6r_QgtEwmLsdE7RD29feu940vxb1mVOlL0dfy5vK3QIDAhXaLdDtUs-UWgkThAcWoJehZWn9lwYg0fscw29U8-I5q0xn5roSYeO0rZAq35c4ciPAJLLGwoyb1mKW8JD7=w408-h306-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Tcr0RIj49z90BCy9ULtOmmbgNf2GY4jebH6gfeuCwgGFh5K1T1TluA-dYsk-LsqS9wQ2VDmmjPnlg7nEYO-nHC2uHxvLlBZDConPD9JhAM5TFQPZ2_NG84OiL3Mj_l_9VScw1t=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9S_xkYxknHTdQlJB1fk3ods4Nz837QaGs9supNbU1egKrkk7ukYPZa-91u3Bjegs16t8tCUYLR0Rn4yoJNhNQTITomqfucZNnIjK5_vLh8KdbjlozIfe0fJz34knJ31zzBdMAicNAIXll4=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0506155464" },
        }
    },

    // 20. قصر الحاسبات (Rating: 4.2)
    new Place
    {
        PlaceName = "قصر الحاسبات",
        Description = "من اكبر موزعين اجهزة الكمبيوتر في المملكة وحالصلين على شهادات التوزيع البرونزية والذهبية من لينوفو و اسس وابل وايسر وديل و اتش بي و تتوفر جميعها بأسعار منافسة ونقدم خدمات ما بعد البيع ويتوفر لدينا قسم خاص بشاشات الألعاب والاكسسورات وقسم طابعات ولدينا قسم خاص بالشركات والقطاع الحكومي للتوريدات ، و تتوفر لدينا تجميعات كمبيوتر الالعاب بأسعار مميزة واحدث القطع ،و كمبيوترات الالعاب , ويتوفر لدينا قسم صيانة في كل فرع , وقسم خاص بمبيعات الجملة حيث نقوم بتغطية جميع مدن المملكة عبر شحن المنتجات من جده والرياض",
        OpeningTime = new TimeOnly(9, 30),
        ClosingTime = new TimeOnly(22, 30),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [WeekDays.الجمعة],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر أجهزة كمبيوتر"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.6825487,
            Longitude = 46.6903928,
            DirectorateId = getGeoIds("6394 3891, العليا، الرياض 12211،، الرياض 11300، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("6394 3891, العليا، الرياض 12211،، الرياض 11300، المملكة العربية السعودية").distId,
            NearestLandmark = "6394 3891, العليا، الرياض 12211،، الرياض 11300، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.2),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-20",
            EntityName = "قصر الحاسبات",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLciG1ACsXnSBD65fmyFvOMpv-yRnCv7RFRc_uKTXHTLjDw8uBIFafGQfsTeioSuj_s2r7aTlI5UjYIHbCLK5fLnL62oHXAChaRU2EIfL3lwUDPgTdYImva1OqDUFT05iJwk-fGkWOQ8GD0F=w408-h306-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Qi1TzUQbhkHWPIwpf99zilsyfSMWy9ksN6gNrUAAe6AX1jyh6GbU72Ako7t1IqLjCeYx-QJri7TtZ869PXCL-F20eRJl8USEMXUyhW0XS1HY1ThrroOSpWh6ZwHXS12Mwrv9HsQfN8Q6S2=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Qn7HauUo9VaX50MS9SNktoMmsO5J6BW1xDZ4yBjyjMK00Dn0lpBNhlp0d4_GIsp47KVEKZjnEom1hdshF8oYbty5__PIjaMAmfu5VGOjgo9HWFQGmVbusrvv7oc3Iz-JErHww=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0112650020" },
        }
    },

    // 21. رسن بيكري وكافيه (Rating: 4.7)
    new Place
    {
        PlaceName = "رسن بيكري وكافيه",
        Description = "كوفي رسن ومخبوزات طازجة",
        OpeningTime = new TimeOnly(4, 30),
        ClosingTime = new TimeOnly(3, 0),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("كافيه"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.4643995,
            Longitude = 45.5781481,
            DirectorateId = getGeoIds("FH7H+Q8G، النخيل، السليل 18651، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("FH7H+Q8G، النخيل، السليل 18651، المملكة العربية السعودية").distId,
            NearestLandmark = "FH7H+Q8G، النخيل، السليل 18651، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.7),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-21",
            EntityName = "رسن بيكري وكافيه",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLe4alBySh-MvAsbxpmLFPMW5_ShNbJo_ghPp16i4CpjnBc87nN_OysIw6vVM7vP30HuFRunTJ5wbu-PaYaf34kRhk_Men9BlBuRoGHcjTXcMfQtbvwMw14RAA2Cuyv1cLQOZA2s1Q=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TslrBL0JNWkC1zkaxZ_1tIdsQs9avqMYWRMOO2WH5TrcSW-Xhkj44ov-W76Et3nIWvlDpOtF0LnznhWe-3Uu8yucmDrz9oVFoXMkN3O9z1lDQIJaZ8imIvUdV2oUKTXAp4jI-ImA=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9TyIxjbiESBR5HRPewnBAhuEGBaPI0JtcZsx5ZA0OyWr1HvmuH1tG_DRuQlDQsmpLyQuArb9dYw_k3mJZWnPNhxIVyfznvS0ijbVvvJu57Mz_ln2BUkN6OHf6RdYx56ZnNbY1xsoxfSFoQh=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0509990604" },
        }
    },

    // 22. ورشة العز (Rating: 4.6)
    new Place
    {
        PlaceName = "ورشة العز",
        Description = "ورشة العز لصيانة وتصليح السيارات",
        OpeningTime = new TimeOnly(8, 0),
        ClosingTime = new TimeOnly(21, 0),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("ورشة إصلاح سيارات"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 20.489076,
            Longitude = 45.5753362,
            DirectorateId = getGeoIds("FHQG+J4P، الزهرة، السليل 18311، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("FHQG+J4P، الزهرة، السليل 18311، المملكة العربية السعودية").distId,
            NearestLandmark = "FHQG+J4P، الزهرة، السليل 18311، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.6),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-22",
            EntityName = "ورشة العز",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLeZVowSjQU_WVJOmmL2D_zGEXlJ61do_i-N0BhlS99hjdajKNJbGmYWxCSfA1ztDNAvYc6305cCcnnvBAlcVpP_4xDmdji2KLnyb7Ia6rIyLeja9VVWbJntqWvZSblBa33wfY6ogcpf2EOR=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9T7zEm0Z87_SVgiOekSvu_Pqv4ZkQ19mPsXkIXZx2Qp1F4B2_VWpAXiw6MSVvVlR9sGbnMyRt2YmpHr9X4levQuEbQyzLLc5yF5T7IXfHS0pggl3EHRoHl3BAt_ZxI7NnV_fnGO3C4_KLx8=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9ScP62rOrbHj8Jn139iM_LunVGrIHauomdo31CaIOV7nn5Y1qYCfqm5drOJ5hJiDuJ_eiOMzA6lK_ciep69lr13uAVsj0mwYcX_HEvUAq7uPHDRwoB9eAUQbALpf87frfvg-r31rBIXEyet=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0508850876" },
        }
    },

    // 23. بقالة مجمع ابن رخيص للخدمات البترولية (Rating: 4.3)
    new Place
    {
        PlaceName = "بقالة مجمع ابن رخيص للخدمات البترولية",
        Description = "بقالة وتموينات مجمع ابن رخيص للخدمات البترولية",
        OpeningTime = new TimeOnly(0, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر بقالة"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 30.0475331,
            Longitude = 42.7440063,
            DirectorateId = getGeoIds("منطقة الحدود الشمالية، 76732، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("منطقة الحدود الشمالية، 76732، المملكة العربية السعودية").distId,
            NearestLandmark = "منطقة الحدود الشمالية، 76732، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.3),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-23",
            EntityName = "بقالة مجمع ابن رخيص للخدمات البترولية",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLf9weejWPZtGnI2ysMdThyTfVPc9DB2FgoPJc_KmW97ER4DqCSvB63VbxdRRjUfzGxA4fToY9WhJFLAi8HjlQm_bWwPT-VeCNkR7cBtbgqQz63LzPErE2g9l4SsMHTaYDQnf9faA_L22V7w=w408-h544-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QrwEYTHiq4wu0zNmYUNfBlxOJ4ysJpVNk7BXqzgQkgt9AZBL8VX1QcjAPKFu-RKi3sY_5tpjC3dDycbGI69gujxQHsFgxs3OMgXvaHpMSZ80q4OlJzqK9I7z9lhmBxD5iUlH7ALzaubZjD=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Si6511CTecHtrkmNJRHPrqAliY_VJC0MrEsg1n41rGcikCEVLlYX2bNQ2ylbKHX_IKJG_6Tv4wiWQmKzqUlMYbBnVIUGgKJSrhTodPw9QiKBSewLq7wVDsUS0JJa_88BNqpnau=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>
        {
            new PhoneNumber { Number = "0552945594" },
        }
    },

    // 24. الرميح للكمبيوتر Alrumaih Pc Computer (Rating: 4.7)
    new Place
    {
        PlaceName = "الرميح للكمبيوتر Alrumaih Pc Computer",
        Description = "Service Accessories Computer Macbook Printer Toner Ink Hp Canon Toshiba\nRepair Epson Printer Zebra Series 3 ID Card Printer Color Sandisk Kingstone itunes Google Play PS4 Giftcard Headphone Speaker Table Laptop Bag Modem Keyboard Laserjet\nمحل كمبيوتر لابتوب حاسب صيانة ماك بوك وايرلس وايفاي مودم راوتر طابعات طابعة احبار حبر صيانة طابعات ابسون فلاش ذاكرة هاردسك سماعة سماعات بلوتوث بطاقة كوبون شحن ايتونز قوقل ستور ستيم مباشر ونكارد مرسول زيبرا طابعة كروت وجهين تمت هنقرستيشن",
        OpeningTime = new TimeOnly(16, 0),
        ClosingTime = new TimeOnly(23, 59),
        CreatedAt = DateTime.UtcNow,
        UnavailableAt = [],
        Request = new Request { CreatedAt = DateTime.UtcNow, State = RequestStates.Accepted, UserId = defaultOwner.Id },
        SubCategoryId = getSubCategoryId("متجر أجهزة كمبيوتر"),
        UserId = defaultOwner.Id,
        Location = new Location
        {
            Latitude = 24.5832521,
            Longitude = 46.6608326,
            DirectorateId = getGeoIds("السويدي العام، السويدي، الرياض 12788، المملكة العربية السعودية").dirId,
            DistrictId = getGeoIds("السويدي العام، السويدي، الرياض 12788، المملكة العربية السعودية").distId,
            NearestLandmark = "السويدي العام، السويدي، الرياض 12788، المملكة العربية السعودية",
        },
        Rates = createReviewsForScore(4.7),
        CommericalRegisteration = new CommericalRegisteration
        {
            CrNumber = "SEED-PLACE-24",
            EntityName = "الرميح للكمبيوتر Alrumaih Pc Computer",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            ImagePath = "",
            User = defaultOwner,
            UserId = defaultOwner.Id
        },
        Images = new List<PlaceImage>
        {
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/grass-cs/AABkmLd_b39TbAMKyR0xi_GfkCaxzGSk5xKGMBgLt0RKGo6o56bo5sSpzXT0aG7cB9XGafMYo0Xc5vRL181b0bJnP-lMpgbvX7JgvleIyA6coXjhmIxEoIZfJ7cS616WyZ1cfnXsdk93l-hVNsdt=w408-h725-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9Q0mq2UxtlmGW3sChlqbhx1rSx-BPuSdNvq3WRzjtffRnOf2kdKOJUvkdyJGDCloTbqCyHNQkmqgyEiu7vcDmDpxnMVGFQN3Acx_SymwW1AGxbGOc3j8HZJ39iV_z45EgAJpNUJB6zdxllx=w1920-h1080-k-no" },
            new PlaceImage { ImageUrl = "https://lh3.googleusercontent.com/gps-cs-s/ANWiy9QYmBKBdjPAW03A2FwbyinrtV1Pg2a8fYGJAtcC5R3WiPAK7JTdwKsvoSZ5xgdQo5vabRjwgruVVV-JdjvNZb8Lh0hbIFmYj7HSszP4w1w31Dv9mFBk0C7bYMndVUu_aQQq5anz_rC3Csxv=w1920-h1080-k-no" },
        },
        Phones = new List<PhoneNumber>()
    }
};

            context.Places.AddRange(placesToAdd);
            await context.SaveChangesAsync();
        }
    }
}