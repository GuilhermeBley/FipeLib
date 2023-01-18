using FipeLib.Services;

namespace FipeLib.Tests;

public class TabelaReferenciaModelTest
{
    private readonly IFipeQuery _fipeQuery = new FipeQuery();

    [Fact]
    public async Task GetAllTabelaReferenciaAsync_GetData_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetAllTabelaReferenciaAsync());
    }

    [Fact]
    public async Task GetAllTabelaReferenciaAsync_GetDataTwoTimes_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetAllTabelaReferenciaAsync());
        Assert.NotEmpty(await _fipeQuery.GetAllTabelaReferenciaAsync());
    }

    [Fact(Timeout = 2000)]
    public async Task GetAllTabelaReferenciaAsync_GetDataTaskWaitAll_Success()
    {
        CancellationTokenSource cts = new(5000);

        var enumerableResult1 = Enumerable.Empty<Model.TabelaReferenciaModel>();
        var task1 = Task.Factory.StartNew(async () =>
        {
            enumerableResult1 =  await _fipeQuery.GetAllTabelaReferenciaAsync();
        }, cts.Token);

        var enumerableResult2 = Enumerable.Empty<Model.TabelaReferenciaModel>();
        var task2 = Task.Run(async () =>
        {
            enumerableResult2 = await _fipeQuery.GetAllTabelaReferenciaAsync();
        }, cts.Token);

        var enumerableResult3 = Enumerable.Empty<Model.TabelaReferenciaModel>();
        var task3 = Task.Run(async () =>
        {
            enumerableResult3 = await _fipeQuery.GetAllTabelaReferenciaAsync();
        }, cts.Token);

        var enumerableResult4 = Enumerable.Empty<Model.TabelaReferenciaModel>();
        var task4 = Task.Run(async () =>
        {
            enumerableResult4 = await _fipeQuery.GetAllTabelaReferenciaAsync();
        }, cts.Token);
        

        await Task.WhenAll(
            _fipeQuery.GetAllTabelaReferenciaAsync(),
            _fipeQuery.GetAllTabelaReferenciaAsync(),
            _fipeQuery.GetAllTabelaReferenciaAsync(),
            task1,
            task2,
            task3,
            task4);

        Assert.NotEmpty(enumerableResult1);
        Assert.NotEmpty(enumerableResult2);
        Assert.NotEmpty(enumerableResult3);
        Assert.NotEmpty(enumerableResult4);
    }
}