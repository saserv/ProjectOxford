using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows81App1.UserControls;
using Microsoft.ProjectOxford.Face;

namespace Windows81App1.Lib
{
    internal class FaceApiHelper
    {
        public int MaxImageSize => 300;

        public async Task<ObservableCollection<Face>> StartFaceDetection(string selectedFile, StorageFile file, string subscriptionKey)
        {
            var detectedFaces = new ObservableCollection<Face>();

            Debug.WriteLine("Request: Detecting {0}", selectedFile);
            var sampleFile = await StorageFile.GetFileFromPathAsync(selectedFile);
            var fs = await FileIO.ReadBufferAsync(sampleFile);

            var imageInfo = await GetImageInfoForRendering(selectedFile);

            using (var stream = fs.AsStream())
            {
                try
                {
                    var client = new FaceServiceClient(subscriptionKey);
                    var faces = await client.DetectAsync(stream, true, true, true);
                    Debug.WriteLine("Response: Success. Detected {0} face(s) in {1}", faces.Length, selectedFile);
                    Debug.WriteLine("{0} face(s) has been detected", faces.Length);

                    foreach (var face in faces)
                    {

                        var fileFaceImage = await FileHelper.SaveFaceImageFile(file, face);

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

                        // calculate rect image
                        newFace = CalculateFaceRectangleForRendering(newFace, MaxImageSize, imageInfo);

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

        private static Face CalculateFaceRectangleForRendering(Face face, int maxSize, Tuple<int, int> imageInfo)
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

            var uiXOffset = (maxSize - uiWidth) / 2;
            var uiYOffset = (maxSize - uiHeight) / 2;
            var scale = (float)uiWidth / imageWidth;

            face.RectLeft = (int) ((face.Left*scale) + uiXOffset);
            face.RectTop = (int) ((face.Top*scale) + uiYOffset);
            face.RectHeight = (int) (face.Height*scale);
            face.RectWidth = (int) (face.Width*scale);

            return face;
        }

        private async Task<Tuple<int, int>> GetImageInfoForRendering(string imageFilePath)
        {
            try
            {
                var sampleFile = await StorageFile.GetFileFromPathAsync(imageFilePath);
                var file = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                var decoder = await BitmapDecoder.CreateAsync(file);
                var pixelWidth = int.Parse(decoder.PixelWidth.ToString());
                var pixelHeight = int.Parse(decoder.PixelHeight.ToString());
                return new Tuple<int, int>(pixelWidth, pixelHeight);
            }
            catch
            {
                return new Tuple<int, int>(0, 0);
            }
        }

    }
}