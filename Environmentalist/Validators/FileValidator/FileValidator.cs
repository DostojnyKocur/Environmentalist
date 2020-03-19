using System.IO;
using System.IO.Abstractions;

namespace Environmentalist.Validators.FileValidator
{
    public sealed class FileValidator : IFileValidator
    {
        private IFileSystem _fileSystem;

        public FileValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void IsExist(string path)
        {
            if(string.IsNullOrWhiteSpace(path) || !_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            
        }
    }
}
