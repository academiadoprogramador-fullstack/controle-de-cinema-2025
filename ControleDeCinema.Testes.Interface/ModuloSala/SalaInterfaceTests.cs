using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

[TestClass]
[TestCategory("Testes de Interface de Sala")]
public sealed class SalaInterfaceTests : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {
        // Arrange
        var salaIndex = new SalaIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        salaIndex
            .ClickCadastrar()
            .PreencherNumero("1")
            .PreencherCapacidade("100")
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala("# 1"));
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
   {
        // Arrange
        var salaIndex = new SalaIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        salaIndex
            .ClickCadastrar()
            .PreencherNumero("1")
            .PreencherCapacidade("100")
            .Confirmar();

        // Act
        salaIndex
            .ClickEditar()
            .PreencherNumero("2")
            .PreencherCapacidade("50")
            .Confirmar();

        // Assert
        Assert.IsTrue(salaIndex.ContemSala("# 2"));
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente()
    {
        // Arrange
        var salaIndex = new SalaIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        salaIndex
            .ClickCadastrar()
            .PreencherNumero("1")
            .PreencherCapacidade("100")
            .Confirmar();

        // Act
        salaIndex
            .ClickExcluir()
            .Confirmar();

        // Assert
        Assert.IsFalse(salaIndex.ContemSala("# 1"));
    }
}