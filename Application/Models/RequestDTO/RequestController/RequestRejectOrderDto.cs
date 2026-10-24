using System.ComponentModel.DataAnnotations;

namespace Application_Layer.Models.ReciveDTOs.RequestDTOs
{
    public class RequestRejectOrderDto
    {
        [Required(ErrorMessage = "معرف الطلب مطلوب")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "سبب الرفض مطلوب")]
        [MaxLength(1000, ErrorMessage = "سبب الرفض لا يمكن أن يتجاوز 1000 حرفاً")]
        public string Reason { get; set; } = null!;
    }
}
