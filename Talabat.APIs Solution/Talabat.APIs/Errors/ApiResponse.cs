namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode , string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request, You Have Made !",
                401 => "Not Authorized !",
                404 => "Resource Was Not Found !",
                500 => "Errors Path of Dark Side......",
                _ => null // default lw mga4 ay haga
            };
        }
    }
}
