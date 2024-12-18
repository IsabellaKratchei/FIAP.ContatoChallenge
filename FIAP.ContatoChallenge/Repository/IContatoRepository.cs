using FIAP.ContatoChallenge.Models;

namespace FIAP.ContatoChallenge.Repository
{
    public interface IContatoRepository
    {
        Task<ContatoModel> Criar(ContatoModel contato);
        Task<ContatoModel> BuscarPorIdAsync(int id);
        Task<List<ContatoModel>> BuscarTodosAsync();
        Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd);
        Task<ContatoModel> Editar(ContatoModel contato);
        Task<bool> ApagarAsync(int id);
    }
}
