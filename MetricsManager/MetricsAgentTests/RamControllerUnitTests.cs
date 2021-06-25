using System;
using System.Collections.Generic;
using AutoMapper;
using MetricsAgent.Controllers;
using MetricsAgent.Models;
using MetricsAgent.Repositories;
using MetricsAgent.Requests;
using MetricsCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MetricsAgentTests
{
    public class RamControllerUnitTests
    {
        private readonly RamMetricsController _controller;
        private readonly DateTimeOffset _fromTime = new(new(2020, 06, 01));
        private readonly DateTimeOffset _toTime = new(new(2021, 06, 01));
        private readonly Percentile _percentile = Percentile.P99;
        private readonly Mock<IRamMetricsRepository> _mockRepository;

        public RamControllerUnitTests()
        {
            _mockRepository = new Mock<IRamMetricsRepository>();
            var mockLogger = new Mock<ILogger<RamMetricsController>>();
            var mockMapper = new Mock<IMapper>();
            _controller = new RamMetricsController(_mockRepository.Object, mockLogger.Object, mockMapper.Object);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Create(It.IsAny<RamMetric>())).Verifiable();
            var result = _controller.Create(new RamMetricCreateRequest { Time = new(new(2020, 02, 04)), Value = 50 });
            _mockRepository.Verify(repository => repository.Create(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }
        [Fact]
        public void Update_ShouldCall_Update_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Update(It.IsAny<RamMetric>())).Verifiable();
            var result = _controller.Update(new RamMetricUpdateRequest { Id = 3, Time = new(new(2020, 02, 04)), Value = 50 });
            _mockRepository.Verify(repository => repository.Update(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void Delete_ShouldCall_Delete_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Delete(It.IsAny<int>())).Verifiable();
            var result = _controller.Delete(new RamMetricDeleteRequest { Id = 3 });
            _mockRepository.Verify(repository => repository.Delete(It.IsAny<int>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAll()).Returns(new List<RamMetric>()).Verifiable();
            var result = _controller.GetAll();
            _mockRepository.Verify(repository => repository.GetAll(), Times.AtLeastOnce());
        }

        [Fact]
        public void GetByTimePeriod_ShouldCall_GetByTimePeriod_From_Repository()
        {
            long startTime = _fromTime.ToUnixTimeSeconds();
            long endTime = _toTime.ToUnixTimeSeconds();

            _mockRepository.Setup(repository => repository.GetByTimePeriod(startTime, endTime)).Returns(new List<RamMetric>()).Verifiable();
            var result = _controller.GetMetrics(_fromTime, _toTime);
            _mockRepository.Verify(repository => repository.GetByTimePeriod(startTime, endTime), Times.AtMostOnce());
        }

        
        [Fact]
        public void GetMetricsByPercentile_ReturnsOk()
        {
            var getMetricsByPercentileResult = _controller.GetMetricsByPercentile(_fromTime, _toTime, _percentile);
            _ = Assert.IsAssignableFrom<IActionResult>(getMetricsByPercentileResult);
        }
    }
}
