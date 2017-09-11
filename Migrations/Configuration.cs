namespace INATEL_T141_DM106_Final_Homework.Migrations
{
    using INATEL_T141_DM106_Final_Homework.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<INATEL_T141_DM106_Final_Homework.Models.INATEL_T141_DM106_Final_HomeworkContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(INATEL_T141_DM106_Final_Homework.Models.INATEL_T141_DM106_Final_HomeworkContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Products.AddOrUpdate(
                p => p.Id,
                new Product
                {
                    Name = "Product_1",
                    Description = "Description Product 1",
                    Color = "blue",
                    Model = "A",
                    Code = "Code1",
                    Price = 10,
                    Weight = 10,
                    Height = 10,
                    Width = 0,
                    Length = 0,
                    Diameter = 10,
                    URL = "http://T141_DM106_Final_Homework/Product_1"
                },

                new Product
                {
                    Name = "Product_2",
                    Description = "Description Product 2",
                    Color = "yellow",
                    Model = "B",
                    Code = "Code2",
                    Price = 8,
                    Weight = 8,
                    Height = 8,
                    Width = 8,
                    Length = 8,
                    Diameter = 0,
                    URL = "http://T141_DM106_Final_Homework/Product_2"
                },

                new Product
                {
                    Name = "Product_3",
                    Description = "Description Product 3",
                    Color = "gray",
                    Model = "C",
                    Code = "Code3",
                    Price = 5,
                    Weight = 5,
                    Height = 5,
                    Width = 5,
                    Length = 5,
                    Diameter = 0,
                    URL = "http://T141_DM106_Final_Homework/Product_3"
                }

            );

        }
    }
}
