using InsightsService.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace InsightsService.Data
{
    // SQL server - reading insights from SQL server DBs
    public class DapperContext
    {
        private readonly DatabaseConnections _connections;

        public DapperContext(IOptions<DatabaseConnections> options) 
            => _connections = options.Value;

        public IDbConnection UsersDb()
            => new SqlConnection(_connections.UsersDb);

        public IDbConnection TasksDb()
            => new SqlConnection(_connections.TasksDb);

        public IDbConnection MoodDb()
            => new SqlConnection(_connections.MoodDb);
    }
}
