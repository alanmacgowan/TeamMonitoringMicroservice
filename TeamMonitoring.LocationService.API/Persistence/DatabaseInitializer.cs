namespace TeamMonitoring.LocationService.API.Persistence
{
    public static class DatabaseInitializer
    {
        public static void Initialize(LocationDbContext context)
        {
            context.Database.EnsureCreated();

        }
    }
}
