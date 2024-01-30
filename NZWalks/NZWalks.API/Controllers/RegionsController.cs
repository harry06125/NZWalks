using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;

        public RegionsController(NZWalksDbContext _dbContext,IRegionRepository regionRepository)
        {
            dbContext = _dbContext;
            this.regionRepository = regionRepository;
        }

        //GET ALL REGIONS
        //GET : https://localhost:7161/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Get data from database - Domain model
            var regionsDomain = await regionRepository.GetAllAsync();
            //Map Domain model to DTO
            var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Name = regionDomain.Name,
                    Code = regionDomain.Code,
                    RegionImageUrl = regionDomain.RegionImageUrl
                });
            }
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
            var regionDto = new RegionDto()
            {
                Id = regionDomain.Id,
                Name = regionDomain.Name,
                Code = regionDomain.Code,
                RegionImageUrl = regionDomain.RegionImageUrl
            };

            return Ok(regionDto);
        }

        //POST To Create New Region
        //POST: https://localhost:7161/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto requestDto)
        {
            //Map or Convert DTO To Domain Model

            var regionDomainModel = new Region()
            {
                Code = requestDto.Code,
                RegionImageUrl = requestDto.RegionImageUrl,
                Name = requestDto.Name,
            };

            //Use Domain Model To Create Region
            regionDomainModel =  await regionRepository.CreateAsync(regionDomainModel);
         
            //Map Domain Model Back To DTO

            var regionDTO = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = regionDTO.Id }, regionDTO);
        }

        //Update region
        //PUT https://localhost:6171/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomainModel = new Region();
            //Map DTO to Domain Model 
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //Convert Domain Model To DTO

            var regionDTO = new RegionDto()
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

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
            var regionDTO = new RegionDto()
            {
                 Id = region.Id,
                 Code = region.Code,
                 Name = region.Name,
                 RegionImageUrl = region.RegionImageUrl
            };
            return Ok(regionDTO);
        }
    }
}
