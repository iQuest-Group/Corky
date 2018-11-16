#region Using Directives

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
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

		#region Public Methods

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

		#endregion

		#region Private Methods

		private async void btnLoad_Click(object sender, RoutedEventArgs e)
		{
			var op = new OpenFileDialog
			{
				Title = "Select a picture",
				Filter =
					"All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
					"Portable Network Graphic (*.png)|*.png"
			};
			if (op.ShowDialog() == true)
			{
				var image = new BitmapImage(new Uri(op.FileName));
				this.imgPhoto.Source = image;
			}

			FileStream imageStream = File.OpenRead(op.FileName);
			PredictionResult result = await PredictWine(imageStream);

			WineImageUri = await WineService.GetPredictedWinePictureUrl(result);
		}

		private async Task<PredictionResult> PredictWine(FileStream stream)
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

		private string wineImageUri;

		#endregion
	}
}