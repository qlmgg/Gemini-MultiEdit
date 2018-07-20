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
using System.Windows.Input;

namespace GodRustEditor.Modules.ArtEditor
{
   [Export(typeof(ArtEditorViewModel))]
   [PartCreationPolicy(CreationPolicy.Any)]
   class ArtEditorViewModel : PersistedDocument
   {
      public string Content { get; set; } = "FUONDF";
      
      

      protected override Task DoLoad(string filePath)
      {
         DisplayName = "FOOLS";
         Content = filePath;
         return TaskUtility.Completed;
      }

      protected override Task DoNew()
      {
         return TaskUtility.Completed;
      }

      protected override Task DoSave(string filePath)
      {
         return TaskUtility.Completed;
      }
   }
}
