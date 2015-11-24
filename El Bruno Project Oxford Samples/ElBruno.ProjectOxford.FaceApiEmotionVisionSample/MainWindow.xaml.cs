using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ElBruno.ProjectOxford.FaceApiEmotionVisionSample.Annotations;
using ElBruno.ProjectOxford.FaceApiEmotionVisionSample.Lib;
using ElBruno.ProjectOxford.FaceApiEmotionVisionSample.UserControls;

namespace ElBruno.ProjectOxford.FaceApiEmotionVisionSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<Face> _detectedFaces;
        private ObservableCollection<Face> _facesRect;
        private string _selectedFile;
        private string _statusInformation;
        private string _imageAnalysis;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker dialog
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files|*.jpg;*.png;*.bmp;*.tif;"
            };
            var result = dlg.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            SelectedFile = dlg.FileName;

            var analyzeEmotions = false;
            if (CheckBoxEmotions.IsChecked != null) analyzeEmotions = CheckBoxEmotions.IsChecked.Value;

            var projectOxfordHelper = new ProjectOxfordHelper(Properties.Settings.Default.FaceApiKey, Properties.Settings.Default.EmotionsApiKey, Properties.Settings.Default.VisionApiKey);
            var returnData = await projectOxfordHelper.StartFaceDetection(SelectedFile, analyzeEmotions);
            
            DetectedFaces = returnData.Item1;
            FacesRect = returnData.Item2;

            ImageAnalysis = "";
            var visionAnalysis = false;
            if (CheckBoxVision.IsChecked != null) visionAnalysis = CheckBoxVision.IsChecked.Value;
            if(visionAnalysis)
                ImageAnalysis = await projectOxfordHelper.AnalyzeImageAsString(SelectedFile);

            var ocr = false;
            if (CheckBoxOcr.IsChecked != null) ocr = CheckBoxOcr.IsChecked.Value;
            if (ocr)
                ImageAnalysis += await projectOxfordHelper.RecognizeTextAsString(SelectedFile);

            StatusInformation = $@"{DetectedFaces.Count} faces datected.";
        }


        public string StatusInformation
        {
            get { return _statusInformation; }
            set { _statusInformation = value; OnPropertyChanged(); }
        }

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

        public string ImageAnalysis
        {
            get { return _imageAnalysis; }
            set
            {
                if (value == _imageAnalysis) return;
                _imageAnalysis = value;
                OnPropertyChanged();
            }
        }

        public int MaxImageSize => 450;
    }
}
