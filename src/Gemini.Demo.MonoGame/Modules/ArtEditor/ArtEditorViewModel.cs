using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Threading;
using Gemini.Modules.UndoRedo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GodRustEditor.Modules.ArtEditor
{
   [Export(typeof(ArtEditorViewModel))]
   [PartCreationPolicy(CreationPolicy.Any)]
   class ArtEditorViewModel : PersistedDocument
   {
        int width = 100;
        int height = 100;
        public WriteableBitmap Content { get; set; }

        ICommand MouseUp;
        //CommandBinding MouseUp = new CommandBinding();

        //ArtEditorViewModel()
        //{
        //    MouseUp.
        //}
        public void MouseUpCommand(object sender, MouseButtonEventArgs e)
        {
            //CURRENTLY NOT USED
            var img = sender as System.Windows.Controls.Image;
            Point position = e.GetPosition(img);
            position.X = (img.Source.Width * (position.X / img.ActualWidth));
            position.Y = (img.Source.Height * (position.Y / img.ActualHeight));
            MessageBox.Show("made it here!");
            
        }

        public void MouseUpCommand2(int PosXScaled, int PosYScaled, MouseButtonEventArgs e)
        {
            int width = 10;
            int height = 10;

            int distToRightedge = Content.PixelWidth - PosXScaled;
            width = distToRightedge > width ? width : distToRightedge;

            int distToBot = Content.PixelHeight - PosYScaled;
            height = distToBot > height ? height : distToBot;

            uint[] pixels = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    red = 0;
                    green = 255 * y / height;
                    blue = 255 * (width - x) / width;
                    alpha = 255;

                    pixels[i] = (uint)((blue << 24) + (green << 16) + (red << 8) + alpha);
                }
            }
            Content.WritePixels(new Int32Rect(PosXScaled, PosYScaled, width, height),pixels, width *4,0);
        }
        public void MouseWheelCommand(object sender, MouseWheelEventArgs e)
        {

        }

        protected override Task DoLoad(string filePath)
      {
         DisplayName = "FOOLS";
            Content =  new WriteableBitmap(new BitmapImage(new Uri(filePath)));
            this.NotifyOfPropertyChange("Content");
            //MouseUp = new DelegateResult(() =>{ })
            return TaskUtility.Completed;
      }



        public void Save()
        {

        }

      protected override Task DoNew()
      {
            Content = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            uint[] pixels = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;

                    red = 0;
                    green = 255 * y / height;
                    blue = 255 * (width - x) / width;
                    alpha = 255;

                    pixels[i] = (uint)((blue << 24) + (green << 16) + (red << 8) + alpha);
                }
            }

            // apply pixels to bitmap
            Content.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            this.NotifyOfPropertyChange("Content");
            return TaskUtility.Completed;
      }



      protected override Task DoSave(string filePath)
      {
         return TaskUtility.Completed;
      }

        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
    }
}
