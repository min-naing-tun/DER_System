using DER_System.Helper;
using DER_System.Model;
using DER_System.Utilities;
using DER_System.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DER_System.Repository
{
    public class CustomerMaterialListingRepository
    {
        private readonly DerDbContext _context;
        public readonly QueryHelper _queryHelper;
        public readonly Constants c = new Constants();

        #region constructor
        public CustomerMaterialListingRepository(DerDbContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }
        #endregion

        #region Main CRUD Process
        public async Task<DataTable> GetAllAsync()
        {
            DataTable dt = new DataTable();
            string sql = "select * from " + c.CustomerMaterialListing;
            List<SqlParameter> parameters = new List<SqlParameter>();
            dt = await _queryHelper.GetDataTableAsync(sql, parameters.ToArray());
            return dt;
        }

        public async Task<ResponseModel> CreateAsync(CustomerMaterialListingModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (activeCheckUser != null)
                {
                    // Material check
                    Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where Code = @code",
                                        new SqlParameter("@code", model.MaterialCode)).SingleOrDefaultAsync();
                    if (material != null)
                    {
                        // Customer check
                        Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                        if (customer != null)
                        {
                            // Customer Material
                            CustomerMaterialListings customerMaterialListing = new CustomerMaterialListings();
                            customerMaterialListing.SysKey = Guid.NewGuid();
                            //customerMaterialListing.AutoID = Convert.ToInt64((await _context.CustomerMaterialListings.FromSqlRaw(@"select top 1 * from " + c.CustomerMaterialListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            customerMaterialListing.CustomerSysKey = customer.SysKey;
                            customerMaterialListing.MaterialSysKey = material.SysKey;
                            customerMaterialListing.CreatedBy = activeCheckUser.SysKey;
                            customerMaterialListing.CreatedDate = DateTime.Now;
                            customerMaterialListing.Active = true;

                            // Save
                            await _context.CustomerMaterialListings.AddAsync(customerMaterialListing);
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Customer material created successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = $"Customer not found for this customer code {model.CustomerCode}!";
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = $"Material not found for this material code {model.MaterialCode}!";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Active user not found!";
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ResponseModel> UpdateAsync(CustomerMaterialListingModel model, string sysKey)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                CustomerMaterialListings? customerMaterialListing = await _context.CustomerMaterialListings.FromSqlRaw(@"select * from " + c.CustomerMaterialListing + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (customerMaterialListing != null)
                {
                    if (activeCheckUser != null)
                    {
                        // Material check
                        Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where Code = @code",
                                            new SqlParameter("@code", model.MaterialCode)).SingleOrDefaultAsync();
                        if (material != null)
                        {
                            // Customer check
                            Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                    new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                            if (customer != null)
                            {
                                // Customer Material
                                //customerMaterialListing.SysKey = Guid.NewGuid(); //no need for update
                                //customerMaterialListing.AutoID = Convert.ToInt64((await _context.CustomerMaterialListings.FromSqlRaw(@"select top 1 * from " + c.CustomerMaterialListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                                customerMaterialListing.CustomerSysKey = customer.SysKey;
                                customerMaterialListing.MaterialSysKey = material.SysKey;
                                customerMaterialListing.UpdatedBy = activeCheckUser.SysKey;
                                customerMaterialListing.UpdatedDate = DateTime.Now;
                                //customerMaterialListing.Active = true; //no need for update

                                // Update
                                await _context.SaveChangesAsync();

                                // Response
                                response.IsSuccess = true;
                                response.Message = "Customer material updated successfully!";
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Customer not found for this customer code {model.CustomerCode}!";
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = $"Material not found for this material code {model.MaterialCode}!";
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Active user not found!";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer material not found!";
                }
            }
            catch (Exception e)
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
                CustomerMaterialListings? customerMaterialListings = await _context.CustomerMaterialListings.FromSqlRaw(@"select * from " + c.CustomerMaterialListing + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                if (customerMaterialListings != null)
                {
                    // Delete
                    _context.CustomerMaterialListings.Remove(customerMaterialListings);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer material deleted successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer material not found!";
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

        #region Customize Process
        public async Task<ResponseModel> PersistAsync(CustomerMaterialListingModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                CustomerMaterialListings? customerMaterialListing = await _context.CustomerMaterialListings.FromSqlRaw(
                                                                @"select cr.* 
                                                                from " + c.CustomerMaterialListing + @" cr
                                                                inner join " + c.Customer + @" c on c.SysKey = cr.CustomerSysKey
                                                                inner join " + c.Material + @" m on m.SysKey = cr.MaterialSysKey
                                                                where c.Code = @customerCode and m.Code = @materialCode",
                                                                new SqlParameter("@customerCode", model.CustomerCode),
                                                                new SqlParameter("@materialCode", model.MaterialCode)).SingleOrDefaultAsync();
                if (customerMaterialListing != null)
                {
                    #region  [-- Update process --]
                    //for update -> check active, If 0 skip otherwise update
                    if (customerMaterialListing.Active == true)
                    {
                        //update
                        if (activeCheckUser != null)
                        {
                            // Material check
                            Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where Code = @code",
                                                new SqlParameter("@code", model.MaterialCode)).SingleOrDefaultAsync();
                            if (material != null)
                            {
                                // Customer check
                                Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                        new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                                if (customer != null)
                                {
                                    // Customer Material
                                    //customerMaterialListing.SysKey = Guid.NewGuid(); //no need for update
                                    //customerMaterialListing.AutoID = Convert.ToInt64((await _context.CustomerMaterialListings.FromSqlRaw(@"select top 1 * from " + c.CustomerMaterialListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                                    customerMaterialListing.CustomerSysKey = customer.SysKey;
                                    customerMaterialListing.MaterialSysKey = material.SysKey;
                                    customerMaterialListing.UpdatedBy = activeCheckUser.SysKey;
                                    customerMaterialListing.UpdatedDate = DateTime.Now;
                                    //customerMaterialListing.Active = true; //no need for update

                                    // Update
                                    await _context.SaveChangesAsync();

                                    // Response
                                    response.IsSuccess = true;
                                    response.Message = "Customer material updated successfully!";
                                }
                                else
                                {
                                    response.IsSuccess = false;
                                    response.Message = $"Customer not found for this customer code {model.CustomerCode}!";
                                }
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = $"Material not found for this material code {model.MaterialCode}!";
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Active user not found!";
                        }
                    }
                    else
                    {
                        //skip update
                        response.IsSuccess = true;
                        response.Message = $"Existing record is not active for this customer code {model.CustomerCode} and material code {model.MaterialCode}!";
                    }
                    #endregion
                }
                else
                {
                    #region  [-- Create process --]
                    // Material check
                    Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where Code = @code",
                                        new SqlParameter("@code", model.MaterialCode)).SingleOrDefaultAsync();
                    if (material != null)
                    {
                        // Customer check
                        Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                        if (customer != null)
                        {
                            // Customer Material
                            customerMaterialListing = new CustomerMaterialListings();
                            customerMaterialListing.SysKey = Guid.NewGuid();
                            //customerMaterialListing.AutoID = Convert.ToInt64((await _context.CustomerMaterialListings.FromSqlRaw(@"select top 1 * from " + c.CustomerMaterialListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            customerMaterialListing.CustomerSysKey = customer.SysKey;
                            customerMaterialListing.MaterialSysKey = material.SysKey;
                            customerMaterialListing.CreatedBy = activeCheckUser!.SysKey;
                            customerMaterialListing.CreatedDate = DateTime.Now;
                            customerMaterialListing.Active = true;

                            // Save
                            await _context.CustomerMaterialListings.AddAsync(customerMaterialListing);
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Customer material created successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = $"Customer not found for this customer code {model.CustomerCode}!";
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = $"Material not found for this material code {model.MaterialCode}!";
                    }
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
