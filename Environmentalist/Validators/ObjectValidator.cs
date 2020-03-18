using System;

namespace Environmentalist.Validators
{
    public class ObjectValidator
    {
        public static void IsNull(object @object, string paramName)
        {
            if(@object is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
