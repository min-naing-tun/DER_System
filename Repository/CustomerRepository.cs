using DER_System.Helper;
using DER_System.Model;
using DER_System.Utilities;
using DER_System.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace DER_System.Repository
{
    public class CustomerRepository
    {
        private readonly DerDbContext _context;
        public readonly QueryHelper _queryHelper;
        public readonly Constants c = new Constants();

        #region constructor
        public CustomerRepository(DerDbContext context, QueryHelper queryHelper)
        
        {
            _context = context;
            _queryHelper = queryHelper;
        }
        #endregion

        #region Main CRUD Process
        public async Task<DataTable> GetAllAsync()
        {
            DataTable dt = new DataTable();
            string sql = "select * from " + c.Customer;
            List<SqlParameter> parameters = new List<SqlParameter>();
            dt = await _queryHelper.GetDataTableAsync(sql, parameters.ToArray());
            return dt;
        }

        public async Task<ResponseModel> CreateAsync(CustomerModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Customers? maxCheckCustomer = await _context.Customers.FromSqlRaw(@"select top 1 * from " + c.Customer + " order by AutoID desc").SingleOrDefaultAsync();
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (maxCheckCustomer != null && activeCheckUser != null)
                {
                    // Customer
                    Customers customer = new Customers();
                    customer.SysKey = Guid.NewGuid();
                    // customer.AutoID = Convert.ToInt64(maxCheckCustomer.AutoID) + 1; // no need because it's auto increment column
                    customer.Code = model.Code;
                    customer.Description = model.Description;
                    customer.AlloBlock = (short)(model.AllocationBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.CentralBlock = (short)(model.CentralBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.CreatedBy = activeCheckUser.SysKey;
                    customer.CreatedDate = DateTime.Now;
                    customer.Active = true;

                    // Save
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer created successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer not found!";
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
        
        public async Task<ResponseModel> UpdateAsync(CustomerModel model, string sysKey)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (customer != null && activeCheckUser != null)
                {
                    // Customer
                    // customer.SysKey = Guid.NewGuid(); // no need for update process
                    // customer.AutoID = Convert.ToInt64(maxCheckCustomer.AutoID) + 1; // no need because it's auto increment column
                    customer.Code = model.Code;
                    customer.Description = model.Description;
                    customer.AlloBlock = (short)(model.AllocationBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.CentralBlock = (short)(model.CentralBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.UpdatedBy = activeCheckUser.SysKey;
                    customer.UpdatedDate = DateTime.Now;
                    // customer.Active = true; // no need for update process

                    // Update
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer updated successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer not found!";
                }
            }
            catch(Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
        
        public async Task<ResponseModel> DeleteAsync(string sysKey)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                if (customer != null)
                {
                    // Delete
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer deleted successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer not found!";
                }
            }
            catch(Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
        #endregion

        #region Customize Process
        public async Task<ResponseModel> PersistAsync(CustomerModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                // Check code
                Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                            new SqlParameter("@code", model.Code)).SingleOrDefaultAsync();
                if(customer != null)
                {
                    #region  [-- Update process --]
                    //for update -> check active, If 0 skip otherwise update
                    if(customer.Active == true)
                    {
                        //update
                        if (customer != null && activeCheckUser != null)
                        {
                            // Customer
                            // customer.SysKey = Guid.NewGuid(); // no need for update process
                            // customer.AutoID = Convert.ToInt64(maxCheckCustomer.AutoID) + 1; // no need because it's auto increment column
                            customer.Code = model.Code;
                            customer.Description = model.Description;
                            customer.AlloBlock = (short)(model.AllocationBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                            customer.CentralBlock = (short)(model.CentralBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                            customer.UpdatedBy = activeCheckUser.SysKey;
                            customer.UpdatedDate = DateTime.Now;
                            // customer.Active = true; // no need for update process

                            // Update
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Customer updated successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Customer not found!";
                        }
                    }
                    else
                    {
                        //skip update
                        response.IsSuccess = true;
                        response.Message = $"Record already exist for this code {model.Code}!";
                    }
                    #endregion
                }
                else
                {
                    #region  [-- Create process --]
                    // Customer
                    customer = new Customers();
                    customer.SysKey = Guid.NewGuid();
                    // customer.AutoID = Convert.ToInt64(maxCheckCustomer.AutoID) + 1; // no need because it's auto increment column
                    customer.Code = model.Code;
                    customer.Description = model.Description;
                    customer.AlloBlock = (short)(model.AllocationBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.CentralBlock = (short)(model.CentralBlock.ToString().Trim().IsNullOrEmpty() ? 0 : 1);
                    customer.CreatedBy = activeCheckUser!.SysKey;
                    customer.CreatedDate = DateTime.Now;
                    customer.Active = true;

                    // Save
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer created successfully!";
                    #endregion
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
        #endregion
    }
}
