using FipeLib.Internal;
using FipeLib.Internal.HttpExtension;
using FipeLib.Model;
using FipeLib.Exceptions;
using System.Text.Json;

namespace FipeLib.Services;

public sealed class FipeQuery : IFipeQuery
{
    private const string UrlApi = "https://veiculos.fipe.org.br/api/veiculos";
    private const string UrlConsultarTabelaDeReferencia = $"{UrlApi}//ConsultarTabelaDeReferencia";
    private const string UrlConsultarMarcas = $"{UrlApi}//ConsultarMarcas";
    private static readonly TabelaReferenciaSession _tabelaReferenciaSession = new();
    private readonly static HttpClient __client = CreateClientTabelaFipe();
    private HttpClient _client => __client;

    public IEnumerable<TabelaReferenciaModel> GetAllTabelaReferencia()
    {
        return GetAllTabelaReferenciaAsync().GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<TabelaReferenciaModel>> GetAllTabelaReferenciaAsync()
    {
        return await _tabelaReferenciaSession.GetListTabelaReferenciaModelAsync();
    }
    public IEnumerable<MarcaModel> GetMarca()
    {
        return GetMarcaAsync().GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<MarcaModel>> GetMarcaAsync()
    {
        return await GetMarcaAsync(
            await GetDefaultTabelaReferencia());
    }

    public IEnumerable<MarcaModel> GetMarca(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        return GetMarcaAsync(tabelaReferenciaModel).GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<MarcaModel>> GetMarcaAsync(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        return await GetAllMarcaAsync(tabelaReferenciaModel);
    }

    private async Task<IEnumerable<MarcaModel>> GetAllMarcaAsync(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        if (tabelaReferenciaModel is null)
            return Enumerable.Empty<MarcaModel>();
        
        return 
            (await GetCarrosAndUtilitariosMarcaAsync(tabelaReferenciaModel))
            .Concat(await GetCaminhoesAndMicroOnibusMarcaAsync(tabelaReferenciaModel))
            .Concat(await GetMotosMarcaAsync(tabelaReferenciaModel));
    }

    private async Task<IEnumerable<MarcaModel>> GetMotosMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(UrlConsultarMarcas, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.Moto));
        
        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            return marcaModel;
        });
    }

    private async Task<IEnumerable<MarcaModel>> GetCarrosAndUtilitariosMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(UrlConsultarMarcas, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.CarrosAndUtilitarios));

        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            return marcaModel;
        });
    }

    private async Task<IEnumerable<MarcaModel>> GetCaminhoesAndMicroOnibusMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(UrlConsultarMarcas, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.CaminhoesAndMicroOnibus));

        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            return marcaModel;
        });
    }

    private (string key, string value)[] GetFormConsultarMarcas(TabelaReferenciaModel tabelaReferenciaModel, TypeMarcaEnum marca)
    {
        return new[] { 
            ("codigoTabelaReferencia", tabelaReferenciaModel.Codigo.ToString()),  
            ("codigoTipoVeiculo", ((sbyte)marca).ToString()),
        };
    }

    private static async Task<TabelaReferenciaModel> GetDefaultTabelaReferencia()
    {
        return (await _tabelaReferenciaSession.GetListTabelaReferenciaModelAsync()).First();
    }

    /// <summary>
    /// Check status code and content response
    /// </summary>
    /// <param name="response">http response</param>
    /// <exception cref="FipeHttpResponseException"></exception>
    /// <exception cref="FipeException"></exception>
    private static async Task CheckAndThrowIfContainsError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new FipeHttpResponseException(await response.Content.ReadAsStringAsync(), response.StatusCode);

        var streamResponse = await response.Content.ReadAsStreamAsync();
        var error = await TryGetError(streamResponse);
        if (error is not null)
            throw new FipeException(error);
        streamResponse.Position = 0;
    }

    private static async Task<ErrorModel?> TryGetError(Stream stream)
    {
        if (stream is null)
            return null;

        try
        {
            return await JsonSerializer.DeserializeAsync<ErrorModel?>(stream);
        }
        catch
        {
            return null;
        }
    }

    private static HttpClient CreateClientTabelaFipe()
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Referrer = new Uri("http://veiculos.fipe.org.br");
        client.DefaultRequestHeaders.Host = "veiculos.fipe.org.br";

        return client;
    }

    /// <summary>
    /// All instances get the data in the same session
    /// </summary>
    private class TabelaReferenciaSession
    {
        private static readonly object lockTabelaReferencia = new();
        private static (DateTime LastRefresh, OrderedEnumerableTabelaReferencia? Data) _tabelaReferenciaEnumerable = (DateTime.Now, null);

        public async Task<IEnumerable<TabelaReferenciaModel>> GetListTabelaReferenciaModelAsync()
        {
            if (ShouldRefreshData())
                _tabelaReferenciaEnumerable = (DateTime.Now, 
                    new OrderedEnumerableTabelaReferencia(await PrivateGetListTabelaReferenciaModel()));

            return _tabelaReferenciaEnumerable.Data!;
        }

        public bool ShouldRefreshData()
        {
            if (_tabelaReferenciaEnumerable.Data is null)
                return true;

            if (_tabelaReferenciaEnumerable.LastRefresh.Year != DateTime.Now.Year ||
                _tabelaReferenciaEnumerable.LastRefresh.Month != DateTime.Now.Month)
                return true;

            return false;
        }

        private async Task<IEnumerable<TabelaReferenciaModel>> PrivateGetListTabelaReferenciaModel()
        {
            using var response = await __client.PostAsync(UrlConsultarTabelaDeReferencia, null);

            using var streamJson = await response.Content.ReadAsStreamAsync();

            await CheckAndThrowIfContainsError(response);

            return await JsonSerializer.DeserializeAsync<IEnumerable<TabelaReferenciaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<TabelaReferenciaModel>).Name}.");
        }
    }
}