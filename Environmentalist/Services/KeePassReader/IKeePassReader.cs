using System.Collections.Generic;

namespace Environmentalist.Services.KeePassReader
{
    public interface IKeePassReader
    {
        Dictionary<string, string> ReadDatabase(string databasePath, string masterPassword);
    }
}
