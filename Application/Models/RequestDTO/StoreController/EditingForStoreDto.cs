namespace Application_Layer.Models.RequestDTO.StoreController
{
    public class EditingForStoreDto
    {
        public int PlaceId { get; set; }
        public PlaceStatus? Status { get; set; }

        // Commercial Register Management
        public string? CommercialRegisterNumber { get; set; }
        public string? CompanyName { get; set; }
        public IFormFile? CommercialRegisterImage { get; set; }

        // Basic Data
        public string? Name { get; set; }
        public int? PrimaryCategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string? Description { get; set; }
        public TimeOnly? OpeningTime { get; set; }
        public TimeOnly? ClosingTime { get; set; }

        // Location
        public int? DirectorateId { get; set; }
        public int? DistrictId { get; set; }
        public string? NearestLandmark { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Additional Images
        public List<IFormFile>? StoreImages { get; set; }

        // Phone Numbers
        public List<string>? PhoneNumbers { get; set; }
    }
}
