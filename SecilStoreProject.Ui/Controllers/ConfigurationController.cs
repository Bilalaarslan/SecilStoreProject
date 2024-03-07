using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecilStoreProject.Entities.Entities;

namespace SecilStoreProject.Ui.Controllers
{
	public class ConfigurationController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public ConfigurationController(IHttpClientFactory httpClientFactory)
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

		



	}
}
