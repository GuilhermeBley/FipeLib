using System.Collections;
using FipeLib.Model;

namespace FipeLib.Internal;

internal class OrderedEnumerableTabelaReferencia : IEnumerable<TabelaReferenciaModel>
{
    private readonly List<TabelaReferenciaModel> _orederedList;

    public OrderedEnumerableTabelaReferencia(IEnumerable<TabelaReferenciaModel> tabelaReferenciaModels)
    {
        if (!tabelaReferenciaModels.Any())
            throw new ArgumentException($"{nameof(tabelaReferenciaModels)} must be have some element.");

        _orederedList = tabelaReferenciaModels.OrderByDescending(o => o.Codigo).ToList();
    }

    public TabelaReferenciaModel GetLastMonth()
    {
        return _orederedList.First();
    }

    public IEnumerator<TabelaReferenciaModel> GetEnumerator()
    {
        return _orederedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _orederedList.GetEnumerator();
    }
}