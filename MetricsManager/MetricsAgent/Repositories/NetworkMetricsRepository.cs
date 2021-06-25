using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MetricsAgent.Models;

namespace MetricsAgent.Repositories
{
    public interface INetworkMetricsRepository : IRepository<NetworkMetric>
    {
    }

    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private readonly SQLiteConnection _connection;

        public NetworkMetricsRepository(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void Create(NetworkMetric item)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Execute("INSERT INTO networkmetrics(value, time) VALUES(@value, @time)",
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
                connection.Execute("DELETE FROM networkmetrics WHERE id=@id", new { id = metricId });
            }
        }

        public IList<NetworkMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                return connection.Query<NetworkMetric>("SELECT id, time, value FROM networkmetrics").ToList();
            }
        }
        
        public IList<NetworkMetric> GetByTimePeriod(long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                return connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE (time>=@fromTime) AND (time<=@toTime)",
                    new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }

        public NetworkMetric GetById(int metricId)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                try
                {
                    return connection.QuerySingle<NetworkMetric>("SELECT * FROM networkmetrics WHERE id = @id",
                        new { id = metricId });
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Update(NetworkMetric item)
        {
            using (var connection = new SQLiteConnection(_connection))
            {
                connection.Execute("UPDATE networkmetrics SET value = @value, time = @time WHERE id=@id;",
                    new { value = item.Value, time = item.Time, id = item.Id });
            }
        }
    }
}
