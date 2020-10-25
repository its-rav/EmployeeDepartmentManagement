namespace BusinessTier.Responses
{
    public class BaseResponse<T> where T : class
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
