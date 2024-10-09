using FIAP.ContatoChallenge.Controllers;
using FIAP.ContatoChallenge.Repository;
using Moq;

namespace ContatoChallenge.Testes
{
    [TestFixture]
    public class ContatoControllerTests
    {
        private Mock<IContatoRepository> _contatoRepositoryMock;
        private Mock<IRegiaoRepository> _regiaoRepositoryMock;
        private ContatoController _controller;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}