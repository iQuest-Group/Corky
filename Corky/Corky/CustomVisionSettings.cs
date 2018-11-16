using System.Configuration;

namespace Corky
{
    public static class CustomVisionSettings
    {
        public static string TrainingKey
        {
            get
            {
                return ConfigurationManager.AppSettings["TrainingKey"];
            }
        }

        public static string PredictionKey
        {
            get
            {
                return ConfigurationManager.AppSettings["PredictionKey"];
            }
        }

        public static string ProjectId
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectId"];
            }
        }
    }
}