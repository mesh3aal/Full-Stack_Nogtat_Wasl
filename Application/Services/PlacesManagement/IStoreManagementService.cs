using Application_Layer.Models.RequestDTO.StoreController;

namespace Application_Layer.Services.PlacesManagement
{
    public interface IStoreManagementService
    {
        Task<Result<string>> DeleteStore(int placeId);
        Task<Result<string>> ChangeStoreStatus(int id, PlaceStatus status);
        Task<Result<SuspendAndActivateStoreDto>> ActivateStore(int placeId);
        Task<Result<SuspendAndActivateStoreDto>> RejectedStore(int placeId);
        Task<Result<StoreStatusTotalPlaseDto>> GetStatusTotalPlase();
        Task<Result<PaginatedResult<StoreResponsDto>>> GetStoreAllAsync(StoreFilterDto filterDto);

        Task<Result<EditStore>> EditingForStore(EditStore model);
        Task<Result<StoreDetailsForEditDto>> GetStoreForEdit(int placeId);
    }
}
