using FipeLib.Model;

namespace FipeLib.Services;

public interface IFipeQuery
{
    Task<IEnumerable<TabelaReferenciaModel>> GetAllTabelaReferenciaAsync();
    IEnumerable<TabelaReferenciaModel> GetAllTabelaReferencia();
    IEnumerable<MarcaModel> GetMarca();
    Task<IEnumerable<MarcaModel>> GetMarcaAsync();
    IEnumerable<MarcaModel> GetMarca(TabelaReferenciaModel? tabelaReferenciaModel);
    Task<IEnumerable<MarcaModel>> GetMarcaAsync(TabelaReferenciaModel? tabelaReferenciaModel);
}