using FIAP.ContatoChallenge.Models;

namespace FIAP.ContatoChallenge.Repository
{
  public interface IContatoRepository
  {
    ContatoModel Adicionar(ContatoModel contato);
    ContatoModel BuscarPorId(int id);
    List<ContatoModel> BuscarTodos();
    ContatoModel Editar(ContatoModel contato);
    bool Apagar(int id);
  }
}
