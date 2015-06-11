using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Windows81App1.UserControls
{
    public class Face : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Face gender text string
        /// </summary>
        private string _gender;

        /// <summary>
        /// Face age text string
        /// </summary>
        private string _age;

        /// <summary>
        /// Person name
        /// </summary>
        private string _personName;

        /// <summary>
        /// Face height in pixel
        /// </summary>
        private int _height;

        /// <summary>
        /// Face position X relative to image left-top in pixel
        /// </summary>
        private int _left;

        /// <summary>
        /// Face position Y relative to image left-top in pixel
        /// </summary>
        private int _top;

        /// <summary>
        /// Face width in pixel
        /// </summary>
        private int _width;

        private string _imageFacePath;

        private Uri _imageFacePathUri;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string Gender
        {
            get
            {
                return _gender;
            }

            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public string Age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        public Int32Rect UiRect
        {
            get
            {
                return new Int32Rect(Left, Top, Width, Height);
            }
        }

        public string ImagePath
        {
            get;
            set;
        }

        public string FaceId
        {
            get;
            set;
        }

        public string PersonName
        {
            get
            {
                return _personName;
            }

            set
            {
                _personName = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public int Left
        {
            get
            {
                return _left;
            }

            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }

        public int Top
        {
            get
            {
                return _top;
            }

            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }

        public string ImageFacePath
        {
            get { return _imageFacePath; }
            set
            {
                _imageFacePath = value;
                ImageFacePathUri = new Uri(_imageFacePath, UriKind.RelativeOrAbsolute);
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public Uri ImageFacePathUri
        {
            get { return _imageFacePathUri; }
            set
            {
                _imageFacePathUri = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        #endregion Methods
    }

    public class Int32Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Int32Rect(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}