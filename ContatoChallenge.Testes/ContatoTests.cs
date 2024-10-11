using FIAP.ContatoChallenge.Controllers;
using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ContatoChallenge.Testes
{
    [TestFixture]
    public class ContatoTests
    {
        private Mock<IContatoRepository> _contatoRepositoryMock;
        private Mock<IRegiaoRepository> _regiaoRepositoryMock;
        private ContatoController _controller;

        [SetUp]
        public void Setup()
        {
            _contatoRepositoryMock = new Mock<IContatoRepository>();

            _controller = new ContatoController(_contatoRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfContatos_WhenDDDIsNull()
        {
            // Arrange
            var contatosMock = new List<ContatoModel>
            {
                new ContatoModel { Id = 1, Nome = "Contato 1", Telefone = "1111-1111", DDD = "11" },
                new ContatoModel { Id = 2, Nome = "Contato 2", Telefone = "2222-2222", DDD = "22" }
            };

            // Certifique-se de que o nome do mock é o mesmo que no código do setup
            _contatoRepositoryMock.Setup(repo => repo.BuscarTodosAsync())
                .ReturnsAsync(contatosMock);

            // Act
            var result = await _controller.Index(null);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "O resultado deve ser um ViewResult");

            var model = viewResult.Model as List<ContatoModel>;
            Assert.IsNotNull(model, "O modelo da view deve ser uma lista de ContatoModel");
            Assert.AreEqual(2, model.Count, "O modelo deve conter 2 contatos"); // Verifica se a lista de contatos contém 2 elementos
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfContatos_WhenDDDIsNotNull()
        {
            // Arrange
            string ddd = "11";
            var contatosMock = new List<ContatoModel>
            {
                new ContatoModel { Id = 1, Nome = "Contato 1", Telefone = "1111-1111", DDD = "11" }
            };

            _contatoRepositoryMock.Setup(repo => repo.BuscarPorDDDAsync(ddd))
                .ReturnsAsync(contatosMock);

            // Act
            var result = await _controller.Index(ddd);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<ContatoModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count); // Verifica se há apenas 1 contato
            Assert.AreEqual(ddd, model[0].DDD); // Verifica se o DDD corresponde
        }

        [Test]
        public async Task Editar_ReturnsNotFound_WhenContatoDoesNotExist()
        {
            // Arrange
            int contatoId = 1;
            _contatoRepositoryMock.Setup(repo => repo.BuscarPorIdAsync(contatoId))
                .ReturnsAsync((ContatoModel)null); // Simula um contato não encontrado

            // Act
            var result = await _controller.Editar(contatoId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result); // Verifica se retorna 404
        }

        [Test]
        public async Task Editar_ReturnsViewResult_WithContatoModel()
        {
            // Arrange
            int contatoId = 1;
            var contatoMock = new ContatoModel { Id = contatoId, Nome = "Contato 1", Telefone = "1111-1111", DDD = "11" };

            _contatoRepositoryMock.Setup(repo => repo.BuscarPorIdAsync(contatoId))
                .ReturnsAsync(contatoMock);

            // Act
            var result = await _controller.Editar(contatoId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ContatoModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(contatoId, model.Id);
        }

        [Test]
        public async Task Criar_Post_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            // Arrange
            var contatoMock = new ContatoModel { Nome = "Novo Contato", Telefone = "1234-5678", DDD = "11" };

            // Act
            var result = await _controller.Criar(contatoMock);

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task Criar_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "", // Nome inválido
                Email = "", // Email inválido
                Telefone = "1111-1111",
                DDD = "11"
            };

            _controller.ModelState.AddModelError("Nome", "Digite o nome do contato"); // Adiciona erro de validação
            _controller.ModelState.AddModelError("Email", "Digite o email do contato"); // Adiciona erro de validação

            // Act
            var result = await _controller.Criar(contato);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ContatoModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(contato, model);
        }


        [Test]
        public async Task Apagar_ReturnsRedirectToAction_WhenCalled()
        {
            // Arrange
            int contatoId = 1;

            // Act
            var result = await _controller.Apagar(contatoId);

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task Criar_Post_ShouldAddContato_WhenDDDIsProvided()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Novo Contato",
                Email = "novo.contato@example.com",
                Telefone = "1111-1111",
                DDD = "11",
                Regiao = "Sudeste"
            };

            // Crie uma instância do controlador sem a necessidade de IRegiaoRepository
            var _controller = new ContatoController(_contatoRepositoryMock.Object);

            // Simula o comportamento do método AdicionarAsync
            _contatoRepositoryMock.Setup(repo => repo.AdicionarAsync(contato))
                .ReturnsAsync(contato);

            // Act
            var result = await _controller.Criar(contato);

            // Assert
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            // Verifique se o contato foi adicionado na lista de contatos
            _contatoRepositoryMock.Verify(repo => repo.AdicionarAsync(contato), Times.Once);
        }
    }
}