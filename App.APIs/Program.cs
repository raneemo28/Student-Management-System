using App.Application.Interfaces;
using App.Application.Repositories;
using App.Application.Services;
using App.Application.Validators;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Repositories;
using App.infra.Services;
using App.infra.Strategies;
using App.infra.FileStorage;
using App.infra.Caching;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching(options =>
{
    options.SizeLimit = 100 * 1024 * 1024;
});

if (builder.Configuration.GetValue<bool>("Caching:UseDistributedCache"))
{
    var redisConfig = builder.Configuration.GetSection("Redis");
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfig["ConnectionString"] ?? "localhost:6379";
        options.InstanceName = redisConfig["InstanceName"] ?? "studentmgmt:";
    });
    builder.Services.AddSingleton<ICacheInvalidationService, RedisCacheInvalidationService>();
}
else
{
    builder.Services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();
}

builder.Services.AddScoped<ICacheService, CacheService>();

    // Register Business Database Context (Write)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BusinessDbConnection")));

// Register Identity Database Context
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDbConnection")));

// Register Read Database Context
builder.Services.AddDbContext<AppReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReadDbConnection")));

// Add Identity Services using AuthDbContext
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Read Repositories
builder.Services.AddScoped<IStudentReadRepository, StudentReadRepository>();
builder.Services.AddScoped<ICourseReadRepository, CourseReadRepository>();
builder.Services.AddScoped<IEnrollmentReadRepository, EnrollmentReadRepository>();
builder.Services.AddScoped<IInstructorReadRepository, InstructorReadRepository>();
builder.Services.AddScoped<ICourseContentReadRepository, CourseContentReadRepository>();
builder.Services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();
builder.Services.AddScoped<IAdvertisementReadRepository, AdvertisementReadRepository>();
builder.Services.AddScoped<IHomeworkReadRepository, HomeworkReadRepository>();
builder.Services.AddScoped<IHomeworkSubmissionReadRepository, HomeworkSubmissionReadRepository>();
builder.Services.AddScoped<IHomeworkSolutionReadRepository, HomeworkSolutionReadRepository>();
builder.Services.AddScoped<IHomeworkQuestionMarkReadRepository, HomeworkQuestionMarkReadRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.Students.Commands.CreateStudentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.Instructors.Commands.CreateInstructorCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.CourseContents.Commands.CreateCourseContentCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.Employees.Commands.CreateEmployeeCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.Advertisements.Commands.CreateAdvertisementCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.Homeworks.Commands.CreateHomeworkCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.HomeworkSubmissions.Commands.SubmitHomeworkCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.HomeworkSolutions.Commands.CreateHomeworkSolutionCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(App.Application.CQRS.HomeworkQuestionMarks.Commands.CreateHomeworkQuestionMarkCommand).Assembly);
});

// Register Strategies
builder.Services.AddScoped<StudentCreatedWriteStrategy>();
builder.Services.AddScoped<StudentUpdatedWriteStrategy>();
builder.Services.AddScoped<StudentDeletedWriteStrategy>();
builder.Services.AddScoped<CourseCreatedWriteStrategy>();
builder.Services.AddScoped<CourseUpdatedWriteStrategy>();
builder.Services.AddScoped<CourseDeletedWriteStrategy>();
builder.Services.AddScoped<EnrollmentCreatedWriteStrategy>();
builder.Services.AddScoped<EnrollmentDeletedWriteStrategy>();
builder.Services.AddScoped<InstructorCreatedWriteStrategy>();
builder.Services.AddScoped<InstructorUpdatedWriteStrategy>();
builder.Services.AddScoped<InstructorDeletedWriteStrategy>();
builder.Services.AddScoped<CourseContentCreatedWriteStrategy>();
builder.Services.AddScoped<CourseContentUpdatedWriteStrategy>();
builder.Services.AddScoped<CourseContentDeletedWriteStrategy>();
builder.Services.AddScoped<EmployeeCreatedWriteStrategy>();
builder.Services.AddScoped<EmployeeUpdatedWriteStrategy>();
builder.Services.AddScoped<EmployeeDeletedWriteStrategy>();
builder.Services.AddScoped<AdvertisementCreatedWriteStrategy>();
builder.Services.AddScoped<AdvertisementUpdatedWriteStrategy>();
builder.Services.AddScoped<AdvertisementDeletedWriteStrategy>();
builder.Services.AddScoped<HomeworkCreatedWriteStrategy>();
builder.Services.AddScoped<HomeworkUpdatedWriteStrategy>();
builder.Services.AddScoped<HomeworkDeletedWriteStrategy>();
builder.Services.AddScoped<HomeworkSubmissionCreatedWriteStrategy>();
builder.Services.AddScoped<HomeworkSubmissionDeletedWriteStrategy>();
builder.Services.AddScoped<HomeworkSolutionCreatedWriteStrategy>();
builder.Services.AddScoped<HomeworkSolutionDeletedWriteStrategy>();
builder.Services.AddScoped<HomeworkQuestionMarkCreatedWriteStrategy>();
builder.Services.AddScoped<HomeworkQuestionMarkDeletedWriteStrategy>();
builder.Services.AddScoped<DomainEventWriteStrategyFactory>();

// Register Background Service
builder.Services.AddHostedService<DomainEventProcessorService>();

// Register Services
builder.Services.AddScoped<IAuthService, App.infra.Services.AuthService>();
builder.Services.AddScoped<IStudentService, App.Application.Services.StudentService>();
builder.Services.AddScoped<ICourseService, App.Application.Services.CourseService>();
builder.Services.AddScoped<ICourseStudentService, App.Application.Services.CourseStudentService>();
builder.Services.AddScoped<IInstructorService, App.Application.Services.InstructorService>();
builder.Services.AddScoped<ICourseContentService, App.Application.Services.CourseContentService>();
builder.Services.AddScoped<IEmployeeService, App.Application.Services.EmployeeService>();
builder.Services.AddScoped<IAdvertisementService, App.Application.Services.AdvertisementService>();
builder.Services.AddScoped<IHomeworkService, App.Application.Services.HomeworkService>();
builder.Services.AddScoped<IHomeworkSubmissionService, App.Application.Services.HomeworkSubmissionService>();
builder.Services.AddScoped<IHomeworkSolutionService, App.Application.Services.HomeworkSolutionService>();
builder.Services.AddScoped<IHomeworkQuestionMarkService, App.Application.Services.HomeworkQuestionMarkService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<ContentTypeHandlerFactory>();
builder.Services.AddScoped<IContentTypeHandler, PdfContentTypeHandler>();
builder.Services.AddScoped<IContentTypeHandler, ImageContentTypeHandler>();
builder.Services.AddScoped<IContentTypeHandler, VideoContentTypeHandler>();

// Register Log Publisher
builder.Services.AddSingleton<ILogPublisher, RabbitMqLogPublisher>();

// Register FluentValidation Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.MapControllers();

app.Run();
