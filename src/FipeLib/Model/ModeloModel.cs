using System.Text.Json.Serialization;

namespace FipeLib.Model;

public class ModeloModel
{
    public string Label { get; } = string.Empty;
    public int Value { get; }

    [System.Text.Json.Serialization.JsonIgnore]
    public MarcaModel? Marca { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    internal IEnumerable<AnoModel> AvailableYears { get; set; } = Enumerable.Empty<AnoModel>();

    public ModeloModel(string label, int value)
    {
        if (string.IsNullOrEmpty(label))
            throw new ArgumentNullException(nameof(label));
            
        if (value == 0)
            throw new ArgumentNullException(nameof(value));

        Label = label;
        Value = value;
    }
}