using Android.Content;
using Android.Provider;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Appointments.App.Droid.Services
{
    public class AndroidContactService : IDeviceContactService
    {
        public void CreateContact(Contact contact)
        {

            List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
            int rawContactInsertIndex = ops.Count;

            ops.Add(ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri)
                .WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null)
                .WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null).Build());
            ops.Add(ContentProviderOperation
                .NewInsert(ContactsContract.Data.ContentUri)
                .WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                .WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, contact.GivenName)
                .Build());
            ops.Add(ContentProviderOperation
                .NewInsert(ContactsContract.Data.ContentUri)
                .WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                .WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, contact.FamilyName)
                .Build());
            ops.Add(ContentProviderOperation
                .NewInsert(ContactsContract.Data.ContentUri)
                .WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.StructuredName.ContentItemType)
                .WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, contact.FamilyName)
                .Build());
            ops.Add(ContentProviderOperation
                .NewInsert(ContactsContract.Data.ContentUri)
                .WithValueBackReference(
                    ContactsContract.Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Phone.ContentItemType)
                .WithValue(ContactsContract.CommonDataKinds.Phone.Number, contact.Phones.FirstOrDefault()?.PhoneNumber)
                .WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type, "mobile").Build()); // Type of mobile number  

            // Asking the Contact provider to create a new contact                 
            try
            {
                Android.App.Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}