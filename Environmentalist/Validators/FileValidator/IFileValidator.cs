namespace Environmentalist.Validators.FileValidator
{
    public interface IFileValidator
    {
        void IsExist(string path);
        void IsNotExist(string path);
    }
}
