using Dunder_Store.Database;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;
using Dunder_Store.Data.Repositories;
using Dunder_Store.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using System.Text;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------
            // CONFIGURAÇÃO DE SERVIÇOS
            // -------------------------------
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API E-commerce", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira 'Bearer {seu token JWT}' para autenticar."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // -------------------------------
            // CONFIGURAÇÃO DE CORS (https + vue)
            // -------------------------------
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",   // Frontend Vue
                            "https://localhost:7136"   // Backend HTTPS
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // -------------------------------
            // DATABASE
            // -------------------------------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ProdutosDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // -------------------------------
            // INJEÇÃO DE DEPENDÊNCIA
            // -------------------------------
            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<IPedidoService, PedidoService>();
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();

            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
            builder.Services.AddScoped<IPedidoProdutoRepository, PedidoProdutoRepository>();

            // -------------------------------
            // JWT
            // -------------------------------
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key");
            var jwtIssuer = jwtSection.GetValue<string>("Issuer");
            var jwtAudience = jwtSection.GetValue<string>("Audience");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // -------------------------------
            // PIPELINE DA APLICAÇÃO
            // -------------------------------
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ⚠️ CORS deve vir antes de autenticação
            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "produtos")
                ),
                RequestPath = "/produtos"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
