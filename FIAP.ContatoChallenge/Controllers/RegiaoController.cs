using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.ContatoChallenge.Controllers
{
  public class RegiaoController : Controller
  {
    private readonly IRegiaoRepository _regiaoRepository;

    public RegiaoController(IRegiaoRepository regiaoRepository)
    {
       _regiaoRepository = regiaoRepository;
    }

    //public IActionResult BuscarDDD(int num)
    //{
    //  RegiaoModel ddd = _regiaoRepository.BuscarPorNum(num);
    //  return View(ddd);
    //}
  }
}
