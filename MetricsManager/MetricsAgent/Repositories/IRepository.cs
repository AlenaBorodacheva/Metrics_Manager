using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Repositories
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();

        void Create(T item);

        void Update(T item);

        void Delete(int id);

        IList<T> GetByTimePeriod(long fromTime, long toTime);

        T GetById(int id);
    }
}
