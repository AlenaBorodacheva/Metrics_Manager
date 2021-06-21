using MetricsManager.Controllers;
using System.Collections.Generic;
using MetricsManager;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MetricsManagerTests
{
    public class AgentsControllerUnitTests
    {
        private readonly AgentsController _controller;
        private readonly List<AgentInfo> _agents = new List<AgentInfo>();

        public AgentsControllerUnitTests()
        {
            _agents.Add(new AgentInfo());
            _controller = new AgentsController(_agents);
        }

        [Fact]
        public void RegisterAgent_ReturnsOk()
        {
            var result = _controller.RegisterAgent(_agents[0]);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void EnableAgentById_ReturnsOk()
        {
            var result = _controller.EnableAgentById(_agents[0].AgentId);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void DisableAgentById_ReturnsOk()
        {
            var result = _controller.DisableAgentById(_agents[0].AgentId);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void ListOfRegisteredObjects_ReturnsOk()
        {
            var result = _controller.ListOfRegisteredObjects();
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
