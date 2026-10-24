
namespace Application_Layer.Models.RequestDTO.PlaceController
{
    public class DirectorateDto
    {
        public int id {  get; set; }
        public string Name { get; set; } = null!;

        public List<DistrictDto> Districts { get; set; }= new List<DistrictDto>();

    }
}
