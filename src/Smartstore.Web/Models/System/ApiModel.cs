namespace Smartstore.Web.Models.System
{
    public class ApiModel
    {
        public bool IsWebsiteClosed { get; set; }
        public bool Unauthorized { get; set; }
    }

    public class GenericApiModel<T>
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public bool NotFound { get; set; }
        public T? Data { get; set; }

        public GenericApiModel<T> Error(string errorMessage = "")
        {
            this.Message = errorMessage;
            this.IsValid = false;
            return this;
        }

        public GenericApiModel<T> Success(T data)
        {
            this.Data = data;
            this.IsValid = true;
            return this;
        }

        public GenericApiModel<T> NoFound()
        {
            this.NotFound = true;
            return this;
        }
    }
}
