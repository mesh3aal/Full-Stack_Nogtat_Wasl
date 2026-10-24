

using Application_Layer.Models.RequestDTO.UserManagementController.UserManagementDto;
using Application_Layer.Services.IdentityAndAccess;

namespace WebAPI.Controllers.Admin
{
    [ApiController]

    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userService;

        public UserManagementController(IUserManagementService userService)
        {
            _userService = userService;
        }


        [Authorize(Roles = "Admin")]

        [TranslateResultToActionResult]

        [HttpGet(Routing.Users.GetAll)]
        public async Task<Result<PaginatedResult<UserManegeResponse>>> GetAllAsync([FromQuery] UserFilterDto filterDto)
        {
            return await _userService.GetAllAsync(filterDto);

        }
        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPatch(Routing.Users.Suspend)]
        public async Task<Result<SuspendAndActivateUserDto>> SuspendAsync(string userId)
        {
            return await _userService.SuspendUserAsync(userId);

        }
        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPatch(Routing.Users.Activate)]
        public async Task<Result<SuspendAndActivateUserDto>> ActivateAsync(string userId)
        {
            return await _userService.ActivateUserAsync(userId);

        }
        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPut(Routing.Users.ChangeStatus)]
        public async Task<Result<string>> ChangeStatusAsync([FromBody] ChangeUserStatusDto dto)
        {
            return await _userService.ChangeUserStatusAsync(dto.UserId, dto.Status);

        }





        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpGet(Routing.Users.Find)]
        public async Task<Result<UserManegeFindResponse>> GetFindByIDAsync(string id)
        {
            return await _userService.GetFindByIDA(id);

        }



        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPut(Routing.Users.Update)]
        public async Task<Result<UpdateUserDto>> UpdateUserAsync([FromBody] UpdateUserDto dto)
        {
            return await _userService.UpdateUserAsync(dto);

        }






        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpPost(Routing.Users.Create)]
        public async Task<Result<CreateNewUserDto>> CreateNewUser([FromBody] CreateNewUserDto dto)
        {
            return await _userService.CreateNewUserAsync(dto);

        }





        [Authorize(Roles = "Admin")]
        [TranslateResultToActionResult]

        [HttpDelete(Routing.Users.Delete)]
        public async Task<Result<string>> DeletUser(string id)
        {
            return await _userService.DeleteUserAsync(id);

        }


    }
}
