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
    IEnumerable<MarcaModel> GetMarca();

    /// <summary>
    /// Get async 'marcas' in current date of 'Tabela Referencia'
    /// </summary>
    Task<IEnumerable<MarcaModel>> GetMarcaAsync();

    /// <summary>
    /// Get 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    IEnumerable<MarcaModel> GetMarca(TabelaReferenciaModel? tabelaReferenciaModel);
    
    /// <summary>
    /// Get async 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    Task<IEnumerable<MarcaModel>> GetMarcaAsync(TabelaReferenciaModel? tabelaReferenciaModel);

    /// <summary>
    /// Get async 'marcas' in current date of 'Tabela Referencia'
    /// </summary>
    IAsyncEnumerable<MarcaModel> GetMarcaAsyncEnumerable();
    
    /// <summary>
    /// Get async 'marcas' with specific 'Tabela Referencia'
    /// </summary>
    IAsyncEnumerable<MarcaModel> GetMarcaAsyncEnumerable(TabelaReferenciaModel? tabelaReferenciaModel);
}