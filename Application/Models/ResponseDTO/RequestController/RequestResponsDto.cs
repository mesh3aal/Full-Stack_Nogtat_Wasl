namespace Application_Layer.Models.SendDTO.RequestController
{
    public class RequestResponsDto
    {
        public int RequestId {  get; set; }
        public string PlaceName { get; set; } = null!;
        public string ImagePlaceUrl { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public DateTime CreatedAt { get; init; }
        public string TypeRequest { get; init; } = "«·„«·ﬂ";
        public RequestStates States { get; init; } 


    }
}
