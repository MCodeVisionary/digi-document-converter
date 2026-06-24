using DigiDocumentConverter.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConversionFactory, ConversionFactory>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

public partial class Program { }
