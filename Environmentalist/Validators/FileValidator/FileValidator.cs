using System;
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
            if(!_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
        }

        public void IsNotExist(string path)
        {
            if (_fileSystem.File.Exists(path))
            {
                throw new ArgumentException($"File {path} already exists");
            }
        }
    }
}
