using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Integracao;

[TestClass]
[TestCategory("Testes de Integração de Gênero de Filme")]
public sealed class RepositorioGeneroFilmeEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        // Act
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        // Assert
        var generoEncontrado = repositorioGeneroFilme?.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, generoEncontrado);
    }

    [TestMethod]
    public void Deve_Editar_GeneroFilme_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Comédia");
        repositorioGeneroFilme?.Cadastrar(genero);
        dbContext?.SaveChanges();

        var generoEditado = new GeneroFilme("Comédia Romântica");

        // Act
        var conseguiuEditar = repositorioGeneroFilme?.Editar(genero.Id, generoEditado);
        dbContext?.SaveChanges();

        // Assert
        var generoEncontrado = repositorioGeneroFilme?.SelecionarRegistroPorId(genero.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, generoEncontrado);
    }
}
