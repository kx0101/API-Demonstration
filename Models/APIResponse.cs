using System.Net;

namespace apiprac
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; }

        public List<string> ErrorMessages { get; set; }

        public object? Data { get; set; }
    }
}
