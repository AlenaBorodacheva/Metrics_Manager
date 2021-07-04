using System.Collections.Generic;

namespace MetricsCommon.SQL_Settings
{
    public enum Tables
    {
        CpuMetrics,
        DotNetMetrics,
        HddMetrics,
        NetworkMetrics,
        RamMetrics
    }
    public enum AgentFields
    {
        Id,
        Value,
        Time
    }
    public enum ManagerFields
    {
        Id,
        AgentId,
        Value,
        Time
    }

    public class SqlSettings : ISqlSettings
    {
        private static readonly string _connectionString = @"Data Source=metrics.db; Version=3;";

        private readonly Dictionary<Tables, string> _tablesDb = new()
        {
            { Tables.CpuMetrics, "cpumetrics" },
            { Tables.DotNetMetrics, "dotnetmetrics" },
            { Tables.HddMetrics, "hddmetrics" },
            { Tables.NetworkMetrics, "networkmetrics" },
            { Tables.RamMetrics, "rammetrics" },
        };
        private readonly Dictionary<AgentFields, string> _agentFields = new()
        {
            { AgentFields.Id, "Id" },
            { AgentFields.Time, "Time" },
            { AgentFields.Value, "Value" },
        };
        private readonly Dictionary<ManagerFields, string> _managerFields = new()
        {
            { ManagerFields.Id, "Id" },
            { ManagerFields.AgentId, "AgentId" },
            { ManagerFields.Time, "Time" },
            { ManagerFields.Value, "Value" },
        };

        public static string ConnectionString => _connectionString;

        public string this[Tables key] => _tablesDb[key];

        public string this[AgentFields key] => _agentFields[key];

        public string this[ManagerFields key] => _managerFields[key];
    }
}
