using System;
using Hackathon.XEditor.Model.Models;

namespace Hackathon.XEditor.ModelSerializerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PetModel: ");
            string json = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(PetModel.Model);
            Console.Write(json);
            System.IO.File.WriteAllText($".\\{PetModel.Model.FullName}.json", json);

            Console.ReadKey();
        }
    }
}
