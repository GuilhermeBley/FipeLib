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
    public string Valor { get; }

    [JsonPropertyName("Marca")]
    public string Marca { get; }

    [JsonPropertyName("Modelo")]
    public string Modeloo { get; }

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
    public ModeloModel? ModeloVehicle { get; internal set; }

    public VehicleModel(string valor, string marca, string modeloo, int anoModelo, string combustivel, string codigoFipe, string mesReferencia, string autenticacao, int tipoVeiculo, string siglaCombustivel, string dataConsulta)
    {
        Valor = valor;
        Marca = marca;
        Modeloo = modeloo;
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