using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.IntegrationTests.TestInfrastructure.MockedHttpClients;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.IntegrationTests.TestInfrastructure;

public class TestServer : IAsyncLifetime
{
    public HttpClient Client = null!;
    public PaymentsRepository PaymentsRepository = null!;
    
    private WebApplicationFactory<Program> _webApplicationFactory = null!;

    public async Task InitializeAsync()
    {
        PaymentsRepository = new PaymentsRepository();

        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IAcquiringBankHttpClient>();
                    services.AddSingleton<IAcquiringBankHttpClient, MockAcquiringBankHttpClient>();

                    services.RemoveAll<IPaymentsRepository>();
                    services.AddSingleton(PaymentsRepository);
                    services.AddSingleton<IPaymentsRepository>(sp => sp.GetRequiredService<PaymentsRepository>());

                    services.RemoveAll<IPaymentsGatewayService>();
                    services.AddScoped<IPaymentsGatewayService, PaymentsGatewayService>();
                });
            });

        Client = _webApplicationFactory.CreateClient();
        await Task.CompletedTask;
    }
    
    public async Task DisposeAsync()
    {
        _webApplicationFactory?.Dispose();
        Client?.Dispose();
        await Task.CompletedTask;
    }
}