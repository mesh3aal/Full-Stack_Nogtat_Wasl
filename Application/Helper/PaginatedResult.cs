
namespace Application_Layer.Helper
{
    public class PaginatedResult<T>
    {

        public List<T> Items { get; set; } = new List<T>();
        public PaginatedResult(List<T> items)
        {
            Items = items;
        }



        public PaginatedResult(List<T> Items , int count, int Page=1,int PageSize=10 )
        {
            
            this.Items = Items;
            this.CurrentPage = Page;
            this.PageSize = PageSize;
            this.TotalCount = count;
            this.TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        }


        public static PaginatedResult<T>Sucess(List<T> Items, int count, int Page, int PageSize)
        {
            return new(Items, count, Page, PageSize);
        }



        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages {  get; set; }
        public int  CurrentPage { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => PageSize * CurrentPage < TotalCount;



    }
}
