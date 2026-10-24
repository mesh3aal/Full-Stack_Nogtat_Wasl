using Application_Layer.Models.RequestDTO.UserManagementController.UserManagementDto;

namespace Application_Layer.Services.IdentityAndAccess
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;


        public UserManagementService(ApplicationDbContext context, RoleManager<Role> roleManager,
            UserManager<User> userManager = null)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<Result<CreateNewUserDto>> CreateNewUserAsync(CreateNewUserDto model)
        {
            var userByUsername = await _userManager.FindByNameAsync(model.Username);
            if (userByUsername != null)
                return Result.Error("اسم المستخدم هذا محجوز مسبقاً، اختر اسماً آخر.");

            var userByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userByEmail != null)
                return Result.Error("هذا البريد الإلكتروني مسجل لدينا بالفعل.");

            var roleName = model.UserType.ToString();
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return Result.Error("نوع المستخدم المختار غير معرف في النظام.");

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                RoleId = (await _roleManager.FindByNameAsync(roleName)).Id
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return Result.Error(result.Errors.FirstOrDefault()?.Description ?? "حدث خطأ أثناء إنشاء الحساب");
            }

            await _userManager.AddToRoleAsync(user, roleName);

            return Result.Success(model);
        }


        public async Task<Result<UpdateUserDto>> UpdateUserAsync(UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return Result.Error("المستخدم غير موجود.");

            if (user.Email != model.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (emailExists != null) return Result.Error("البريد الإلكتروني مستخدم بالفعل.");
            }

            user.UserName = model.FullName;
            user.Email = model.Email;
            user.Status = (UserStatus)model.UserStatus;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result.Error(updateResult.Errors + "حدث خطأ أثناء تحديث البيانات.");


            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = model.UserType.ToString();
            if (!currentRoles.Contains(newRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, newRole);
            }

            await _context.SaveChangesAsync();
            return Result.Success(model);
        }


        public async Task<Result<PaginatedResult<UserManegeResponse>>> GetAllAsync(UserFilterDto filterDto)
        {
            var Quere = _context.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filterDto.Search))
            {
                Quere = Quere.Where(x =>
                    EF.Functions.Like(x.UserName, $"%{filterDto.Search}%") ||
                    EF.Functions.Like(x.Email, $"%{filterDto.Search}%") && x.UserName != "admin");
            }

            if (filterDto.Status.HasValue)
            {
                Quere = Quere.Where(x => x.Status == filterDto.Status.Value && x.UserName != "admin");
            }


            var page = filterDto.Page <= 0 ? 1 : filterDto.Page;
            var pageSize = filterDto.PageSize <= 0 ? 10 : filterDto.PageSize;


            var RatingCount = await Quere.CountAsync();

            var users = Quere.Skip((page - 1) * pageSize)
                .Take(pageSize).Where(x => x.UserName != "admin")
                .Select(x => new UserManegeResponse
                {
                    ID = x.Id,
                    Name = x.UserName,
                    Email = x.Email,
                    Rating = x.Rates.Count(),
                    Status = x.Status.ToString(),
                    CreatedAt = x.CreatedAt.ToString(),
                    Avatar = x.Avatar,
                }).ToList();

            var pagination = PaginatedResult<UserManegeResponse>.Sucess(users, RatingCount, page, pageSize);

            return Result<PaginatedResult<UserManegeResponse>>.Success(pagination, "تم جلب بيانات");
        }


        public async Task<Result<UserManegeFindResponse>> GetFindByIDA(string id)
        {
            var Quere = _context.Users.Where(x => x.Id == id).AsNoTracking();

            var users = await Quere
                .Select(x => new UserManegeFindResponse
                {
                    id = x.Id,
                    fullName = x.UserName,
                    email = x.Email,
                    phone = x.PhoneNumber,
                    userStatus = x.Status.ToString(),
                    UserType = x.Role.Name,
                }).FirstOrDefaultAsync();


            return Result<UserManegeFindResponse>.Success(users, "تم جلب بيانات");
        }


        public async Task<Result<SuspendAndActivateUserDto>> SuspendUserAsync(string UserID)
        {
            var User = await _context.Users.FirstOrDefaultAsync(p => p.Id == UserID);

            if (User == null)
            {
                return Result<SuspendAndActivateUserDto>.NotFound("المستخدم غير موجود");
            }

            if (User.Status == UserStatus.Suspended)
            {
                return Result<SuspendAndActivateUserDto>.Conflict("هذا المستخدم معلق بالفعل");
            }

            User.Status = UserStatus.Suspended;
            try
            {
                await _context.SaveChangesAsync();

                return Result<SuspendAndActivateUserDto>.Success(new SuspendAndActivateUserDto
                {
                    UserID = User.Id
                }, "تم تعليق المحل بنجاح");
            }
            catch (Exception ex)
            {
                return Result<SuspendAndActivateUserDto>.Error("حدث خطأ أثناء تحديث حالة المستخدم");
            }
        }

        public async Task<Result<SuspendAndActivateUserDto>> ActivateUserAsync(string UserId)
        {
            var Users = await _context.Users.FirstOrDefaultAsync(p => p.Id == UserId);
            if (Users == null)
            {
                return Result<SuspendAndActivateUserDto>.NotFound("المحل غير موجود");
            }

            if (Users.Status == UserStatus.Active)
            {
                return Result<SuspendAndActivateUserDto>.Conflict("المحل نشط بالفعل");
            }

            Users.Status = UserStatus.Active;

            try
            {
                await _context.SaveChangesAsync();

                return Result<SuspendAndActivateUserDto>.Success(new SuspendAndActivateUserDto
                {
                    UserID = Users.Id
                }, "تم تنشيط المحل بنجاح");
            }
            catch (Exception)
            {
                return Result<SuspendAndActivateUserDto>.Error("حدث خطأ أثناء محاولة التنشيط");
            }
        }

        public async Task<Result<string>> ChangeUserStatusAsync(int userId, UserStatus newStatus)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result<string>.NotFound("المستخدم غير موجود");
            }

            user.Status = newStatus;
            await _context.SaveChangesAsync();
            return Result<string>.Success("تم تغيير حالة المستخدم بنجاح");
        }


        public async Task<Result<string>> DeleteUserAsync(string Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return Result<string>.NotFound("المستخدم غير موجود");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Result<string>.Success("تم حذف المستخدم بنجاح");
        }
    }
}