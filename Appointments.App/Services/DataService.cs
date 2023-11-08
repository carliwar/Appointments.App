using Appointments.App.Data;
using Appointments.App.Infrastructure;
using Appointments.App.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Appointments.App.Services
{
    public class DataService : IDataService
    {
        private SQLiteAsyncConnection _database;
        public DataService()
        {
            _database = new SQLiteAsyncConnection(AppConfiguration.DatabasePath, AppConfiguration.Flags);

        }

        public async Task<User> CreateUser(User person)
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);
            await db.Insert(person);
            return person;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            await _database.CreateTablesAsync<User, User>();
            var db = new Repository<User>(_database);

            return await db.Get();
        }

        public async Task<IEnumerable<PersonType>> GetPersonTypes()
        {
            await _database.CreateTablesAsync<PersonType, PersonType>();
            var db = new Repository<PersonType>(_database);

            return await db.Get();
        }
        


        #region API Call implementation
        //private readonly string _apiURL;
        //private HttpClient _client;
        //public DataService()
        //{
        //    var handler = new HttpClientHandler();
        //    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //    _client = new HttpClient(handler);
        //    _apiURL = AppConfiguration.API_URL;
        //}
        //public async Task<IEnumerable<PersonType>> GetPersonTypes()
        //{
        //    var url = $"{"https://192.168.1.17:444/"}{AppConfiguration.USER_ENDPOINT}/getusers";

        //    try
        //    {
        //        HttpResponseMessage response = await _client.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            var personTypes = JsonConvert.DeserializeObject<IEnumerable<PersonType>>(result);
        //            return personTypes;
        //        }
        //        else
        //        {
        //            throw new Exception("Error al obtener los tipos de personas");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //} 
        #endregion
    }
}
