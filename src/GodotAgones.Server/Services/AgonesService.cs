using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GodotAgones.Server.Services
{
    using Models;
    

    public class AgonesService
    {
        private const string KeyPrefix = "stable.agones.dev/sdk-";
        private const string NameKey = KeyPrefix + "name";
        private const string MaxPlayersKey = KeyPrefix + "max-players";
        private const string CurrentPlayersKey = KeyPrefix + "num-players";
        private const string PasswordHashKey = KeyPrefix + "password-sha256";

        private readonly HttpClient httpClient;

        public AgonesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Server>> ListServers()
        {
            var response = await httpClient.GetAsync("fleetallocations");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic root = JObject.Parse(json);
            var servers = ((IEnumerable<dynamic>)root.items)
                .Select(fla => fla.status.gameServer)
                .Select(FromDynamic);
            return servers;
        }

        public async Task<Server> CreateServer(Server server)
        {
            var response = await httpClient.PostAsJsonAsync("fleetallocations", new
            {
                apiVersion = "stable.agones.dev/v1alpha1",
                kind = "FleetAllocation",
                metadata = new
                {
                    generateName = "fleet-example-alloc-"
                },
                spec = new
                {
                    fleetName = "fleet-example",
                    metadata = new
                    {
                        annotations = new Dictionary<string, string>
                        {
                            { NameKey, server.Name },
                            { MaxPlayersKey, server.MaxPlayers.ToString() },
                            { CurrentPlayersKey, server.CurrentPlayers.ToString() },
                            { PasswordHashKey, server.PasswordHash }
                        }
                    }
                }
            });
            var json = await response.Content.ReadAsStringAsync();
            dynamic root = JObject.Parse(json);
            return FromDynamic(root.status.gameServer);
        }

        private static Server FromDynamic(dynamic gs)
        {
            return new Server()
            {
                Id = gs.metadata.name,
                Name = gs.metadata.annotations[NameKey],
                IpAddress = gs.status.address,
                Port = (ushort)gs.status.ports[0].port,
                MaxPlayers = gs.metadata.annotations[MaxPlayersKey],
                CurrentPlayers = gs.metadata.annotations[CurrentPlayersKey] ?? (byte)0,
                PasswordHash = gs.metadata.annotations[PasswordHashKey]
            };
        }
    }
}