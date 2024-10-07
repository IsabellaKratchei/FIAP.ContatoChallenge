using FIAP.ContatoChallenge.Models;

namespace FIAP.ContatoChallenge.Repository
{
  public interface IContatoRepository
  {
    Task<ContatoModel> AdicionarAsync(ContatoModel contato);
    Task<ContatoModel> BuscarPorIdAsync(int id);
    Task<List<ContatoModel>> BuscarTodosAsync();
    Task<ContatoModel> EditarAsync(ContatoModel contato);
    Task<bool> ApagarAsync(int id);
  }
}
