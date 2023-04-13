using APIConsultaOperadora.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region  Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
#region Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#endregion

// Add endpoints to the app.
#region Add endpoints to the app.  

app.MapConsultaOperadora();

#endregion


app.Run();

