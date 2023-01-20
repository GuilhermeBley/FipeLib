using FipeLib.Services;

namespace FipeLib.Tests.ModelsTests;

public class ModeloModelTest : ModelTestBase
{
    [Fact]
    public async Task GetModelosAsync_GetData_Success()
    {
        Assert.NotEmpty(
            await _fipeQuery.GetModelosAsync(await GetRandomValidMarca())
        );
    }

    [Fact]
    public async Task GetModelosAsyncEnumerable_GetDataAsyncEnumerable_Success()
    {
        var marca = await GetRandomValidMarca();
        List<Model.ModeloModel> modelos = new();
        await foreach (var model in _fipeQuery.GetModelosAsyncEnumerable(marca))
        {
            if (modelos.Count == 10)
                break;
            modelos.Add(model);
        }

        Assert.NotEmpty(modelos);
    }

    [Fact]
    public async Task GetModelosAsyncEnumerable_CheckMarcasIsEqual_Success()
    {
        var marca = await GetRandomValidMarca();

        Assert.NotNull(marca);

        Assert.All(await _fipeQuery.GetModelosAsyncEnumerable(marca).ToListAsync(), (currentModelo) =>
        {
            Assert.Equal(marca, currentModelo.Marca);
        });
    }

    [Fact]
    public async Task GetModelosAsync_GetErrorWithInvalidMarca_Failed()
    {
        var invalidMarcaModel = new Model.MarcaModel("Invalid Label", "Invalid Value.");
        Utils.ReflectionUtils.SetPrivatePropertyValue(invalidMarcaModel, nameof(Model.MarcaModel.TabelaReferencia), 
            new Model.TabelaReferenciaModel(-1, "Invalid 'mes'"));

        await Assert.ThrowsAnyAsync<Exceptions.FipeException>(
            () => _fipeQuery.GetModelosAsync(invalidMarcaModel));
    }

    [Fact]
    public async Task GetModelosAsync_GetEmptyModelosWithNullTabelaReferencia_Success()
    {
        Assert.Empty(await _fipeQuery.GetModelosAsync(null));
    }

    [Fact]
    public async Task GetModelosAsync_GetEmptyModelosWithNullModel_Success()
    {
        Model.MarcaModel? invalidMarcaModel = null;

        Assert.Empty(await _fipeQuery.GetModelosAsync(invalidMarcaModel));
    }
}