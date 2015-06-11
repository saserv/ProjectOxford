using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows81App1.Annotations;
using Windows81App1.Lib;
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
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            var file = await openPicker.PickSingleFileAsync();
            if (file == null) return;

            FileHelper.ClearTempFolder();

            var newSourceFile = await FileHelper.CreateCopyOfSelectedImage(file);
            var uriSource = new Uri(newSourceFile.Path);
            SelectedFileBitmapImage = new BitmapImage(uriSource);
            
            // start face api detection
            var faceApi = new FaceApiHelper();
            DetectedFaces = await faceApi.StartFaceDetection(newSourceFile.Path, file, "4c138b4d82b947beb2e2926c92d1e514");
        }

        
        #region Properties
        private ObservableCollection<Face> _detectedFaces;
        private BitmapImage _selectedFileBitmapImage;

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

        public BitmapImage SelectedFileBitmapImage
        {
            get
            {
                return _selectedFileBitmapImage;
            }
            set
            {
                if (value == _selectedFileBitmapImage) return;
                _selectedFileBitmapImage = value;
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
