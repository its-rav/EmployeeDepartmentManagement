namespace BusinessTier.Responses
{
    public class BaseResponse<T> where T : class
    {
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
