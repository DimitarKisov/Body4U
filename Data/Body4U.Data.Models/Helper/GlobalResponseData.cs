namespace Body4U.Data.Models.Helper
{
    public class GlobalResponseData<T> : GlobalResponse
    {
        private GlobalResponseData(T data, bool isValid, string errorKey = null) : base(isValid, errorKey)
        {
            this.Data = data;
        }

        public T Data { get; set; }

        public static new GlobalResponseData<T> BadResponse(string errorKey)
        {
            return new GlobalResponseData<T>(default(T), false, errorKey);
        }

        public static new GlobalResponseData<T> BadResponse(T t, string errorKey)
        {
            return new GlobalResponseData<T>(t, false, errorKey);
        }

        public static GlobalResponseData<T> CorrectResponse(T data)
        {
            return new GlobalResponseData<T>(data, true);
        }
    }
}
