using SampleAppWithCaching;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration); // My custom startup class.

startup.ConfigureServices(builder.Services); // Add services to the container.

var app = builder.Build();

startup.Configure(app, app.Environment); // Configure the HTTP request pipeline.

app.Run();

// Add services to the container.

//builder.Services.AddControllers();

//var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
