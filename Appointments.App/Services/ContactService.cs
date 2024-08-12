using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Appointments.App.Services
{
    public class ContactService : IDeviceContactService
    {
        public async Task SaveDeviceContact(Contact contact)
        {
            try
            {
                await CheckDevicePermissionsAsync();

                if (!string.IsNullOrWhiteSpace(contact.Id))
                {
                    var deviceContacts = await Contacts.GetAllAsync();
                    var existingContact = deviceContacts.FirstOrDefault(t => t.Id == contact.Id);

                    if (existingContact != null)
                    {
                        existingContact.NamePrefix = contact.NamePrefix;
                        existingContact.GivenName = contact.GivenName;
                        existingContact.Phones = contact.Phones;
                        existingContact.Emails = contact.Emails;

                        // TODO Update contact in device

                        // TODO STH LIKE THIS OR SINGLE SAVE ?
                        //await DependencyService.Get<IDeviceContactService>().UpdateContact(contact);
                    }
                }
                else
                {
                    await DependencyService.Get<IDeviceContactService>().SaveDeviceContact(contact);
                }

                UserDialogs.Instance.ShowLoading();                
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                await Application.Current.MainPage.DisplayAlert(
                    "Alerta: ",
                    "Creado correctamente en la App pero no en el Dispositivo.",
                    "Ok"
                );
                await Application.Current.MainPage.Navigation.PopAsync();
            }

            UserDialogs.Instance.HideLoading();
        }

        private static async Task CheckDevicePermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsWrite>();

            if (status != PermissionStatus.Granted)
            {
                UserDialogs.Instance.HideLoading();

                var request = await Permissions.RequestAsync<Permissions.ContactsWrite>();

                if (request != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error:",
                        "No se puede crear contactos. Por favor, agrega el permiso desde la Configuración > Apps.",
                        "Ok"
                    );
                }
            }
        }
    }
}
