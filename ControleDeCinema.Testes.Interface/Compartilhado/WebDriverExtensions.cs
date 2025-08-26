using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.Compartilhado;

public static class WebDriverExtensions
{
    public static void DumpOnFailure(this IWebDriver driver, string prefix)
    {
        try
        {
            Screenshot shot = ((ITakesScreenshot)driver).GetScreenshot();
            string png = Path.Combine(Path.GetTempPath(), $"{prefix}-{DateTime.Now:HHmmss}.png");
            shot.SaveAsFile(png);

            string html = Path.Combine(Path.GetTempPath(), $"{prefix}-{DateTime.Now:HHmmss}.html");
            File.WriteAllText(html, driver.PageSource);
        }
        catch
        {
            throw;
        }
    }
}