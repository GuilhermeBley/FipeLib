using System.Text.Json.Serialization;

namespace FipeLib.Model;

public class AnoModel
{
    [JsonPropertyName("Label")]
    public string Label { get; }

    [JsonPropertyName("Value")]
    public string Value { get; }

    public AnoModel(string label, string value)
    {
        if (string.IsNullOrEmpty(label))
            throw new ArgumentNullException(nameof(label));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        Label = label;
        Value = value;
    }
}