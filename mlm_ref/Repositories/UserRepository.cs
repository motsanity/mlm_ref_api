using mlm_ref.Infrastructure.Database; // or the correct namespace where DbConnectionFactory is defined
using Dapper;

namespace mlm_ref.Repositories
{
    public class UserReadRepository
    {
        private readonly DbConnectionFactory _factory;

        public UserReadRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<dynamic>> GetDownlines(string parentRef, int limit = 15)
        {
            using var conn = _factory.CreateConnection();

            var sql = @"
                SELECT ref, username, position, status, created_at
                FROM users
                WHERE placement_ref = @parentRef
                ORDER BY created_at
                LIMIT @limit;
            ";

            return await conn.QueryAsync(sql, new { parentRef, limit });
        }
    }

}