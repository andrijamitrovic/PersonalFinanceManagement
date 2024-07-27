
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PersonalFinanceManagement.CsvHelper;
using PersonalFinanceManagement.Database;
using PersonalFinanceManagement.Database.Repositories;
using PersonalFinanceManagement.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var myLocalHostPolicy = "MyCORSPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myLocalHostPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddScoped<IRequestErrorService, RequestErrorService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase)
        );
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TransactionDbContext>(opt =>
{
    opt.UseNpgsql(CreateConnectionString(builder.Configuration));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseCors(myLocalHostPolicy);

app.Run();

string CreateConnectionString(IConfiguration configuration)
{
    var username = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
    var pass = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
    var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "PersonalFinanceManagementDatabase";
    var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5555";

    var connBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = host,
        Port = int.Parse(port),
        Username = username,
        Database = databaseName,
        Password = pass,
        Pooling = true,
        Timeout = 300
    };

    return connBuilder.ConnectionString;
}
