using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DER_System.Helper
{
    public class QueryHelper
    {
        private readonly IConfiguration _config;

        public QueryHelper(IConfiguration config)
        {
            _config = config;
        }

        public Task<DataTable> GetDataTableAsync(string sSQL, SqlParameter[] para)
        {
            var _conStr = _config.GetConnectionString("DefaultConnection");
            int _timeout = _config.GetSection("DbTimeout").Value != null ? int.Parse(_config.GetSection("DbTimeout").Value) : 3600;
            return Task.Run(() =>
            {
                using (var newCon = new SqlConnection(_conStr))
                using (var adapt = new SqlDataAdapter(sSQL, newCon))
                {
                    newCon.Open();

                    DataTable dt = new DataTable();

                    adapt.SelectCommand.CommandType = CommandType.Text;
                    adapt.SelectCommand.CommandTimeout = _timeout;

                    if (para != null)
                        adapt.SelectCommand.Parameters.AddRange(para);

                    adapt.Fill(dt);
                    newCon.Close();
                    return dt;
                }
            });
        }
    }
}
