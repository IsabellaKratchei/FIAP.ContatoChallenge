using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.ContatoChallenge.Models
{
  public class ContatoModel
  {
    public int Id { get; set; }
    [Required(ErrorMessage ="Digite o nome do contato")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "Digite o email do contato")]
    [EmailAddress(ErrorMessage ="O email informado nao é válido!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Digite o celular do contato")]
    [Phone]
    public string Telefone { get; set; }
    [Required(ErrorMessage = "Digite o DDD do telefone do contato")]
    [ValidateNever]
    public string Regiao { get; set; }
    public string DDD { get; set; }

  }
}
