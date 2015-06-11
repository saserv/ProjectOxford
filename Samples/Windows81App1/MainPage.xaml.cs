using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows81App1.Annotations;
using Windows81App1.UserControls;

namespace Windows81App1
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary,
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail
            };

            // Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            // Open the file picker.
            var file = await openPicker.PickSingleFileAsync();

            // file is null if user cancels the file picker.
            if (file != null)
            {
                //var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                //using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                //{
                //    bitmapImage.SetSource(fileStream);
                //    ImageDisplay.Source = bitmapImage;
                //}
                SelectedFileUri = new Uri(file.Path, UriKind.RelativeOrAbsolute);
                //var faceApi = new Lib.FaceApiHelper();
                //DetectedFaces = await faceApi.StartFaceDetection(SelectedFile, file, "4c138b4d82b947beb2e2926c92d1e514");
            }
        }

        #region Properties
        private ObservableCollection<Face> _detectedFaces;
        private Uri _selectedFileUri;

        public ObservableCollection<Face> DetectedFaces
        {
            get { return _detectedFaces; }
            set
            {
                if (Equals(value, _detectedFaces)) return;
                _detectedFaces = value;
                OnPropertyChanged();
            }
        }

        public Uri SelectedFileUri
        {
            get
            {
                return _selectedFileUri;
            }
            set
            {
                if (value == _selectedFileUri) return;
                _selectedFileUri = value;
                OnPropertyChanged();
            }
        }

        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        #endregion

        #region On Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
