using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public abstract class ModelTestBase
{
    protected IFipeQuery _fipeQuery = new FipeQuery();

    protected async Task<Model.VehicleModel> GetValidVehicle()
    {
        var tuple = await GetRandomValidModeloAndYear();
        return await _fipeQuery.GetVehicleOrDefaultAsync(tuple.Modelo, tuple.Ano) ?? throw new ArgumentNullException();
    }

    protected async Task<(Model.ModeloModel Modelo, Model.AnoModel Ano)> GetRandomValidModeloAndYear()
    {
        var ramdomModel = await GetRandomValidModelo();
        var yearsModels = await _fipeQuery.GetAnosByModeloAsync(ramdomModel);
        int randomIndex = new Random().Next(0, yearsModels.Count()-1);
        return (ramdomModel, yearsModels.ElementAt(randomIndex));
    }

    protected async Task<Model.ModeloModel> GetRandomValidModelo()
    {
        var randomMarca = await GetRandomValidMarca();
        var modelos = await _fipeQuery.GetModelosAsync(randomMarca);
        int randomIndex = new Random().Next(0, modelos.Count()-1);
        return modelos.ElementAt(randomIndex);
    }

    protected async Task<Model.MarcaModel> GetRandomValidMarca()
    {
        var marcas = await _fipeQuery.GetMarcasAsync();
        int randomIndex = new Random().Next(0, marcas.Count()-1);
        return marcas.ElementAt(randomIndex);
    }
}