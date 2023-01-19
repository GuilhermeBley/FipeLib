using System.Text.Json.Serialization;

namespace FipeLib.Model;

public class VehicleModel
{
    /// <summary>
    /// Zero km
    /// </summary>
    public const int ZERO_KM_YEAR = 32000;

    /// <summary>
    /// Ano mínimo
    /// </summary>
    public const int MIN_YEAR = 1900;

    [JsonPropertyName("Valor")]
    public string Valor { get; }

    [JsonPropertyName("Marca")]
    public string Marca { get; }

    [JsonPropertyName("Modelo")]
    public string NomeModelo { get; }

    [JsonPropertyName("AnoModelo")]
    public int AnoModelo { get; }

    [JsonPropertyName("Combustivel")]
    public string Combustivel { get; }

    [JsonPropertyName("CodigoFipe")]
    public string CodigoFipe { get; }

    [JsonPropertyName("MesReferencia")]
    public string MesReferencia { get; }

    [JsonPropertyName("Autenticacao")]
    public string Autenticacao { get; }

    [JsonPropertyName("TipoVeiculo")]
    public int TipoVeiculo { get; }

    [JsonPropertyName("SiglaCombustivel")]
    public string SiglaCombustivel { get; }

    [JsonPropertyName("DataConsulta")]
    public string DataConsulta { get; }

    [JsonIgnore]
    public ModeloModel? Modelo { get; set; }

    public VehicleModel(string valor, string marca, string modelo, int anoModelo, string combustivel, string codigoFipe, string mesReferencia, string autenticacao, int tipoVeiculo, string siglaCombustivel, string dataConsulta)
    {
        Valor = valor;
        Marca = marca;
        NomeModelo = modelo;
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