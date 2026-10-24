namespace Application_Layer.Models.RequestDTO.PlaceController;

public enum SortingOptions
{
    LowestRating = 1, 
    MostReviewed = 2,
    LeastReviewed = 3,
    Newest = 4,
    Earliest = 5
}

public record CategoriesPageRequest
{
    [Required(ErrorMessage = "ادخل اسم القسم مطلوب")]
    [MaxLength(150, ErrorMessage = "اسم القسم لا يمكن أن يتجاوز 150 حرفاً")]
    public string CategoryName { get; set; } = null!;
    
    [MaxLength(150, ErrorMessage = "اسم القسم الفرعي لا يمكن أن يتجاوز 150 حرفاً")]
    public string? SubCategoryName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "رقم الصفحة يجب أن يكون 1 أو أكثر")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "حجم الصفحة يجب أن يكون بين 1 و 100")]
    public int PageSize { get; set; } = 10;
    
    public SortingOptions? SortingOptions { get; set; } = null;
}
