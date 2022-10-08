using System;
using System.Collections.Generic;
using UPS.Assessment.Infrastructure.Helpers;

namespace UPS.Assessment.Infrastructure.Extensions;

public static class ServiceResponseExtensions
{
    public static ServiceResponse WithMessage(this ServiceResponse serviceResponse, string message)
    {
        serviceResponse.Message = message;
        return serviceResponse;
    }

    public static ServiceResponse WithError(this ServiceResponse serviceResponse, string error)
    {
        serviceResponse.AddError(error);
        return serviceResponse;
    }

    public static ServiceResponse WithError(this ServiceResponse serviceResponse, string errorCode, string errorMessage, string errorDetails)
    {
        serviceResponse.AddError(errorCode, errorMessage, errorDetails);
        return serviceResponse;
    }



    public static ServiceResponse WithErrors(this ServiceResponse serviceResponse, List<ResponseError> errors)
    {
        serviceResponse.AddErrors(errors);
        return serviceResponse;
    }

    public static ServiceResponse Failed(this ServiceResponse serviceResponse)
    {
        serviceResponse.Succeeded = false;
        return serviceResponse;
    }

    public static ServiceResponse Successful(this ServiceResponse serviceResponse)
    {
        serviceResponse.Succeeded = true;
        return serviceResponse;
    }

    public static ServiceResponse WithException(this ServiceResponse serviceResponse, Exception exception)
    {
        serviceResponse.Exception = exception;
        return serviceResponse;
    }

    public static ServiceResponse<T> WithMessage<T>(this ServiceResponse<T> serviceResponse, string message)
    {
        serviceResponse.Message = message;
        return serviceResponse;
    }

    public static ServiceResponse<T> WithData<T>(this ServiceResponse<T> serviceResponse, T data)
    {
        serviceResponse.Data = data;
        return serviceResponse;
    }

    public static ServiceResponse<T> WithError<T>(this ServiceResponse<T> serviceResponse, string error)
    {
        serviceResponse.AddError(error);
        return serviceResponse;
    }

    public static ServiceResponse<T> WithErrors<T>(this ServiceResponse<T> serviceResponse, List<ResponseError> errors)
    {
        serviceResponse.AddErrors(errors);
        return serviceResponse;
    }

    public static ServiceResponse<T> WithError<T>(this ServiceResponse<T> serviceResponse, string errorCode, string errorMessage, string errorDetails)
    {
        serviceResponse.AddError(errorCode, errorMessage, errorDetails);
        return serviceResponse;
    }

    public static ServiceResponse<T> Failed<T>(this ServiceResponse<T> serviceResponse)
    {
        serviceResponse.Succeeded = false;
        return serviceResponse;
    }

    public static ServiceResponse<T> Successful<T>(this ServiceResponse<T> serviceResponse)
    {
        serviceResponse.Succeeded = true;
        return serviceResponse;
    }

    public static ServiceResponse<T> WithException<T>(this ServiceResponse<T> serviceResponse, Exception exception)
    {
        serviceResponse.Exception = exception;
        return serviceResponse;
    }

    public static ServiceResponse<T> WithCount<T>(this ServiceResponse<T> serviceResponse, int count)
    {
        serviceResponse.ItemsCount = count;
        return serviceResponse;
    }



}