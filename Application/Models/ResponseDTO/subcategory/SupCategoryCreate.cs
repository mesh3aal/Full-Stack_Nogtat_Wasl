using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Layer.Models.SendDTO.subcategory
{
    public class SupCategoryCreate
    {
        //public int id { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public string? SubCategoryIcon { get; set; }



        public static SubCategory ToEntity(SupCategoryCreate dto)
        {
            return new SubCategory
            {
                supCategoryIcon = dto.SubCategoryIcon,
                Name = dto.SubCategoryName,
                

            };
        }

    }
}
