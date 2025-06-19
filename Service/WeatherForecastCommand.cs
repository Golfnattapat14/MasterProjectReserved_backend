//using Microsoft.AspNetCore.Mvc;

//namespace SWA.Service
//{
//    public class WeatherForecastCommand
//    {
//        public class Add
//        {
//            public static WeatherForecast AddForecast(List<WeatherForecast> forecasts, WeatherForecast newForecast)
//            {
//                newForecast.Id = forecasts.Count > 0 ? forecasts.Max(f => f.Id) + 1 : 1;
//                forecasts.Add(newForecast);
//                return newForecast;
//            }
//        }
//        public class Edit
//        {
//            public static WeatherForecast? EditForecast(List<WeatherForecast> forecasts, int id, WeatherForecast updatedForecast)
//            {
//                var existingForecast = forecasts.FirstOrDefault(f => f.Id == id);
//                if (existingForecast != null)
//                {
//                    existingForecast.City = updatedForecast.City;
//                    existingForecast.Date = updatedForecast.Date;
//                    existingForecast.TemperatureC = updatedForecast.TemperatureC;
//                    existingForecast.Summary = updatedForecast.Summary;
//                    return existingForecast;
//                }
//                return null;
//            }
//        }
//        public class Delete
//        {
//            public static bool RemoveForecast(List<WeatherForecast> forecasts, int id)
//            {
//                var forecastRemove = forecasts.FirstOrDefault(f => f.Id == id);
//                if (forecastRemove == null) return false;
//                {
//                    forecasts.Remove(forecastRemove);
//                    return true;
//                }
//            }
//        }
//    }

//}
