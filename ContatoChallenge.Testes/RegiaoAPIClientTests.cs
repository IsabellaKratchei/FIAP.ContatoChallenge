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

namespace ContatoChallenge.Testes
{
    [TestFixture]
    public class RegiaoAPIClientTests :IDisposable
    {
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private RegiaoAPIClient _regiaoAPIClient;

        [SetUp]
        public void Setup()
        {
            // Criação de um HttpMessageHandler mockado
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:7294/api/")
            };

            // Instancia o RegiaoAPIClient com o HttpClient mockado
            _regiaoAPIClient = new RegiaoAPIClient(_httpClient);
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

        [Test]
        public async Task BuscarRegiaoPorDDDAsync_ShouldReturnRegiao_WhenApiReturnsSuccess()
        {
            // Arrange
            var ddd = "11";
            var responseContent = "{\"ddd\":\"11\",\"Regiao\":\"Sudeste\"}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            // Configura o mock para responder à chamada HTTP com sucesso
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().EndsWith($"Regiao/{ddd}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _regiaoAPIClient.BuscarRegiaoPorDDDAsync(ddd);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DDD, Is.EqualTo("11"));
            Assert.That(result.Regiao, Is.EqualTo("Sudeste"));
        }

        [Test]
        public async Task BuscarRegiaoPorDDDAsync_ShouldReturnNull_WhenApiReturnsNotFound()
        {
            // Arrange
            var ddd = "99";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };

            // Configura o mock para retornar um 404 Not Found
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().EndsWith($"Regiao/{ddd}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _regiaoAPIClient.BuscarRegiaoPorDDDAsync(ddd);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void BuscarRegiaoPorDDDAsync_ShouldThrowException_WhenApiFails()
        {
            // Arrange
            var ddd = "11";

            // Configura o mock para lançar uma exceção
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Erro na API"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<HttpRequestException>(() => _regiaoAPIClient.BuscarRegiaoPorDDDAsync(ddd));
            Assert.That(ex.Message, Is.EqualTo("Erro na API"));
        }
    }
}
