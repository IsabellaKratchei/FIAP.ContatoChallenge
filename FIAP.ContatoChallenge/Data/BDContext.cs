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

    public DbSet<RegiaoModel> DDDs { get; set; }
  }
}
