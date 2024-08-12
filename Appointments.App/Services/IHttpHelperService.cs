using System.Threading.Tasks;

namespace Appointments.App.Services
{
    public interface IHttpHelperService
    {
        Task<T> GetAsync<T>(string client, string method);
        Task<T> PostAsync<T>(string client, string method, object body);
        Task<T> PutAsync<T>(string client, string method, string command, object body);
    }
}
