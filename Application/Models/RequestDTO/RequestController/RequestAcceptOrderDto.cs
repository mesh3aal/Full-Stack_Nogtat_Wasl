namespace Application_Layer.Models.RequestDTO.RequestController
{
    public class RequestAcceptOrderDto
    {
        [Required(ErrorMessage = "معرف الطلب مطلوب")]
        public int OrderId { get; set; }
    }
}
