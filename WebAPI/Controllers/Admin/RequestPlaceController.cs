
using Application_Layer.Models.RequestDTO.RequestController;
using Application_Layer.Models.SendDTO.RequestController;
using Application_Layer.Services.PlacesManagement;

namespace WebAPI.Controllers.Admin
{
    [ApiController]

    public class RequestPlaceController : ControllerBase
    {


        private readonly IRequestService _requestService;
        public RequestPlaceController(IRequestService requestService)
        {
            _requestService = requestService;
        }

       // [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.Requests.GetAll)]
        public async Task<Result<PaginatedResult<RequestResponsDto>>> GetAllRequestAsync([FromQuery] RequestFilterDto filterDto)
        {
            return await _requestService.GetRequestOrdersAsync(filterDto);

        }

        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPut(Routing.Requests.Accept)]
        public async Task<Result<RequestAcceptOrderDto>> AcceptAsync(int orderId)
        {
            return await _requestService.AcceptOrderAsync(orderId);

        }

        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]
        [HttpPut(Routing.Requests.Reject)] 
        public async Task<Result<RequestRejectOrderDto>> RejectAsync([FromBody] RejectOrderRequest request)
        {
            return await _requestService.RejectOrderAsync(request.OrderId, request.Reason);
        }



        //[Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.Requests.Details)]
        public async Task<Result<RequestDetailsDto>> RequestDetails(int Id)
        {
            return await _requestService.RequestDetails(Id);

        }





    }
}
