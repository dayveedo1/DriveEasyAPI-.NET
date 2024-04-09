using System.Text.Json;

namespace DriveEasy.API.DriveEasy.Dto
{
    public class ViewApiResponse
    {
        public int ResponseStatus { get; set; }
        public string? ResponseMessage { get; set; }
        public object? ResponseData { get; set; }

    }


    /* Global Exception Object Class (Not in use) */
    public class ViewErrorResponse
    {
        public int ResponseStatus { get; set; }
        public string? ResponseMessage { get; set; }
        public object? ResponseData { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
