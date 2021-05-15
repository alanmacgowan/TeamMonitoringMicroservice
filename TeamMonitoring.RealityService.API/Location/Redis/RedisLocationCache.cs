using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace TeamMonitoring.RealityService.Location.Redis
{
    public class RedisLocationCache : ILocationCache
    {
        protected readonly ILogger _logger;
        protected readonly IConnectionMultiplexer _connection;

        public RedisLocationCache(ILogger<RedisLocationCache> logger,
                                  IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _connection = connectionMultiplexer;

            _logger.LogInformation($"Using redis location cache - {connectionMultiplexer.Configuration}");
        }

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

        public MemberLocation Get(Guid teamId, Guid memberId)
        {
            IDatabase db = _connection.GetDatabase();

            var value = (string)db.HashGet(teamId.ToString(), memberId.ToString());
            MemberLocation ml = MemberLocation.FromJsonString(value);
            return ml;
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