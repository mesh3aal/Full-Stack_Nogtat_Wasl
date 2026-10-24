
using Application_Layer.Models.RequestDTO.CommericalRegisterationController;
using Application_Layer.Models.RequestDTO.RequestController;
using Application_Layer.Models.SendDTO.RequestController;

namespace Application_Layer.Services.PlacesManagement
{
    public class RequestService(ApplicationDbContext context) : IRequestService
    {
        public async Task<Result<PaginatedResult<RequestResponsDto>>> GetRequestOrdersAsync(RequestFilterDto filterDto)
        {

            var query = context.Requests.Include(x=>x.Place).Include(x=>x.User).AsNoTracking();
            if (!string.IsNullOrEmpty(filterDto.Search))
            {

                query = query.Where(r => r.Place.PlaceName.Contains(filterDto.Search) || r.User.UserName.Contains(filterDto.Search));

            }
            if (filterDto.Status.HasValue)
            {
                query = query.Where(x => x.State == filterDto.Status);
            }

            var totalCount = await query.CountAsync();
            var data = await query.OrderByDescending(x=>x.CreatedAt).Skip((filterDto.Page - 1) * filterDto.PageSize).Take(filterDto.PageSize).Select(x => new RequestResponsDto
            {
                RequestId= x.RequestId,
                States = x.State,
                CreatedAt = x.CreatedAt,
                ImagePlaceUrl = x.Place.Images.Select(x => x.ImageUrl).FirstOrDefault(),
                OwnerName=x.User.UserName,
                PlaceName = x.Place.PlaceName,
                TypeRequest = x.State.ToString()

            }).ToListAsync();

            var paginated = PaginatedResult<RequestResponsDto>.Sucess(data, totalCount, filterDto.Page, filterDto.PageSize);

            return Result<PaginatedResult<RequestResponsDto>>.Success(paginated);

        }

        public async Task<Result<RequestAcceptOrderDto>> AcceptOrderAsync(int orderId)
        {
            var Find = await context.Requests.Include(x=>x.Place).FirstOrDefaultAsync(x=>x.RequestId==orderId);
            if (Find != null)
            {
                if (Find.State == RequestStates.Accepted)
                    return Result<RequestAcceptOrderDto>.Conflict( "هذا طلب نشط بلفعل");

                Find.State = RequestStates.Accepted;

                Find.Place.State = PlaceStatus.Active;
                

                var notification = new Notification
                {
                    Title = "🎉 مبروك! تم قبول محلك بنجاح",
                    Body = $"يسعدنا إبلاغك بأن محلك '{Find.Place.PlaceName}' أصبح نشطاً الآن وجاهزاً لاستقبال آراء العملاء. نتمنى لك رحلة موفقة وناجحة!",
                    CreatedAt = DateTime.UtcNow,
                    UserId = Find.UserId
                };
                await context.Notifications.AddAsync(notification);
                await context.SaveChangesAsync();
                return Result<RequestAcceptOrderDto>.Success(new RequestAcceptOrderDto
                {
                    OrderId = Find.RequestId

                }, "تم تنشيط طلب");
            }
            return Result<RequestAcceptOrderDto>.NotFound( "الطلب غير موجود");


        }
        public async Task<Result<RequestRejectOrderDto>> RejectOrderAsync(int orderId, string reason)
        {
            var request = await context.Requests.Include(x=>x.Place).FirstOrDefaultAsync(x=>x.RequestId==orderId);
            if (request == null)
            {
                return Result<RequestRejectOrderDto>.NotFound("الطلب غير موجود");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return Result<RequestRejectOrderDto>.Error("الرجاء إدخال سبب الرفض");
            }

            request.State = RequestStates.Rejected;
            request.RejectionReason = reason;


            request.Place.State= PlaceStatus.Rejected;
            

            var notification = new Notification
            {
                Title = "❌ تم رفض طلب تفعيل المحل",
                Body = $"نعتذر منك، لقد تم رفض طلب تفعيل محلك '{request.Place.PlaceName}' بسبب: ({reason}). يرجى تحديث البيانات وإعادة التقديم.",
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId
            };
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            return Result<RequestRejectOrderDto>.Success(new RequestRejectOrderDto
            {
                OrderId = request.RequestId,
                Reason = request.RejectionReason
            }, "تم رفض المحل");
        }
        
        public async Task<Result<RequestDetailsDto>> RequestDetails(int RequestID)
        {
            var dto = await context.Requests
                .Where(x => x.RequestId == RequestID)
                .Select(x => new RequestDetailsDto
                {
                    State = x.State,
                    CreatedAt = x.CreatedAt,

                    DataPlase = new RequestForDataPlaseDto
                    {
                        PlaceName = x.Place.PlaceName,
                        Description = x.Place.Description,
                        MineCategoryName = x.Place.SubCategory.Category.CategoryName,
                        SupCategoryName = x.Place.SubCategory.Name,
                    },
                    WorkHours = x.Place.OpeningTime + " to " + x.Place.ClosingTime,

                    CommericalRegisteration = x.Place.CommericalRegisteration == null ? null : new CommericalRegisterationDto
                    {
                        CompanyName = x.Place.CommericalRegisteration.EntityName,
                        CrNumber = x.Place.CommericalRegisteration.CrNumber,
                        ImgeUrl = x.Place.CommericalRegisteration.ImagePath
                    },

                    LocationAndAddres = new RequestForLocationDto
                    {
                        NearestLandmark = x.Place.Location.NearestLandmark,
                        Latitude = x.Place.Location.Latitude,
                        Longitude = x.Place.Location.Longitude,
                        NameDirectorate = x.Place.Location.Directorate.Name,
                        NameDistrict = x.Place.Location.District.Name
                    },

                    Phones = x.Place.Phones.Select(p => p.Number).ToList(),

                    Images = x.Place.Images.Select(i => i.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return Result<RequestDetailsDto>.NotFound("Request not found");

            return Result<RequestDetailsDto>.Success(dto, "Request Details");
        }












    }
}
