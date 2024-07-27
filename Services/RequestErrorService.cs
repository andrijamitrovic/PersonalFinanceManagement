using PersonalFinanceManagement.Models;

namespace PersonalFinanceManagement.Services
{
    public class RequestErrorService : IRequestErrorService
    {
        public RequestError GetFileNameError()
        {
            return new RequestError
            {
                Tag = "file-name-error",
                Error = "invalid-format",
                Message = "File type must be .csv"
            };
        }
    }
}
