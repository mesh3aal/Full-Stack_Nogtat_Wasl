
namespace Application_Layer.Helper
{
    public class ErrorDetails
    {

        public string ErrorMessage { get; set; }
        public string Field { get; set; }

        public ErrorDetails(string ErrorMessage ,string Field)
        {
            this.ErrorMessage=ErrorMessage;
            this.Field = Field;
        }
        public ErrorDetails()
        {
            
        }


    }
}
