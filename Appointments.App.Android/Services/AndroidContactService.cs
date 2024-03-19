using Android.Content;
using Android.Provider;
using Appointments.App.Droid.Services;
using Appointments.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;


[assembly: Xamarin.Forms.Dependency(typeof(AndroidContactService))]
namespace Appointments.App.Droid.Services
{
    public class AndroidContactService : IDeviceContactService
    {
        public void CreateContact(Contact contact)
        {

            List<ContentProviderOperation> ops = new List<ContentProviderOperation>();

            ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
            ops.Add(builder.Build());

            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                                           ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, contact.FamilyName);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, contact.GivenName);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.Prefix, contact.NamePrefix);
            ops.Add(builder.Build());

            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                                           ContactsContract.CommonDataKinds.Phone.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, contact.Phones.FirstOrDefault().PhoneNumber);
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                                           ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Mobile");
            ops.Add(builder.Build());

            
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