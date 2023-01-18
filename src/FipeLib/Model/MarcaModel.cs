namespace FipeLib.Model;

public class MarcaModel
{
    public string Label { get; } = string.Empty;
    public string Value { get; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public TabelaReferenciaModel? TabelaReferencia { get; internal set; }

    public MarcaModel(string label, string value)
    {
        if (string.IsNullOrEmpty(label))
            throw new ArgumentNullException(nameof(label));
            
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        Label = label;
        Value = value;
    }
}