using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIAP.ContatoChallenge.Testes
{
    [TestFixture]
    public class ContatoRepositoryTests
    {
        private ContatoRepository _contatoRepository;
        private Mock<IRegiaoRepository> _regiaoRepositoryMock;
        private BDContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Configurando o contexto do banco de dados em memória para os testes
            var options = new DbContextOptionsBuilder<BDContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _dbContext = new BDContext(options);
            _regiaoRepositoryMock = new Mock<IRegiaoRepository>();
            _contatoRepository = new ContatoRepository(_dbContext, _regiaoRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Limpar o banco de dados após cada teste
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose(); // Dispose o contexto do banco de dados
        }

        [Test]
        public async Task AdicionarAsync_ShouldAddContato_WhenAllDataIsProvided()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Novo Contato",
                Email = "novo.contato@example.com", // Adicionando email
                Telefone = "1111-1111",
                DDD = "11",
                Regiao = "São Paulo" // Adicionando região
            };

            var regiao = new RegiaoModel
            {
                DDD = "11",
                Regiao = "São Paulo"
            };

            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync(contato.DDD))
                .ReturnsAsync(regiao);

            // Act
            var result = await _contatoRepository.AdicionarAsync(contato);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Nome, Is.EqualTo(contato.Nome));
            Assert.That((await _contatoRepository.BuscarTodosAsync()).Count, Is.EqualTo(1));
        }

        [Test]
        public void AdicionarAsync_ShouldThrowException_WhenDDDIsNull()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Novo Contato",
                Email = "novo.contato@example.com", // Adicionando email
                Telefone = "1111-1111",
                DDD = null // DDD não fornecido
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _contatoRepository.AdicionarAsync(contato));
            Assert.That(ex.Message, Is.EqualTo("o DDD deve ser informado."));
        }

        [Test]
        public async Task EditarAsync_ShouldUpdateContato_WhenExists()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Contato Existente",
                Email = "contato.existente@example.com", // Adicionando email
                Telefone = "2222-2222",
                DDD = "21",
                Regiao = "Rio de Janeiro" // Adicionando região
            };

            var regiao = new RegiaoModel
            {
                DDD = "21",
                Regiao = "Rio de Janeiro"
            };

            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync(contato.DDD))
                .ReturnsAsync(regiao);

            // Adicionando um contato para editar
            await _contatoRepository.AdicionarAsync(contato);
            contato.Nome = "Contato Atualizado";

            // Act
            var result = await _contatoRepository.EditarAsync(contato);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Nome, Is.EqualTo("Contato Atualizado"));
        }

        //[Test]
        //public async Task ApagarAsync_ShouldRemoveContato_WhenExists()
        //{
        //    // Arrange
        //    var contato = new ContatoModel
        //    {
        //        Nome = "Contato a Ser Removido",
        //        Email = "contato.removido@example.com", // Adicionando email
        //        Telefone = "3333-3333",
        //        DDD = "31",
        //        Regiao = "Sudeste" // Adicionando região
        //    };

        //    await _contatoRepository.AdicionarAsync(contato);

        //    // Act
        //    var result = await _contatoRepository.ApagarAsync(contato.Id);

        //    // Assert
        //    Assert.That(result, Is.True);
        //    Assert.That((await _contatoRepository.BuscarTodosAsync()).Count, Is.EqualTo(0));
        //}

        [Test]
        public async Task ApagarAsync_ShouldRemoveContato_WhenExists()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Contato a Ser Removido",
                Email = "contato.removido@example.com", // Adicionando email
                Telefone = "3333-3333",
                DDD = "31",
                Regiao = "Sudeste" // Adicionando região
            };

            var regiao = new RegiaoModel
            {
                DDD = "31",
                Regiao = "Sudeste"
            };

            // Simula o retorno da busca por região
            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync(contato.DDD))
                .ReturnsAsync(regiao);

            await _contatoRepository.AdicionarAsync(contato);

            // Act
            var result = await _contatoRepository.ApagarAsync(contato.Id);

            // Assert
            Assert.That(result, Is.True);
            Assert.That((await _contatoRepository.BuscarTodosAsync()).Count, Is.EqualTo(0));
        }

        [Test]
        public void ApagarAsync_ShouldThrowException_WhenContatoDoesNotExist()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _contatoRepository.ApagarAsync(999)); // ID que não existe
            Assert.That(ex.Message, Is.EqualTo("Houve um erro na exclusao do contato!"));
        }

        [Test]
        public async Task BuscarPorIdAsync_ShouldReturnContato_WhenExists()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Contato Para Buscar",
                Email = "contato.buscar@example.com", // Adicionando email
                Telefone = "4444-4444",
                DDD = "41",
                Regiao = "Sul" // Adicionando região
            };

            var regiao = new RegiaoModel
            {
                DDD = "41",
                Regiao = "Sul"
            };

            // Simula o retorno da busca por região
            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync(contato.DDD))
                .ReturnsAsync(regiao);

            await _contatoRepository.AdicionarAsync(contato);

            // Act
            var result = await _contatoRepository.BuscarPorIdAsync(contato.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Nome, Is.EqualTo(contato.Nome));
        }

        [Test]
        public async Task BuscarTodosAsync_ShouldReturnAllContatos()
        {
            // Arrange
            var regiao1 = new RegiaoModel { DDD = "51", Regiao = "Sul" };
            var regiao2 = new RegiaoModel { DDD = "61", Regiao = "Centro-Oeste" };

            // Simula o retorno da busca por região para cada DDD
            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync("51"))
                .ReturnsAsync(regiao1);
            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync("61"))
                .ReturnsAsync(regiao2);

            await _contatoRepository.AdicionarAsync(new ContatoModel
            {
                Nome = "Contato 1",
                Email = "contato1@example.com",
                Telefone = "5555-5555",
                DDD = "51",
                Regiao = "Sul"
            });

            await _contatoRepository.AdicionarAsync(new ContatoModel
            {
                Nome = "Contato 2",
                Email = "contato2@example.com",
                Telefone = "6666-6666",
                DDD = "61",
                Regiao = "Centro-Oeste"
            });

            // Act
            var result = await _contatoRepository.BuscarTodosAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task BuscarPorDDDAsync_ShouldReturnContatos_WhenDDDExists()
        {
            // Arrange
            var regiao = new RegiaoModel
            {
                DDD = "71",
                Regiao = "Nordeste"
            };

            // Simula o retorno da busca por região para o DDD "71"
            _regiaoRepositoryMock.Setup(repo => repo.BuscarRegiaoPorDDDAsync("71"))
                .ReturnsAsync(regiao);

            // Arrange
            await _contatoRepository.AdicionarAsync(new ContatoModel { Nome = "Contato 1", Email = "contato1@example.com", Telefone = "7777-7777", DDD = "71", Regiao = "Nordeste" });
            await _contatoRepository.AdicionarAsync(new ContatoModel { Nome = "Contato 2", Email = "contato2@example.com", Telefone = "8888-8888", DDD = "71", Regiao = "Nordeste" });

            // Act
            var result = await _contatoRepository.BuscarPorDDDAsync("71");

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
