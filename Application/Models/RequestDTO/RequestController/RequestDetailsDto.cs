
using Application_Layer.Models.RequestDTO.CommericalRegisterationController;

namespace Application_Layer.Models.RequestDTO.RequestController
{
    public class RequestDetailsDto
    {
        public RequestStates State { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public string? WorkHours { get; set; }


        public CommericalRegisterationDto CommericalRegisteration { get; set; } = null!;
        public RequestForDataPlaseDto DataPlase { get; set; } = null!;
        public RequestForLocationDto LocationAndAddres { get; set; } = null!;

        public List<string>? Phones { get; set; } = new List<string>();
        public List<string>? Images { get; set; } = new List<string>();

    }


    public class RequestForDataPlaseDto
    {

        public string PlaceName {  get; set; }
        public string MineCategoryName {  get; set; }
        public string SupCategoryName {  get; set; }
        public string? Description { get; set; }


    }



    public class RequestForLocationDto
    {

        public string NameDistrict { get; set; }
        public string NameDirectorate { get; set; }
        public string NearestLandmark { get; set; } = null!;
        // خط العرض
        public double? Latitude { get; set; }
        // خط طول
        public double? Longitude { get; set; }


    }


}
