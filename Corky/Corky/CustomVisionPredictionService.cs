using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomVision
{
    public class CustomVisionPredictionService
    {
        private static readonly string _apiUri = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/{0}/image?iterationId=e9fcedc4-f5eb-4f1f-b76a-594f3782ef42";

        public static async Task<PredictionResult> PredictImage(byte[] byteData)
        {
            string result = await MakeRequest(byteData);

            var predictionResult = JsonConvert.DeserializeObject<PredictionResult>(result);

            return predictionResult;
        }

        private static async Task<string> MakeRequest(byte[] byteData)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", CustomVisionSettings.PredictionKey);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                string uri = string.Format(_apiUri, CustomVisionSettings.ProjectId);
                HttpResponseMessage response = await client.PostAsync(uri, content);

                string stringContent = await response.Content.ReadAsStringAsync();

                return stringContent;
            }
        }
    }
}