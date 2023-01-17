namespace FipeLib.Model;

public class TabelaReferenciaModel
{
    public int Codigo { get; }
    public string Mes { get; } = string.Empty;

    public TabelaReferenciaModel(int codigo, string mes)
    {
        Codigo = codigo;
        Mes = mes;
    }
}