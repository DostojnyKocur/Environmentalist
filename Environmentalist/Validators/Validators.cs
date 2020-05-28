using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Validators
{
    public sealed class Validators : IValidators
    {
        public Validators(
            IFileValidator fileValidator,
            IObjectValidator objectValidator,
            IStringValidator stringValidator)
        {
            FileValidator = fileValidator;
            ObjectValidator = objectValidator;
            StringValidator = stringValidator;
        }

        public IFileValidator FileValidator { get; }
        public IObjectValidator ObjectValidator { get; }
        public IStringValidator StringValidator { get; }
    }
}
