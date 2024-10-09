using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.ContatoChallenge.Repository
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private readonly BDContext _bdContext;

        public RegiaoRepository(BDContext bdContext)
        {
            this._bdContext = bdContext;
        }

        public async Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd)
        {
            return await _bdContext.DDDs.FirstOrDefaultAsync(x => x.DDD == ddd);
        }

    }
}
