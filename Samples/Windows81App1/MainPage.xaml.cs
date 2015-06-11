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
                var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    bitmapImage.SetSource(fileStream);
                    ImageDisplay.Source = bitmapImage;
                }
                SelectedFile = file.Path;
                var faceApi = new Lib.FaceApiHelper();
                var returnData = await faceApi.StartFaceDetection(SelectedFile, "");
                DetectedFaces = returnData.Item1;
                FacesRect = returnData.Item2;
            }
        }

        #region Properties
        private ObservableCollection<Face> _detectedFaces;
        private ObservableCollection<Face> _facesRect;
        private string _selectedFile;
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

        public ObservableCollection<Face> FacesRect
        {
            get { return _facesRect; }
            set
            {
                if (Equals(value, _facesRect)) return;
                _facesRect = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (value == _selectedFile) return;
                _selectedFile = value;
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
