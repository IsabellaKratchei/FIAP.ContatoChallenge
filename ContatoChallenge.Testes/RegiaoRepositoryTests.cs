using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FIAP.ContatoChallenge.Testes
{
    [TestFixture]
    public class RegiaoRepositoryTests
    {
        private RegiaoRepository _regiaoRepository;
        private BDContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // Configurando o contexto do banco de dados em memória para os testes
            var options = new DbContextOptionsBuilder<BDContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _dbContext = new BDContext(options);
            _regiaoRepository = new RegiaoRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Limpar o banco de dados após cada teste
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose(); // Dispose o contexto do banco de dados
        }

        [Test]
        public async Task BuscarRegiaoPorDDDAsync_ShouldReturnRegiao_WhenDDDExists()
        {
            // Arrange
            var regiao = new RegiaoModel
            {
                DDD = "11",
                Regiao = "Sudeste"
            };

            // Adiciona a região ao banco de dados em memória
            await _dbContext.DDDs.AddAsync(regiao);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _regiaoRepository.BuscarRegiaoPorDDDAsync("11");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DDD, Is.EqualTo(regiao.DDD));
            Assert.That(result.Regiao, Is.EqualTo(regiao.Regiao));
        }

        [Test]
        public async Task BuscarRegiaoPorDDDAsync_ShouldReturnNull_WhenDDDDoesNotExist()
        {
            // Act
            var result = await _regiaoRepository.BuscarRegiaoPorDDDAsync("99");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
