using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(IRegionRepository regionRepository,IMapper mapper)
        {
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        //GET ALL REGIONS
        //GET : https://localhost:7161/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Get data from database - Domain model
            var regionsDomain = await regionRepository.GetAllAsync();
            //Map Domain model to DTO
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
            //return DTOs
            return Ok(regionsDto);
        }

        //GET REGION BY ID 
        //GET: https://localhost:7161/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain == null)
            {
                return NotFound();
            }

            //Map Domain Model to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomain);    

            return Ok(regionDto);
        }

        //POST To Create New Region
        //POST: https://localhost:7161/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto requestDto)
        {
            //Map or Convert DTO To Domain Model

            var regionDomainModel = mapper.Map<Region>(requestDto);

            //Use Domain Model To Create Region
            regionDomainModel =  await regionRepository.CreateAsync(regionDomainModel);
         
            //Map Domain Model Back To DTO
            var regionDTO = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
        }

        //Update region
        //PUT https://localhost:6171/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //Convert Domain Model To DTO

            var regionDTO = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDTO);
        }

        //Delete A Region
        //Delete: https://localhost:7161/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id); 

            if (region == null)
            {
                return NotFound();
            }
            var regionDTO = mapper.Map<RegionDto> (region);
            return Ok(regionDTO);
        }
    }
}
