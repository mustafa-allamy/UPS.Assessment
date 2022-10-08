using System;
using System.Collections.Generic;

namespace UPS.Assessment.Infrastructure.Helpers;

public class ServiceResponse : IServiceResponse
{
    private readonly List<IResponseError> _errors = new();

    public ServiceResponse()
    {
        Succeeded = true;
    }
    public bool Failed => !Succeeded;
    public IReadOnlyCollection<IResponseError> Errors => _errors.AsReadOnly();
    public string? Message { get; set; }
    public bool Succeeded { get; internal set; }
    public int ItemsCount { get; set; } = 1;
    public Exception Exception { get; set; }
    public string MessageWithErrors => $"{string.Join(',', _errors)}";
    public bool IsPaginated => ItemsCount > 1;
    public void AddError(string errorMessage)
    {
        _errors.Add(new ResponseError(errorMessage));
    }
    public void AddErrors(IEnumerable<IResponseError> errors)
    {
        _errors.AddRange(errors);
    }
    public void AddError(string errorCode, string errorMessage, string errorDetails)
    {
        _errors.Add(new ResponseError(errorCode, errorMessage, errorDetails));
    }
}

public class ServiceResponse<T> : ServiceResponse, IServiceResponse<T>
{
    public ServiceResponse() : base() { }
    public T? Data { get; internal set; }
    public void SetSuccessResponse(T value, string? msg = null)
    {
        Message = msg;
        Data = value;
        Succeeded = true;
    }
}