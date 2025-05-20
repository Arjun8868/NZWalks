using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
   
    //api/Walks
    [Route("api/[controller]")] //[Route("api/Walks")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;
       
       public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
           this.mapper = mapper;
           this.walkRepository = walkRepository;
          
        }

        //CREATE Walk
        //POST: /api/Walks
        [HttpPost]
        //[ValidateModelAttributes]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            if (ModelState.IsValid)
            {


                //Map DTO to Domain Model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
                walkDomainModel = await walkRepository.CreateAsync(walkDomainModel);

                //Map Domain Model back to DTO
                var walkDTo = mapper.Map<WalkDTO>(walkDomainModel);
                return Ok(walkDTo);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        //Get Walks
        //GET: /api/Walks?filterOn=Name&filterQuery=Track&sortBy=Name&IsAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
                                                     [FromQuery] string? sortBy, [FromQuery] bool isAscending,
                                                     [FromQuery] int pageNum=1, [FromQuery] int pageSize= 1000)
        {
            var walkDomain = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNum, pageSize);

            //Create an exception to check ExceptionHandlerMiddleware
            throw new Exception("This is an Exception");

            //Map Domain Model to DTO
            var walkDTO = mapper.Map<List<WalkDTO>>(walkDomain);
            return Ok(walkDTO);

        }
        //Get Walks By Id
        //GET: /api/Walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var walkDomain= await walkRepository.GetByIdAsync(id);
            if(walkDomain == null)
            {
                return NotFound();
            }
            var walkDTO = mapper.Map<WalkDTO>(walkDomain);
            return Ok(walkDTO);
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteByIdAsync([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.DeleteAsync(id);
            if(walkDomain == null)
            {
                return NotFound();
            }
            var walkDTO = mapper.Map<WalkDTO>(walkDomain);
            return Ok(walkDTO);
        }

        //UPDATE Walk By Id
        //PUT:/api/Walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        //[ValidateModelAttributes]
        public async Task<IActionResult> UpdateByIdAsync([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            if (ModelState.IsValid)
            {

                var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);
                walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);
                if (walkDomainModel == null)
                {
                    return NotFound();
                }

                var walkDto = mapper.Map<WalkDTO>(walkDomainModel);
                return Ok(walkDto);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

    }
}
