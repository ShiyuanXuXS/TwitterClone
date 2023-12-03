using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using TableDependency.SqlClient;
using TwitterClone.Models;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.SubscribeTableDependencies
{
    public class SubscribeNotificationTableDependency : ISubscribeTableDependency
    {
        // SqlTableDependency<Notification> tableDependency;

        public void SubscribeTableDepedency(string connectionString)
        {
            // tableDependency = new SqlTableDependency<Notification>(connectionString);
            // tableDependency.OnChanged += TableDependency_Changed;
            // tableDependency.OnError += TableDependency_OnError;
            // tableDependency.Start();
        }

        // private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        // {
        //     Console.WriteLine($"{nameof(Notification)} SqlTableDependency error : {e.Error.Message}");
        // }

        // private void TableDependency_OnChanged(Object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Notification> e)
        // {
        //     throw new NotImplementedException();
        // }
    }
}