using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly List<WeatherForecast> _weatherHolder;

        public WeatherForecastController(List<WeatherForecast> holder)
        {
            _weatherHolder = holder;
        }

        //сохранить температуру в указанное время
        [HttpPost("create")]
        public IActionResult Create([FromQuery] int? temperature, [FromQuery] DateTime? time)
        {
            if (temperature.HasValue && time.HasValue)
            {
                _weatherHolder.Add(new WeatherForecast { Date = time.Value, TemperatureC = temperature.Value });
                return Ok();
            }
            return BadRequest();
        }

        //прочитать список показателей температуры за указанный промежуток времени
        [HttpGet("read")]
        public IActionResult Read([FromQuery] DateTime? fromTime, [FromQuery] DateTime? toTime)
        {
            fromTime = fromTime.HasValue ? fromTime : DateTime.MinValue;
            toTime = toTime.HasValue ? toTime : DateTime.MaxValue;
            if (fromTime < toTime)
            {
                var selectTemperatures = from WeatherForecast in _weatherHolder
                    where WeatherForecast.Date >= fromTime.Value && WeatherForecast.Date <= toTime.Value
                    select WeatherForecast;
                return Ok(selectTemperatures);
            }
            return BadRequest();
        }

        //удалить показатель температуры в указанный промежуток времени
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] DateTime? fromTime, [FromQuery] DateTime? toTime)
        {
            if (fromTime.HasValue && toTime.HasValue)
            {
                var selectTemperatures = from WeatherForecast in _weatherHolder
                                         where WeatherForecast.Date >= fromTime.Value && WeatherForecast.Date <= toTime.Value
                                         select WeatherForecast;
                foreach (var temperature in selectTemperatures.ToList())
                {
                    _weatherHolder.Remove(temperature);
                }
                return Ok();
            }
            return BadRequest();
        }

        //отредактировать показатель температуры в указанное время
        [HttpPut("update")]
        public IActionResult Update([FromQuery] DateTime? time, [FromQuery] int? newTempetarure)
        {
            if (time.HasValue && newTempetarure.HasValue)
            {
                var update = _weatherHolder.SingleOrDefault(WeatherForecast => WeatherForecast.Date == time.Value);
                if (update != null)
                {
                    update.TemperatureC = newTempetarure.Value;
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
