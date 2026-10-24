using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Layer.Models.SendDTO.RequestController
{
    public class RejectOrderRequest
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
}
