using Application_Layer.Models.RequestDTO.StoreController;
using Application_Layer.Services.PlacesManagement;

namespace WebAPI.Controllers.Admin
{
    [ApiController]

    public class StoreManagementController : ControllerBase
    {
        private readonly IStoreManagementService _storeService;

        public StoreManagementController(IStoreManagementService storeService)
        {
            _storeService = storeService;
        }

        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.Stores.GetAll)]
        public async Task<Result<PaginatedResult<StoreResponsDto>>> GetAllAsync([FromQuery] StoreFilterDto filterDto)
        {
            return await _storeService.GetStoreAllAsync(filterDto);

        }
        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.Stores.Statistics)]
        public async Task<Result<StoreStatusTotalPlaseDto>> GetStatisticsAsync()
        {
            return await _storeService.GetStatusTotalPlase();

        }

        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPatch(Routing.Stores.Suspend)]
        public async Task<Result<SuspendAndActivateStoreDto>> SuspendAsync(int placeId)
        {
            return await _storeService.RejectedStore(placeId);

        }

       // [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPatch(Routing.Stores.Activate)]
        public async Task<Result<SuspendAndActivateStoreDto>> ActivateAsync(int placeId)
        {
            return await _storeService.ActivateStore(placeId);
        }
       // [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpDelete(Routing.Stores.Delete)]
        public async Task<Result<string>> DeleteAsync(int id)
        {
            return await _storeService.DeleteStore(id);
        }

       // [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPut(Routing.Stores.ChangeStatus)]
        public async Task<Result<string>> ChangeStatusAsync([FromBody] ChangeStoreStatusDto dto)
        {
            return await _storeService.ChangeStoreStatus(dto.Id, dto.Status);

        }

        [Authorize]
        [TranslateResultToActionResult]
        [HttpPatch(Routing.Stores.Edit)]
        public async Task<Result<EditStore>> EditStoreAsync([FromForm] EditStore model)
        {
            return await _storeService.EditingForStore(model);
        }

       // [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpGet(Routing.Stores.GetForEdit)]
        public async Task<Result<StoreDetailsForEditDto>> GetStoreForEditAsync(int id)
        {
            return await _storeService.GetStoreForEdit(id);
        }

    }
}
