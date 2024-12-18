using FIAP.ContatoChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.ContatoChallenge.Data
{
  public class BDContext : DbContext
  {
    public BDContext(DbContextOptions<BDContext> options) : base(options)
    {
        
    }

    //Remoçao da configuraçao de Contato devido a utilizaçao da API própria de Contatos
    //public DbSet<ContatoModel> Contatos { get; set; }

    //Remoçao da configuraçao de DDDs devido a utilizaçao da API própria de Regiao
    //public DbSet<RegiaoModel> DDDs { get; set; }

  }
}
