using Hackathon.XEditor.Model.Contact.Facets;
using Sitecore.XConnect.Schema;

namespace Hackathon.XEditor.Model.Models
{
    public class PetModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("PetModel", new XdbModelVersion(1, 0));
            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);

            modelBuilder.DefineFacet<Sitecore.XConnect.Contact, Pet>("PetInfo");

            return modelBuilder.BuildModel();
        }
    }
}
