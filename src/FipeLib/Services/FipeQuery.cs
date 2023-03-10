using FipeLib.Internal;
using FipeLib.Internal.HttpExtension;
using FipeLib.Model;
using FipeLib.Exceptions;
using System.Text.Json;
using System.Linq.Expressions;

namespace FipeLib.Services;

public sealed class FipeQuery : IFipeQuery
{
    private const string URL_API = "https://veiculos.fipe.org.br/api/veiculos";
    private const string URL_CONSULTA_TABELA_REFERENCIA = $"{URL_API}//ConsultarTabelaDeReferencia";
    private const string URL_CONSULTA_MARCAS = $"{URL_API}//ConsultarMarcas";
    private const string URL_CONSULTA_MODELOS = $"{URL_API}//ConsultarModelos";
    private const string URL_CONSULTA_ANOS = $"{URL_API}//ConsultarAnoModelo";
    private const string URL_CONSULTA_VEHICLE = $"{URL_API}//ConsultarValorComTodosParametros";
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
        return GetVehicleOrDefaultAsync(modeloModel, year).GetAwaiter().GetResult();
    }

    public async Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, int year)
    {
        return await TryGetVehicle(modeloModel, GetCorrectYearFromModeloOrDefault(modeloModel, year));
    }

    public VehicleModel? GetVehicleOrDefault(ModeloModel? modeloModel, AnoModel? anoModel)
    {
        return GetVehicleOrDefaultAsync(modeloModel, anoModel).GetAwaiter().GetResult();
    }

    public async Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, AnoModel? anoModel)
    {
        return await TryGetVehicle(modeloModel, GetCorrectYearFromModeloOrDefault(modeloModel, anoModel));
    }

    public async IAsyncEnumerable<VehicleModel> GetVehicles(
        Expression<Func<TabelaReferenciaModel, bool>>? whereTabelaReferenciaModel = null, 
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null, 
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null, 
        Expression<Func<AnoModel, bool>>? whereAnoModel = null)
    {
        await foreach (var vehicleModel in GetAllVehiclesWithExpressions(whereTabelaReferenciaModel, 
            whereMarcaModel, whereModeloModel, whereAnoModel))
            yield return vehicleModel;
    }

    public async IAsyncEnumerable<VehicleModel> GetVehiclesWithYear(
        Expression<Func<TabelaReferenciaModel, bool>>? whereTabelaReferenciaModel = null, 
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null, 
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null, 
        Expression<Func<int, bool>>? whereAnoModel = null)
    {
        await foreach (var vehicleModel in GetAllVehiclesWithExpressions(whereTabelaReferenciaModel, 
            whereMarcaModel, whereModeloModel, ParseAnoModelExpression(whereAnoModel)))
            yield return vehicleModel;
    }

    public async IAsyncEnumerable<VehicleModel> GetVehiclesWithDefaultTable(
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null, 
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null, 
        Expression<Func<AnoModel, bool>>? whereAnoModel = null)
    {
        var defaultTabelaModel = await GetDefaultTabelaReferencia();
        await foreach (var vehicleModel in GetAllVehiclesWithExpressions(
            (model) => model.Equals(defaultTabelaModel), 
            whereMarcaModel, whereModeloModel, whereAnoModel))
            yield return vehicleModel;
    }

    public async IAsyncEnumerable<VehicleModel> GetVehiclesWithDefaultTableAndYear(
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null, 
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null, 
        Expression<Func<int, bool>>? whereAnoModel = null)
    {
        await foreach (var vehicleModel in GetVehiclesWithDefaultTable(
            whereMarcaModel, whereModeloModel, ParseAnoModelExpression(whereAnoModel)))
            yield return vehicleModel;
    }

    private async IAsyncEnumerable<VehicleModel> GetAllVehiclesWithExpressions(
        Expression<Func<TabelaReferenciaModel, bool>>? whereTabelaReferenciaModel = null, 
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null, 
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null, 
        Expression<Func<AnoModel, bool>>? whereAnoModel = null)
    {
        var tabelasReferenciasModels = await GetAllTabelaReferenciaAsync();

        if (whereTabelaReferenciaModel is not null)
            tabelasReferenciasModels = tabelasReferenciasModels.Where(whereTabelaReferenciaModel.Compile());
        
        foreach (var tabelaReferenciaModel in tabelasReferenciasModels)
        {
            var marcas = GetMarcasAsyncEnumerable(tabelaReferenciaModel);

            if (whereMarcaModel is not null)
                marcas = marcas.Where(whereMarcaModel.Compile());
            
            await foreach (var marca in marcas)
            {
                var modelos = GetModelosAsyncEnumerable(marca);

                if (whereModeloModel is not null)
                    modelos = modelos.Where(whereModeloModel.Compile());

                await foreach (var modelo in modelos)
                {
                    var years = modelo.AvailableYears;

                    if (whereAnoModel is not null)
                        years = years.Where(whereAnoModel.Compile());

                    foreach (var year in years)
                    {
                        var vehicle = await GetVehicleOrDefaultAsync(modelo, year);
                        if (vehicle is not null)
                            yield return vehicle;
                    }
                }
            }
        }
    }

    private async Task<VehicleModel?> TryGetVehicle(ModeloModel? modeloModel, AnoModel? anoModel)
    {
        if (modeloModel is null ||
            anoModel is null ||
            modeloModel.Marca is null ||
            modeloModel.Marca.TabelaReferencia is null)
            return null;

        var marcaModel = modeloModel.Marca;

        using var response = await _client.PostUrlEncondedAsync(
            URL_CONSULTA_VEHICLE, 
            ("codigoTipoVeiculo", ((sbyte)marcaModel.CodigoMarca).ToString()),
            ("codigoTabelaReferencia", marcaModel.TabelaReferencia.Codigo.ToString()),
            ("codigoMarca", marcaModel.Value),
            ("codigoModelo", modeloModel.Value.ToString()),
            ("anoModelo", anoModel.Year.ToString()),
            ("codigoTipoCombustivel", anoModel.TipoCombustivel.ToString()),
            ("tipoConsulta", "tradicional"));
        
        using var streamJson = await response.Content.ReadAsStreamAsync();
        var s = await response.Content.ReadAsStringAsync();

        await CheckAndThrowFipeHttpResponseException(response);

        if (await ContainsFipeException(response))
            return null;

        var vehicleModel = await JsonSerializer.DeserializeAsync<VehicleModel>(streamJson, 
            new JsonSerializerOptions { IncludeFields = true,  })
            ?? throw new ArgumentNullException($"Fail to collect {nameof(VehicleModel)}.");

        if (string.IsNullOrEmpty(vehicleModel.CodigoFipe))
            throw new ArgumentNullException(nameof(vehicleModel.CodigoFipe));

        vehicleModel.ModeloVehicle = modeloModel;

        return vehicleModel;
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
        await CheckAndThrowFipeHttpResponseException(response);
        await CheckAndThrowFipeException(response);
    }

    private static async Task CheckAndThrowFipeHttpResponseException(HttpResponseMessage response)
    {
        if (ContainsFipeHttpResponseException(response))
            throw new FipeHttpResponseException(await response.Content.ReadAsStringAsync(), response.StatusCode);
    }

    private static async Task CheckAndThrowFipeException(HttpResponseMessage response)
    {
        var error = await TryGetError(response);
        if (error is not null)
            throw new FipeException(error);
    }

    private static bool ContainsFipeHttpResponseException(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return true;
        return false;
    }

    private static async Task<bool> ContainsFipeException(HttpResponseMessage response)
    {
        if (await TryGetError(response) is not null)
            return true;
        return false;
    }

    private static async Task<ErrorModel?> TryGetError(HttpResponseMessage response)
    {
        System.IO.Stream? streamResponse = null;
        try
        {
            streamResponse = await response.Content.ReadAsStreamAsync();
            return await TryGetError(streamResponse);
        }
        finally
        {
            if (streamResponse is not null)
                streamResponse.Position = 0;
        }
    }

    private static async Task<ErrorModel?> TryGetError(Stream stream)
    {
        if (stream is null)
            return null;

        try
        {
            return await JsonSerializer.DeserializeAsync<ErrorModel?>(stream, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

    private AnoModel? GetCorrectYearFromModeloOrDefault(ModeloModel? modelo, AnoModel? anoModel)
    {
        if (anoModel is null)
            return null;
        return GetCorrectYearFromModeloOrDefault(modelo, anoModel.Year);
    }

    private AnoModel? GetCorrectYearFromModeloOrDefault(ModeloModel? modelo, int year)
    {
        if (year < AnoModel.MIN_YEAR || year > AnoModel.ZERO_KM_YEAR)
            throw new ArgumentNullException(nameof(modelo));

        if (modelo is null || modelo.AvailableYears is null)
            throw new ArgumentNullException(nameof(modelo));

        var foundValues = modelo.AvailableYears.Where(
            anoModel => anoModel.Value.StartsWith(year.ToString())).ToList();

        if (foundValues.Count != 1)
            return null;

        return foundValues.First();
    }

    private Expression<Func<AnoModel, bool>>? ParseAnoModelExpression (in Expression<Func<int, bool>>? exAnoModel)
    {
        if (exAnoModel is null)
            return null;
        
        Expression<Func<AnoModel, bool>>? whereObjAnoModel = null;        
        var functionWhere = exAnoModel.Compile();
        whereObjAnoModel = (anoModel) => functionWhere(anoModel.Year);
        return whereObjAnoModel;
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