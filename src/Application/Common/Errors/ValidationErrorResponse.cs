namespace Application.Common.Errors;

public class ValidationErrorResponse : ErrorResponse
{
    public ValidationErrorResponse() : base(400, null)
    {
    }
    public Dictionary<string,IEnumerable<string>> Errors { get; set; }
}