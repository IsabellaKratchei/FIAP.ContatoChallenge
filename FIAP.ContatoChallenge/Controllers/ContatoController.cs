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

        [HttpGet]
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

        [HttpGet("Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            ContatoModel contato = await _contatoRepository.BuscarPorIdAsync(id);

            if (contato == null)
            {
                return NotFound();
            }
            return View(contato);
        }

        [HttpPut("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, ContatoModel contato)
        {
            if (id != contato.Id)
            {
                return BadRequest("O ID informado não corresponde ao contato.");
            }

            if (!ModelState.IsValid)
            {
                return View(contato);
            }

            try
            {
                await _contatoRepository.Editar(contato);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(contato);
            }
        }

        [HttpGet("Criar")]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(ContatoModel contato)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        await _contatoRepository.Criar(contato);
            //        return RedirectToAction("Index");
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.AddModelError("", ex.Message);
            //    }
            //}
            //return View(contato);
            if (!ModelState.IsValid)
            {
                return View(contato);
            }

            try
            {
                await _contatoRepository.Criar(contato);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(contato);
            }
        }

        [HttpGet("ApagarConfirmacao/{id}")]
        public async Task<IActionResult> ApagarConfirmacao(int id)
        {
            ContatoModel contato = await _contatoRepository.BuscarPorIdAsync(id);
            if (contato == null)
            {
                return NotFound();
            }
            return View(contato);
        }

        [HttpDelete("Apagar/{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            await _contatoRepository.ApagarAsync(id);
            return RedirectToAction("Index");
        }
    }
}
