using System;
using System.Collections.Generic;
using Environmentalist;
using Environmentalist.Models;
using Environmentalist.Services.LogicProcessor;
using Environmentalist.Validators.ObjectValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.LogicProcessor
{
    [TestFixture]
    public class LogicProcessorTests
    {
        private const string TemplateKey1 = nameof(TemplateKey1);
        private const string TemplateKey2 = nameof(TemplateKey2);
        private const string TemplateValue1 = nameof(TemplateValue1);
        private const string TemplateValue2 = nameof(TemplateValue2);
        private const string ConfigValue1 = nameof(ConfigValue1);
        private const string ConfigValue2 = nameof(ConfigValue2);
        private const string EnvironmentVariableKey1 = nameof(EnvironmentVariableKey1);
        private const string EnvironmentVariableValue1 = nameof(EnvironmentVariableValue1);
        private const string SecretKey1 = nameof(SecretKey1);
        private const string SecretValue1 = nameof(SecretValue1);

        private static readonly string SecretKeyLine1 = $"{Consts.KeePassTagName}({SecretKey1})";

        private TemplateModel TemplateModel;
        private ProfileModel TemplateProfile;
        private List<SecretEntryModel> Secrets;

        private Mock<IObjectValidator> _objectValidatorMock;
        private ILogicProcessor _sut;

        [SetUp]
        public void Init()
        {
            TemplateModel = new TemplateModel();
            TemplateProfile = new ProfileModel();
            Secrets = new List<SecretEntryModel>();

            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.LogicProcessor.LogicProcessor(
                _objectValidatorMock.Object);
        }

        [Test]
        public void When_process_Then_returns_filled_model()
        {
            TemplateModel.Fields.Add(TemplateKey1, TemplateValue1);
            TemplateModel.Fields.Add(TemplateKey2, TemplateValue2);

            TemplateProfile.Fields.Add(TemplateValue1, ConfigValue1);
            TemplateProfile.Fields.Add(TemplateValue2, ConfigValue2);

            var result = _sut.Process(TemplateModel, TemplateProfile, Secrets);

            Assert.AreEqual(ConfigValue1, result.Fields[TemplateKey1]);
            Assert.AreEqual(ConfigValue2, result.Fields[TemplateKey2]);
        }

        [Test]
        public void When_process_And_template_contains_secrets_Then_returns_filled_model()
        {
            TemplateModel.Fields.Add(TemplateKey1, SecretKeyLine1);
            TemplateModel.Fields.Add(TemplateKey2, TemplateValue2);

            TemplateProfile.Fields.Add(TemplateValue1, ConfigValue1);
            TemplateProfile.Fields.Add(TemplateValue2, ConfigValue2);

            Secrets.Add(new SecretEntryModel
                {
                    Title = SecretKey1,
                    Password = SecretValue1
                });

            var result = _sut.Process(TemplateModel, TemplateProfile, Secrets);

            Assert.AreEqual(SecretValue1, result.Fields[TemplateKey1]);
            Assert.AreEqual(ConfigValue2, result.Fields[TemplateKey2]);
        }

        [Test]
        public void When_process_And_template_model_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Process(null, TemplateProfile, Secrets));
        }

        [Test]
        public void When_process_And_template_config_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Process(TemplateModel, null, Secrets));
        }


        [Test]
        public void When_process_And_secrets_collection_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Process(TemplateModel, TemplateProfile, null));
        }
    }
}
