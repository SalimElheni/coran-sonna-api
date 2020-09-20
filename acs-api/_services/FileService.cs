using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acs._services
{
    public interface IFileService
    {
        public string GetContent();
        public void SetContent(string content);
    }
    public class FileService: IFileService
    {
        public FileService(string path)
        {
            FilePath = path;
        }
        public string FilePath { get; set; }
        public string GetContent()
        {
            return System.IO.File.ReadAllText(FilePath);
        }
        public void SetContent(string content)
        {
            System.IO.File.WriteAllText(FilePath, content);
        }
    }
}
