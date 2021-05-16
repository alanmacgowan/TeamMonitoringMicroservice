using TeamMonitoring.Events;

namespace TeamMonitoring.LocationReporter.API.Models
{
    public interface ICommandEventConverter
    {
        MemberLocationRecordedEvent CommandToEvent(LocationReport locationReport);
    }
}