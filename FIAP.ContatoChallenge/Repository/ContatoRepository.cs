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
        private readonly IRegiaoRepository _regiaoRepository;

        public ContatoRepository(BDContext bdContext, IRegiaoRepository regiaoRepository)
        {
            this._bdContext = bdContext;
            this._regiaoRepository = regiaoRepository;
        }

        public async Task<ContatoModel> BuscarPorIdAsync(int id)
        {
            return await _bdContext.Contatos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ContatoModel>> BuscarTodosAsync()
        {
            return await _bdContext.Contatos.ToListAsync();
        }

        public async Task<ContatoModel> AdicionarAsync(ContatoModel contato)
        {
            try
            {
                // Verifica se o DDD é fornecido
                if (string.IsNullOrEmpty(contato.DDD))
                {
                    throw new Exception("o DDD deve ser informado.");
                }

                // Consultar a região correspondente ao DDD fornecido
                RegiaoModel regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);

                // Se a região não for encontrada, pode lançar uma exceção ou tratar conforme necessário
                if (regiao == null)
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
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

        public async Task<ContatoModel> EditarAsync(ContatoModel contato)
        {
            try
            {
                // Verificar se o DDD é fornecido
                if (string.IsNullOrEmpty(contato.DDD))
                {
                    throw new Exception("o DDD deve ser informado.");
                }

                // Consultar a região correspondente ao DDD fornecido
                RegiaoModel regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);

                // Se a região não for encontrada, lançar uma exceção informativa
                if (regiao == null)
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
                }

                // Buscar o contato existente no banco de dados
                ContatoModel contatoBD = await BuscarPorIdAsync(contato.Id);
                if (contatoBD == null)
                {
                    throw new Exception("Houve um erro na atualização do contato!");
                }

                contatoBD.Nome = contato.Nome;
                contatoBD.Email = contato.Email;
                contatoBD.DDD = contato.DDD;
                contatoBD.Regiao = contato.Regiao;
                contatoBD.Telefone = contato.Telefone;


                _bdContext.Contatos.Update(contatoBD);
                await _bdContext.SaveChangesAsync();
                return contatoBD;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao editar o contato:" + ex.Message);
            }
        }

        public async Task<bool> ApagarAsync(int id)
        {
            ContatoModel contatoBD = await BuscarPorIdAsync(id);
            if (contatoBD == null) throw new System.Exception("Houve um erro na exclusao do contato!");

            try
            {
                _bdContext.Contatos.Remove(contatoBD);
                await _bdContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao apagar o contato:" + ex.Message);
            }
        }
    }
}
