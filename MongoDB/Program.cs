using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB
{
    class Program
    {
        public static string randomWord = "";
        public static string encryptedString2 = "";
        static void Main(string[] args)
        {
            MongoClient dbClient = new MongoClient("mongodb://margaretaG:test@cluster0-shard-00-00.m84mg.mongodb.net:27017,cluster0-shard-00-01.m84mg.mongodb.net:27017,cluster0-shard-00-02.m84mg.mongodb.net:27017/Lab7?ssl=true&replicaSet=atlas-1086yx-shard-0&authSource=admin&retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("UsersDatabaseName");

            var collection = database.GetCollection<BsonDocument>("UsersCollectionName");

            var document = new BsonDocument();

            BsonDocument userExample1 =
                new BsonDocument {
                    { "Id", "ldksj-28292-ddd-3232" },
                    { "name", "John DOe" },
                    { "email", "john@gmail.com" },
                    { "dateBirth", "15/08/1980" },
                    { "hobby", "sport"},
                };

            BsonDocument userExample2 =
                new BsonDocument {
                    { "Id", "8d86d6-2ee92-dd2fd-iod32" },
                    { "name", "Tim SPark" },
                    { "email", "tim@gmail.com" },
                    { "dateBirth", "8/10/1971" },
                    { "hobby", "sport"},
                };

            BsonDocument userExample3 =
                new BsonDocument {
                    { "Id", "12343-oo2h22-11ddd-jd232" },
                    { "name", "Jack Sparrow" },
                    { "email", "jack_sparrow@gmail.com" },
                    { "dateBirth", "06/10/1973" },
                    { "hobby", "sport"},
                };

          /*  database.DropCollection("UsersCollectionName");*/

         /*   collection.InsertMany(new List<BsonDocument> { userExample1, userExample2, userExample3 });*/
           
            Console.WriteLine(document.ToString());
            var filter = Builders<BsonDocument>.Filter.Eq("hobby", "sport");
            var result = collection.Find(filter).ToList();

           //updateDBWithEncryptedValue("email", result, collection);
           updateDBWithDecryptedValue("email", result, collection);
        }

        public static void updateDBWithEncryptedValue(string dbField, List<BsonDocument> filtersResult, IMongoCollection<BsonDocument> collection)
        {
            foreach (var doc in filtersResult)
            {
                var value = doc.GetValue(dbField);

                var thisFilter = Builders<BsonDocument>.Filter.Eq("_id", doc.GetValue("_id"));

                Console.WriteLine("Value: " + value.ToString());
                Console.WriteLine("\n*=======*\n");
                string encryptedString = Encrypt(value.ToString());
                encryptedString2 = Encrypt(encryptedString);
                Console.WriteLine("encrypted value: " + encryptedString2);
                Console.WriteLine("\n*=======*\n");

                var update = Builders<BsonDocument>.Update.Set(dbField, encryptedString2);

                collection.UpdateOne(thisFilter, update);
            }
        }

        public static void updateDBWithDecryptedValue(string dbField, List<BsonDocument> filtersResult, IMongoCollection<BsonDocument> collection)
        {
            foreach (var doc in filtersResult)
            {
                var value = doc.GetValue(dbField).AsString;

                var thisFilter = Builders<BsonDocument>.Filter.Eq("_id", doc.GetValue("_id"));

                string decryptedString = Decrypt(value);
                string decryptedString2 = Decrypt(decryptedString);
                Console.WriteLine("decrypted value: " + decryptedString2);
                Console.WriteLine("\n*=======*\n");
                var update = Builders<BsonDocument>.Update.Set(dbField, decryptedString2);

                collection.UpdateOne(thisFilter, update);
            }
        }

        public static string Encrypt(string text)
        {
            string EncryptionKey = "EncryptedKey";
            byte[] clearBytes = Encoding.Unicode.GetBytes(text);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    text = Convert.ToBase64String(ms.ToArray());
                }
            }
            return text;
        }

        public static string Decrypt(string text)
        {
            string EncryptionKey = "EncryptedKey";
            text = text.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(text);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    text = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return text;
        }
    }

}





