using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows81App1.UserControls;
using Microsoft.ProjectOxford.Face;

namespace Windows81App1.Lib
{
    internal class FaceApiHelper
    {
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

                        // get face file
                        var faceStartPoint = new Point(face.FaceRectangle.Left, face.FaceRectangle.Top);
                        var faceSize = new Size(face.FaceRectangle.Width, face.FaceRectangle.Height);

                        // save face file
                        var fileName = string.Format(@"Temp\{0}.jpg", face.FaceId);
                        var fileFaceImage = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                        await CropBitmap.SaveCroppedBitmapAsync(file, fileFaceImage, faceStartPoint, faceSize);

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
    }
}