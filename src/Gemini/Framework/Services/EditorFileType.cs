using System.Collections.Generic;

namespace Gemini.Framework.Services
{
    public class EditorFileType
    {
        public string Name { get; set; }
        public string FileExtension { get; set; }

        public EditorFileType(string name, string fileExtension)
        {
            Name = name;
            FileExtension = fileExtension;
        }

        public EditorFileType()
        {
            
        }

        //public string Name { get; set; }
        //public IEnumerable<string> FileExtension { get; set; }

        //public EditorFileType(string name, string fileExtension)
        //{
        //    Name = name;
        //    FileExtension = fileExtension;
        //}

        //public EditorFileType()
        //{

        //}
    }
}
