using Microsoft.AspNetCore.Mvc;
using System;
using TeamMonitoring.LocationService.API.Models;

namespace TeamMonitoring.LocationService.API.Controllers
{

    [Route("locations/{memberId}")]
    public class LocationRecordController : ControllerBase
    {

        private ILocationRecordRepository locationRepository;

        public LocationRecordController(ILocationRecordRepository repository)
        {
            this.locationRepository = repository;
        }

        [HttpPost]
        public IActionResult AddLocation(Guid memberId, [FromBody] LocationRecord locationRecord)
        {
            locationRepository.Add(locationRecord);
            return this.Created($"/locations/{memberId}/{locationRecord.ID}", locationRecord);
        }

        [HttpGet]
        public IActionResult GetLocationsForMember(Guid memberId)
        {
            return this.Ok(locationRepository.AllForMember(memberId));
        }

        [HttpGet("latest")]
        public IActionResult GetLatestForMember(Guid memberId)
        {
            return this.Ok(locationRepository.GetLatestForMember(memberId));
        }
    }
}