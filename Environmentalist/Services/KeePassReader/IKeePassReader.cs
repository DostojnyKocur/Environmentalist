using System.Collections.Generic;
using Environmentalist.Models;

namespace Environmentalist.Services.KeePassReader
{
    public interface IKeePassReader
    {
        ICollection<SecretEntryModel> ReadDatabase(string databasePath, string masterPassword);
    }
}
