using DER_System.Helper;
using DER_System.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DER_System.Repository
{
    public class CustomerRepository
    {
        private readonly DerDbContext _context;
        public readonly QueryHelper _queryHelper;
        public readonly Constants c = new Constants();
        public CustomerRepository(DerDbContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }

        public async Task<DataTable> GetAllAsync()
        {
            DataTable dt = new DataTable();

            string sql = $"select top 10 * from {c.Customer}";

            List<SqlParameter> parameters = new List<SqlParameter>();

            dt = await _queryHelper.GetDataTableAsync(sql, parameters.ToArray());

            return dt;
        }
    }
}
