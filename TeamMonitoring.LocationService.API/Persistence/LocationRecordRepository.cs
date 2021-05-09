// using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Collections.Generic;
using TeamMonitoring.LocationService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamMonitoring.LocationService.API.Persistence
{
    public class LocationRecordRepository : ILocationRecordRepository
    {
        private LocationDbContext context;

        public LocationRecordRepository(LocationDbContext context)
        {
            this.context = context;
        }

        public LocationRecord Add(LocationRecord locationRecord)
        {
            this.context.Add(locationRecord);
            this.context.SaveChanges();
            return locationRecord;
        }

        public LocationRecord Update(LocationRecord locationRecord)
        {
            this.context.Entry(locationRecord).State = EntityState.Modified;
            this.context.SaveChanges();
            return locationRecord;
        }

        public LocationRecord Get(Guid memberId, Guid recordId)
        {
            return this.context.LocationRecords.FirstOrDefault(lr => lr.MemberID == memberId && lr.ID == recordId);
        }

        public LocationRecord Delete(Guid memberId, Guid recordId)
        {
            LocationRecord locationRecord = this.Get(memberId, recordId);
            this.context.Remove(locationRecord);
            this.context.SaveChanges();
            return locationRecord;
        }

        public LocationRecord GetLatestForMember(Guid memberId)
        {
            LocationRecord locationRecord = this.context.LocationRecords.
                Where(lr => lr.MemberID == memberId).
                OrderBy(lr => lr.Timestamp).
                Last();
            return locationRecord;
        }

        public ICollection<LocationRecord> AllForMember(Guid memberId)
        {
            return this.context.LocationRecords.
                Where(lr => lr.MemberID == memberId).
                OrderBy(lr => lr.Timestamp).
                ToList();
        }
    }
}
