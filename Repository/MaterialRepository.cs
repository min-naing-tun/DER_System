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
    public class MaterialRepository
    {
        private readonly DerDbContext _context;
        public readonly QueryHelper _queryHelper;
        public readonly Constants c = new Constants();

        #region constructor
        public MaterialRepository(DerDbContext context, QueryHelper queryHelper)

        {
            _context = context;
            _queryHelper = queryHelper;
        }
        #endregion

        #region Main CRUD Process
        public async Task<DataTable> GetAllAsync()
        {
            DataTable dt = new DataTable();
            string sql = "select * from " + c.Material;
            List<SqlParameter> parameters = new List<SqlParameter>();
            dt = await _queryHelper.GetDataTableAsync(sql, parameters.ToArray());
            return dt;
        }

        public async Task<ResponseModel> CreateAsync(MaterialModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (activeCheckUser != null)
                {
                    // Check material type
                    MaterialTypes? materialType = await _context.MaterialTypes.FromSqlRaw(@"select * from " + c.MaterialType + " where Code = @code",
                                                        new SqlParameter("@code", model.MaterialType)).SingleOrDefaultAsync();
                    if(materialType != null)
                    {
                        // Material
                        Materials material = new Materials();
                        material.SysKey = Guid.NewGuid();
                        //material.AutoID = Convert.ToInt64((await _context.Materials.FromSqlRaw(@"select top 1 * from " + c.Material + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                        material.Code = model.Code;
                        material.Description = model.Description;
                        material.MaterialGroupSysKey = materialType.SysKey;
                        material.CreatedBy = activeCheckUser.SysKey;
                        material.CreatedDate = DateTime.Now;
                        material.Active = true;

                        // Save
                        await _context.Materials.AddAsync(material);
                        await _context.SaveChangesAsync();

                        // Response
                        response.IsSuccess = true;
                        response.Message = "Material created successfully!";
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = $"Material type not found for this code { model.MaterialType }";
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

        public async Task<ResponseModel> UpdateAsync(MaterialModel model, string sysKey)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                if (material != null)
                {
                    if(activeCheckUser != null)
                    {
                        // Check material type
                        MaterialTypes? materialType = await _context.MaterialTypes.FromSqlRaw(@"select * from " + c.MaterialType + " where Code = @code",
                                                            new SqlParameter("@code", model.MaterialType)).SingleOrDefaultAsync();
                        if (materialType != null)
                        {
                            // Material
                            //material.SysKey = Guid.NewGuid(); // no need for update process
                            //material.AutoID = Convert.ToInt64((await _context.Materials.FromSqlRaw(@"select top 1 * from " + c.Material + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            material.Code = model.Code;
                            material.Description = model.Description;
                            material.MaterialGroupSysKey = materialType.SysKey;
                            material.UpdatedBy = activeCheckUser.SysKey;
                            material.UpdatedDate = DateTime.Now;
                            //material.Active = true; // no need for update process

                            // Update
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Material updated successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Material type not found!";
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
                    response.Message = "Material not found!";
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
                Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where SysKey = @key",
                                        new SqlParameter("@key", sysKey)).SingleOrDefaultAsync();
                if (material != null)
                {
                    // Delete
                    _context.Materials.Remove(material);
                    await _context.SaveChangesAsync();

                    // Response
                    response.IsSuccess = true;
                    response.Message = "Material deleted successfully!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Material not found!";
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
        public async Task<ResponseModel> PersistAsync(MaterialModel model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Users? activeCheckUser = await _context.Users.FromSqlRaw(@"select top 1 * from " + c.User + " where Active = 1").SingleOrDefaultAsync();
                Materials? material = await _context.Materials.FromSqlRaw(@"select * from " + c.Material + " where Code = @key",
                                        new SqlParameter("@key", model.Code)).SingleOrDefaultAsync();
                if (material != null)
                {
                    #region  [-- Update process --]
                    //for update -> check active, If 0 skip otherwise update
                    if (material.Active == true)
                    {
                        if (activeCheckUser != null)
                        {
                            // Check material type
                            MaterialTypes? materialType = await _context.MaterialTypes.FromSqlRaw(@"select * from " + c.MaterialType + " where Code = @code",
                                                                new SqlParameter("@code", model.MaterialType)).SingleOrDefaultAsync();
                            if (materialType != null)
                            {
                                // Material
                                //material.SysKey = Guid.NewGuid(); // no need for update process
                                //material.AutoID = Convert.ToInt64((await _context.Materials.FromSqlRaw(@"select top 1 * from " + c.Material + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                                material.Code = model.Code;
                                material.Description = model.Description;
                                material.MaterialGroupSysKey = materialType.SysKey;
                                material.UpdatedBy = activeCheckUser.SysKey;
                                material.UpdatedDate = DateTime.Now;
                                //material.Active = true; // no need for update process

                                // Update
                                await _context.SaveChangesAsync();

                                // Response
                                response.IsSuccess = true;
                                response.Message = "Material updated successfully!";
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = "Material type not found!";
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
                        response.Message = $"Existing record is not active for this code {model.Code}!";
                    }
                    #endregion
                }
                else
                {
                    #region  [-- Create process --]
                    if (activeCheckUser != null)
                    {
                        // Check material type
                        MaterialTypes? materialType = await _context.MaterialTypes.FromSqlRaw(@"select * from " + c.MaterialType + " where Code = @code",
                                                            new SqlParameter("@code", model.MaterialType)).SingleOrDefaultAsync();
                        if (materialType != null)
                        {
                            // Material
                            material = new Materials();
                            material.SysKey = Guid.NewGuid();
                            //material.AutoID = Convert.ToInt64((await _context.Materials.FromSqlRaw(@"select top 1 * from " + c.Material + " order by AutoID desc").SingleOrDefaultAsync())!.AutoID) + 1; // no need because it's auto increment column
                            material.Code = model.Code;
                            material.Description = model.Description;
                            material.MaterialGroupSysKey = materialType.SysKey;
                            material.CreatedBy = activeCheckUser.SysKey;
                            material.CreatedDate = DateTime.Now;
                            material.Active = true;

                            // Save
                            await _context.Materials.AddAsync(material);
                            await _context.SaveChangesAsync();

                            // Response
                            response.IsSuccess = true;
                            response.Message = "Material created successfully!";
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = $"Material type not found for this code {model.MaterialType}";
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Active user not found!";
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
