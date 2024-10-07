using FIAP.ContatoChallenge.Models;

namespace FIAP.ContatoChallenge.Repository
{
  public interface IRegiaoRepository
  {
    Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd);
  }
}
