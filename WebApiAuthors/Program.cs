using WebApiAuthors;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);



var app = builder.Build();

var loggerServices = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));


startup.Configure(app, app.Environment, loggerServices);

app.Run();

