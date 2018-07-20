using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gemini.Framework.Services
{
    public interface IEditorProvider
	{
        IEnumerable<EditorFileType> FileTypes { get; }

		bool Handles(string path);

        IDocument Create(string chosenFile);

        Task New(IDocument document, string name);
        Task Open(IDocument document, string path);
	}
}
