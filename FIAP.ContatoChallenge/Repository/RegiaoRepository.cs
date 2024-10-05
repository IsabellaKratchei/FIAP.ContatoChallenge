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

      //public RegiaoModel BuscarPorNum(int num)
      //{
      //    return _bdContext.DDDs.FirstOrDefault(x => x.DDD == num);
      //}

  }
}
