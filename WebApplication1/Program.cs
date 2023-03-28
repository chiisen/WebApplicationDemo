using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 取得 appsettings.json 的 Seq 設定
var seqJson_ = builder.Configuration.GetSection("Seq");


// Use the Seq logging configuration in appsettings.json
builder.Host.ConfigureLogging(loggingBuilder =>
    loggingBuilder.AddSeq(seqJson_));

var app = builder.Build();
app.Logger.LogInformation("🐛🐛🐛🐛🐛🐛🐛🐛🐛 程式啟動 🐛🐛🐛🐛🐛🐛🐛🐛🐛");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
