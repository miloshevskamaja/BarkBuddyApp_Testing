namespace BarkBuddyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class final : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderViewModels", "Buyer_Id", "dbo.Buyers");
            DropIndex("dbo.OrderViewModels", new[] { "Buyer_Id" });
            AddColumn("dbo.ShoppingCartItems", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.DogBreeds", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Producers", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Producers", "Logo", c => c.String(nullable: false));
            AlterColumn("dbo.Producers", "Description", c => c.String(maxLength: 2000));
            AlterColumn("dbo.GroomingDogs", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Groomings", "DogBreed", c => c.String(nullable: false));
            AlterColumn("dbo.OrderViewModels", "Buyer_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ShoppingCartItems", "ProductName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Toys", "Name", c => c.String(nullable: false));
            CreateIndex("dbo.OrderViewModels", "Buyer_Id");
            CreateIndex("dbo.ShoppingCartItems", "ProductId");
            AddForeignKey("dbo.ShoppingCartItems", "ProductId", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderViewModels", "Buyer_Id", "dbo.Buyers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderViewModels", "Buyer_Id", "dbo.Buyers");
            DropForeignKey("dbo.ShoppingCartItems", "ProductId", "dbo.Products");
            DropIndex("dbo.ShoppingCartItems", new[] { "ProductId" });
            DropIndex("dbo.OrderViewModels", new[] { "Buyer_Id" });
            AlterColumn("dbo.Toys", "Name", c => c.String());
            AlterColumn("dbo.ShoppingCartItems", "ProductName", c => c.String());
            AlterColumn("dbo.OrderViewModels", "Buyer_Id", c => c.Int());
            AlterColumn("dbo.Groomings", "DogBreed", c => c.String());
            AlterColumn("dbo.GroomingDogs", "Name", c => c.String());
            AlterColumn("dbo.Producers", "Description", c => c.String());
            AlterColumn("dbo.Producers", "Logo", c => c.String());
            AlterColumn("dbo.Producers", "Name", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.DogBreeds", "Name", c => c.String());
            DropColumn("dbo.ShoppingCartItems", "ProductId");
            CreateIndex("dbo.OrderViewModels", "Buyer_Id");
            AddForeignKey("dbo.OrderViewModels", "Buyer_Id", "dbo.Buyers", "Id");
        }
    }
}
