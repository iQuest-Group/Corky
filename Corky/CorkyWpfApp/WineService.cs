#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CustomVision;

#endregion

namespace CorkyWpfApp
{
	internal class WineService
	{
		#region Public Methods

		public static async Task<string> GetPredictedWinePictureUrl(PredictionResult winePrediction)
		{
			IEnumerable<Prediction> validPredictions = winePrediction.Predictions.Where(x => x.Probability > 0.4);
			if (!validPredictions.Any())
			{
				return "https://i.pinimg.com/originals/3c/4e/65/3c4e650ad01f42649349ea2b7ea7d235.jpg";
			}

			string wineTag = GetMostDominantWineTag(validPredictions);
			return await RandomWineBottleImageUriBasedOnTag(wineTag);
		}

		#endregion

		#region Private Methods

		private static string GetMostDominantWineTag(IEnumerable<Prediction> validPredictions)
		{
			IEnumerable<IGrouping<string, Prediction>> predictionGroupByName = validPredictions.GroupBy(x => x.TagName);

			return predictionGroupByName.OrderByDescending(x => x.Count()).First().Key;
		}

		private static async Task<string> RandomWineBottleImageUriBasedOnTag(string wineTag)
		{
			string webShopUri = WineShopUris[wineTag];

			string webShopContent = await GetWebShopContent(webShopUri);
			string[] productImageUri = GetProductImageUris(webShopContent);

			return productImageUri[new Random().Next(productImageUri.Length - 1)];
		}

		private static async Task<string> GetWebShopContent(string webShopUri)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
			                                       SecurityProtocolType.Tls;

			HttpResponseMessage response = await new HttpClient().GetAsync(webShopUri);

			return await response.Content.ReadAsStringAsync();
		}

		private static string[] GetProductImageUris(string webShopContent)
		{
			var matcher = new Regex("<img class=\"replace-2x img-responsive\" src=\"([a-zA-Z0-9._+=/:-]+)\"");

			return
				matcher.Matches(webShopContent)
					.OfType<Match>()
					.Select(x => x.Groups.OfType<Capture>().Skip(1).First().Value)
					.ToArray();
		}

		#endregion

		#region Constants and Fields

		private static readonly Dictionary<string, string> WineShopUris = new Dictionary<string, string>
		{
			{ "white", "https://www.marvin-wineshop.ro/24-vinuri-albe" },
			{ "red", "https://www.marvin-wineshop.ro/23-vinuri-rosii" },
			{ "rose", "https://www.marvin-wineshop.ro/25-vinuri-roze" }
		};

		#endregion
	}
}