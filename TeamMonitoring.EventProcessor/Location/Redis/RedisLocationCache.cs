using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace TeamMonitoring.EventProcessor.Location.Redis
{
    public class RedisLocationCache : ILocationCache
    {
        private ILogger _logger;
        private IConnectionMultiplexer _connection;

        public RedisLocationCache(ILogger<RedisLocationCache> logger,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _connection = connectionMultiplexer;

            logger.LogInformation($"Using redis location cache - {connectionMultiplexer.Configuration}");
        }

        // This is a hack required to get injection working
        // because Steeltoe's redis connector injected the concrete class as binding
        // and not the interface.
        public RedisLocationCache(ILogger<RedisLocationCache> logger,
            ConnectionMultiplexer connectionMultiplexer) : this(logger, (IConnectionMultiplexer)connectionMultiplexer)
        {

        }

        public IList<MemberLocation> GetMemberLocations(Guid teamId)
        {
            IDatabase db = _connection.GetDatabase();

            RedisValue[] vals = db.HashValues(teamId.ToString());

            return ConvertRedisValsToLocationList(vals);
        }

        public void Put(Guid teamId, MemberLocation memberLocation)
        {
            IDatabase db = _connection.GetDatabase();

            db.HashSet(teamId.ToString(), memberLocation.MemberID.ToString(), memberLocation.ToJsonString());
        }

        private IList<MemberLocation> ConvertRedisValsToLocationList(RedisValue[] vals)
        {
            List<MemberLocation> memberLocations = new List<MemberLocation>();

            for (int x = 0; x < vals.Length; x++)
            {
                string val = (string)vals[x];
                MemberLocation ml = MemberLocation.FromJsonString(val);
                memberLocations.Add(ml);
            }

            return memberLocations;
        }
    }
}