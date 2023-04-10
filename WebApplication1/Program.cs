using Coravel;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApplication1;
using WebApplication1.Schedule;

// 指定短版的 Guid
int len_ = 12;//指定 Guid 的長度
string convertedUUID_ = Guid.NewGuid().ToString().Substring(0, len_);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


#region AddSingleton
// 處理 appsettings.json 要傳送的 MS-SQL 設定
builder.Services.AddSingleton<ISqlSettings, MsSqlSettings>();
MsSqlSettings MsSqlSettings_ = builder.Configuration.GetSection("MSSQL").Get<MsSqlSettings>();
builder.Services.AddSingleton<ISqlSettings>(MsSqlSettings_);

// 處理 appsettings.json 要傳送的 Redis 設定
builder.Services.AddSingleton<IRedisSettings, RedisSettings>();
RedisSettings RedisSettings_ = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
builder.Services.AddSingleton<IRedisSettings>(RedisSettings_);
# endregion AddSingleton


# region AddScheduler
// 註冊 Coravel 的 Scheduler
builder.Services.AddScheduler();

builder.Services.AddTransient<DemoSchedule>();
#endregion AddScheduler


// 取的 AssemblyVersion 與 FileVersion
var AssemblyVersion_ = Assembly.GetEntryAssembly()?.GetName().Version;
var FileVersion_ = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = $"AssemblyVersion: {AssemblyVersion_}, FileVersion: {FileVersion_}",
        Title = $"簡單的 CRUD 範例",
        Description = $"ASP.NET Core Web API 簡單的 CRUD 範例 - {convertedUUID_}",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

// 取得 appsettings.json 的 Seq 設定
var seqJson_ = builder.Configuration.GetSection("Seq");

// Use the Seq logging configuration in appsettings.json
builder.Host.ConfigureLogging(loggingBuilder =>
    loggingBuilder.AddSeq(seqJson_));

var app = builder.Build();


//配置 Schedule 任務
app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<DemoSchedule>().EveryMinute().PreventOverlapping("DemoScheduleLock").RunOnceAtStart();
});


// 顯示目前的 Seq 基本設定
string ServerUrl_ = builder.Configuration.GetValue<string>("Seq:ServerUrl");
app.Logger.LogInformation($"目前 Seq 的 ServerUrl {ServerUrl_}");

string ApiKey_ = builder.Configuration.GetValue<string>("Seq:ApiKey");
app.Logger.LogInformation($"目前 Seq 的 ApiKey {ApiKey_}");

var AppId_ = Environment.GetEnvironmentVariable("AP_ID");
app.Logger.LogInformation($"目前的 AppId {AppId_}");

app.Logger.LogInformation($"🐛🐛🐛🐛🐛🐛🐛🐛🐛 {convertedUUID_} 程式啟動 🐛🐛🐛🐛🐛🐛🐛🐛🐛");

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
