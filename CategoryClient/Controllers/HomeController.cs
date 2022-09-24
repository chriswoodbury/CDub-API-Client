using CategoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace CategoryClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
             return View();
        }

        public async Task<IActionResult> CategoryList()
        {
            var categoryList = await GetCategories();
            return View(categoryList);
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            IEnumerable<Category> categoryList;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://cdub-api-categories.azurewebsites.net/api/categories/"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoryList = JsonConvert.DeserializeObject<IEnumerable<Category>>(apiResponse);
                }
            }
            return categoryList.OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name);
        }


        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                //category.CreatedDateTime = DateTime.Now;

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(category);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("https://cdub-api-categories.azurewebsites.net/api/categories/", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        category = JsonConvert.DeserializeObject<Category>(apiResponse);
                    }
                }
                return View("CategoryList", await GetCategories());
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            Category category = new();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://cdub-api-categories.azurewebsites.net/api/categories/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<Category>(apiResponse);
                }
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://cdub-api-categories.azurewebsites.net/");
                    var response = httpClient.PutAsJsonAsync("api/categories", category).Result;
                }
                return View("CategoryList", await GetCategories());
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = new();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://cdub-api-categories.azurewebsites.net/api/categories/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    category = JsonConvert.DeserializeObject<Category>(apiResponse);
                }
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Category category)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://cdub-api-categories.azurewebsites.net/api/categories/" + category.Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return View("CategoryList", await GetCategories());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}