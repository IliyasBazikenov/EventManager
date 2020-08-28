using Entities.LinkModels;
using Entities.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EventManager.IntegrationTests
{
    public class AccountControllerIntegrationTest : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public AccountControllerIntegrationTest(TestingWebAppFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAccounts_WhenCalled_ReturnsAllAccounts()
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            var response = await _httpClient.GetAsync("api/accounts");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<Entity>>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAccountsHATEOAS_WhenCalled_ReturnsAllAcounts()
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.iliyas.hateoas+json");
            var response = await _httpClient.GetAsync("api/accounts");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<LinkCollectionWrapper<Entity>>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, result.Value.Count);
        }

        [Fact]
        public async Task GetAccountsWithParameters_WhenCalled_ReturnAllAccounts()
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.iliyas.hateoas+json");
            var response = await _httpClient.GetAsync("api/accounts?fields=firstName");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<LinkCollectionWrapper<Entity>>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, result.Value.Count);
        }
    }
}
