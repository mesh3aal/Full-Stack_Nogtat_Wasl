
namespace Persistence_Layer.Persistence.SeedClasses;

public static class DirectoratesAndDistricts
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        if (!context.Directorates.Any() && !context.Districts.Any())
        {
            var directorates = new Directorate[]
            {
                new(){Name = "المكلا" },
                new(){ Name = "أرياف المكلا" },
                new(){ Name = "الشحر" },
                new(){ Name = "غيل باوزير" },
                new(){ Name = "الديس الشرقية" },
                new(){ Name = "الريدة وقصيعر" },
                new(){ Name = "بروم ميفع" },
                new(){ Name = "حجر" },
                new(){ Name = "يبعث" },
                new(){ Name = "غيل بن يمين" },
                new(){ Name = "الضليعة" },
                new(){ Name = "سيئون" },
                new(){ Name = "تريم" },
                new(){ Name = "شبام" },
                new(){ Name = "القطن" },
                new(){ Name = "ساه" },
                new(){ Name = "وادي العين وحورة" },
                new(){ Name = "حريضة" },
                new(){ Name = "عمد" },
                new(){ Name = "رخية" },
                new(){ Name = "دوعن" },
                new(){ Name = "العبر" },
                new(){ Name = "زمخ ومنوخ" },
                new(){ Name = "حجر الصيعر" },
                new(){ Name = "قف العوامر" },
                new(){ Name = "ثمود" },
                new(){ Name = "رماه" },
                new(){ Name = "السوم" },
                new(){ Name = "منوخ" },
                new(){ Name = "ساه" }
            };
            
            context.Directorates.AddRange(directorates);
            await context.SaveChangesAsync();
            
            var districts = new District[]
            {
                new() { Name = "الشرج", DirectorateId = directorates[0].Id },
                new() { Name = "الديس", DirectorateId = directorates[0].Id },
                new() { Name = "المكلا القديمة", DirectorateId = directorates[0].Id },
                new() { Name = "فوه", DirectorateId = directorates[0].Id },
                new() { Name = "فوه القديمة", DirectorateId = directorates[0].Id },
                new() { Name = "روكب", DirectorateId = directorates[0].Id },
                new() { Name = "بويش", DirectorateId = directorates[0].Id },
                new() { Name = "خلف", DirectorateId = directorates[0].Id },

                new() { Name = "حي المنصورة", DirectorateId = directorates[2].Id },
                new() { Name = "حي مجرف", DirectorateId = directorates[2].Id },
                new() { Name = "حي عيديد الساحل", DirectorateId = directorates[2].Id },

                new() { Name = "حي السحيل", DirectorateId = directorates[11].Id },
                new() { Name = "حي القرن", DirectorateId = directorates[11].Id },
                new() { Name = "حي الوحدة", DirectorateId = directorates[11].Id },

                new() { Name = "حي الخليف", DirectorateId = directorates[12].Id },
                new() { Name = "حي الروضة", DirectorateId = directorates[12].Id }
            };

            context.Districts.AddRange(districts);
            await context.SaveChangesAsync();
        }
    }
}
