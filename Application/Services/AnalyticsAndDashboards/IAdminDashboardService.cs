
namespace Application_Layer.Services.AnalyticsAndDashboards
{
    public interface IAdminDashboardService
    {
        Task<Ardalis.Result.Result<StatesDto>> OverviewStates();
        Task<Ardalis.Result.Result<List<LastOrderDto>>> LastRequest();
    }
}
