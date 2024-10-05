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
            return _bdContext.Contatos
              .FirstOrDefault(x => x.Id == id);
            //return _bdContext.Contatos.FirstOrDefault(x => x.Id == id);
        }

        public List<ContatoModel> BuscarTodos()
        {
            return _bdContext.Contatos.ToList();
        }

        public async Task<ContatoModel> AdicionarAsync(ContatoModel contato)
        {
            try
            {
                // Consultar a região correspondente ao DDD fornecido
                RegiaoModel regiao = await _bdContext.DDDs
                    .FirstOrDefaultAsync(x => x.DDD == contato.DDD);

                // Se a região não for encontrada, pode lançar uma exceção ou tratar conforme necessário
                if (regiao == null)
                {
                    throw new Exception("DDD não encontrado.");
                }

                // Atribuir a região ao contato
                contato.Regiao = regiao.Regiao;

                // Gravar o contato no banco de dados
                await _bdContext.Contatos.AddAsync(contato);
                await _bdContext.SaveChangesAsync();

                return contato;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao adicionar o contato: " + ex.Message);
            }
        }

        public ContatoModel Editar(ContatoModel contato)
        {
            ContatoModel contatoBD = BuscarPorId(contato.Id);
            if (contatoBD == null) throw new System.Exception("Houve um erro na atualizaçao do contato!");

            contatoBD.Nome = contato.Nome;
            contatoBD.Email = contato.Email;
            contatoBD.DDD = contato.DDD;
            contatoBD.Telefone = contato.Telefone;

            try
            {
                _bdContext.Contatos.Update(contatoBD);
                _bdContext.SaveChangesAsync();
                return contatoBD;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao editar o contato:" + ex.Message);
            }
        }

        public bool Apagar(int id)
        {
            ContatoModel contatoBD = BuscarPorId(id);
            if (contatoBD == null) throw new System.Exception("Houve um erro na exclusao do contato!");

            try
            {
                _bdContext.Contatos.Remove(contatoBD);
                _bdContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao apagar o contato:" + ex.Message);
            }
        }
    }
}
