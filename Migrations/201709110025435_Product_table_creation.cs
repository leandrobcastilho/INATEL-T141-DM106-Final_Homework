namespace INATEL_T141_DM106_Final_Homework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Product_table_creation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Color = c.String(),
                        Model = c.String(nullable: false),
                        Code = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Weight = c.Double(nullable: false),
                        Height = c.Double(nullable: false),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        Diameter = c.Double(nullable: false),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
