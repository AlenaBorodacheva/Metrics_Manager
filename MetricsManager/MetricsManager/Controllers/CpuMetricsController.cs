﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MetricsCommon;
using MetricsManager.DTO;
using MetricsManager.Models;
using MetricsManager.Repositories;
using MetricsManager.Responses;
using Microsoft.Extensions.Logging;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {
        private readonly ILogger<CpuMetricsController> _logger;
        private readonly ICpuMetricsRepository _repository;
        private readonly IMapper _mapper;

        public CpuMetricsController(ILogger<CpuMetricsController> logger, ICpuMetricsRepository repository, IMapper mapper)
        {
            _logger = logger;
            _logger.LogDebug(1, "CpuMetricsController created");
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает метрики CPU агента за указанный промежуток времени
        /// </summary>
        /// <param name="agentId">ID агента</param>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <returns>Метрики CPU</returns>
        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogTrace(1, $"Query GetCpuMetrics with params: AgentID={agentId}, FromTime={fromTime}, ToTime={toTime}");

            var metrics = _repository.GetByTimePeriodByAgentId(agentId, fromTime.ToUnixTimeSeconds(), toTime.ToUnixTimeSeconds());
            var response = new SelectByTimePeriodCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Возвращает метрики CPU агента за указанный промежуток времени с указанным перцентилем
        /// </summary>
        /// <param name="agentId">ID агента</param>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <param name="percentile">Перцентиль</param>
        /// <returns>Метрики CPU с указанным перцентилем</returns>
        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogTrace($"Query GetPercentileByAgentID with params: AgentID={agentId}, FromTime={fromTime}, ToTime={toTime}, Percentile={percentile}");

            var orderedMetrics = _repository.GetByTimePeriodByAgentId(agentId, fromTime.ToUnixTimeSeconds(), toTime.ToUnixTimeSeconds())
                .OrderBy(metrics => metrics.Value);
            var response = GetPercentile(orderedMetrics.ToList(), percentile);

            return Ok(response);
        }

        /// <summary>
        /// Возвращает метрики CPU кластера за указанный промежуток времени
        /// </summary>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <returns>Метрики CPU</returns>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogTrace($"Query GetCpuMetrics for cluster with params: FromTime={fromTime}, ToTime={toTime}");

            var metrics = _repository.GetByTimePeriodFromAllAgents(fromTime.ToUnixTimeSeconds(), toTime.ToUnixTimeSeconds());
            var response = new SelectByTimePeriodCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Возвращает метрики CPU кластера за указанный промежуток времени с указанным перцентилем
        /// </summary>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <param name="percentile">Перцентиль</param>
        /// <returns>Метрики CPU с указанным перцентилем</returns>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogTrace($"Query GetPercentileForCluster with params: FromTime={fromTime}, ToTime={toTime}, Percentile={percentile}");

            var orderedMetrics = _repository.GetByTimePeriodFromAllAgents(fromTime.ToUnixTimeSeconds(), toTime.ToUnixTimeSeconds())
                .OrderBy(metrics => metrics.Value);
            var response = GetPercentile(orderedMetrics.ToList(), percentile);

            return Ok(response);
        }

        private static int GetPercentile(List<CpuMetric> orderedMetrics, Percentile percentile)
        {
            if (!orderedMetrics.Any())
            {
                return 0;
            }

            int index = 0;
            switch (percentile)
            {
                case Percentile.Median:
                    index = (int)(orderedMetrics.Count() / 2);
                    break;
                case Percentile.P75:
                    index = (int)(orderedMetrics.Count() * 0.75);
                    break;
                case Percentile.P90:
                    index = (int)(orderedMetrics.Count() * 0.90);
                    break;
                case Percentile.P95:
                    index = (int)(orderedMetrics.Count() * 0.95);
                    break;
                case Percentile.P99:
                    index = (int)(orderedMetrics.Count() * 0.99);
                    break;
            }
            return orderedMetrics.ElementAt(index).Value;
        }
    }
}
