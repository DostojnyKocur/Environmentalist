using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Validators
{
    public interface IValidators
    {
        /// <summary>
        /// Gets file validator
        /// </summary>
        IFileValidator FileValidator { get; }

        /// <summary>
        /// Gets object validator
        /// </summary>
        IObjectValidator ObjectValidator { get; }

        /// <summary>
        /// Gets string validator
        /// </summary>
        IStringValidator StringValidator { get; }
    }
}
