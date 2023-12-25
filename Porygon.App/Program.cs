using Flapple.DataAccess.MySQL;
using Porygon.Entity.Data;
using Porygon.Entity.Manager;
using Porygon.Entity.MySql.Data;
using Porygon.Identity.MySql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSystemWebAdapters();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();
builder.Services.AddFreeSql(@"Server=localhost;Port=3306;Database=flapple;Uid=root;Pwd=p@ssw0rd;");
builder.Services.AddScoped<IEntityDataManager, MySqlEntityDataManager>();
builder.Services.AddScoped<EntityManager>();

builder.Services.AddMySqlIdentity();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapRazorPages();

app.UseSystemWebAdapters();

app.Run();
