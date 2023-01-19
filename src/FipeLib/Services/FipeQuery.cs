using FipeLib.Internal;
using FipeLib.Internal.HttpExtension;
using FipeLib.Model;
using FipeLib.Exceptions;
using System.Text.Json;
using System.Linq;

namespace FipeLib.Services;

public sealed class FipeQuery : IFipeQuery
{
    private const string URL_API = "https://veiculos.fipe.org.br/api/veiculos";
    private const string URL_CONSULTA_TABELA_REFERENCIA = $"{URL_API}//ConsultarTabelaDeReferencia";
    private const string URL_CONSULTA_MARCAS = $"{URL_API}//ConsultarMarcas";
    private const string URL_CONSULTA_MODELOS = $"{URL_API}//ConsultarModelos";
    private const string URL_CONSULTA_ANOS = $"{URL_API}//ConsultarAnoModelo";
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
    public IEnumerable<MarcaModel> GetMarcas()
    {
        return GetMarcasAsync().GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<MarcaModel>> GetMarcasAsync()
    {
        return await GetMarcasAsync(
            await GetDefaultTabelaReferencia());
    }

    public IEnumerable<MarcaModel> GetMarcas(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        return GetMarcasAsync(tabelaReferenciaModel).GetAwaiter().GetResult();
    }

    
    public async IAsyncEnumerable<MarcaModel> GetMarcasAsyncEnumerable()
    {
        var tabelaReferenciaModel = await GetDefaultTabelaReferencia();
        await foreach (var marcaModel in GetMarcasAsyncEnumerable(tabelaReferenciaModel))
            yield return marcaModel;
    }

    public async IAsyncEnumerable<MarcaModel> GetMarcasAsyncEnumerable(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        await foreach (var marcaModel in GetAllMarcaAsyncAsyncEnumerable(tabelaReferenciaModel))
            yield return marcaModel;
    }

    public async Task<IEnumerable<MarcaModel>> GetMarcasAsync(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        return await GetAllMarcaAsync(tabelaReferenciaModel);
    }

    public IEnumerable<ModeloModel> GetModelos(MarcaModel? marcaModel)
    {
        return GetModelosAsync(marcaModel).GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<ModeloModel>> GetModelosAsync(MarcaModel? marcaModel)
    {
        return await GetModelosAsyncEnumerable(marcaModel).ToListAsync();
    }

    public async IAsyncEnumerable<ModeloModel> GetModelosAsyncEnumerable(MarcaModel? marcaModel)
    {
        await foreach (var modelo in GetAllModelos(marcaModel))
            yield return modelo;
    }
    
    public IEnumerable<AnoModel> GetAnosByModelo(ModeloModel? modeloModel)
    {
        return GetAnosByModeloAsync(modeloModel).GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<AnoModel>> GetAnosByModeloAsync(ModeloModel? modeloModel)
    {
        return await GetAnosByModeloAsyncEnumerable(modeloModel).ToListAsync();
    }

    public async IAsyncEnumerable<AnoModel> GetAnosByModeloAsyncEnumerable(ModeloModel? modeloModel)
    {
        await foreach (var anoModel in GetAllAnosByModeloAsyncEnumerable(modeloModel))
        {
            yield return anoModel;
        }
    }
    
    public VehicleModel? GetVehicleOrDefault(ModeloModel? modeloModel, int year)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, int year)
    {
        throw new NotImplementedException();
    }

    public VehicleModel? GetVehicleOrDefault(ModeloModel? modeloModel, AnoModel anoModel)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, AnoModel anoModel)
    {
        throw new NotImplementedException();
    }

    private async IAsyncEnumerable<AnoModel> GetAllAnosByModeloAsyncEnumerable(ModeloModel? modeloModel)
    {
        if (modeloModel is null)
            yield break;

        if (modeloModel.Marca is null)
            yield break;

        if (modeloModel.Marca.TabelaReferencia is null)
            yield break;

        var marcaModel = modeloModel.Marca;

        using var response = await _client.PostUrlEncondedAsync(
            URL_CONSULTA_ANOS, 
            ("codigoTipoVeiculo", ((sbyte)marcaModel.CodigoMarca).ToString()),
            ("codigoTabelaReferencia", marcaModel.TabelaReferencia.Codigo.ToString()),
            ("codigoMarca", marcaModel.Value),
            ("codigoModelo", modeloModel.Value.ToString()));
        
        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var anosModels =  await JsonSerializer.DeserializeAsync<IEnumerable<AnoModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<AnoModel>).Name}.");

        modeloModel.AvailableYears = anosModels;

