using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public class MarcaModelTest : ModelTestBase
{
    [Fact]
    public async Task GetMarcaAsync_GetData_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetMarcasAsync());
    }

    [Fact]
    public async Task GetMarcaAsync_GetDataTwoTimes_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetMarcasAsync());
        Assert.NotEmpty((await _fipeQuery.GetMarcasAsync()).ToList());
    }

    [Fact]
    public async Task GetMarcaAsync_GetDataTaskWaitAll_Success()
    {
        CancellationTokenSource cts = new(5000);

        var enumerableResult1 = Enumerable.Empty<Model.MarcaModel>();
        var task1 = Task.Run(async ()=>
        {
            enumerableResult1 =  await _fipeQuery.GetMarcasAsync();
        }, cts.Token);

        var enumerableResult2 = Enumerable.Empty<Model.MarcaModel>();
        var task2 = Task.Run(async ()=>
        {
            enumerableResult2 = await _fipeQuery.GetMarcasAsync();
        }, cts.Token);

        var enumerableResult3 = Enumerable.Empty<Model.MarcaModel>();
        var task3 = Task.Run(async ()=>
        {
            enumerableResult3 = await _fipeQuery.GetMarcasAsync();
        }, cts.Token);

        var enumerableResult4 = Enumerable.Empty<Model.MarcaModel>();
        var task4 = Task.Run(async ()=>
        {
            enumerableResult4 = await _fipeQuery.GetMarcasAsync();
        }, cts.Token);
        

        await Task.WhenAll(
            _fipeQuery.GetMarcasAsync(),
            _fipeQuery.GetMarcasAsync(),
            _fipeQuery.GetMarcasAsync(),
            task1,
            task2,
            task3,
            task4);

        Assert.NotEmpty(enumerableResult1);
        Assert.NotEmpty(enumerableResult2);
        Assert.NotEmpty(enumerableResult3);
        Assert.NotEmpty(enumerableResult4);
    }

    [Fact]
    public async Task GetMarcaAsyncEnumerable_GetDataAsyncEnumerable_Success()
    {
        List<Model.MarcaModel> marcas = new();
        await foreach (var model in _fipeQuery.GetMarcasAsyncEnumerable())
        {
            if (marcas.Count == 10)
                break;
            marcas.Add(model);
        }

        Assert.NotEmpty(marcas);
    }

    [Fact]
    public async Task GetMarcaAsync_CheckTabela_Success()
    {
        var marcas = await _fipeQuery.GetMarcasAsync();
        var tabelaReferenciaExpectedToAll = marcas.Select(m => m.TabelaReferencia).FirstOrDefault();

        Assert.NotNull(tabelaReferenciaExpectedToAll);

        Assert.All(marcas.Select(m => m.TabelaReferencia), (currentTabelaReferencia) =>
        {
            Assert.Equal(tabelaReferenciaExpectedToAll, currentTabelaReferencia);
        });
    }

    [Fact]
    public async Task GetMarcaAsync_GetErrorWithInvalidTabelaReferencia_Failed()
    {
        var invalidTabelaReferencia = new Model.TabelaReferenciaModel(-1, "Invalid 'mes'");

        await Assert.ThrowsAnyAsync<Exceptions.FipeException>(
            () => _fipeQuery.GetMarcasAsync(invalidTabelaReferencia));
    }

    [Fact]
    public async Task GetMarcasAsync_GetEmptyModelosWithNullModel_Success()
    {
        Model.TabelaReferenciaModel? invalidTabelaReferencia = null;

        Assert.Empty(await _fipeQuery.GetMarcasAsync(invalidTabelaReferencia));
    }
}