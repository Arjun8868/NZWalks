using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalks.API.Data
{
    public class NZWalksAuthDbContext: IdentityDbContext
    {
        public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> dbContextOptions):base(dbContextOptions) 
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "01256e5b-aeaa-47c9-af4a-c791518a712f";
            var writerRoleId = "50efbc3e-1eb0-48cf-bbab-da3900eab7c0";

            var role = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id=readerRoleId,
                    ConcurrencyStamp=readerRoleId,
                    Name="Reader",
                    NormalizedName="Reader".ToUpper()                
                    
                },
                new IdentityRole()
                {
                    Id=writerRoleId,
                    ConcurrencyStamp=writerRoleId,
                    Name="Writer",
                    NormalizedName="Writer".ToUpper()
                }

            };

            builder.Entity<IdentityRole>().HasData(role);
        }
    }
}
