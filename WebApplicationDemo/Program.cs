using Coravel;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using WebApplicationDemo.Models.AppSettings.CacheSettings;
using WebApplicationDemo.Models.AppSettings.RedisSettings;
using WebApplicationDemo.Models.AppSettings.SqlSettings;
using WebApplicationDemo.Schedules;
using WebApplicationDemo.Services.Common;

Console.OutputEncoding = Encoding.Unicode;

// 指定短版的 Guid
int len_ = 12;//指定 Guid 的長度
string convertedUUID_ = Guid.NewGuid().ToString().Substring(0, len_);

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, logger) => {
    logger
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region 處理 appsettings.json
// 處理 appsettings.json 要傳送的 MS-SQL 設定
builder.Services.AddSingleton<ISqlSettings, MsSqlSettings>();
MsSqlSettings MsSqlSettings_ = builder.Configuration.GetSection("MSSQL").Get<MsSqlSettings>();
builder.Services.AddSingleton<ISqlSettings>(MsSqlSettings_);

// 處理 appsettings.json 要傳送的 Redis 設定
builder.Services.AddSingleton<ICacheSettings, RedisSettings>();
RedisSettings RedisSettings_ = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
builder.Services.AddSingleton<ICacheSettings>(RedisSettings_);
# endregion 處理 appsettings.json

#region 處理 cache 的連線
// 處理 cache (redis) 的連線
builder.Services.AddSingleton<ICacheService, CacheService>();
#endregion 處理 cache 的連線


# region AddScheduler
// 註冊 Coravel 的 Scheduler
builder.Services.AddScheduler();

builder.Services.AddTransient<DemoSchedule>();
builder.Services.AddTransient<PreventOverlappingSchedule>();
builder.Services.AddTransient<CronSchedule>();
#endregion AddScheduler


// 取的 AssemblyVersion 與 FileVersion
var AssemblyVersion_ = Assembly.GetEntryAssembly()?.GetName().Version;
var FileVersion_ = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
builder.Services.AddSwaggerGen(options =>
{
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

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


# region 配置 Schedule 任務
//配置 Schedule 任務
app.Services.UseScheduler(scheduler =>
{
    // ! 呼叫 DemoSchedule 類別，每分鐘執行一次，並且在啟動時執行一次
    //scheduler.Schedule<DemoSchedule>().EverySeconds(10).PreventOverlapping("DemoScheduleLock").RunOnceAtStart();

    // ! 呼叫 PreventOverlappingSchedule 類別，每10秒執行一次，並且在啟動時執行一次，由於 PreventOverlapping 會防止重複執行，所以 20 秒那次會取消，30秒後才會執行下一次
    //scheduler.Schedule<PreventOverlappingSchedule>().EverySeconds(10).PreventOverlapping("PreventOverlappingScheduleLock").RunOnceAtStart();

    // ! At minute 0,20, and 40.
    scheduler.Schedule<CronSchedule>().Cron("0,20,40 * * * *").Zoned(TimeZoneInfo.Local);
});
#endregion 配置 Schedule 任務

// 顯示目前的 Seq 基本設定
string ServerUrl_ = builder.Configuration.GetValue<string>("Seq:ServerUrl");
app.Logger.LogInformation($"目前 Seq 的 ServerUrl {ServerUrl_}");

string ApiKey_ = builder.Configuration.GetValue<string>("Seq:ApiKey");
app.Logger.LogInformation($"目前 Seq 的 ApiKey {ApiKey_}");

var AppId_ = Environment.GetEnvironmentVariable("AP_ID");
app.Logger.LogInformation($"目前的 AppId 【{AppId_}】");

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
