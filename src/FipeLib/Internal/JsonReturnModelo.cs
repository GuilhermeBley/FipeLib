using FipeLib.Model;

namespace FipeLib.Internal;

public sealed class JsonReturnModeloModel
{
    [System.Text.Json.Serialization.JsonPropertyName("Modelos")]
    public IEnumerable<ModeloModel> Modelos { get; set; } = Enumerable.Empty<ModeloModel>();

    [System.Text.Json.Serialization.JsonPropertyName("Anos")]
    public IEnumerable<AnoModel> Anos { get; set; } = Enumerable.Empty<AnoModel>();
}