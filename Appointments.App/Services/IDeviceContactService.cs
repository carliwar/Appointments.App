using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Appointments.App.Services
{
    public interface IDeviceContactService
    {
        void CreateContact(Contact contact);
    }
}
