using System;
using MetricsAgent.Controllers;
using MetricsCommon;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MetricsAgentTests
{
    public class RamControllerUnitTests
    {
        private readonly RamMetricsController _controller;
        private readonly TimeSpan _fromTime = TimeSpan.FromSeconds(0);
        private readonly TimeSpan _toTime = TimeSpan.FromSeconds(100);
        private readonly Percentile _percentile = Percentile.P99;

        public RamControllerUnitTests()
        {
            _controller = new RamMetricsController();
        }

        [Fact]
        public void GetMetrics_ReturnsOk()
        {
            //Act
            var getMetricsResult = _controller.GetMetrics(_fromTime, _toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(getMetricsResult);
        }

        [Fact]
        public void GetMetricsByPercentile_ReturnsOk()
        {
            //Act
            var getMetricsByPercentileResult = _controller.GetMetricsByPercentile(_fromTime, _toTime, _percentile);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(getMetricsByPercentileResult);
        }
    }
}
