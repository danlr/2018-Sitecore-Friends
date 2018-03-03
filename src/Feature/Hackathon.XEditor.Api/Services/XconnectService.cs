using System.ComponentModel;
using System.Reflection;
using JSNLog.Infrastructure;

namespace Hackathon.XEditor.Api.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dto;

    using Model.Contact.Facets;

    using Sitecore.XConnect;
    using Sitecore.XConnect.Client;
    using Sitecore.XConnect.Collection.Model;

    public class XconnectService
    {
        public async Task<ContactDto> GetContact(Guid contactId)
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

                    result.ContactId = contact.Id ?? Guid.Empty;
                    result.PersonalInformation = new PersonalDto(contact.Personal());
                    result.Email = contact.Emails()?.PreferredEmail?.SmtpAddress;
                    result.Phone = contact.PhoneNumbers()?.PreferredPhoneNumber?.Number;
                    result.IsAvatar = contact.Avatar() != null && contact.Avatar().Picture != null;
                    if (result.IsAvatar)
                    {
                        result.Avatar = contact.Avatar().Picture;
                    }
                    return result;
                }
                catch (XdbExecutionException ex)
                {
                    Sitecore.Diagnostics.Log.Error(ex.Message, this);
                }
            }

            return null;
        }

    

        public async Task<IReadOnlyDictionary<string, Facet>> GetContactFacets(Guid contactId)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    var availableFacetDefinitions = client.Model.Facets.Where(f => f.Target == EntityType.Contact);

                    List<string> availableKeys = new List<string>();
                    foreach (var defenition in availableFacetDefinitions)
                    {
                        availableKeys.Add(defenition.Name);
                    }

                    ContactReference reference = new ContactReference(contactId);
                    var contactTask = client.GetAsync<Contact>(
                        reference,
                        new ContactExpandOptions(availableKeys.ToArray())
                    );

                    var contact = await contactTask;

                    if (contact == null)
                    {
                        return null;
                    }

                    return contact.Facets;

                }
                catch (XdbExecutionException ex)
                {
                }
            }

            return null;
        }

        public async Task<bool> UpdateContactFacet(FacetDto facet)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    var availableFacetDefinitions = client.Model.Facets.Where(f => f.Target == EntityType.Contact);

                    List<string> availableKeys = new List<string>();
                    foreach (var defenition in availableFacetDefinitions)
                    {
                        availableKeys.Add(defenition.Name);
                    }

                    ContactReference reference = new ContactReference(Guid.Parse(facet.ContactId));
                    var contactTask = client.GetAsync<Contact>(
                        reference,
                        new ContactExpandOptions(availableKeys.ToArray())
                    );

                    var contact = await contactTask;

                    if (contact == null)
                    {
                        return false;
                    }

                    var facets =  contact.Facets;
                    foreach (var f in facets)
                    {
                       var facetName = f.Key;

                        if (facetName == facet.FacetName)
                        {
                            Type type= f.Value.GetType();
                            if (string.IsNullOrEmpty(facet.Container))
                            {
                                var property = type.GetProperty(facet.FieldName);
                                if (property == null) return false;
                                property.SetValue(f.Value, Convert.ChangeType(facet.Value, property.PropertyType),
                                    null);
                            }
                            else
                            {
                                var paths = facet.Container.Split(new[]{"$"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                                PropertyInfo property;
                                while (paths.Count > 0)
                                {
                                    var current = paths[0];
                                    paths.RemoveAt(0);
                                    property = type.GetProperty(current);
                                    if (property != null)
                                    {
                                        var value = property.GetValue(f.Value);
                                        type = property.PropertyType;
                                        property = type.GetProperty(facet.FieldName);
                                        if (property != null)
                                        {
                                            property.SetValue(value, Convert.ChangeType(facet.Value, property.PropertyType), null);
                                        }

                                    }
                                }
                              
                            }
                            client.SetFacet<Facet>(contact,facet.FacetName, f.Value);
                            await client.SubmitAsync();
                            return true;
                        }
                    }
                   

                }
                catch (XdbExecutionException ex)
                {
                }
            }

            return false;
        }

        public bool UpdateFieldValue(ref dynamic obj, FacetDto facet)
        {

            return false;
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
                            PhoneNumberList.DefaultFacetKey,
                            Pet.DefaultFacetKey)
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
                    var phoneNumbers = new PhoneNumberList(preferredPhoneNumber, "Work phone");

                    var preferredEmail = new EmailAddress(email, true);
                    var emails = new EmailAddressList(preferredEmail, "Work email");

                    var petFacet = contact.GetFacet<Pet>(Pet.DefaultFacetKey);

                    if (petFacet == null)
                    {
                        petFacet = new Pet()
                        {
                            Breed = "cat",
                            Name = "Kitty"
                        };
                    }

                    client.SetFacet<Pet>(contact, petFacet);
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

        public async Task<bool> UpdateContactInformation(
            Guid contactId,
            string firstName,
            string middleName,
            string lastName,
            string title,
            string jobTitle,
            string phone,
            string email,
            string gender,
            byte[] avatar = null)
        {
            using (XConnectClient client = GetClient())
            {
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

                    Contact contact = await contactTask;

                    if (contact == null)
                    {
                        return false;
                    }

                    var personal = contact.Personal();

                    if (personal == null)
                    {
                        personal = new PersonalInformation();
                    }

                    personal.FirstName = firstName;
                    personal.MiddleName = middleName;
                    personal.LastName = lastName;
                    personal.Title = title;
                    personal.JobTitle = jobTitle;
                    personal.Gender = gender;

                    var phoneNumbers = contact.PhoneNumbers();
                    var preferredPhoneNumber = new PhoneNumber(string.Empty, phone);

                    if (phoneNumbers == null)
                    {
                        phoneNumbers = new PhoneNumberList(preferredPhoneNumber, "Work phone");
                    }
                    else
                    {
                        phoneNumbers.PreferredPhoneNumber = preferredPhoneNumber;
                    }

                    var emails = contact.Emails();
                    var preferredEmail = new EmailAddress(email, true);

                    if (emails == null)
                    {
                        emails = new EmailAddressList(preferredEmail, "Work email");
                    }
                    else
                    {
                        emails.PreferredEmail = preferredEmail;
                    }

                    if (avatar != null)
                    {
                        var avatarFacet = contact.Avatar();

                        if (avatarFacet == null)
                        {
                            avatarFacet = new Avatar("image/jpeg", avatar);
                        }
                        else
                        {
                            avatarFacet.Picture = avatar;
                        }

                        client.SetAvatar(contact, avatarFacet);
                    }

                    client.SetPhoneNumbers(contact, phoneNumbers);
                    client.SetPersonal(contact, personal);
                    client.SetEmails(contact, emails);

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
