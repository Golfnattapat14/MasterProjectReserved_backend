//using Microsoft.AspNetCore.Mvc;
//using SWA.Service;

//namespace SWA.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class WeatherForecastController : ControllerBase
//    {
//        private static readonly List<WeatherForecast> Forecasts = new List<WeatherForecast>
//        {
            
//        };

//        private readonly ILogger<WeatherForecastController> _logger;

//        public WeatherForecastController(ILogger<WeatherForecastController> logger)
//        {
//            _logger = logger;
//        }
        
//        //Get
//        [HttpGet(Name = "GetWeatherForecast")]
//        public IEnumerable<WeatherForecast> Get()
//        {
//            return Forecasts;
//        }

//        //Post
//        [HttpPost(Name = "CreateWeatherForecast")]
//        public ActionResult<WeatherForecast> Post(WeatherForecast newForecast)
//        {
//            var result = WeatherForecastCommand.Add.AddForecast(Forecasts, newForecast);
//            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
//        }

//        //Put
//        [HttpPut("{id}", Name = "UpdateWeatherForecast")]
//        public IActionResult Put(int id, WeatherForecast updatedForecast)
//        {
//            var result = WeatherForecastCommand.Edit.EditForecast(Forecasts, id, updatedForecast);
//            if (result == null)
//                return NotFound();

//            return Ok(result);
//        }

//        [HttpDelete("{id}", Name = "DeleteWeatherForecast")]
//        public IActionResult Delete(int id)
//        {
//            bool success = WeatherForecastCommand.Delete.RemoveForecast(Forecasts, id);
//            if(!success)
//                return NotFound();
//            return Ok();
//        }
//    }
//}



