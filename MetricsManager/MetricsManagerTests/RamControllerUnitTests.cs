using System;
using MetricsCommon;
using MetricsManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MetricsManagerTests
{
    public class RamControllerUnitTests
    {
        private readonly RamMetricsController _controller;
        private readonly int _agentId = 1;
        private readonly TimeSpan _fromTime = TimeSpan.FromSeconds(0);
        private readonly TimeSpan _toTime = TimeSpan.FromSeconds(100);
        private readonly Percentile _percentile = Percentile.P99;

        public RamControllerUnitTests()
        {
            _controller = new RamMetricsController();
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            var result = _controller.GetMetricsFromAgent(_agentId, _fromTime, _toTime);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent_ReturnsOk()
        {
            var result = _controller.GetMetricsByPercentileFromAgent(_agentId, _fromTime, _toTime, _percentile);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsFromAllCluster_ReturnsOk()
        {
            var result = _controller.GetMetricsFromAllCluster(_fromTime, _toTime);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster_ReturnsOk()
        {
            var result = _controller.GetMetricsByPercentileFromAllCluster(_fromTime, _toTime, _percentile);
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
