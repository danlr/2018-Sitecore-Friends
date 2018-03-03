namespace Hackathon.XEditor.Api.Dto
{
    using System;

    public class ContactDto
    {
        public Guid ContactId { get; set; }

        public PersonalDto PersonalInformation { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool IsAvatar { get; set; }

        public byte[] Avatar { get; set; }
    }
}
