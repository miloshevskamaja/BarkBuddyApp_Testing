namespace BarkBuddyApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nineth : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groomings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReservationDateTime = c.DateTime(nullable: false),
                        DogBreed = c.String(),
                        DogAge = c.Int(nullable: false),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Groomings");
        }
    }
}
