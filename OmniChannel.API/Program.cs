using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using OmniChannel.API.Data;
using OmniChannel.API.Hubs;
using OmniChannel.API.Repositories;
using OmniChannel.API.Services;

var builder = WebApplication.CreateBuilder(args);

var blobStorageConnectionString =
    builder.Configuration.GetConnectionString("AzureStorageConn")
    ?? throw new InvalidOperationException("AzureStorageConn is missing");

var sqlConnectionString =
    builder.Configuration.GetConnectionString("AzureDbConn")
    ?? throw new InvalidOperationException("AzureDbConn is missing");

var tableStorageConnectionString =
    builder.Configuration.GetConnectionString("AzureTableConn")
    ?? throw new InvalidOperationException("AzureTableConn is missing");

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        sqlConnectionString,
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);

builder.Services.AddSingleton(new BlobServiceClient(blobStorageConnectionString));
builder.Services.AddSingleton(new TableServiceClient(tableStorageConnectionString));

builder.Services.AddSignalR();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<ClientSQLRepository>();
builder.Services.AddScoped<ClientAzureTableRepository>();
builder.Services.AddScoped<IClientRepository, ClientResilientRepository>();
builder.Services.AddScoped<IActivityUploadRepository, ActivityUploadRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:7039"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

var app = builder.Build();

app.UseRouting();
app.UseCors("DefaultCorsPolicy");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OmniChannel API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<LogHub>("/logHub");

app.Run();
