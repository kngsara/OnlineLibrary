using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Repository;
using Shared;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OnlineLibraryContext>();
builder.Services.AddScoped < IAuthorRepository, AuthorRepository > ();
builder.Services.AddScoped < IBookRepository, BookRepository> ();
builder.Services.AddScoped < IMemberRepository, MemberRepository> ();
builder.Services.AddScoped < ILoanRepository, LoanRepository> ();
builder.Services.AddScoped < ILogRepository, LogRepository> ();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OnlineLibraryContext>();
    var memberRepository = scope.ServiceProvider.GetRequiredService<IMemberRepository>();
    var member = context.Members.FirstOrDefault(m=>m.Username=="admin");
    if (member == null)
    {

        RegisterDTO registerDTO = new RegisterDTO
        {
            Username = "admin",
            Lozinka = PasswordHash("admin")

        };
        var result = await memberRepository.Register(registerDTO);
    }

}

app.Run();

static string PasswordHash(string password)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha256.ComputeHash(bytes);

        // Convert the byte array to a hexadecimal string
        StringBuilder result = new StringBuilder();
        foreach (byte b in hash)
        {
            result.Append(b.ToString("x2"));
        }
        return result.ToString();
    }
}