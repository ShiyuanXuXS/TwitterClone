using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.SubscribeTableDependencies
{
    public interface ISubscribeTableDependency
    {
        void SubscribeTableDepedency(string connectionString);
    }
}