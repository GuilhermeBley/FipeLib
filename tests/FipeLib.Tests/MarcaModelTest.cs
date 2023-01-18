using FipeLib.Services;

namespace FipeLib.Tests;

public class MarcaModelTest
{
    private readonly IFipeQuery _fipeQuery = new FipeQuery();

    [Fact]
    public async Task GetMarcaAsync_GetData_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetMarcaAsync());
    }

    [Fact]
    public async Task GetMarcaAsync_GetDataTwoTimes_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetMarcaAsync());
        Assert.NotEmpty((await _fipeQuery.GetMarcaAsync()).ToList());
    }

    [Fact]
    public async Task GetMarcaAsync_GetDataTaskWaitAll_Success()
    {
        CancellationTokenSource cts = new(5000);

        var enumerableResult1 = Enumerable.Empty<Model.MarcaModel>();
        var task1 = Task.Run(async ()=>
        {
            enumerableResult1 =  await _fipeQuery.GetMarcaAsync();
        }, cts.Token);

        var enumerableResult2 = Enumerable.Empty<Model.MarcaModel>();
        var task2 = Task.Run(async ()=>
        {
            enumerableResult2 = await _fipeQuery.GetMarcaAsync();
        }, cts.Token);

        var enumerableResult3 = Enumerable.Empty<Model.MarcaModel>();
        var task3 = Task.Run(async ()=>
        {
            enumerableResult3 = await _fipeQuery.GetMarcaAsync();
        }, cts.Token);

        var enumerableResult4 = Enumerable.Empty<Model.MarcaModel>();
        var task4 = Task.Run(async ()=>
        {
            enumerableResult4 = await _fipeQuery.GetMarcaAsync();
        }, cts.Token);
        

        await Task.WhenAll(
            _fipeQuery.GetMarcaAsync(),
            _fipeQuery.GetMarcaAsync(),
            _fipeQuery.GetMarcaAsync(),
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
    public async Task GetMarcaAsync_GetDataAsyncEnumerable_Success()
    {
        List<Model.MarcaModel> marcas = new();
        await foreach (var model in _fipeQuery.GetMarcaAsyncEnumerable())
        {
            if (marcas.Count == 10)
                break;
            marcas.Add(model);
        }

        Assert.NotEmpty(marcas);
    }
}