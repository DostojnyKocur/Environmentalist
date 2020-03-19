using System;

namespace Environmentalist.Validators.ObjectValidator
{
    public sealed class ObjectValidator : IObjectValidator
    {
        public void IsNull(object @object, string paramName)
        {
            if(@object is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
