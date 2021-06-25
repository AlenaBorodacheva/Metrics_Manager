using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MetricsAgent.DTO;
using MetricsAgent.Models;
using MetricsAgent.Repositories;
using MetricsAgent.Requests;
using MetricsAgent.Responses;
using MetricsCommon;
using Microsoft.Extensions.Logging;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : ControllerBase
    {
        private readonly ILogger<RamMetricsController> _logger;
        private readonly IRamMetricsRepository _repository;
        private readonly IMapper _mapper;

        public RamMetricsController(IRamMetricsRepository repository, ILogger<RamMetricsController> logger, IMapper mapper)
        {
            _logger = logger;
            _logger.LogDebug(1, "RamMetricsController created");
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] RamMetricCreateRequest request)
        {
            _repository.Create(_mapper.Map<RamMetric>(request));

            _logger.LogTrace(1, $"Query Create Metric with params: Value={request.Value}, Time={request.Time}");

            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] RamMetricUpdateRequest request)
        {
            _repository.Update(_mapper.Map<RamMetric>(request));

            _logger.LogTrace(1, $"Query Update Metric with params: ID={request.Id}, Value={request.Value}, Time={request.Time}");

            return Ok();
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var metrics = _repository.GetAll();

            var response = new AllRamMetricsResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            _logger.LogTrace(1, $"Query GetAll Metrics without params");

            return Ok(response);
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromBody] RamMetricDeleteRequest request)
        {
            _repository.Delete(request.Id);
            _logger.LogTrace(1, $"Query Delete Metric with params: ID={request.Id}");
            return Ok();
        }

        [HttpGet("getmetric/{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            _logger.LogTrace(1, $"Query GetByID Metrics with params: ID={id}");
            RamMetric metric = _repository.GetById(id);
            var response = _mapper.Map<RamMetricDto>(metric);
            return Ok(response);
        }


        /// <summary>
        /// Возвращает метрики Ram за указанный промежуток времени
        /// </summary>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <returns>Метрики Ram</returns>
        [HttpGet("available/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetrics([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogTrace(1, $"Query GetRamMetrics with params: FromTime={fromTime}, ToTime={toTime}");

            var metrics = _repository.GetByTimePeriod(fromTime.ToUnixTimeSeconds(), toTime.ToUnixTimeSeconds());
            var response = new AllRamMetricsResponse()
            {
                Metrics = new List<RamMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Возвращает метрики Ram за указанный промежуток времени с указанным перцентилем
        /// </summary>
        /// <param name="fromTime">Начальное время</param>
        /// <param name="toTime">Конечное время</param>
        /// <param name="percentile">Перцентиль</param>
        /// <returns>Метрики Ram с указанным перцентилем</returns>
        [HttpGet("available/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentile([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogTrace($"Query GetRamMetrics with params: FromTime={fromTime}, ToTime={toTime}, Percentile={percentile}");
            return Ok();
        }
    }
}
