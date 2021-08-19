using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteDB;
namespace botspace
{
    // Create your POCO class entity
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }

    public class litedbsample
    {

        public void rebuildDb(string oldPass, string newPass)
        {
            ConnectionString connectionString = new ConnectionString();
            connectionString.Password = oldPass;
            connectionString.Filename = @"C:\Temp\MyData2.db";
                
            using (var db = new LiteDatabase(connectionString))
            {
                LiteDB.Engine.RebuildOptions rebuildOptions = new LiteDB.Engine.RebuildOptions();
             
                rebuildOptions.Password = newPass;
                db.Rebuild(rebuildOptions);
            }
        }

        public void test(string pass)
        {
            ConnectionString connectionString = new ConnectionString();
            connectionString.Password = pass;
            connectionString.Filename = @"C:\Temp\MyData2.db";

            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(connectionString))
            {

                LiteDB.Engine.RebuildOptions rebuildOptions = new LiteDB.Engine.RebuildOptions();
                rebuildOptions.Password = "123";
                db.Rebuild(rebuildOptions);

                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Customer>("customers");

                // Create your new customer instance
                var customer = new Customer
                {
                    Name = "Жон До",
                    Phones = new string[] { "80ллл00-0000", "9000-0000" },
                    IsActive = true
                };

                // Insert new customer document (Id will be auto-incremented)
                col.Insert(customer);

                // Update a document inside a collection
                customer.Name = "ЖaЛЛЛne Doe";

                col.Update(customer);

                // Index document using document Name property
                col.EnsureIndex(x => x.Name);

                // Use LINQ to query documents (filter, sort, transform)
                var results = col.Query()
                    .Where(x => x.Name.StartsWith("Ж"))
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
                    .Limit(10)
                    .ToList();

                // Let's create an index in phone numbers (using expression). It's a multikey index
                col.EnsureIndex(x => x.Phones);

                // and now we can query phones
                var r = col.FindOne(x => x.Phones.Contains("80ллл00-0000"));

                //Console.OutputEncoding = System.Text.Encoding.UTF8;

                if (r != null)
                {
                    //Console.WriteLine(r.Name);
                    //Console.ReadLine();
                }
            }
        }
    }
}

