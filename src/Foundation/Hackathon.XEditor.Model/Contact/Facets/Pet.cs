using System;
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;

namespace Hackathon.XEditor.Model.Contact.Facets
{
    [FacetKey(DefaultFacetKey)]
    [PIISensitive]
    public class Pet : Facet
    {
        public const string DefaultFacetKey = "Pet";

        public string Breed { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public string MimeType { get; set; }

        [DoNotIndex]
        [PIISensitive]
        public byte[] Picture { get; set; }

    }
}
