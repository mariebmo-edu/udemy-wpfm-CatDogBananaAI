using CatDogBananaAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CatDogBananaAI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            OpenFileDialog dialog = new();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures );

            if (dialog.ShowDialog() == true) {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage( new Uri( fileName ) );

                MakePredictionAsync( fileName );
            }
        }

        private async Task MakePredictionAsync( string fileName ) {
            string url = "https://northeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/9e1da7ec-5d0c-4ea8-9347-03eec86957eb/classify/iterations/Iteration1/image";
            string prediction_key = "0848c6c8365949419f0dca2aa0a74364";
            string content_type = "application/octet-stream";
            var file = File.ReadAllBytes( fileName );

            using(HttpClient client = new()) {
                client.DefaultRequestHeaders.Add( "Prediction-Key", prediction_key );
                
                using(var content = new ByteArrayContent( file )) {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue( content_type );
                    var response = await client.PostAsync( url, content );

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>( responseString )).Predictions;
                    predictionsListView.ItemsSource = predictions;
                }
            }

        }
    }
}
