namespace FipeLib.Model;

public class MarcaModel
{
    public string Label { get; } = string.Empty;
    public string Value { get; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public TabelaReferenciaModel? TabelaReferencia { get; internal set; }

    public MarcaModel(string label, string value)
    {
        Label = label;
        Value = value;
    }
}