using Evaluacion.API.Store.Contratos.Repositorio;
using Evaluacion.API.Store.Implementacion.Repositorio;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
        services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();  
    })

    .Build();

host.Run();


