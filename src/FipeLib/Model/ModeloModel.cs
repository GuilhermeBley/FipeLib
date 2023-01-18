namespace FipeLib.Model;

public class ModeloModel
{
    public string Label { get; } = string.Empty;
    public string Value { get; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public MarcaModel? Marca { get; internal set; }

    public ModeloModel(string label, string value)
    {
        if (string.IsNullOrEmpty(label))
            throw new ArgumentNullException(nameof(label));
            
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        Label = label;
        Value = value;
    }
}