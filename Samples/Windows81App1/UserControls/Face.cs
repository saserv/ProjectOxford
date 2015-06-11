﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;

namespace Windows81App1.UserControls
{
    public class Face : INotifyPropertyChanged
    {
        #region Fields

        private string _gender;
        private string _age;
        private string _personName;
        private int _height;
        private int _left;
        private int _top;
        private int _width;
        private string _imageFacePath;

        private BitmapImage _imageFaceBitmapImage;

        #endregion Fields

        #region Events

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
                ImageFaceBitmapImage = new BitmapImage(new Uri(_imageFacePath));
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

        public BitmapImage ImageFaceBitmapImage
        {
            get { return _imageFaceBitmapImage; }
            set
            {
                _imageFaceBitmapImage = value;
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
}