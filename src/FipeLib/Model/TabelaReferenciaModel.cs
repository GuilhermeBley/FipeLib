namespace FipeLib.Model;

public class TabelaReferenciaModel
{
    public int Codigo { get; }
    public string Mes { get; }

    public TabelaReferenciaModel(int codigo, string mes)
    {
        if (codigo == 0)
            throw new ArgumentException(nameof(codigo));

        if (string.IsNullOrEmpty(mes))
            throw new ArgumentException(nameof(mes));
            
        Codigo = codigo;
        Mes = mes;
    }
}