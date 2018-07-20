using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Gemini.Demo.Modules.TextEditor.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using GodRustEditor.Modules.ArtEditor;
using System.Linq;

namespace Gemini.Demo.Modules.TextEditor
{
	[Export(typeof(IEditorProvider))]
	public class EditorProvider : IEditorProvider
	{
		private readonly List<string> _extensions = new List<string>
        {
            ".txt",
            ".cmd",
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        public IEnumerable<EditorFileType> FileTypes
        {
            get
            {
                return new List<EditorFileType>()
                {
                    new EditorFileType("Text File", ".txt"),
                    new EditorFileType("Image", ".jpg"),
                    new EditorFileType("Image", ".png"),
                    new EditorFileType("Image", ".bmp"),
                    new EditorFileType("Image", ".jpeg"),
                };
                }
        }

		public bool Handles(string path)
		{
			var extension = Path.GetExtension(path);
			return _extensions.Contains(extension);
		}
        
        public IDocument Create(string ChosenFile)
        {
            switch (ChosenFile.Split('.').Last())
            {
                case "jpg":
                case "jpeg":
                case "bmp":
                case "png":
                    return new ArtEditorViewModel();

                default:
                    return new EditorViewModel();
            }
        }

        public async Task New(IDocument document, string name)
        {
            if (document is EditorViewModel)
                await ((EditorViewModel) document).New(name);
            else
                await ((ArtEditorViewModel)document).New(name);

        }

        public async Task Open(IDocument document, string path)
		{
            if (document is EditorViewModel)
                await ((EditorViewModel)document).Load(path);
            else
                await ((ArtEditorViewModel)document).Load(path);
        }
	}
}
