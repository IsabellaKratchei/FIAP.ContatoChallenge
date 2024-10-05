using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.ContatoChallenge.Controllers
{
  public class ContatoController : Controller
  {
    private readonly IContatoRepository _contatoRepository;

    public ContatoController(IContatoRepository contatoRepository)
    {
      _contatoRepository = contatoRepository;
    }

    public IActionResult Index()
    {
      List<ContatoModel> contatos = _contatoRepository.BuscarTodos();
      return View(contatos);
    }
    public IActionResult Editar(int id)
    {
      ContatoModel contato = _contatoRepository.BuscarPorId(id);
      return View(contato);
    }

    [HttpPost]
    public IActionResult Alterar(ContatoModel contato)
    {
      _contatoRepository.Editar(contato);
      return RedirectToAction("Index");
    }

    public IActionResult Criar()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Criar(ContatoModel contato)
    {
      if (ModelState.IsValid) 
      {
        await _contatoRepository.AdicionarAsync(contato);
        return RedirectToAction("Index");
      }
       
       return View(contato);
    }

    public IActionResult ApagarConfirmacao(int id)
    {
      ContatoModel contato = _contatoRepository.BuscarPorId(id);
      return View(contato);
    }

    public IActionResult Apagar(int id)
    {
      _contatoRepository.Apagar(id);
      return RedirectToAction("Index");
    }
  }
}
