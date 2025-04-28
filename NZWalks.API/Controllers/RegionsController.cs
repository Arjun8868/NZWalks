using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    //https://localhost:portnumber/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
       public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            
        }


        //GET All Regions
        //GET: https://localhost:portnumber/api/Regions
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
          
            //Get data from database - domain models
            var regionsDomain = await regionRepository.GetAllAsync();

            //Map region domain models to DTO
            var regionDTO = new List<RegionDTO>();
            foreach (var regionDomain in regionsDomain)
            {
                regionDTO.Add(new RegionDTO()
                {
                    Id= regionDomain.Id,
                    Name= regionDomain.Name,
                    Code= regionDomain.Code,
                    RegionImageUrl= regionDomain.RegionImageUrl                   

                });

            }
            //return DTOs 
            return Ok(regionDTO);
        }

        //Get Regions by Id
        //GET: https://localhost:portnumber/api/Regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id) 
        {
            //var region= dbContext.Regions.Find(id); //Find will work only for the primary key
            //Get region domail model from db
            var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);

            if (regionDomain == null) {
                return NotFound();
            }
            //Map region domain models to DTO
            var regionDTO = new List<RegionDTO>();
            
            regionDTO.Add(new RegionDTO()
            {
                 Id = regionDomain.Id,
                 Name = regionDomain.Name,
                 Code = regionDomain.Code,
                 RegionImageUrl = regionDomain.RegionImageUrl

            });
            
            //return DTO
            return Ok(regionDTO);

        }

        //POST to create new region
        //POST: https://localhost:portnumber/api/Regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]AddRegionRequestDto addRegionRequestDto)
        {
            //Map DTO to domain model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };
            //Use Domain Model to create Region
            await dbContext.Regions.AddAsync(regionDomainModel);
            await dbContext.SaveChangesAsync();

            //Map Domain Model back to DTO
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }
        //Update Region
        //PUT: https://localhost:portnumber/api/Regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //check if region exists
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }
            //Map DTO to domain model
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            //Convert Domain Model to DTO
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok(regionDto);

        }
        //Delete: to delete a region
        //DELETE: https://localhost:portnumber/api/Regions{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel =await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            if (regionDomainModel == null)
                return NotFound();

            //Delete region
            dbContext.Regions.Remove(regionDomainModel);
            await dbContext.SaveChangesAsync();
            //Map Domain Model to DTO
            var regionDto = new RegionDTO
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(regionDto);

        }
    }
}                                            
