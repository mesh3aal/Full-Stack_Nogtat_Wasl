using Application_Layer.Models.RequestDTO.RequestController;
using Application_Layer.Models.SendDTO.RequestController;

namespace Application_Layer.Services.PlacesManagement
{
    public interface IRequestService
    {
        Task<Result<PaginatedResult<RequestResponsDto>>> GetRequestOrdersAsync(RequestFilterDto filterDto);
        Task<Result<RequestAcceptOrderDto>> AcceptOrderAsync(int orderId);
        Task<Result<RequestRejectOrderDto>> RejectOrderAsync(int orderId, string Reason);
        Task<Result<RequestDetailsDto>> RequestDetails(int RequestID);


    }
}
