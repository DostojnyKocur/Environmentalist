using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Helpers;
using Environmentalist.Models;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Services.ProfileReader;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.ProfileRepository
{
    public sealed class ProfileRepository : IProfileRepository
    {
        private readonly IProfileReader _profileReader;
        private readonly IEnvironmentVariableReader _environmentVariableReader;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public ProfileRepository(
            IProfileReader profileReader,
            IEnvironmentVariableReader environmentVariableReader,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _profileReader = profileReader;
            _environmentVariableReader = environmentVariableReader;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task<ProfileModel> GetProfile(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));

            var profile = await _profileReader.Read(path);

            var environmentVariables = _profileReader.ExtractEnvironmentVariables(profile);

            var environmentVariablesValues = _environmentVariableReader.Read(environmentVariables);

            profile = ProcessEnvironmentVariables(profile, environmentVariablesValues);

            return profile;
        }

        private ProfileModel ProcessEnvironmentVariables(ProfileModel profile, IDictionary<string, string> environmentVariables)
        {
            _objectValidator.IsNull(profile, nameof(profile));
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));

            var newProfile = new ProfileModel();

            foreach (var field in profile.Fields)
            {
                var value = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(field.Value, environmentVariables);
                newProfile.Fields.Add(field.Key, value);
            }

            return newProfile;
        }
    }
}
