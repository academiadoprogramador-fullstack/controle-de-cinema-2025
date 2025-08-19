using ControledeCinema.Dominio.Compartilhado;
using System.Diagnostics.CodeAnalysis;

namespace ControleDeCinema.Dominio.ModuloGeneroFilme;

public class GeneroFilme : EntidadeBase<GeneroFilme>
{
    public string Descricao { get; set; }

    [ExcludeFromCodeCoverage]
    protected GeneroFilme() { }

    public GeneroFilme(string descricao)
    {
        Id = Guid.NewGuid();
        Descricao = descricao;
    }

    public override void AtualizarRegistro(GeneroFilme registroEditado)
    {
        Descricao = registroEditado.Descricao;
    }
}
