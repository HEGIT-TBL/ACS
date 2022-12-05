using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ACS.Core.Contracts.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Runtime.InteropServices;
using System.Threading;

namespace ACS.Services
{
    public class GenericAPIPoster<T>
        where T : class, IHasGuid
    {
        private string _baseUrl = "https://localhost:7051/" + typeof(T).Name + "/";
        private HttpClient _client;

        public GenericAPIPoster()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private HttpResponseMessage Post(object body, string apiMethodAddress)
        {
            var res = _client.PostAsync(_baseUrl + apiMethodAddress,
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            try
            {
                res.Result.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return res.Result;
        }

        private async Task<HttpResponseMessage> PostAsync(object body, string apiMethodAddress, CancellationToken cancellationToken)
        {
            var res = await _client.PostAsync(_baseUrl + apiMethodAddress,
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"), cancellationToken);
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return res;
        }

        public void Attach(T item)
        {
            Post(item, "Attach");
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await PostAsync(null, "GetAllAsync", cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<IEnumerable<T>>(json);
        }

        public async Task<T> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await PostAsync(id, "GetOneAsync", cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task CreateAsync(T item, CancellationToken cancellationToken)
        {
            await PostAsync(item, "CreateAsync", cancellationToken);
        }

        public void Update(T otherItem)
        {
            Post(otherItem, "Update");
        }

        public void Delete(T item)
        {
            Post(item, "Delete");
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await PostAsync(id, "DeleteAsync", cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await PostAsync(null, "SaveChangesAsync", cancellationToken);
        }

        public async Task<bool> ElementExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await PostAsync(id, "ElementExistsAsync", cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<bool>(json);
        }
    }
}
