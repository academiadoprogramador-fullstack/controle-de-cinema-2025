using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected static IWebDriver? driver;
    protected static string? enderecoBase;

    private static IConfiguration? configuracao;
    private static DotNet.Testcontainers.Networks.INetwork? rede;

    private static IDatabaseContainer? dbContainer;
    private readonly static int dbPort = 5432;

    private static IContainer? appContainer;
    private readonly static int appPort = 8080;

    private static IContainer? seleniumContainer;
    private readonly static int seleniumPort = 4444;

    private static ControleDeCinemaDbContext? dbContext;

    [AssemblyInitialize]
    public static async Task ConfigurarTestes(TestContext _)
    {
        configuracao = new ConfigurationBuilder()
            .AddUserSecrets<TestFixture>()
            .AddEnvironmentVariables()
            .Build();

        rede = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync();

        await InicializarAplicacaoAsync();

        await InicializarWebDriverAsync();

        RegistrarContaEmpresarial();
    }

    [AssemblyCleanup]
    public static async Task EncerrarTestes()
    {
        await EncerrarWebDriverAsync();

        await EncerrarAplicacaoAsync();

        await EncerrarBancoDadosAsync();

        driver?.Manage().Cookies.DeleteAllCookies();
    }

    [TestInitialize]
    public virtual void InicializarTeste()
    {
        if (dbContainer is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado.");

        dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(dbContainer.GetConnectionString());

        ConfigurarTabelas(dbContext);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);
        dbContext.Ingressos.RemoveRange(dbContext.Ingressos);
        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
        dbContext.Salas.RemoveRange(dbContext.Salas);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsync()
    {
        dbContainer = new PostgreSqlBuilder()
          .WithImage("postgres:16")
          .WithPortBinding(dbPort, true)
          .WithNetwork(rede)
          .WithNetworkAliases("controle-de-cinema-e2e-testdb")
          .WithName("controle-de-cinema-e2e-testdb")
          .WithDatabase("ControleDeCinemaTestDb")
          .WithUsername("postgres")
          .WithPassword("YourStrongPassword")
          .WithCleanUp(true)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(dbPort)
          )
          .Build();

        await dbContainer.StartAsync();
    }

    private static async Task InicializarAplicacaoAsync()
    {
        // Configura a connection string para a rede: "Host=teste-facil-e2e-testdb;Port=5432;Database=TesteFacilDb;Username=postgres;Password=YourStrongPassword"
        var connectionStringRede = dbContainer?.GetConnectionString()
            .Replace(dbContainer.Hostname, "controle-de-cinema-e2e-testdb")
            .Replace(dbContainer.GetMappedPublicPort(dbPort).ToString(), dbPort.ToString());

        // Configura o container da aplicação e inicializa o enderecoBase
        appContainer = new ContainerBuilder()
            .WithImage("controledecinemawebapp")
            .WithPortBinding(appPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-webapp")
            .WithName("controle-de-cinema-webapp")
            .WithEnvironment("SQL_CONNECTION_STRING", connectionStringRede)
            .WithEnvironment("NEWRELIC_LICENSE_KEY", configuracao?["NEWRELIC_LICENSE_KEY"])
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(appPort)
                .UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)appPort).ForPath("/health"))
            )
            .WithCleanUp(true)
            .Build();

        await appContainer.StartAsync();

        // URL interno: http://controle-de-cinema-webapp:8080
        enderecoBase = $"http://{appContainer.Name}:{appPort}";
    }

    private static async Task InicializarWebDriverAsync()
    {
        seleniumContainer = new ContainerBuilder()
            .WithImage("selenium/standalone-chrome:nightly")
            .WithPortBinding(seleniumPort, true)
            .WithNetwork(rede)
            .WithNetworkAliases("controle-de-cinema-selenium-e2e")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .WithName("controle-de-cinema-selenium-e2e")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(seleniumPort)
            )
            .Build();

        await seleniumContainer.StartAsync();

        var enderecoSelenium = new Uri($"http://{seleniumContainer.Hostname}:{seleniumContainer.GetMappedPublicPort(seleniumPort)}/wd/hub");

        var options = new ChromeOptions();

        driver = new RemoteWebDriver(enderecoSelenium, options);
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (dbContainer is not null)
            await dbContainer.DisposeAsync();
    }

    private static async Task EncerrarAplicacaoAsync()
    {
        if (appContainer is not null)
            await appContainer.DisposeAsync();
    }

    private static async Task EncerrarWebDriverAsync()
    {
        driver?.Quit();
        driver?.Dispose();

        if (seleniumContainer is not null)
            await seleniumContainer.DisposeAsync();
    }

    protected static void RegistrarContaEmpresarial()
    {
        if (driver is null)
            throw new ArgumentNullException(nameof(driver));

        driver.Navigate().GoToUrl($"{enderecoBase}/autenticacao/registro");

        var inputEmail = driver.FindElement(By.Id("Email"));
        var inputSenha = driver.FindElement(By.Id("Senha"));
        var inputConfirmarSenha = driver.FindElement(By.Id("ConfirmarSenha"));
        var selectTipoUsuario = new SelectElement(driver.FindElement(By.Id("Tipo")));

        inputEmail.Clear();
        inputEmail.SendKeys("empresa@dominio.com");

        inputSenha.Clear();
        inputSenha.SendKeys("Teste@123");

        inputConfirmarSenha.Clear();
        inputConfirmarSenha.SendKeys("Teste@123");

        selectTipoUsuario.SelectByText("Empresa");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
        {
            IWebElement btn = d.FindElement(By.CssSelector("button[type='submit']"));

            if (!btn.Enabled || !btn.Displayed) return false;

            btn.Click();

            return true;
        });

        wait.Until(d => !d.Url.Contains("/autenticacao/registro", StringComparison.OrdinalIgnoreCase));
    }
}