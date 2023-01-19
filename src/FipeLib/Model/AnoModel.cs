using System.Text.Json.Serialization;

namespace FipeLib.Model;

public class AnoModel
{
    /// <summary>
    /// Zero km
    /// </summary>
    public const int ZERO_KM_YEAR = 32000;

    /// <summary>
    /// Ano mínimo
    /// </summary>
    public const int MIN_YEAR = 1900;

    [JsonPropertyName("Label")]
    public string Label { get; }

    [JsonPropertyName("Value")]
    public string Value { get; }

    [JsonIgnore]
    internal int Year { get; }

    [JsonIgnore]
    internal int TipoCombustivel { get; }

    public AnoModel(string label, string value)
    {
        if (string.IsNullOrEmpty(label))
            throw new ArgumentNullException(nameof(label));

        if (string.IsNullOrEmpty(value) ||
            !int.TryParse(value.Split('-').First(), out int year) ||
            !int.TryParse(value.Split('-').Last(), out int tipoCombustivel))
            throw new ArgumentNullException(nameof(value));

        TipoCombustivel = tipoCombustivel;
        Year = year;
        Label = label;
        Value = value;
    }
}