namespace Hackathon.XEditor.Api.Dto
{
    using Sitecore.XConnect.Collection.Model;

    public class PersonalDto
    {
        public PersonalDto(PersonalInformation p)
        {
            if (p == null)
            {
                return;
            }

            FirstName = p.FirstName;
            LastName = p.LastName;
            MiddleName = p.MiddleName;
            Gender = p.Gender;
            JobTitle = p.JobTitle;
            Title = p.Title;
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string JobTitle { get; set; }

        public string Title { get; set; }
    }
}
