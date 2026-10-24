using Microsoft.AspNetCore.Mvc;

namespace Application_Layer.Models.RequestDTO.StoreController;

public record EditStore
{
    [FromQuery]
    public int PlaceId { get; set; } 
    public IFormFile? CommercialRegisterImage { get; set; }
    public List<IFormFile>? StoreImages { get; set; }
    public PlaceStatus? Status { get; set; }
    public string? CommercialRegisterNumber { get; set; }
    public string? CompanyName { get; set; }
    [MinLength(6,ErrorMessage = "لايمكن ان يكون اقل من 6 احرف"), MaxLength(50,ErrorMessage = "لايمكن ان يتجاوز اكثر من 50 حرفاً")]
    public string? Name { get; set; }
    public int? SubCategoryId { get; set; }
    public string? Description { get; set; }
    public TimeOnly? OpeningTime { get; set; } 
    public TimeOnly? ClosingTime { get; set; }
    public int? DirectorateId { get; set; }
    public int? DistrictId { get; set; }
    public string? NearestLandmark { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; } 
    public List<string>? PhoneNumbers { get; set; }
}