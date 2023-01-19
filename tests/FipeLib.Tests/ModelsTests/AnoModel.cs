using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public class AnoModel : ModelTestBase
{
    [Fact]
    public async Task GetAnosByModeloAsync_GetData_Success()
    {
        Assert.NotEmpty(await _fipeQuery.GetAnosByModeloAsync(
            await GetRandomValidModelo()
        ));
    }

    [Fact]
    public async Task GetAnosByModeloAsync_GetEmptyWithNullModelo_Success()
    {
        Assert.Empty(await _fipeQuery.GetAnosByModeloAsync(
            null
        ));
    }

}