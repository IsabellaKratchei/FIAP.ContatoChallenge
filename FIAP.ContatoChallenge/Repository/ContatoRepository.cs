using FIAP.ContatoChallenge.Controllers;
using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FIAP.ContatoChallenge.Repository
{
  public class ContatoRepository : IContatoRepository
  {
      private readonly BDContext _bdContext;

      public ContatoRepository(BDContext bdContext)
      {
          this._bdContext = bdContext;
      }

      public ContatoModel BuscarPorId(int id)
      {
          return _bdContext.Contatos.FirstOrDefault(x => x.Id == id);
      }

      public List<ContatoModel> BuscarTodos()
      {
          return _bdContext.Contatos.ToList();
      }

      public ContatoModel Adicionar(ContatoModel contato)
      {
      //gravar o contato no banco de dado
          var aux = Convert.ToUInt32(contato.Telefone.Substring(0, 2));
          var ddd=_bdContext.DDDs.FirstOrDefault(x => x.Num_DDD == aux);
          //contato.Regiao = ddd.Estado_DDD+" - "+ddd.Regiao_DDD;
          _bdContext.Contatos.Add(contato);
          _bdContext.SaveChanges();

          return contato;
      }

      public ContatoModel Editar(ContatoModel contato)
      {
          ContatoModel contatoBD = BuscarPorId(contato.Id);
          if (contatoBD == null) throw new System.Exception("Houve um erro na atualizaçao do contato!");

          contatoBD.Nome = contato.Nome;
          contatoBD.Email = contato.Email;
          contatoBD.Telefone = contato.Telefone;

          var aux = Convert.ToUInt32(contatoBD.Telefone.Substring(0, 2));
          var ddd = _bdContext.DDDs.FirstOrDefault(x => x.Num_DDD == aux);
          //contatoBD.Regiao = ddd.Estado_DDD + " - " + ddd.Regiao_DDD;

          _bdContext.Contatos.Update(contatoBD);
          _bdContext.SaveChanges();
          return contatoBD;
      }

      public bool Apagar(int id)
      {
          ContatoModel contatoBD = BuscarPorId(id);
          if (contatoBD == null) throw new System.Exception("Houve um erro na exclusao do contato!");
          _bdContext.Contatos.Remove(contatoBD);
          _bdContext.SaveChanges();
          return true;
      }
  }
}
