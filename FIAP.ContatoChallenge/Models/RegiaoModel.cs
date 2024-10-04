using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.ContatoChallenge.Models
{
  public class RegiaoModel
  {
    public int Id { get; set; }
    public int Num_DDD { get; set; }
    public string Estado_DDD { get; set; }
    public string Regiao_DDD { get; set; }

  }
}
