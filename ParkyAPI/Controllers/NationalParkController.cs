using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "v1")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParkController : ControllerBase
    {
        private readonly INationalParkRepository _parkRepository;
        private readonly IMapper _mapper;

        public NationalParkController(INationalParkRepository parkRepository, IMapper mapper)
        {
            _parkRepository = parkRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of national parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetParks()
        {
            var objList = _parkRepository.GetNationalParks();

            //var objDto = new List<NationalParkDto>();

            //foreach (var obj in objList) 
            //{
            //    objDto.Add(_mapper.Map<NationalParkDto>(obj));
            //}

            var vmDto = _mapper.Map<List<NationalParkDto>>(objList);

            return Ok(vmDto);
        }

        /// <summary>
        /// Get individual national park.
        /// </summary>
        /// <param name="nationalParkId"> The Id of national Park </param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public IActionResult GetPark(int nationalParkId) 
        {
            var obj = _parkRepository.GetNationalPark(nationalParkId);
            if (obj == null) 
            {
                return NotFound();
            }

            var vmDto = _mapper.Map<NationalParkDto>(obj);

            return Ok(vmDto);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePark([FromBody] NationalParkDto nationalParkDto) 
        {
            if (nationalParkDto == null) 
            {
                return BadRequest(ModelState);
            }

            if (_parkRepository.NationalParkExist(nationalParkDto.Name)) 
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_parkRepository.CreateNationalPark(nationalParkObj)) 
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetPark", new { nationalParkId = nationalParkObj.Id}, nationalParkObj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdatePark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto) 
        {
            if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_parkRepository.UpdataNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updateing the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeletePark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePark(int nationalParkId) 
        {
            if (!_parkRepository.NationalParkExist(nationalParkId)) 
            {
                return NotFound();
            }

            var nationalParkObj = _parkRepository.GetNationalPark(nationalParkId);
            if (!_parkRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
