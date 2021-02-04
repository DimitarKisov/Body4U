namespace Body4U.Data.Models.Helper
{
    public class ResponseData<T> : Response
    {
        private ResponseData(T data, bool isValid, string errorKey = null) : base(isValid, errorKey)
        {
            this.Data = data;
        }

        public T Data { get; set; }

        public static new ResponseData<T> BadResponse(string errorKey)
        {
            return new ResponseData<T>(default(T), false, errorKey);
        }

        public static new ResponseData<T> BadResponse(T t, string errorKey)
        {
            return new ResponseData<T>(t, false, errorKey);
        }

        public static ResponseData<T> CorrectResponse(T data)
        {
            return new ResponseData<T>(data, true);
        }
    }
}
