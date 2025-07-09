namespace BarkBuddyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eight : DbMigration
    {
        public override void Up()
        {
            CreateTable(
              "dbo.ShoppingCartItems",
              c => new
              {
                  Id = c.Int(nullable: false, identity: true),
                  ProductName = c.String(),
                  Price = c.Double(nullable: false),
                  Quantity = c.Int(nullable: false),
                  OrderId = c.Int(nullable: false), // This column is a foreign key
              })
              .PrimaryKey(t => t.Id)
              .ForeignKey("dbo.OrderViewModels", t => t.OrderId, cascadeDelete: true)
              .Index(t => t.OrderId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCartItems", "OrderId", "dbo.OrderViewModels");
            DropIndex("dbo.ShoppingCartItems", new[] { "OrderId" });
            DropTable("dbo.ShoppingCartItems");
        }
    }
}
