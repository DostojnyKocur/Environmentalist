using System.Collections.Generic;
using Environmentalist.Models;

namespace Environmentalist.Services.Readers.KeePassReader
{
    public interface IKeePassReader
    {
        ICollection<SecretEntryModel> ReadDatabase(string databasePath, string masterPassword);
    }
}
