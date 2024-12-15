using FIAP.ContatoChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.ContatoChallenge.Data
{
  public class BDContext : DbContext
  {
    public BDContext(DbContextOptions<BDContext> options) : base(options)
    {
        
    }

    public DbSet<ContatoModel> Contatos { get; set; }

    //Remo�ao da configura�ao de DDDs devido a utiliza�ao da API pr�pria de Regiao
    //public DbSet<RegiaoModel> DDDs { get; set; }

  }
}
