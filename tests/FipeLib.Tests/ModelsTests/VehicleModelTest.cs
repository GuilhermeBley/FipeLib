using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public class VehicleModelTest : ModelTestBase
{
    [Fact]
    public async Task GetVehicleOrDefaultAsync_GetData_Success()
    {
        var tuple = await GetRandomValidModeloAndYear();
        Assert.NotNull(await _fipeQuery.GetVehicleOrDefaultAsync(
            tuple.Modelo, tuple.Ano
        ));
    }

    [Fact]
    public async Task GetVehicleOrDefaultAsync_CheckModel_Success()
    {
        var tuple = await GetRandomValidModeloAndYear();
        var vehicle = await _fipeQuery.GetVehicleOrDefaultAsync(
            tuple.Modelo, tuple.Ano
        ) ?? throw new ArgumentNullException("vehicle");

        Assert.NotNull(vehicle.Modelo);
    }

    [Fact]
    public async Task GetVehicleOrDefaultAsync_CheckWithMinYear_SuccessReturnNull()
    {
        var tuple = await GetRandomValidModeloAndYear();

        bool hasThrow = false;
        try
        {
            await _fipeQuery.GetVehicleOrDefaultAsync(tuple.Modelo, Model.AnoModel.MIN_YEAR);
        }
        catch
        {
            hasThrow = true;
        }

        Assert.False(hasThrow);
    }
    
    [Fact]
    public async Task GetVehicleOrDefaultAsync_CheckWithZeroKmYear_SuccessReturnNull()
    {
        var tuple = await GetRandomValidModeloAndYear();

        bool hasThrow = false;
        try
        {
            await _fipeQuery.GetVehicleOrDefaultAsync(tuple.Modelo, Model.AnoModel.ZERO_KM_YEAR);
        }
        catch
        {
            hasThrow = true;
        }

        Assert.False(hasThrow);
    }

    [Fact]
    public async Task GetVehicleOrDefaultAsync_CheckWithInvalidMinYear_Failed()
    {
        var tuple = await GetRandomValidModeloAndYear();

        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => _fipeQuery.GetVehicleOrDefaultAsync(tuple.Modelo, Model.AnoModel.MIN_YEAR-1));
    }

    [Fact]
    public async Task GetVehicleOrDefaultAsync_CheckWithInvalidMaxYear_Failed()
    {
        var tuple = await GetRandomValidModeloAndYear();

        await Assert.ThrowsAnyAsync<ArgumentException>(
            () => _fipeQuery.GetVehicleOrDefaultAsync(tuple.Modelo, Model.AnoModel.ZERO_KM_YEAR+1));
    }

    [Fact]
    public async Task GetVehiclesWithDefaultTableAndYear_GetUniqueVehicle_Success()
    {
        var vehicleModel = await GetValidVehicle();

        int validModelId = vehicleModel.Modelo?.Value ?? throw new ArgumentNullException("Modelo");
        string validMarcaId = vehicleModel.Modelo?.Marca?.Value ?? throw new ArgumentNullException("Marca");
        int validAnoVehicle = vehicleModel.AnoModelo;

        var cts = new CancellationTokenSource(3000);

        Assert.NotNull(
            await _fipeQuery.GetVehiclesWithDefaultTableAndYear(
                (marca) => marca.Value == validMarcaId, 
                (modelo) => modelo.Value == validModelId,
                (ano) => ano == validAnoVehicle)
            .FirstOrDefaultAsync(cancellationToken: cts.Token));
    }
}