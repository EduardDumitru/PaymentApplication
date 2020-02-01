using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentApplication.Domain;
using PaymentApplication.Shared;

namespace PaymentApplication.Services
{
    public class FileService : IFileService
    {
        public async Task SaveToJson(CreditCard creditCard)
        {
            try
            {
                var creditCards = new List<CreditCard>();
                // Find if file exists
                if (File.Exists(SharedValues.FilePath))
                {
                    // Take all of the existing payments from the json file and add the one that we just made to it
                    // This way we won't delete the existing payments from the file
                    creditCards = await GetCreditCards();
                }
                else
                {
                    // serialize JSON to a string and then write string to a file
                    File.WriteAllText(SharedValues.FilePath, JsonConvert.SerializeObject(creditCard));
                }

                // serialize JSON directly to a file
                await using (StreamWriter file = File.CreateText(SharedValues.FilePath))
                {
                    creditCards.Add(creditCard);
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, creditCards);
                }

                Console.WriteLine("It was saved to JSON with success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public Task<List<CreditCard>> GetCreditCards()
        {
            using var reader = new StreamReader(SharedValues.FilePath);
            string json = reader.ReadToEnd();
            return Task.FromResult(JsonConvert.DeserializeObject<List<CreditCard>>(json));
        }
    }
}
