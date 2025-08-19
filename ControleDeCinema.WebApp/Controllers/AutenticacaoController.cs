using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("autenticacao")]
public class AutenticacaoController : Controller
{
    private readonly AutenticacaoAppService autenticacaoAppService;

    public AutenticacaoController(AutenticacaoAppService autenticacaoAppService)
    {
        this.autenticacaoAppService = autenticacaoAppService;
    }

    [HttpGet("registro")]
    public IActionResult Registro()
    {
        var registroVm = new RegistroViewModel();

        return View(registroVm);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroViewModel registroVm)
    {
        var usuario = new Usuario
        {
            UserName = registroVm.Email,
            Email = registroVm.Email,
        };

        var resultado = await autenticacaoAppService.RegistrarAsync(
            usuario,
            registroVm.Senha ?? string.Empty,
            registroVm.Tipo
        );

        if (resultado.IsFailed)
            return RedirectToAction(nameof(Registro));

        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = string.Empty });
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var loginVm = new LoginViewModel();

        return View(loginVm);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginVm)
    {
        var resultado = await autenticacaoAppService.LoginAsync(
            loginVm.Email ?? string.Empty,
            loginVm.Senha ?? string.Empty
        );

        if (resultado.IsFailed)
            return RedirectToAction(nameof(Login));

        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = string.Empty });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await autenticacaoAppService.LogoutAsync();

        return RedirectToAction(nameof(Login));
    }
}