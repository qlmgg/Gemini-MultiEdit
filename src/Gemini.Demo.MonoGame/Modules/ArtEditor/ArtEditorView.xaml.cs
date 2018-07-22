using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace GodRustEditor.Modules.ArtEditor
{
   /// <summary>
   /// Interaction logic for ArtEditorView.xaml
   /// </summary>
   public partial class ArtEditorView : UserControl
   {
      public ArtEditorView()
      {
         InitializeComponent();
      }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)image.RenderTransform;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }
    }
}
