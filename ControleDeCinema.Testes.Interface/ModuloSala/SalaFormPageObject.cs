using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

public class SalaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SalaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form[data-se='formPrincipal']")).Displayed);
    }

    public SalaFormPageObject PreencherNumero(string numero)
    {
        var inputNumero = driver?.FindElement(By.Id("Numero"));
        inputNumero?.Clear();
        inputNumero?.SendKeys(numero);

        return this;
    }

    public SalaFormPageObject PreencherCapacidade(string capacidade)
    {
        var inputCapacidade = driver?.FindElement(By.Id("Capacidade"));
        inputCapacidade?.Clear();
        inputCapacidade?.SendKeys(capacidade);

        return this;
    }

    public SalaIndexPageObject Confirmar()
    {
        try
        {
            wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

            wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);
        }
        catch (Exception)
        {
            driver.DumpOnFailure("ControleDeCinema");

            throw;
        }

        return new SalaIndexPageObject(driver!);
    }
}