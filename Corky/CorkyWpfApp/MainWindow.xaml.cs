using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CustomVision;
using Microsoft.Win32;

namespace CorkyWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double _minPredictionProbability = 0.4;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() != true)
            {
                return;
            }

            CleanImagePredictions();
            imgPhoto.Source = new BitmapImage(new Uri(op.FileName));

            FileStream imageStream = File.OpenRead(op.FileName);
            PredictionResult predictionResult = await PredictWine(imageStream);

            var accuratePredictions = GetAccuratePredictions(predictionResult);
            if (accuratePredictions.Any())
            {
                ShowPredictionsOnImage(accuratePredictions);
            }
        }

        private IList<Prediction> GetAccuratePredictions(PredictionResult predictionResult)
        {
            IEnumerable<Prediction> accuratePredictions = predictionResult.Predictions
                .Where(x => x.Probability > _minPredictionProbability)
                .OrderByDescending(x => x.Probability)
                .Take(3);

            return accuratePredictions.Any() ? accuratePredictions.ToList() : new List<Prediction>();
        }

        private void ShowPredictionsOnImage(IList<Prediction> predictions)
        {
            try
            {
                foreach (var prediction in predictions)
                {
                    var rectangle = CalculateRectangle(prediction.BoundingBox);
                    imgGrid.Children.Add(rectangle);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private Rectangle CalculateRectangle(BoundingBox boundingBox)
        {
            var rectangle = new Rectangle
            {
                Width = (int) (boundingBox.Width * imgPhoto.ActualWidth),
                Height = (int) (boundingBox.Height * imgPhoto.ActualHeight),
            };
            Canvas.SetLeft(rectangle, (int) (boundingBox.Left * imgPhoto.ActualWidth));
            Canvas.SetTop(rectangle, (int) (boundingBox.Top * imgPhoto.ActualHeight));

            var blackBrush = new SolidColorBrush { Color = Colors.Aquamarine };
            rectangle.StrokeThickness = 2;
            rectangle.Stroke = blackBrush;

            return rectangle;
        }

        private void CleanImagePredictions()
        {
            imgGrid.Children.Clear();
        }

        private async Task<PredictionResult> PredictWine(FileStream stream)
        {
            byte[] imageBytes = ImageToByte(stream);

            PredictionResult result = await CustomVisionPredictionService.PredictImage(imageBytes);
            return result;
        }

        public Byte[] ImageToByte(Stream stream)
        {
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (var br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32) stream.Length);
                }
            }

            return buffer;
        }
    }
}