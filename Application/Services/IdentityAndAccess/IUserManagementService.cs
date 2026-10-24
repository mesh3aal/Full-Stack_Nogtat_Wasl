
using Application_Layer.Models.RequestDTO.UserManagementController.UserManagementDto;

namespace Application_Layer.Services.IdentityAndAccess
{
    public interface IUserManagementService
    {
        Task<Result<string>> ChangeUserStatusAsync(int userId, UserStatus newStatus);
        Task<Result<SuspendAndActivateUserDto>> ActivateUserAsync(string UserId);
        Task<Result<SuspendAndActivateUserDto>> SuspendUserAsync(string UserID);
        Task<Result<PaginatedResult<UserManegeResponse>>> GetAllAsync(UserFilterDto filterDto);
        Task<Result<CreateNewUserDto>> CreateNewUserAsync(CreateNewUserDto model);
        Task<Result<string>> DeleteUserAsync(string Id);
        Task<Result<UpdateUserDto>> UpdateUserAsync(UpdateUserDto model);
        Task<Result<UserManegeFindResponse>> GetFindByIDA(string id);
    }
}
