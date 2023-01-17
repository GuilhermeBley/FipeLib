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
}