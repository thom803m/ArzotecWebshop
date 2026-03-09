using ArzotecWebshop.Core.Interfaces.Services;
using ArzotecWebshop.Core.Models;
using ArzotecWebshop.Infrastructure.Integrations.Rackbeat;

namespace ArzotecWebshop.Infrastructure.Services
{
    public class RackbeatSyncService : IRackbeatSyncService
    {
        private readonly RackbeatClient _rackbeatClient;

        public RackbeatSyncService(RackbeatClient rackbeatClient)
        {
            _rackbeatClient = rackbeatClient;
        }

        public async Task SyncProductsAsync()
        {
            var json = await _rackbeatClient.GetProductsRawAsync();

            Console.WriteLine(json);
        }
    }
}
