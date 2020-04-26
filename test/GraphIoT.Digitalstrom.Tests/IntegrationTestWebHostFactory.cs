using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromDssMock;
using PhilipDaubmeier.DigitalstromTimeSeriesApi.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class IntegrationTestWebHostFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public MockedRequest MockedEventResponse { get; private set; } = null!;

        public async Task<IntegrationTestDbContext> InitDb()
        {
            if (!(Server.Host.Services.GetRequiredService<IDigitalstromDbContext>() is IntegrationTestDbContext dbContext) || dbContext.Database is null)
                throw new NullReferenceException("No database service available");

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            return dbContext;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                          .UseStartup<TStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var integrationTestConfig = new ConfigurationBuilder()
                .AddJsonFile("integrationtestsettings.json")
                .Build();

            builder
            .UseSolutionRelativeContentRoot("src/GraphIoT.Digitalstrom")
            .ConfigureAppConfiguration(config =>
            {
                config.AddConfiguration(integrationTestConfig);
            })
            .ConfigureServices(services =>
            {
                // Build a http mock for requests to the digitalstrom server
                services.AddSingleton(fac =>
                {
                    var mockHttp = new MockHttpMessageHandler();
                    mockHttp.AddAuthMock()
                        .AddStructureMock()
                        .AddCircuitZonesMocks(new Zone[] { 4, 32027 })
                        .AddEnergyMeteringMocks()
                        .AddSensorMocks()
                        .AddInitialAndSubscribeMocks();

                    MockedEventResponse = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                            .WithExactQueryString($"subscriptionID=10&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                            .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

                    return mockHttp;
                });

                // Build a database context using an in-memory database for testing
                var dbRoot = new InMemoryDatabaseRoot();
                void dbConfig(DbContextOptionsBuilder options)
                {
                    options.UseInMemoryDatabase("InMemoryDbIntegrationTest", dbRoot);
                    options.UseInternalServiceProvider(new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider());
                }

                // Add all digitalstrom services using the mocked in-memory db
                services.AddDigitalstromHost<IntegrationTestDbContext>(dbConfig,
                    integrationTestConfig.GetSection("DigitalstromConfig"),
                    integrationTestConfig.GetSection("TokenStoreConfig"),
                    integrationTestConfig.GetSection("Network")
                );

                // Replace the digitalstrom connection provider with a http mock for testing
                services.Remove(services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDigitalstromConnectionProvider)));
                services.AddTransient<IDigitalstromConnectionProvider, DigitalstromConnectionProvider>(fac =>
                    fac.GetRequiredService<MockHttpMessageHandler>().ToMockProvider());
            });
        }
    }
}