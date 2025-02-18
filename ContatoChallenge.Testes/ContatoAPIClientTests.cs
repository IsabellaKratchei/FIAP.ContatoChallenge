using FIAP.ContatoChallenge.Service;
using Moq.Protected;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FIAP.ContatoChallenge.Models;
using System.Net.Http.Json;
using System.Text.Json;
using FIAP.ContatoChallenge.Repository;

namespace ContatoChallenge.Testes
{
    [TestFixture]
    public class ContatoAPIClientTests : IDisposable
    {
        private ContatoAPIClient _contatoApiClient;
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private HttpClient _httpClient;
        private Mock<IRegiaoRepository> _regiaoRepository;
        private const string BaseUrl = "http://localhost";

        [SetUp]
        public void Setup()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            // Criando o mock do IRegiaoRepository
            _regiaoRepository = new Mock<IRegiaoRepository>();

            // Passando tanto o HttpClient quanto o mock do IRegiaoRepository
            _contatoApiClient = new ContatoAPIClient(_httpClient, _regiaoRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose do HttpClient após cada teste
            _httpClient.Dispose();
        }

        public void Dispose()
        {
            // Chamado quando a classe de teste é descartada
            _httpClient?.Dispose();
        }

        //[Test]
        //public async Task AdicionarAsync_DeveAdicionarContato_ComRegiaoPreenchida_QuandoDDDValido()
        //{
        //    // Arrange
        //    var contato = new ContatoModel
        //    {
        //        Nome = "Contato Válido",
        //        Email = "contato.valido@example.com",
        //        Telefone = "911111111",
        //        DDD = "11"
        //    };

        //    var regiao = new RegiaoModel
        //    {
        //        DDD = "11",
        //        Regiao = "Sudeste"
        //    };

        //    // Mock da API de Região - garantir que o repositório de região esteja mockado corretamente
        //    _regiaoRepository.Setup(repo => repo.BuscarRegiaoPorDDDAsync("11"))
        //                     .ReturnsAsync(regiao);

        //    // Mock da API de Contato (Post)
        //    _httpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.Method == HttpMethod.Post &&
        //                req.RequestUri == new Uri($"{BaseUrl}api/Contato")),
        //            ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.Created,
        //            Content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json")
        //        });

        //    // Act
        //    var result = await _contatoApiClient.Criar(contato);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Regiao, Is.EqualTo("Sudeste"));
        //    Assert.That(result.Nome, Is.EqualTo(contato.Nome));
        //}


        //[Test]
        //public void AdicionarAsync_DeveLancarExcecao_QuandoApiDeContatoRetornaErro()
        //{
        //    // Arrange
        //    var contatoInvalido = new ContatoModel
        //    {
        //        Nome = "Invalido",
        //        Email = "invalido@example.com",
        //        Telefone = "933333333",
        //        DDD = "41"
        //    };

        //    _httpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.Method == HttpMethod.Post &&
        //                req.RequestUri == new Uri($"{BaseUrl}api/Contato")),
        //            ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.BadRequest,
        //            Content = new StringContent("Erro na API de Contato", Encoding.UTF8, "application/json")
        //        });

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<HttpRequestException>(async () =>
        //        await _contatoApiClient.Criar(contatoInvalido));

        //    Assert.That(ex.Message, Does.Contain("Erro na API de Contato"));
        //}

        //[Test]
        //public async Task AdicionarAsync_DeveAdicionarContato_ComRegiaoNula_QuandoAPIDeRegiaoFalhar()
        //{
        //    // Arrange
        //    var contato = new ContatoModel
        //    {
        //        Nome = "Contato sem Região",
        //        Email = "sem.regiao@example.com",
        //        Telefone = "922222222",
        //        DDD = "99" // DDD inválido
        //    };

        //    // Mock da API de Região retornando erro
        //    _httpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.Method == HttpMethod.Get &&
        //                req.RequestUri == new Uri($"{BaseUrl}Regiao/99")),
        //            ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.NotFound
        //        });

        //    // Mock da API de Contato (Post)
        //    _httpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.Method == HttpMethod.Post &&
        //                req.RequestUri == new Uri($"{BaseUrl}api/Contato")),
        //            ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.Created,
        //            Content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json")
        //        });

        //    // Act
        //    var result = await _contatoApiClient.Criar(contato);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Regiao, Is.Null);
        //    Assert.That(result.Nome, Is.EqualTo(contato.Nome));
        //}

        #region Buscar Contato por ID
        [Test]
        public async Task BuscarPorIdAsync_DeveRetornarContato_QuandoIdExistente()
        {
            // Arrange
            var contato = new ContatoModel { Id = 1, Nome = "Contato Teste" };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(contato), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _contatoApiClient.BuscarPorIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Nome, Is.EqualTo("Contato Teste"));
        }

        [Test]
        public void BuscarPorIdAsync_DeveLancarExcecao_QuandoNaoEncontrado()
        {
            // Arrange
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            // Act & Assert
            var ex = Assert.ThrowsAsync<HttpRequestException>(() => _contatoApiClient.BuscarPorIdAsync(99));
            Assert.That(ex.Message, Does.Contain("404"));
        }
        #endregion

        #region Buscar por DDD
        [Test]
        public async Task BuscarPorDDDAsync_DeveRetornarContatos_QuandoExistirem()
        {
            // Arrange
            var contatos = new List<ContatoModel>
        {
            new ContatoModel { Nome = "Teste 1", DDD = "11" },
            new ContatoModel { Nome = "Teste 2", DDD = "11" }
        };

            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(contatos), Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _contatoApiClient.BuscarPorDDDAsync("11");

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Nome, Is.EqualTo("Teste 1"));
        }
        #endregion

        #region Apagar Contato
        [Test]
        public async Task ApagarAsync_DeveRetornarTrue_QuandoSucesso()
        {
            // Arrange
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent });

            // Act
            var result = await _contatoApiClient.ApagarAsync(1);

            // Assert
            Assert.That(result, Is.True);
        }
        #endregion
    }
}
