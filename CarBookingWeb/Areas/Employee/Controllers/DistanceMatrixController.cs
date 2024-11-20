using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CarBooking.Models;



namespace BulkyWeb.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class DistanceMatrixController : Controller
    {
        private readonly HttpClient httpClient;

        public DistanceMatrixController()
        {
            httpClient = new HttpClient();
        }

        public IActionResult CalculateDistance()
        {

            return View(new DistanceMatrixViewModel());
        }

        [HttpPost]
        public IActionResult CalculateDistance(DistanceMatrixViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string apiKey = "AIzaSyDc4j_VNbQIuJX62STIvVu51ODS1LUlMd0";

            string apiUrl = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={model.Origin}&destinations={model.Destination}&key={apiKey}";



            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = httpClient.GetAsync(apiUrl).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                model.Distance = result.rows[0].elements[0].distance.text;
                model.Duration = result.rows[0].elements[0].duration.text;

                //return new JsonResult(model);
                return View(model);
            }

            return View();
        }

        
    }
}