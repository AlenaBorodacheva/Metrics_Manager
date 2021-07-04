﻿using System;
using System.Collections.Generic;
using AutoFixture;
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
    public class HddControllerUnitTests
    {
        private readonly HddMetricsController _controller;
        private readonly DateTimeOffset _fromTime = new(new(2020, 06, 01));
        private readonly DateTimeOffset _toTime = new(new(2021, 06, 01));
        private readonly Percentile _percentile = Percentile.P99;
        private readonly Mock<IHddMetricsRepository> _mockRepository;
        private readonly List<HddMetric> _initialData;

        public HddControllerUnitTests()
        {
            _mockRepository = new Mock<IHddMetricsRepository>();
            var mockLogger = new Mock<ILogger<HddMetricsController>>();
            var mockMapper = new Mock<IMapper>();
            _controller = new HddMetricsController(_mockRepository.Object, mockLogger.Object, mockMapper.Object);
            _initialData = new Fixture().Create<List<HddMetric>>();
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Create(It.IsAny<HddMetric>())).Verifiable();
            var result = _controller.Create(new HddMetricCreateRequest { Time = new(new(2020, 02, 04)), Value = 50 });
            _mockRepository.Verify(repository => repository.Create(It.IsAny<HddMetric>()), Times.AtMostOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
       
        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAll()).Returns(new List<HddMetric>()).Verifiable();
            var result = _controller.GetAll();
            _mockRepository.Verify(repository => repository.GetAll(), Times.AtLeastOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetByTimePeriod_ShouldCall_GetByTimePeriod_From_Repository()
        {
            long startTime = _fromTime.ToUnixTimeSeconds();
            long endTime = _toTime.ToUnixTimeSeconds();

            _mockRepository.Setup(repository => repository.GetByTimePeriod(startTime, endTime)).Returns(_initialData).Verifiable();
            var result = _controller.GetMetrics(_fromTime, _toTime);
            _mockRepository.Verify(repository => repository.GetByTimePeriod(startTime, endTime), Times.AtMostOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        
        [Fact]
        public void GetMetricsByPercentile_ReturnsOk()
        {
            long startTime = _fromTime.ToUnixTimeSeconds();
            long endTime = _toTime.ToUnixTimeSeconds();

            _mockRepository.Setup(repository => repository.GetByTimePeriod(startTime, endTime)).Returns(_initialData).Verifiable();
            var result = _controller.GetMetricsByPercentile(_fromTime, _toTime, _percentile);
            _mockRepository.Verify(repository => repository.GetByTimePeriod(startTime, endTime), Times.AtMostOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
