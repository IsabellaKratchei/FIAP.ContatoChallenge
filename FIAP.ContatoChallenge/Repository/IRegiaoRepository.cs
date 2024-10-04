using FIAP.ContatoChallenge.Models;

namespace FIAP.ContatoChallenge.Repository
{
  public interface IRegiaoRepository
  {
    RegiaoModel BuscarPorNum(int num);
  }
}
