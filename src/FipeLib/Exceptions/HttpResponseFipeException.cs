using System.Net;

namespace FipeLib.Exceptions;

/// <summary>
/// Fipe exception
/// </summary>
public class FipeHttpResponseException : HttpRequestException
{
    public override string? Source { get; set; } = "FipeLib";
    
    public FipeHttpResponseException(string? message, HttpStatusCode? statusCode) 
        : base(message, null, statusCode)
    {
    }

    public FipeHttpResponseException(string? message, Exception? inner, HttpStatusCode? statusCode) 
        : base(message, inner, statusCode)
    {
    }
}