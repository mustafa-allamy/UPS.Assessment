namespace UPS.Assessment.Infrastructure.Helpers;

public interface IResponseError
{
    string Message { get; set; }
    string DetailMessage { get; set; }
    string ErrorCode { get; set; }
}