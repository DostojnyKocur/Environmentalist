using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Models;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Serilog;

namespace Environmentalist.Services.KeePassReader
{
    public sealed class KeePassReader : IKeePassReader
    {
        public ICollection<SecretEntryModel> ReadDatabase(string databasePath, string masterPassword)
        {
            //How to read KeePass database taken from https://stackoverflow.com/a/9028433

            var result = default(ICollection<SecretEntryModel>);

            var connection = new IOConnectionInfo { Path = databasePath };
            var compositeKey = new CompositeKey();
            var password = new KcpPassword(masterPassword);
            compositeKey.AddUserKey(password);
            var database = new KeePassLib.PwDatabase();

            try
            {
                database.Open(connection, compositeKey, null);

                var readData = from entry in database.RootGroup.GetEntries(true)
                               select new
                               {
                                   Group = entry.ParentGroup.Name,
                                   Title = entry.Strings.ReadSafe("Title"),
                                   UserName = entry.Strings.ReadSafe("UserName"),
                                   Password = entry.Strings.ReadSafe("Password"),
                                   URL = entry.Strings.ReadSafe("URL"),
                                   Notes = entry.Strings.ReadSafe("Notes")

                               };
                result = readData.Select(data => new SecretEntryModel
                {
                    Title = data.Title,
                    UserName = data.UserName,
                    Password = data.Password,
                }).ToList();
            }
            catch (Exception exception)
            {
                Log.Logger.Error(exception, $"During reading KeePass database {databasePath} an error has been occured");
            }
            finally
            {
                if (database.IsOpen)
                {
                    database.Close();
                }
            }

            return result;
        }
    }
}
