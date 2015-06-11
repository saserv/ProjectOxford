using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows81App1.UserControls;
using Microsoft.ProjectOxford.Face;

namespace Windows81App1.Lib
{
    internal class FaceApiHelper
    {
        #region FaceApi

        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        public async Task<ObservableCollection<Face>> StartFaceDetection(string selectedFile, StorageFile file, string subscriptionKey)
        {
            var detectedFaces = new ObservableCollection<Face>();

            Debug.WriteLine("Request: Detecting {0}", selectedFile);
            var sampleFile = await StorageFile.GetFileFromPathAsync(selectedFile);
            var fs = await FileIO.ReadBufferAsync(sampleFile);
            using( var stream = fs.AsStream())
            { 
                try
                {
                    var client = new FaceServiceClient(subscriptionKey);
                    var faces = await client.DetectAsync(stream, true, true, true);
                    Debug.WriteLine("Response: Success. Detected {0} face(s) in {1}", faces.Length, selectedFile);
                    var imageInfo = await GetImageInfoForRendering(selectedFile);
                    Debug.WriteLine("{0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {

                        // get face file
                        var startingPoint = new Point(face.FaceRectangle.Left, face.FaceRectangle.Top);
                        var tbSize = new Size(face.FaceRectangle.Width, face.FaceRectangle.Height);

                        var fileName = string.Format("{0}.jpg", face.FaceId);
                        var fileFaceImage = await KnownFolders.PicturesLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                        // save face file
                        var newSourceFileName = string.Format("{0}.jpg", Guid.NewGuid());
                        var newSourceFile = await KnownFolders.PicturesLibrary.CreateFileAsync(newSourceFileName, CreationCollisionOption.ReplaceExisting);
                        await file.CopyAndReplaceAsync(newSourceFile);
                        await CropBitmap.SaveCroppedBitmapAsync(newSourceFile, fileFaceImage, startingPoint, tbSize);

                        var newFace = new Face
                        {
                            ImagePath = selectedFile,
                            Left = face.FaceRectangle.Left,
                            Top = face.FaceRectangle.Top,
                            Width = face.FaceRectangle.Width,
                            Height = face.FaceRectangle.Height,
                            FaceId = face.FaceId.ToString(),
                            Gender = face.Attributes.Gender,
                            Age = string.Format("{0:#} years old", face.Attributes.Age),
                            ImageFacePath = fileFaceImage.Path
                    };
                        detectedFaces.Add(newFace);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return detectedFaces;
        }
        #endregion

        #region Methods

        private static IEnumerable<Face> CalculateFaceRectangleForRendering(IEnumerable<Microsoft.ProjectOxford.Face.Contract.Face> faces, int maxSize, Tuple<int, int> imageInfo)
        {
            var imageWidth = imageInfo.Item1;
            var imageHeight = imageInfo.Item2;
            float ratio = (float)imageWidth / imageHeight;
            int uiWidth;
            int uiHeight;
            if (ratio > 1.0)
            {
                uiWidth = maxSize;
                uiHeight = (int)(maxSize / ratio);
            }
            else
            {
                uiHeight = maxSize;
                uiWidth = (int)(ratio * uiHeight);
            }

            int uiXOffset = (maxSize - uiWidth) / 2;
            int uiYOffset = (maxSize - uiHeight) / 2;
            float scale = (float)uiWidth / imageWidth;

            return faces.Select(face => new Face()
            {
                FaceId = face.FaceId.ToString(),
                Left = (int)((face.FaceRectangle.Left * scale) + uiXOffset),
                Top = (int)((face.FaceRectangle.Top * scale) + uiYOffset),
                Height = (int)(face.FaceRectangle.Height * scale),
                Width = (int)(face.FaceRectangle.Width * scale),
            });
        }

        public async Task<Tuple<int, int>> GetImageInfoForRendering(string imageFilePath)
        {
            try
            {
                var sampleFile = await StorageFile.GetFileFromPathAsync(imageFilePath);
                var file = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                var decoder = await BitmapDecoder.CreateAsync(file);

                int pixelWidth = int.Parse(decoder.PixelWidth.ToString());
                int pixelHeight = int.Parse(decoder.PixelHeight.ToString());

                return new Tuple<int, int>(pixelWidth, pixelHeight);
            }
            catch
            {
                return new Tuple<int, int>(0, 0);
            }
        }

        public static void UpdateFace(ObservableCollection<Face> collections, string path, Microsoft.ProjectOxford.Face.Contract.Face face)
        {
            collections.Add(new Face()
            {
                ImagePath = path,
                Left = face.FaceRectangle.Left,
                Top = face.FaceRectangle.Top,
                Width = face.FaceRectangle.Width,
                Height = face.FaceRectangle.Height,
                FaceId = face.FaceId.ToString(),
            });
        }

        #endregion Methods
    }
}