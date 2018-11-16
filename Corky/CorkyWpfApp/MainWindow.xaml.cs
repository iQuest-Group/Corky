using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CustomVision;
using Microsoft.Win32;

namespace CorkyWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            if (op.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(op.FileName));
                imgPhoto.Source = image;
            }

            FileStream imageStream = File.OpenRead(op.FileName);
            PredictionResult result = await PredictWine(imageStream);
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
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }
    }
}