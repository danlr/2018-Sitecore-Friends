namespace Hackathon.XEditor.Api.Services
{
    using System;
    using System.Threading.Tasks;

    using Dto;

    using Sitecore.XConnect;
    using Sitecore.XConnect.Client;
    using Sitecore.XConnect.Collection.Model;

    public class XconnectService
    {
        public async Task<object> ReadFacetsAsync(Guid contactId)
        {
            using (XConnectClient client = GetClient())
            {
                ContactDto result = new ContactDto();
                try
                {
                    ContactReference reference = new ContactReference(contactId);
                    var contactTask = client.GetAsync<Contact>(
                        reference,
                        new ContactExpandOptions(
                            PersonalInformation.DefaultFacetKey,
                            EmailAddressList.DefaultFacetKey,
                            ContactBehaviorProfile.DefaultFacetKey,
                            PhoneNumberList.DefaultFacetKey,
                            Avatar.DefaultFacetKey)
                    );

                    var contact = await contactTask;

                    if (contact == null)
                    {
                        return null;
                    }

                    result.ContactId = contact.Id.ToString();
                    result.PersonalInformation = new PersonalDto(contact.Personal());
                    result.Email = contact.Emails()?.PreferredEmail?.SmtpAddress;
                    result.Phone = contact.PhoneNumbers()?.PreferredPhoneNumber?.Number;
                    result.Avatar = contact.Avatar() != null && contact.Avatar().Picture != null;
                    return result;
                }
                catch (XdbExecutionException ex)
                {
                }
            }

            return null;
        }

        protected XConnectClient GetClient()
        {
            return Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient();
        }
    }
}
