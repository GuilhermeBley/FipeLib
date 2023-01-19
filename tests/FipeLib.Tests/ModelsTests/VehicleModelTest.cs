using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public class VehicleModelTest : ModelTestBase
{
    [Fact]
    public async Task GetAllTabelaReferenciaAsync_GetData_Success()
    {
        var tuple = await GetRandomValidModeloAndYear();
        Assert.NotNull(await _fipeQuery.GetVehicleOrDefaultAsync(
            tuple.Modelo, tuple.Ano
        ));
    }
}