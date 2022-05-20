using FaceRecognition.NET.Entity;
using OpenCvSharp;
using OpenCvSharp.Face;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Window = System.Windows.Window;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;

namespace FaceRecognition.NET
{
    public partial class MainWindow : Window
    {
        public struct FaceMat
        {
            public int id;
            public string name;
            public int lable;
            public Mat img;
        }
        private bool IsCheckFace = false;
        private bool IsOpenCamera = false;
        private bool IsFaceRec = false;
        private bool IsTrain = false;
        
        private CancellationTokenSource tokenSource;    
        public ObservableCollection<User> users = new ObservableCollection<User>();
        public List<FaceMat> faceMats = null;
        public FrameSource frame = null;        
        public LBPHFaceRecognizer faceRecognizer = LBPHFaceRecognizer.Create(1, 8, 8, 8, 100);
        public CascadeClassifier faceCascade = new CascadeClassifier(@"Res\weights\haarcascade_frontalface_alt.xml");
        //FaceImg Info
        public readonly int img_chanel = 1;
        public readonly int img_rows = 256;
        public readonly int img_cols = 256;
        //Administrator
        private readonly string admin_name = "admin";
        private readonly string admin_pass = "admin";

        private string Lable = "Unknow";
        public MainWindow()
        {
            InitializeComponent();
        }

        void Camera()
        {
            tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => {
                Mat mat = new Mat();
                frame = Cv2.CreateFrameSource_Camera(0);

                while (!tokenSource.IsCancellationRequested)
                {
                    frame.NextFrame(mat);
                    //Image inversion
                    Cv2.Flip(mat, mat, FlipMode.Y);
                    if (IsCheckFace)
                    {
                        Rect[] rects = faceCascade.DetectMultiScale(mat);
                        foreach (var item in rects)
                        {
                            var name = CheckFace(mat,rects);
                            if (item != null) {
                                if (name.Equals("Unknow"))
                                {
                                    if (item != null)
                                    {
                                        var color = Scalar.Red;
                                        name = "Unknow";
                                        Cv2.Rectangle(mat, item, color);
                                        Cv2.PutText(mat, name, new Point(item.X, item.Y), HersheyFonts.HersheySimplex, 0.75, color);
                                    }
                                }
                                else
                                {
                                    //Face recognized
                                    if (item != null)
                                    {
                                        var color = Scalar.Green;
                                        Cv2.Rectangle(mat, item, color);
                                        Cv2.PutText(mat, name, new Point(item.X, item.Y), HersheyFonts.HersheySimplex, 0.75, color);
                                    }
                                }
                            }
                        }
                    }
                    Bitmap bitmap = BitmapConverter.ToBitmap(mat);
                    //Display
                    img_Camera.Dispatcher.Invoke(() =>
                    {
                        img_Camera.Source = BitmapToBitmapImage(bitmap);
                    });
                }
            }, tokenSource.Token);
        }
        private string CheckFace(Mat mat, Rect[] rects)
        {
            if (!IsTrain || !IsFaceRec) return "Unknow";
            Mat m = new Mat();
            Cv2.CvtColor(mat, m, ColorConversionCodes.BGR2GRAY);
            var face = GetFaceArea(m,rects);
            Cv2.Resize(face, face, new Size(img_rows, img_cols));
            if (face == null) return "Unknow";

            int result;
            double confidence;
            faceRecognizer.Predict(face, out result, out confidence);
            var i = (10000 - (int)(confidence * 100)) / 100.0;
            if (confidence < 70)
            {
                //识别
                var user = GetUserInfo(faceMats.Where(x => x.id == result).First().id);
                //return user.Name+"("+user.Role+")" + " Cf: " + i;
                Lable = user.Name + "(" + user.Role + ") - LBPH";
            }
            else
            {
                Lable = "Unknow";
            }

            return Lable;
        }

        private Mat GetFaceArea(Mat mat,Rect[] rects)
        {
            return mat.SubMat(rects[0]);
        }
        public User GetUserInfo(int userId)
        {
            return users.Where(w => w.Id == userId).FirstOrDefault();
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
    }
}
