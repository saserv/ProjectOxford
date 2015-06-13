using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
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

            var imageInfo = await FileHelper.GetImageInfoForRendering(file.Path);
            NewImageSizeWidth = 300;
            NewImageSizeHeight = NewImageSizeWidth * imageInfo.Item2 / imageInfo.Item1;

            var newSourceFile = await FileHelper.CreateCopyOfSelectedImage(file);
            var uriSource = new Uri(newSourceFile.Path);
            SelectedFileBitmapImage = new BitmapImage(uriSource);


            // start face api detection
            var faceApi = new FaceApiHelper();
            DetectedFaces = await faceApi.StartFaceDetection(newSourceFile.Path, newSourceFile, imageInfo, "");

            // draw rectangles 
            var color = Colors.Blue;
            var bg = Colors.Transparent;
            CanvasDisplay.Children.Clear();
            foreach (var rectangle in from detectedFace in DetectedFaces let margin = new Thickness(detectedFace.RectLeft, detectedFace.RectTop, 0, 0) select new Rectangle
            {
                Stroke = new SolidColorBrush(color),
                Fill = new SolidColorBrush(bg),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Height = detectedFace.RectHeight,
                Width = detectedFace.RectWidth,
                Margin = margin
            })
            {
                CanvasDisplay.Children.Add(rectangle);
            }
        }

        
        #region Properties
        private ObservableCollection<Face> _detectedFaces;
        private BitmapImage _selectedFileBitmapImage;
        private int _newImageSizeWidth;
        private int _newImageSizeHeight;

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

        public int NewImageSizeWidth
        {
            get { return _newImageSizeWidth; }
            set
            {
                if (value == _newImageSizeWidth) return;
                _newImageSizeWidth = value;
                OnPropertyChanged();
            }
        }

        public int NewImageSizeHeight
        {
            get { return _newImageSizeHeight; }
            set
            {
                if (value == _newImageSizeHeight) return;
                _newImageSizeHeight = value;
                OnPropertyChanged();
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
