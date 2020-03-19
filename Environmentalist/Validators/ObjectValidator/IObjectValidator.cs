namespace Environmentalist.Validators.ObjectValidator
{
    public interface IObjectValidator
    {
        void IsNull(object @object, string paramName);
    }
}
