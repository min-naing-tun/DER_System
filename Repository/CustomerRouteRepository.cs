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
    public class CustomerRouteRepository
    {
        private readonly DerDbContext _context;
        public readonly QueryHelper _queryHelper;
        public readonly Constants c = new Constants();

        #region constructor
        public CustomerRouteRepository(DerDbContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }
        #endregion

        #region Main CRUD Process
        public async Task<DataTable> GetAllAsync()
        {
            DataTable dt = new DataTable();
            string sql = "select * from " + c.CustomerRouteListing;
            List<SqlParameter> parameters = new List<SqlParameter>();
            dt = await _queryHelper.GetDataTableAsync(sql, parameters.ToArray());
            return dt;
        }

        public async Task<ResponseModel> CreateAsync(CustomerRouteListModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (activeCheckUser != null)
                {
                    // Route check
                    Routes? route = await _context.Routes.FromSqlRaw(@"select * from " + c.Route + " where Code = @code",
                                        new SqlParameter("@code", model.RouteCode)).SingleOrDefaultAsync();
                    if (route != null)
                    {
                        // Customer check
                        Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                        if (customer != null)
                        {
                            // Customer Route
                            CustomerRouteListings customerRouteListing = new CustomerRouteListings();
                            customerRouteListing.SysKey = Guid.NewGuid();
                            //customerRouteListing.AutoID = Convert.ToInt64((await _context.CustomerRouteListings.FromSqlRaw(@"select top 1 * from " + c.CustomerRouteListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            customerRouteListing.CustomerSysKey = customer.SysKey;
                            customerRouteListing.RouteSysKey = route.SysKey;
                            customerRouteListing.CreatedBy = activeCheckUser.SysKey;
                            customerRouteListing.CreatedDate = DateTime.Now;
                            customerRouteListing.Active = true;

                            // Save
                            await _context.CustomerRouteListings.AddAsync(customerRouteListing);
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Customer route created successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = $"Customer not found for this customer code { model.CustomerCode }!";
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = $"Route not found for this route code { model.RouteCode }!";
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

        public async Task<ResponseModel> UpdateAsync(CustomerRouteListModel model, string sysKey)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                CustomerRouteListings? customerRouteListing = await _context.CustomerRouteListings.FromSqlRaw(@"select * from " + c.CustomerRouteListing + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (customerRouteListing != null)
                {
                    if (activeCheckUser != null)
                    {
                        // Route check
                        Routes? route = await _context.Routes.FromSqlRaw(@"select * from " + c.Route + " where Code = @code",
                                            new SqlParameter("@code", model.RouteCode)).SingleOrDefaultAsync();
                        if (route != null)
                        {
                            // Customer check
                            Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                    new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                            if (customer != null)
                            {
                                // Customer Route
                                //customerRouteListing.SysKey = Guid.NewGuid(); //no need for update
                                //customerRouteListing.AutoID = Convert.ToInt64((await _context.CustomerRouteListings.FromSqlRaw(@"select top 1 * from " + c.CustomerRouteListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                                customerRouteListing.CustomerSysKey = customer.SysKey;
                                customerRouteListing.RouteSysKey = route.SysKey;
                                customerRouteListing.UpdatedBy = activeCheckUser.SysKey;
                                customerRouteListing.UpdatedDate = DateTime.Now;
                                //customerRouteListing.Active = true; //no need for update

                                // Update
                                await _context.SaveChangesAsync();

                                // Response
                                response.IsSuccess = true;
                                response.Message = "Customer route updated successfully!";
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
                            response.Message = $"Route not found for this route code {model.RouteCode}!";
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
                    response.Message = "Customer route not found!";
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
                CustomerRouteListings? customerRouteListings = await _context.CustomerRouteListings.FromSqlRaw(@"select * from " + c.CustomerRouteListing + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                if (customerRouteListings != null)
                {
                    // Delete
                    _context.CustomerRouteListings.Remove(customerRouteListings);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Customer route deleted successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer route not found!";
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
        public async Task<ResponseModel> PersistAsync(CustomerRouteListModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                CustomerRouteListings? customerRouteListing = await _context.CustomerRouteListings.FromSqlRaw(
                                                                @"select cr.* 
                                                                from " + c.CustomerRouteListing + @" cr
                                                                inner join " + c.Customer + @" c on c.SysKey = cr.CustomerSysKey
                                                                inner join " + c.Route + @" r on r.SysKey = cr.RouteSysKey
                                                                where c.Code = @customerCode and r.Code = @routeCode",
                                                                new SqlParameter("@customerCode", model.CustomerCode),
                                                                new SqlParameter("@routeCode", model.RouteCode)).SingleOrDefaultAsync();
                if (customerRouteListing != null)
                {
                    #region  [-- Update process --]
                    //for update -> check active, If 0 skip otherwise update
                    if (customerRouteListing.Active == true)
                    {
                        //update
                        if (activeCheckUser != null)
                        {
                            // Route check
                            Routes? route = await _context.Routes.FromSqlRaw(@"select * from " + c.Route + " where Code = @code",
                                                new SqlParameter("@code", model.RouteCode)).SingleOrDefaultAsync();
                            if (route != null)
                            {
                                // Customer check
                                Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                        new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                                if (customer != null)
                                {
                                    // Customer Route
                                    //customerRouteListing.SysKey = Guid.NewGuid(); //no need for update
                                    //customerRouteListing.AutoID = Convert.ToInt64((await _context.CustomerRouteListings.FromSqlRaw(@"select top 1 * from " + c.CustomerRouteListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                                    customerRouteListing.CustomerSysKey = customer.SysKey;
                                    customerRouteListing.RouteSysKey = route.SysKey;
                                    customerRouteListing.UpdatedBy = activeCheckUser.SysKey;
                                    customerRouteListing.UpdatedDate = DateTime.Now;
                                    customerRouteListing.Active = model.Active.ToString().Trim().IsNullOrEmpty() ? false : true;

                                    // Update
                                    await _context.SaveChangesAsync();

                                    // Response
                                    response.IsSuccess = true;
                                    response.Message = "Customer route updated successfully!";
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
                                response.Message = $"Route not found for this route code {model.RouteCode}!";
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
                        response.Message = $"Existing record is not active for this customer code { model.CustomerCode } and route code { model.RouteCode }!";
                    }
                    #endregion
                }
                else
                {
                    #region  [-- Create process --]
                    // Route check
                    Routes? route = await _context.Routes.FromSqlRaw(@"select * from " + c.Route + " where Code = @code",
                                        new SqlParameter("@code", model.RouteCode)).SingleOrDefaultAsync();
                    if (route != null)
                    {
                        // Customer check
                        Customers? customer = await _context.Customers.FromSqlRaw(@"select * from " + c.Customer + " where Code = @code",
                                                new SqlParameter("@code", model.CustomerCode)).SingleOrDefaultAsync();
                        if (customer != null)
                        {
                            // Customer Route
                            customerRouteListing = new CustomerRouteListings();
                            customerRouteListing.SysKey = Guid.NewGuid();
                            //customerRouteListing.AutoID = Convert.ToInt64((await _context.CustomerRouteListings.FromSqlRaw(@"select top 1 * from " + c.CustomerRouteListing + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            customerRouteListing.CustomerSysKey = customer.SysKey;
                            customerRouteListing.RouteSysKey = route.SysKey;
                            customerRouteListing.CreatedBy = activeCheckUser!.SysKey;
                            customerRouteListing.CreatedDate = DateTime.Now;
                            customerRouteListing.UpdatedBy = activeCheckUser!.SysKey;
                            customerRouteListing.UpdatedDate = DateTime.Now;
                            customerRouteListing.Active = model.Active.ToString().Trim().IsNullOrEmpty() ? false : true;

                            // Save
                            await _context.CustomerRouteListings.AddAsync(customerRouteListing);
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Customer route created successfully!";
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
                        response.Message = $"Route not found for this route code {model.RouteCode}!";
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
