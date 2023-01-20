using System.Text.Json.Serialization;

namespace FipeLib.Model;

public class VehicleModel
{
    /// <summary>
    /// Vehicle Zero km
    /// </summary>
    public const int ZERO_KM_YEAR = AnoModel.ZERO_KM_YEAR;

    /// <summary>
    /// Min year vehicle
    /// </summary>
    public const int MIN_YEAR = AnoModel.MIN_YEAR;

    [JsonPropertyName("Valor")]
    public string Valor { get; internal set; } = string.Empty;

    [JsonPropertyName("Marca")]
    public string Marca { get; internal set; } = string.Empty;

    [JsonPropertyName("Modelo")]
    public string NameModelo { get; internal set; } = string.Empty;

    [JsonPropertyName("AnoModelo")]
    public int AnoModelo { get; internal set; }

    [JsonPropertyName("Combustivel")]
    public string Combustivel { get; internal set; } = string.Empty;

    [JsonPropertyName("CodigoFipe")]
    public string CodigoFipe { get; internal set; } = string.Empty;

    [JsonPropertyName("MesReferencia")]
    public string MesReferencia { get; internal set; } = string.Empty;

    [JsonPropertyName("Autenticacao")]
    public string Autenticacao { get; internal set; } = string.Empty;

    [JsonPropertyName("TipoVeiculo")]
    public int TipoVeiculo { get; internal set; }

    [JsonPropertyName("SiglaCombustivel")]
    public string SiglaCombustivel { get; internal set; } = string.Empty;

    [JsonPropertyName("DataConsulta")]
    public string DataConsulta { get; internal set; } = string.Empty;

    [JsonIgnore]
    public ModeloModel? Modelo { get; internal set; }

    public VehicleModel(string valor, string marca, string nameModelo, int anoModelo, string combustivel, string codigoFipe, string mesReferencia, string autenticacao, int tipoVeiculo, string siglaCombustivel, string dataConsulta)
    {
        if (string.IsNullOrEmpty(CodigoFipe))
            throw new ArgumentNullException(nameof(codigoFipe));

        Valor = valor;
        Marca = marca;
        NameModelo = nameModelo;
        AnoModelo = anoModelo;
        Combustivel = combustivel;
        CodigoFipe = codigoFipe;
        MesReferencia = mesReferencia;
        Autenticacao = autenticacao;
        TipoVeiculo = tipoVeiculo;
        SiglaCombustivel = siglaCombustivel;
        DataConsulta = dataConsulta;
    }
}