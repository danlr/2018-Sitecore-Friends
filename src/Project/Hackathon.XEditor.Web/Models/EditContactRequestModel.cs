namespace Hackathon.XEditor.Web.Models
{
    using System;

    public class EditContactRequestModel
    {
        public Guid ContactId { get; set; }
        
        public string Email { get; set; }

        public string Phone { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string JobTitle { get; set; }

        public string Title { get; set; }
    }
}