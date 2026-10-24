using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Layer.Models.ReciveDTOs.UserManagementController.UserManagementDto
{
    public class UserManegeFindResponse
    {

        public string id { get; set; } = null!;
        public string fullName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string userStatus { get; set; }
        public string UserType { get; set; }

    }
}
