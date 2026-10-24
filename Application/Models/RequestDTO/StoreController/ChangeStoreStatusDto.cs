namespace Application_Layer.Models.RequestDTO.StoreController
{
    public class ChangeStoreStatusDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الحالة الجديدة مطلوبة")]
        public PlaceStatus Status { get; set; }
    }
}
