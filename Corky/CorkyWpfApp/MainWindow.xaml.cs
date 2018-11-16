#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CustomVision;

using Microsoft.Win32;

#endregion

namespace CorkyWpfApp
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region Constructors

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		#endregion

		#region Public Properties

		public string WineImageUri
		{
			get
			{
				return this.wineImageUri;
			}
			set
			{
				this.wineImageUri = value;
				OnPropertyChanged(nameof(WineImageUri));
			}
		}

		#endregion

		#region Public Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Private Methods

		private static byte[] ImageToByte(Stream stream)
		{
			byte[] buffer = null;
			if (stream != null && stream.Length > 0)
			{
				using (var br = new BinaryReader(stream))
				{
					buffer = br.ReadBytes((int)stream.Length);
				}
			}

			return buffer;
		}

		private async void btnLoad_Click(object sender, RoutedEventArgs e)
		{
			var op = new OpenFileDialog
			{
				Title = "Select a picture",
				Filter =
					"All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
					"Portable Network Graphic (*.png)|*.png"
			};
			if (op.ShowDialog() != true)
			{
				return;
			}

			CleanImagePredictions();
			this.imgPhoto.Source = new BitmapImage(new Uri(op.FileName));

			FileStream imageStream = File.OpenRead(op.FileName);
			PredictionResult predictionResult = await PredictWine(imageStream);

			IList<Prediction> accuratePredictions = GetAccuratePredictions(predictionResult);
			if (accuratePredictions.Any())
			{
				ShowPredictionsOnImage(accuratePredictions);
			}

			WineImageUri = await WineService.GetPredictedWinePictureUrl(predictionResult);
		}

		private static IList<Prediction> GetAccuratePredictions(PredictionResult predictionResult)
		{
			IEnumerable<Prediction> accuratePredictions =
				predictionResult.Predictions.Where(x => x.Probability > MinPredictionProbability)
					.OrderByDescending(x => x.Probability)
					.Take(3);

			return accuratePredictions.ToList();
		}

		private void ShowPredictionsOnImage(IList<Prediction> predictions)
		{
			try
			{
				foreach (Prediction prediction in predictions)
				{
					Border rectangle = CalculateRectangle(prediction.BoundingBox);
					this.imgGrid.Children.Add(rectangle);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private Border CalculateRectangle(BoundingBox boundingBox)
		{
			var left = (int)(boundingBox.Left * this.imgPhoto.ActualWidth);
			var top = (int)(boundingBox.Top * this.imgPhoto.ActualHeight);
			var rectangle = new Border
			{
				Width = (int)(boundingBox.Width * this.imgPhoto.ActualWidth),
				Height = (int)(boundingBox.Height * this.imgPhoto.ActualHeight),
				Margin = new Thickness(left, top, 0, 0),
				BorderThickness = new Thickness(2),
				BorderBrush = new SolidColorBrush { Color = Colors.Aquamarine },
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top
			};

			return rectangle;
		}

		private void CleanImagePredictions()
		{
			this.imgGrid.Children.Clear();
		}

		private static async Task<PredictionResult> PredictWine(FileStream stream)
		{
			byte[] imageBytes = ImageToByte(stream);

			PredictionResult result = await CustomVisionPredictionService.PredictImage(imageBytes);
			return result;
		}

		private void OnPropertyChanged(string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Constants and Fields

		private const double MinPredictionProbability = 0.4;

		private string wineImageUri;

		#endregion
	}
}