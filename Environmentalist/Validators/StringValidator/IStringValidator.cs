namespace Environmentalist.Validators.StringValidator
{
    public interface IStringValidator
    {
        void IsNullOrWhitespace(string @string, string paramName);
    }
}
