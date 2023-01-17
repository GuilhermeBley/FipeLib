using FipleLib.Model;

namespace FipeLib.Services;

public interface IFipeQuery
{
    Task<IEnumerable<TabelaReferenciaModel>> GetAllTabelaReferenciaAsync();
    IEnumerable<TabelaReferenciaModel> GetAllTabelaReferencia();
}