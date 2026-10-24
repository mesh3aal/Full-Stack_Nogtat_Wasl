using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Layer.Models.SendDTO.subcategory
{
    public class SupCategoryUpdatedto
    {
        public int id { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public string? SubCategoryIcon { get; set; }



        public static SubCategory Update(SubCategory subCategory, SupCategoryUpdatedto dto)
        {

            subCategory.supCategoryIcon = dto.SubCategoryIcon;
            subCategory.Name = dto.SubCategoryName;
            return subCategory;


            
        }







        public static SubCategory ToEntity(SupCategoryUpdatedto dto)
        {
            return new SubCategory
            {
                supCategoryIcon = dto.SubCategoryIcon,
                Name = dto.SubCategoryName,


            };
        }



    }
}
