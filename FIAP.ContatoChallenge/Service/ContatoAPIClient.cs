using FIAP.ContatoChallenge.Models;
using FIAP.ContatoChallenge.Repository;
using System.Web.Mvc;

namespace FIAP.ContatoChallenge.Service
{
    public class ContatoAPIClient : IContatoRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IRegiaoRepository _regiaoRepository;

        public ContatoAPIClient(HttpClient httpClient, IRegiaoRepository regiaoRepository)
        {
            _httpClient = httpClient;
            _regiaoRepository = regiaoRepository;
        }

        [HttpPost]
        public async Task<ContatoModel> Criar(ContatoModel contato)
        {
            var regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);
            if (regiao == null)
            {
                throw new InvalidOperationException("Região não encontrada para o DDD informado.");
            }

            contato.Regiao = regiao.Regiao;

            var response = await _httpClient.PostAsJsonAsync("Contato/Criar", contato);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao adicionar contato: {response.StatusCode}, Detalhes: {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<ContatoModel>()
                   ?? throw new InvalidOperationException("Resposta nula ao criar contato.");
        }

        public async Task<bool> ApagarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Contato/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd)
        {
            var response = await _httpClient.GetAsync($"Contato/ddd/{ddd}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ContatoModel>>();
        }

        public async Task<ContatoModel> BuscarPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Contato/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ContatoModel>();
        }

        public async Task<List<ContatoModel>> BuscarTodosAsync()
        {
            var response = await _httpClient.GetAsync("Contato");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ContatoModel>>();
        }

        [HttpPut]
        public async Task<ContatoModel> Editar(ContatoModel contato)
        {
            var regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);
            if (regiao == null)
            {
                throw new InvalidOperationException("Região não encontrada para o DDD informado.");
            }

            contato.Regiao = regiao.Regiao;

            var response = await _httpClient.PutAsJsonAsync($"Editar/{contato.Id}", contato);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao editar contato: {response.StatusCode}, Detalhes: {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<ContatoModel>()
                   ?? throw new InvalidOperationException("Resposta nula ao editar contato.");

        }
    }
}
