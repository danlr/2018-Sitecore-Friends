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
        public async Task<ContactDto> ReadFacetsAsync(Guid contactId)
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

        public async Task<bool> CreateContact(
            string source,
            string identifier,
            string firstName,
            string lastName,
            string title,
            string phone,
            string email)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    var contactTask = client.GetAsync<Contact>(
                        reference,
                        new ContactExpandOptions(
                            PersonalInformation.DefaultFacetKey,
                            EmailAddressList.DefaultFacetKey,
                            PhoneNumberList.DefaultFacetKey)
                    );

                    Contact existingContact = await contactTask;

                    if (existingContact != null)
                    {
                        return false;
                    }

                    var contactIdentifier = new[]
                    {
                        new ContactIdentifier(source, identifier, ContactIdentifierType.Known)
                    };

                    Contact contact = new Contact(contactIdentifier);

                    var personal = new PersonalInformation
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Title = title
                    };

                    var preferredPhoneNumber = new PhoneNumber(string.Empty, phone);
                    var phoneNumbers = new PhoneNumberList(preferredPhoneNumber, "Work");

                    var preferredEmail = new EmailAddress(email, true);
                    var emails = new EmailAddressList(preferredEmail, "Work");

                    client.AddContact(contact);
                    client.SetPhoneNumbers(contact, phoneNumbers);
                    client.SetPersonal(contact, personal);
                    client.SetEmails(contact, emails);

                    Interaction interaction = new Interaction(contact, InteractionInitiator.Brand, Guid.NewGuid(), "test");
                    var ev = new PageViewEvent(DateTime.UtcNow, Guid.Parse("{11111111-1111-1111-1111-111111111111}"), 1, "en");
                    interaction.Events.Add(ev);

                    client.AddInteraction(interaction);

                    await client.SubmitAsync();

                    return true;
                }
                catch (XdbExecutionException ex)
                {
                    return false;
                }
            }
        }

        protected XConnectClient GetClient()
        {
            return Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient();
        }
    }
}
