using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Collections.Generic;
using NZWalks.API.CustomActionFilters;
using Microsoft.AspNetCore.Authorization;

namespace NZWalks.API.Controllers
{
    //https://localhost:portnumber/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        //private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(/*NZWalksDbContext dbContext,*/IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
           // this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger= logger;
        }


        //GET All Regions
        //GET: https://localhost:portnumber/api/Regions
        [HttpGet]
        //[Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll() 
        {
            logger.LogError("Get All Method in Region Controller");
          
            //Get data from database - domain models
            var regionsDomain = await regionRepository.GetAllAsync();
          
            //Map region domain models to DTO
            //var regionDTO = new List<RegionDTO>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionDTO.Add(new RegionDTO()
            //    {
            //        Id= regionDomain.Id,
            //        Name= regionDomain.Name,
            //        Code= regionDomain.Code,
            //        RegionImageUrl= regionDomain.RegionImageUrl                   

            //    });

            //}

            //Map region domain models to DTO using automapper
            var regionDTO = mapper.Map<List<RegionDTO>>(regionsDomain);
            //return DTOs 
            return Ok(regionDTO);
        }

        //Get Regions by Id
        //GET: https://localhost:portnumber/api/Regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
       // [Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]Guid id) 
        {
            //var region= dbContext.Regions.Find(id); //Find will work only for the primary key
            //Get region domail model from db
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null) {
                return NotFound();
            }
            //Map region domain models to DTO
            //var regionDTO = new RegionDTO
            //{
            //     Id = regionDomain.Id,
            //     Name = regionDomain.Name,
            //     Code = regionDomain.Code,
            //     RegionImageUrl = regionDomain.RegionImageUrl

            //};

            //return DTO
            var regionDTO = mapper.Map<RegionDTO>(regionDomain);
            return Ok(regionDTO);

        }

        //POST to create new region
        //POST: https://localhost:portnumber/api/Regions
        [HttpPost]
        //[Authorize(Roles = "Writer")]
        //[ValidateModelAttributes]
        public async Task<IActionResult> Create([FromBody]AddRegionRequestDto addRegionRequestDto)
        {
            if(ModelState.IsValid)
            {
                //Map DTO to domain model
                //var regionDomainModel = new Region
                //{
                //    Code = addRegionRequestDto.Code,
                //    Name = addRegionRequestDto.Name,
                //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
                //};

                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                //Use Domain Model to create Region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //Map Domain Model back to DTO
                //var regionDto = new RegionDTO
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl
                //};
                var regionDto = mapper.Map<RegionDTO>(regionDomainModel);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = regionDto.Id }, regionDto);

            }
            else
            {
                return BadRequest(ModelState);
            }
           
        }
        //Update Region
        //PUT: https://localhost:portnumber/api/Regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
       // [Authorize(Roles = "Writer")]
        // [ValidateModelAttributes]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            if (ModelState.IsValid)
            {


                //Map DTO to Domain Model
                //var regionDomainModel = new Region
                //{
                //    Code=updateRegionRequestDto.Code,
                //    Name = updateRegionRequestDto.Name,
                //    RegionImageUrl=updateRegionRequestDto.RegionImageUrl
                //};

                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }


                //Convert Domain Model to DTO
                //var regionDto = new RegionDTO
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl
                //};
                var regionDto = mapper.Map<RegionDTO>(regionDomainModel);

                return Ok(regionDto);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
        //Delete: to delete a region
        //DELETE: https://localhost:portnumber/api/Regions{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel =await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
                return NotFound();

            //Map Domain Model to DTO
            //var regionDto = new RegionDTO
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDTO>(regionDomainModel);
            return Ok(regionDto);

        }
    }
}                                            
