using DER_System;
using DER_System.Helper;
using DER_System.Repository;
using DER_System.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NKC_Resource_Allocation.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Connectin string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddDbContext<DerDbContext>(opts => opts.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddNewtonsoftJson();


//Dependency Injection
builder.Services.AddScoped<QueryHelper>();

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<CustomerMaterialListingRepository>();
builder.Services.AddScoped<CustomerRouteRepository>();
builder.Services.AddScoped<MaterialRepository>();

// Bind individual keys from root
builder.Services.Configure<KeyConfig>(builder.Configuration);


builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddResponseCompression();



builder.Services.AddSwaggerGen();


var app = builder.Build();


// Middleware
app.UseMiddleware<Guard>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "/swagger/{documentname}/swagger.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "MBL Drough Empty Return System API Services");
    });
    app.UseDeveloperExceptionPage();
}
else if (app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger(c => {
        c.RouteTemplate = "/swagger/{documentname}/swagger.json";
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });

    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "MBL Drough Empty Return System API Services");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(builder => builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());

app.Run();
