namespace FipeLib.Exceptions;

/// <summary>
/// Fipe exception
/// </summary>
public class FipeException : FipeHttpResponseException
{

    public string Codigo { get; }
    public string Erro { get; }
    public override string? Source { get; set; } = "FipeLib";
    public override string Message => $"{Codigo}|{Erro}";

    public FipeException(string codigo, string erro) 
        : base($"{codigo}|{erro}", null, System.Net.HttpStatusCode.OK)
    {
        Codigo = codigo;
        Erro = erro;
    }

    public FipeException(string codigo, string erro, Exception? innerException) 
        : base($"{codigo}|{erro}", innerException, System.Net.HttpStatusCode.OK)
    {
        Codigo = codigo;
        Erro = erro;
    }

    internal FipeException(Internal.ErrorModel model) : this(model.Codigo, model.Erro)
    {
    }

    internal FipeException(Internal.ErrorModel model, Exception? innerException) : this(model.Codigo, model.Erro, innerException)
    {
    }
}