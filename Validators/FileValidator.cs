using System.IO;

namespace Environmentalist.Validators
{
    internal static class FileValidator
    {
        public static void IsExist(string path)
        {
            if(string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            
        }
    }
}
