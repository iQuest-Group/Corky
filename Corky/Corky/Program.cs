using System;
using System.IO;

namespace Corky
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Making a prediction:");

            var testImageFilePath = "path-to-image";
            var testImageBytes = GetImageAsByteArray(testImageFilePath);

            var result = CustomVisionPredictionService.PredictImage(testImageBytes);


            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int) fileStream.Length);
        }
    }
}