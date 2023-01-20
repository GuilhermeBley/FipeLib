using System.Linq.Expressions;
using FipeLib.Model;

namespace FipeLib.Services;

public interface IFipeQuery
{
    /// <summary>
    /// Get async enumerable of 'Tabela Referencia'
    /// </summary>
    Task<IEnumerable<TabelaReferenciaModel>> GetAllTabelaReferenciaAsync();

    /// <summary>
    /// Get enumerable of 'Tabela Referencia'
    /// </summary>
    IEnumerable<TabelaReferenciaModel> GetAllTabelaReferencia();

    /// <summary>
    /// Get 'marcas' in current date of 'Tabela Referencia'
    /// </summary>
    IEnumerable<MarcaModel> GetMarcas();

    /// <summary>
    /// Get async 'marcas' in current date of 'Tabela Referencia'
    /// </summary>
    Task<IEnumerable<MarcaModel>> GetMarcasAsync();

    /// <summary>
    /// Get 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    IEnumerable<MarcaModel> GetMarcas(TabelaReferenciaModel? tabelaReferenciaModel);
    
    /// <summary>
    /// Get async 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    Task<IEnumerable<MarcaModel>> GetMarcasAsync(TabelaReferenciaModel? tabelaReferenciaModel);

    /// <summary>
    /// Get async 'marcas' in current date of 'Tabela Referencia'
    /// </summary>
    IAsyncEnumerable<MarcaModel> GetMarcasAsyncEnumerable();
    
    /// <summary>
    /// Get async 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    IAsyncEnumerable<MarcaModel> GetMarcasAsyncEnumerable(TabelaReferenciaModel? tabelaReferenciaModel);
    IEnumerable<ModeloModel> GetModelos(MarcaModel? marcaModel);
    Task<IEnumerable<ModeloModel>> GetModelosAsync(MarcaModel? marcaModel);
    IAsyncEnumerable<ModeloModel> GetModelosAsyncEnumerable(MarcaModel? marcaModel);
    IEnumerable<AnoModel> GetAnosByModelo(ModeloModel? modeloModel);
    Task<IEnumerable<AnoModel>> GetAnosByModeloAsync(ModeloModel? modeloModel);
    IAsyncEnumerable<AnoModel> GetAnosByModeloAsyncEnumerable(ModeloModel? modeloModel);
    VehicleModel? GetVehicleOrDefault(ModeloModel? modeloModel, int year);
    Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, int year);
    VehicleModel? GetVehicleOrDefault(ModeloModel? modeloModel, AnoModel? anoModel);
    Task<VehicleModel?> GetVehicleOrDefaultAsync(ModeloModel? modeloModel, AnoModel? anoModel);
    IAsyncEnumerable<VehicleModel> GetVehicles(
        Expression<Func<TabelaReferenciaModel, bool>>? whereTabelaReferenciaModel = null,
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null,
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null,
        Expression<Func<AnoModel, bool>>? whereAnoModel = null);
    IAsyncEnumerable<VehicleModel> GetVehiclesWithYear(
        Expression<Func<TabelaReferenciaModel, bool>>? whereTabelaReferenciaModel = null,
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null,
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null,
        Expression<Func<int, bool>>? whereAnoModel = null);

    IAsyncEnumerable<VehicleModel> GetVehiclesWithDefaultTable(
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null,
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null,
        Expression<Func<AnoModel, bool>>? whereAnoModel = null);

    IAsyncEnumerable<VehicleModel> GetVehiclesWithDefaultTableAndYear(
        Expression<Func<MarcaModel, bool>>? whereMarcaModel = null,
        Expression<Func<ModeloModel, bool>>? whereModeloModel = null,
        Expression<Func<int, bool>>? whereAnoModel = null);
}