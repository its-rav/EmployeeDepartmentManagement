namespace BusinessTier.Responses
{
    public class ErrorResponse
    {
        public ErrorDetailResponse Error { get; private set; }

        public ErrorResponse(string errorCode, string message)
        {
            Error = new ErrorDetailResponse
            {
                Code = errorCode,
                Message = message
            };
        }
        public ErrorResponse(string errorCode)
        {
            Error = new ErrorDetailResponse
            {
                Code = errorCode
            };
        }

    }

    public class ErrorDetailResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
