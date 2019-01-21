using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GodotAgones.Server.Controllers
{
    using Models;
    using Services;

    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly AgonesService agonesService;

        public ServersController(AgonesService agonesService)
        {
            this.agonesService = agonesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> Get(
            [FromQuery] bool isNotFull = false,
            [FromQuery] bool isNotEmpty = false,
            [FromQuery] bool isNotPasswordProtected = false)
        {
            bool Exclude(bool isNot, bool isServer) => isNot ? !isServer : true;
            var servers = await agonesService.ListServers();
            return servers
                .Where(s => Exclude(isNotFull, s.IsFull))
                .Where(s => Exclude(isNotEmpty, s.IsEmpty))
                .Where(s => Exclude(isNotPasswordProtected, s.IsPasswordProtected))
                .ToList();
        }

        [HttpPost]
        public async Task<ActionResult<Server>> Post([FromBody] Server model)
        {
            var server = await agonesService.CreateServer(model);
            return server;
        }
    }
}