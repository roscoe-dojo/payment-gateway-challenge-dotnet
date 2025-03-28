using FluentValidation;

using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Validators;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentsGatewayService, PaymentsGatewayService>();

builder.Services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

builder.Services.AddHttpClient<IAcquiringBankHttpClient, AcquiringBankHttpClient>(cilent =>
{
    cilent.BaseAddress = builder.Configuration.GetSection("AcquiringBank:BaseUrl").Get<Uri>();
});

var app = builder.Build();

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

public partial class Program { }