using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository: IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;
        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<Walk>IWalkRepository.CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;

        }

        async Task<Walk?> IWalkRepository.DeleteAsync(Guid id)
        {
            var existingWalk=await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if(existingWalk == null)
            {
                return null;
            }
            dbContext.Walks.Remove(existingWalk);//remove method is not asynchronus
            await dbContext.SaveChangesAsync();
            return existingWalk;
        }

        async Task<List<Walk>> IWalkRepository.GetAllAsync(string? filterOn = null, string? filterQuery = null, 
                                                           string? sortBy = null, bool isAscending = true,
                                                           int pageNum = 1, int pageSize = 100)
        {
            var walks= dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            //Filtering
            if(string.IsNullOrWhiteSpace(filterOn)==false && string.IsNullOrWhiteSpace(filterQuery)==false)
            {
                if(filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                   walks=walks.Where(x=>x.Name.Contains(filterQuery));
                }

            }
            //Sorting
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                //Sort by Name
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name):walks.OrderByDescending(x=> x.Name);
                }
                //Sort by Length
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInkm) : walks.OrderByDescending(x => x.LengthInkm);
                }
            }
            //Pagination
            var skipResults = (pageNum - 1) * pageSize;
            /* This calculates how many results to skip based on the current page number (pageNum) and the page size (pageSize).

            For example:

            If pageNum = 1 and pageSize = 10, then skipResults = 0 (skip 0 rows, show rows 1–10).

            If pageNum = 2, then skipResults = 10 (skip first 10 rows, show rows 11–20).

            If pageNum = 3, then skipResults = 20 (skip first 20 rows, show rows 21–30), and so on. */

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
           // return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();

        }

        async Task<Walk?> IWalkRepository.GetByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(w => w.Id == id);
        }

        async Task<Walk?> IWalkRepository.UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk=await dbContext.Walks.FirstOrDefaultAsync(x=>x.Id== id);
            if(existingWalk == null)
            {
                return null;
            }
            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInkm = walk.LengthInkm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.RegionId= walk.RegionId;
            existingWalk.DifficultyId= walk.DifficultyId;
            
            await dbContext.SaveChangesAsync();
            return existingWalk;
        }
    }
}
