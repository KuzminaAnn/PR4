using System;
using System.Net.Http;

using System.Threading.Tasks;
using WpfAppPR4.Models;

namespace WpfAppPR4.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5225")
            };
        }

      
    }
}