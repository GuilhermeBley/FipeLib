using FipleLib.Model;
using System.Text.Json;

namespace FipeLib.Services;

public sealed class FipeQuery : IFipeQuery
{
    private const string UrlApi = "https://veiculos.fipe.org.br/api/veiculos";
    private const string UrlConsultarTabelaDeReferencia = $"{UrlApi}//ConsultarTabelaDeReferencia";
    private static readonly TabelaReferenciaSession _tabelaReferenciaSession = new();
    private static HttpClient __client { get; } = CreateClientTabelaFipe();
    private HttpClient _client => __client;

    public IEnumerable<TabelaReferenciaModel> GetAllTabelaReferencia()
    {
        return GetAllTabelaReferenciaAsync().GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<TabelaReferenciaModel>> GetAllTabelaReferenciaAsync()
    {
        return await _tabelaReferenciaSession.GetListTabelaReferenciaModelAsync();
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
        private static (DateTime LastRefresh, IEnumerable<TabelaReferenciaModel>? Data) _tabelaReferenciaEnumerable = (DateTime.Now, null);

        public async Task<IEnumerable<TabelaReferenciaModel>> GetListTabelaReferenciaModelAsync()
        {
            if (ShouldRefreshData())
                _tabelaReferenciaEnumerable.Data = await PrivateGetListTabelaReferenciaModel();

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

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            using Stream stremResponse = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<IEnumerable<TabelaReferenciaModel>>(stremResponse)
                ?? throw new ArgumentNullException($"Fail to collect {typeof(IEnumerable<TabelaReferenciaModel>).Name}.");
        }
    }
}