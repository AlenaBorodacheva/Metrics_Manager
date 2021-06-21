using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly List<AgentInfo> _registeredAgents;

        public AgentsController(List<AgentInfo> registeredAgents)
        {
            _registeredAgents = registeredAgents;
        }

        /// <summary>
        /// Регистрирует нового агента
        /// </summary>
        /// <param name="agentInfo">Информация об агенте</param>
        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
        {
            return Ok();
        }

        /// <summary>
        /// Активирует агента по его ID
        /// </summary>
        /// <param name="agentId">ID агента</param>
        [HttpPut("enable/{agentId}")]
        public IActionResult EnableAgentById([FromRoute] int agentId)
        {
            return Ok();
        }

        /// <summary>
        /// Деактивирует агента по его ID
        /// </summary>
        /// <param name="agentId">ID агента</param>
        [HttpPut("disable/{agentId}")]
        public IActionResult DisableAgentById([FromRoute] int agentId)
        {
            return Ok();
        }

        /// <summary>
        /// Считывает зарегистрированных агентов
        /// </summary>
        [HttpGet("read")]
        public IActionResult ListOfRegisteredObjects()
        {
            return Ok();
        }
    }
}
