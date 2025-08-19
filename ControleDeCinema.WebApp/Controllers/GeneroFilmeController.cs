using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ControleDeCinema.WebApp.Controllers;

[Authorize(Roles = "Empresa")]
[Route("generos")]
public class GeneroFilmeController : Controller
{
    private readonly GeneroFilmeAppService generoFilmeAppService;

    public GeneroFilmeController(GeneroFilmeAppService generoFilmeAppService)
    {
        this.generoFilmeAppService = generoFilmeAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var resultado = generoFilmeAppService.SelecionarTodos();

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }

            return RedirectToAction("erro", "home");
        }

        var registros = resultado.Value;

        var visualizarVM = new VisualizarGenerosFilmeViewModel(registros);

        var existeNotificacao = TempData.TryGetValue(nameof(NotificacaoViewModel), out var valor);

        if (existeNotificacao && valor is string jsonString)
        {
            var notificacaoVm = JsonSerializer.Deserialize<NotificacaoViewModel>(jsonString);

            ViewData.Add(nameof(NotificacaoViewModel), notificacaoVm);
        }

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarGeneroFilmeViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarGeneroFilmeViewModel cadastrarVM)
    {
        var entidade = FormularioGeneroFilmeViewModel.ParaEntidade(cadastrarVM);

        var resultado = generoFilmeAppService.Cadastrar(entidade);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroDuplicado")
                {
                    ModelState.AddModelError("CadastroUnico", erro.Reasons[0].Message);
                    break;
                }
            }

            return View(cadastrarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);

                break;
            }

            return RedirectToAction(nameof(Index));
        }

        var registroSelecionado = resultado.Value;

        var editarVM = new EditarGeneroFilmeViewModel(
            id,
            registroSelecionado.Descricao
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarGeneroFilmeViewModel editarVM)
    {
        var entidadeEditada = FormularioGeneroFilmeViewModel.ParaEntidade(editarVM);

        var resultado = generoFilmeAppService.Editar(id, entidadeEditada);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                if (erro.Metadata["TipoErro"].ToString() == "RegistroDuplicado")
                {
                    ModelState.AddModelError("CadastroUnico", erro.Reasons[0].Message);
                    break;
                }
            }

            return View(editarVM);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }

            return RedirectToAction(nameof(Index));
        }

        var registroSelecionado = resultado.Value;

        var excluirVM = new ExcluirGeneroFilmeViewModel(
            registroSelecionado.Id,
            registroSelecionado.Descricao
        );

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(ExcluirGeneroFilmeViewModel excluirVm)
    {
        var resultado = generoFilmeAppService.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            foreach (var erro in resultado.Errors)
            {
                var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                    erro.Message,
                    erro.Reasons[0].Message
                );

                TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
                break;
            }
        }

        var detalhesVm = DetalhesGeneroFilmeViewModel.ParaDetalhesVm(resultado.Value);

        return View(detalhesVm);
    }
}
