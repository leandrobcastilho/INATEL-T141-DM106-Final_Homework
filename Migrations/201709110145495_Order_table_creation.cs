namespace INATEL_T141_DM106_Final_Homework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order_table_creation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserEmail = c.String(),
                        Date = c.DateTime(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceShip = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Double(nullable: false),
                        ProductId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductOrders", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.ProductOrders", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductOrders", new[] { "OrderId" });
            DropIndex("dbo.ProductOrders", new[] { "ProductId" });
            DropTable("dbo.ProductOrders");
            DropTable("dbo.Orders");
        }
    }
}
