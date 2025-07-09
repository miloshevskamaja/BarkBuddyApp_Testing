namespace BarkBuddyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DogBreeds", "Product_Id", "dbo.Products");
            DropIndex("dbo.DogBreeds", new[] { "Product_Id" });
            CreateTable(
                "dbo.ProductDogBreeds",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        DogBreedId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductId, t.DogBreedId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.DogBreeds", t => t.DogBreedId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.DogBreedId);
            
            DropColumn("dbo.DogBreeds", "Product_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DogBreeds", "Product_Id", c => c.Int());
            DropForeignKey("dbo.ProductDogBreeds", "DogBreedId", "dbo.DogBreeds");
            DropForeignKey("dbo.ProductDogBreeds", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductDogBreeds", new[] { "DogBreedId" });
            DropIndex("dbo.ProductDogBreeds", new[] { "ProductId" });
            DropTable("dbo.ProductDogBreeds");
            CreateIndex("dbo.DogBreeds", "Product_Id");
            AddForeignKey("dbo.DogBreeds", "Product_Id", "dbo.Products", "Id");
        }
    }
}