        foreach (var anoModel in anosModels)
        {
            yield return anoModel;
        }
    }

    private async IAsyncEnumerable<ModeloModel> GetAllModelos(MarcaModel? marcaModel)
    {
        if (marcaModel is null)
            yield break;

        if (marcaModel.TabelaReferencia is null)
            yield break;

        using var response = await _client.PostUrlEncondedAsync(
            URL_CONSULTA_MODELOS, 
            ("codigoTipoVeiculo", ((sbyte)marcaModel.CodigoMarca).ToString()),
            ("codigoTabelaReferencia", marcaModel.TabelaReferencia.Codigo.ToString()),
            ("codigoMarca", marcaModel.Value));
        
        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var internalModelosModels =  await JsonSerializer.DeserializeAsync<JsonReturnModeloModel>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<ModeloModel>).Name}.");

        foreach (var modelo in internalModelosModels.Modelos)
        {
            modelo.Marca = marcaModel;
            modelo.AvailableYears = internalModelosModels.Anos;
            yield return modelo;
        }
    }

    private async Task<IEnumerable<MarcaModel>> GetAllMarcaAsync(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        return await GetAllMarcaAsyncAsyncEnumerable(tabelaReferenciaModel).ToListAsync();
    }

    private async IAsyncEnumerable<MarcaModel> GetAllMarcaAsyncAsyncEnumerable(TabelaReferenciaModel? tabelaReferenciaModel)
    {
        if (tabelaReferenciaModel is null)
            yield break;
        
        foreach (var marcaCarroAndUtilitario in await GetCarrosAndUtilitariosMarcaAsync(tabelaReferenciaModel))
        {
            yield return marcaCarroAndUtilitario;
        }
        
        foreach (var marcaCaminhaoAndMicroOnibus in await GetCaminhoesAndMicroOnibusMarcaAsync(tabelaReferenciaModel))
        {
            yield return marcaCaminhaoAndMicroOnibus;
        }
        
        foreach (var marcaMotos in await GetMotosMarcaAsync(tabelaReferenciaModel))
        {
            yield return marcaMotos;
        }
    }

    private async Task<IEnumerable<MarcaModel>> GetMotosMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(URL_CONSULTA_MARCAS, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.Moto));
        
        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            marcaModel.CodigoMarca = TypeMarcaEnum.Moto;
            return marcaModel;
        });
    }

    private async Task<IEnumerable<MarcaModel>> GetCarrosAndUtilitariosMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(URL_CONSULTA_MARCAS, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.CarrosAndUtilitarios));

        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            marcaModel.CodigoMarca = TypeMarcaEnum.CarrosAndUtilitarios;
            return marcaModel;
        });
    }

    private async Task<IEnumerable<MarcaModel>> GetCaminhoesAndMicroOnibusMarcaAsync(TabelaReferenciaModel tabelaReferenciaModel)
    {
        using var response = await _client.PostUrlEncondedAsync(URL_CONSULTA_MARCAS, 
            GetFormConsultarMarcas(tabelaReferenciaModel, TypeMarcaEnum.CaminhoesAndMicroOnibus));

        using var streamJson = await response.Content.ReadAsStreamAsync();

        await CheckAndThrowIfContainsError(response);

        var marcasModels =  await JsonSerializer.DeserializeAsync<IEnumerable<MarcaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<MarcaModel>).Name}.");

        return marcasModels.Select(marcaModel => 
        {
            marcaModel.TabelaReferencia = tabelaReferenciaModel;
            marcaModel.CodigoMarca = TypeMarcaEnum.CaminhoesAndMicroOnibus;
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
        private static readonly SemaphoreSlim _semaphoreSlim = new(1,1);
        private static (DateTime LastRefresh, OrderedEnumerableTabelaReferencia? Data) _tabelaReferenciaEnumerable = (DateTime.Now, null);

        public async Task<IEnumerable<TabelaReferenciaModel>> GetListTabelaReferenciaModelAsync()
        {
            if (ShouldRefreshData())
                await UpdateWithSemaphore();

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

        private async Task UpdateWithSemaphore(CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);

                if (ShouldRefreshData())
                    _tabelaReferenciaEnumerable = (DateTime.Now, 
                        new OrderedEnumerableTabelaReferencia(await PrivateGetListTabelaReferenciaModel()));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<IEnumerable<TabelaReferenciaModel>> PrivateGetListTabelaReferenciaModel()
        {
            using var response = await __client.PostAsync(URL_CONSULTA_TABELA_REFERENCIA, null);

            using var streamJson = await response.Content.ReadAsStreamAsync();

            await CheckAndThrowIfContainsError(response);

            return await JsonSerializer.DeserializeAsync<IEnumerable<TabelaReferenciaModel>>(streamJson)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<TabelaReferenciaModel>).Name}.");
        }
    }
}