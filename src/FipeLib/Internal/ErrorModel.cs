using System.Text.Json.Serialization;

namespace FipeLib.Internal;

internal class ErrorModel
{
    [JsonPropertyName("codigo")]
    public string Codigo { get; }

    [JsonPropertyName("erro")]
    public string Erro { get; }

    public ErrorModel(string codigo, string erro)
    {
        if (string.IsNullOrEmpty(codigo))
            throw new ArgumentNullException(nameof(codigo));
        
        if (string.IsNullOrEmpty(erro))
            throw new ArgumentNullException(nameof(erro));

        Codigo = codigo;
        Erro = erro;
    }
}