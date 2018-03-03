namespace Hackathon.XEditor.Api.Dto
{
    public class ContactDto
    {
        public string ContactId { get; set; }

        public PersonalDto PersonalInformation { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool Avatar { get; set; }
    }
}
