using System;
using CountryData.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

public class HomeController : Controller
{
    public async Task<ActionResult> Index()
    {
        var api = "https://restcountries.com/v3.1/all";

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(api);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var jArray = JArray.Parse(jsonString);

                var countryViewModel = new CountryModel
                {
                    CountryNames = jArray.Select(item => item["name"]["common"].ToString()).ToList()
                };

                return View(countryViewModel);
            }
            catch (HttpRequestException e)
            {
                return View(new CountryModel());
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult> SubmitAction(string selectedCountry)
    {
        if (string.IsNullOrEmpty(selectedCountry))
        {
            return View("Error");
        }

        var api = $"https://restcountries.com/v3.1/name/{selectedCountry}?fullText=true";

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.GetAsync(api);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var jArray = JArray.Parse(jsonString);

                // first element of array
                var countryInfo = jArray[0];

                var currencies = countryInfo["currencies"]?.ToObject<Dictionary<string, JObject>>();
                var currencyNames = new List<string>();
                if (currencies != null)
                {
                    foreach (var currencyCode in currencies.Keys)
                    {
                        var currencyName = currencies[currencyCode]?["name"]?.ToString();
                        if (!string.IsNullOrEmpty(currencyName))
                        {
                            currencyNames.Add(currencyName);
                        }
                    }
                }

                var country = new Country
                {
                    Name = countryInfo["name"]["common"].ToString(),
                    ID = countryInfo["ccn3"].ToString(),
                    Capital = countryInfo["capital"]?[0]?.ToString(),
                    Currency = string.Join(", ", currencyNames),
                    Population = long.Parse(countryInfo["population"]?.ToString() ?? "0"),
                    Flag = countryInfo["flags"]["png"].ToString()
                };

                return View("CountryDetails", country);
            }
            catch (HttpRequestException e)
            {
                return View("Error");
            }
        }
    }
}
