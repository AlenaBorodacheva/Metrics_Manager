using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MetricsAgent.Models;

namespace MetricsAgent.Repositories
{
    public interface IHddMetricsRepository : IRepository<HddMetric>
    {
    }

    public class HddMetricsRepository : IHddMetricsRepository
    {
        private readonly SQLiteConnection _connection;

        public HddMetricsRepository(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void Create(HddMetric item)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Execute("INSERT INTO hddmetrics(value, time) VALUES(@value, @time)",
                    new
                    {
                        value = item.Value,
                        time = item.Time
                    });
            }
        }

        public void Delete(int metricId)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Execute("DELETE FROM hddmetrics WHERE id=@id", new { id = metricId });
            }
        }

        public IList<HddMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                return connection.Query<HddMetric>("SELECT id, time, value FROM hddmetrics").ToList();
            }
        }
        
        public IList<HddMetric> GetByTimePeriod(long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                return connection.Query<HddMetric>("SELECT * FROM hddmetrics WHERE (time>=@fromTime) AND (time<=@toTime)",
                    new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }

        public HddMetric GetById(int metricId)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                try
                {
                    return connection.QuerySingle<HddMetric>("SELECT * FROM hddmetrics WHERE id = @id",
                        new { id = metricId });
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Update(HddMetric item)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Execute("UPDATE hddmetrics SET value = @value, time = @time WHERE id=@id;",
                    new { value = item.Value, time = item.Time, id = item.Id });
            }
        }
    }
}
