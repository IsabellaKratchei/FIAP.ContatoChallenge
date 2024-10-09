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

        public async Task<IActionResult> Index(string ddd)
        {
            List<ContatoModel> contatos;

            // Verifica se foi fornecido um DDD
            if (!string.IsNullOrEmpty(ddd))
            {
                contatos = await _contatoRepository.BuscarPorDDDAsync(ddd);
                ViewData["ddd"] = ddd; // Para manter o DDD na view após a busca
            }
            else
            {
                contatos = await _contatoRepository.BuscarTodosAsync();
            }

            return View(contatos);
        }

        public async Task<IActionResult> Editar(int id)
        {
            ContatoModel contato = await _contatoRepository.BuscarPorIdAsync(id);

            if (contato == null)
            {
                return NotFound();
            }
            return View(contato);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ContatoModel contato)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _contatoRepository.EditarAsync(contato);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(contato);
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
                try
                {
                    await _contatoRepository.AdicionarAsync(contato);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(contato);
        }

        public async Task<IActionResult> ApagarConfirmacao(int id)
        {
            ContatoModel contato = await _contatoRepository.BuscarPorIdAsync(id);
            if (contato == null)
            {
                return NotFound();
            }
            return View(contato);
        }

        [HttpPost]
        public async Task<IActionResult> Apagar(int id)
        {
            await _contatoRepository.ApagarAsync(id);
            return RedirectToAction("Index");
        }
    }
}
