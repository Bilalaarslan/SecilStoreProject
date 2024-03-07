using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecilStoreProject.Entities.Entities;
using System.Text;

namespace SecilStoreProject.Ui.Controllers
{
	public class MainConfigurationController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public MainConfigurationController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index(string applicationName)
		{
			applicationName ="string";
			var client = _httpClientFactory.CreateClient();
			var responseMessage = await client.GetAsync($"https://localhost:7141/api/Configuration/{applicationName}");
			if (responseMessage.IsSuccessStatusCode)
			{
				var jsonData = await responseMessage.Content.ReadAsStringAsync();
				var values = JsonConvert.DeserializeObject<List<ConfigurationModel>>(jsonData);
				return View(values);
			}
			return View();
		}

		[HttpGet]
		public IActionResult CreateConfiguration()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult>CreateConfiguration(ConfigurationModel configurationModel)
		{
			var jsonData = JsonConvert.SerializeObject(configurationModel);
			StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

			var client = _httpClientFactory.CreateClient();
			var responseMessage = await client.PostAsync("https://localhost:7141/api/Configuration/", content);

			if (responseMessage.IsSuccessStatusCode)
			{
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
        public async Task<IActionResult> UpdateConfiguration([FromRoute] string id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7141/api/Configuration/GetConfigurationById/{id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ConfigurationModel>(jsonData);
                return View(value);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateConfiguration(ConfigurationModel configurationModel)
        {
            var jsonData = JsonConvert.SerializeObject(configurationModel);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.PutAsync("https://localhost:7141/api/Configuration/", content);

            if (responseMessage.IsSuccessStatusCode)
                return RedirectToAction("Index");

            return View();
        }

    }
}
