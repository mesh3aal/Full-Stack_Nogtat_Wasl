
namespace Application_Layer.Services.AnalyticsAndDashboards
{
    public class AdminDashboardService : IAdminDashboardService
    {

        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Result<StatesDto>> OverviewStates()
        {
            var newOrders = await _context.Requests.Where(x => x.State == RequestStates.Pending).CountAsync();
            var totalPlases = await _context.Places.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var states = new StatesDto
            {
                NewOrders = newOrders,
                TotalPlases = totalPlases,
                TotalUsers = totalUsers

            };
            if (states != null)
            {
                return Result<StatesDto>.Success(states, "تم جلب الإحصائيات بنجاح");

            }
            else
            {
                return Result<StatesDto>.NotFound("لا يوجد بيانات");

            }

        }

        public async Task<Result<List<LastOrderDto>>> LastRequest()
        {
            var lastOrders = await _context.Requests
                .Include(x => x.Place)
                .ThenInclude(x => x.Images)
                .OrderByDescending(x => x.CreatedAt)
                .Take(4)
                .Select(x => new LastOrderDto
                {
                    Id = x.RequestId,
                    CreatedAt = x.CreatedAt,
                    ImgUrl = x.Place.Images.Select(i => i.ImageUrl).FirstOrDefault()!,
                    NamePlace = x.Place.PlaceName,
                    State = x.State
                })
                .ToListAsync();

            if (lastOrders.Any())
            {
                return Result<List<LastOrderDto>>.Success(lastOrders, "تم جلب آخر طلب بنجاح");
            }
            else
            {
                return Result<List<LastOrderDto>>.NotFound("لا يوجد بيانات");
            }
        }
    }
}
