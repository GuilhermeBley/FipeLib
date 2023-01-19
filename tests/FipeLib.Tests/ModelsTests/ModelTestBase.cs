using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public abstract class ModelTestBase
{
    protected IFipeQuery _fipeQuery = new FipeQuery();

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