using EduTrailblaze.Entities;
using EduTrailblaze.Repositories;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Mappings;
using EduTrailblaze.Services.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Polly;
using SendGrid;
using Shared.Configurations;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using Infrastructure.Extensions;
using EventBus.Messages.Interfaces;
using EduTrailblaze.Services.Consumer;
using static Org.BouncyCastle.Math.EC.ECCurve;
using EventBus.Messages.Events;

namespace EduTrailblaze.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusSetting = configuration.GetSection(nameof(EventBusSetting)).Get<EventBusSetting>();
            services.AddSingleton(eventBusSetting);
            return services;
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var setting = services.GetOptions<EventBusSetting>("EventBusSetting");
            if (setting == null || string.IsNullOrEmpty(setting.HostAddress))
            {
                throw new ArgumentNullException("EventBus is not configuration");
            }

            var mqConnection = new Uri(setting.HostAddress);
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetCourseConsumer>(); x.AddConsumersFromNamespaceContaining<GetCourseConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(mqConnection);
                    cfg.ReceiveEndpoint("course-service-queue", e =>
                    {
                        e.ConfigureConsumer<GetCourseConsumer>(context);
                    }); cfg.ReceiveEndpoint("review-service-queue", e =>
                    {
                        e.ConfigureConsumer<ReviewConsumer>(context);
                    });

                    //cfg.ReceiveEndpoint("course-queue", e =>
                    //{
                    //    e.Bind<GetCourseRequest>();
                    //    e.ConfigureConsumer<GetCourseConsumer>(context);
                    //});
                    //cfg.ConfigureEndpoints(context);
                });
               // x.AddRequestClient<GetCourseRequest>();
            });
            //services.AddMassTransitHostedService();
        }
        public static IServiceCollection AddInfrastructor(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            // Add DbContext
            services.AddDbContextPool<EduTrailblazeDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOption =>
                sqlOption.EnableRetryOnFailure());
            });

            // Identity Configuration
            services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Lockout.MaxFailedAccessAttempts = 2;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireDigit = false;
                option.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                //    options.SignIn.RequireConfirmedEmail = true;
                //    options.User.RequireUniqueEmail = true;
            }
            ).AddEntityFrameworkStores<EduTrailblazeDbContext>().AddDefaultTokenProviders();

            //Application Insight
            services.AddApplicationInsightsTelemetry();
            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' [space] and then your token"
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
                                   new string[] { }
                               }
                           });
            });

            // JWT Configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration["JwtToken:Issuer"],
                            ValidAudience = configuration["JwtToken:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtToken:Key"]))
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    context.HttpContext.Request.Path.StartsWithSegments("/notifications-hub"))
                                {
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    })
                    .AddGoogle(options =>
                    {
                        options.ClientId = configuration["Google:ClientId"];
                        options.ClientSecret = configuration["Google:ClientSecret"];
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/account/google-login";
                        options.LogoutPath = "/account/logout";
                    })
                    .AddFacebook(authenticationScheme =>
                    {
                        authenticationScheme.ClientSecret = configuration["Facebook:ClientSecret"];
                        authenticationScheme.ClientId = configuration["Facebook:ClientId"];
                    });
            //Author
            //services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = null;
            //});


            // Add Caching for response Middleware 
            services.AddResponseCaching();

            // Add FluentValidation and auto validation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<GetCoursesRequestValidator>();

            // Add endpoints
            services.AddEndpointsApiExplorer();

            // Add AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Add services to Dependency Container
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IAIService, AIService>();
            services.AddHttpClient<IAIService, AIService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(60);
            });
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IClamAVService, ClamAVService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICourseCouponService, CourseCouponService>();
            services.AddScoped<ICourseDiscountService, CourseDiscountService>();
            services.AddScoped<ICourseInstructorService, CourseInstructorService>();
            services.AddScoped<ICourseLanguageService, CourseLanguageService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseClassService, CourseClassService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICourseTagService, CourseTagService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILectureService, LectureService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuizAnswerService, QuizAnswerService>();
            services.AddScoped<IQuizHistoryService, QuizHistoryService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IRedisLock, RedisLock>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserCertificateService, UserCertificateService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserProgressService, UserProgressService>();
            services.AddScoped<IUserCourseCouponService, UserCourseCouponService>();
            services.AddScoped<IUserTagService, UserTagService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IVNPAYService, VNPAYService>();
            services.AddScoped<IMoMoService, MoMoService>();
            services.AddTransient<TokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ISendMail, SendMail>();
            services.AddScoped<IPayPalService, PayPalService>();
            services.AddScoped<IInstructorDashboardService, InstructorDashboardService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddHttpClient<IPdfService, PdfService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(600);
            });

            services.AddHttpClient<ICurrencyExchangeService, CurrencyExchangeService>();

            services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri(configuration["Elastic:Uri"]))
                .DefaultIndex("courses").BasicAuthentication(configuration["Elastic:Username"], configuration["Elastic:Password"]);

                //var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("courses");

                var retryPolicy = Polly.Policy
                    .Handle<Exception>() // Handle any exception, or you can be more specific

                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),

                        (exception, timeSpan, retryCount, context) =>
                        {
                            Console.WriteLine($"Attempt {retryCount} to connect to Elasticsearch failed. Error: {exception.Message}");
                        });

                IElasticClient client = null;
                try
                {
                    retryPolicy.ExecuteAsync(async () =>
                    {
                        client = new ElasticClient(settings);
                        var pingResponse = await client.PingAsync();
                        if (!pingResponse.IsValid)
                        {
                            throw new Exception("Elasticsearch ping failed.");
                        }
                    }).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Elasticsearch connection failed after retries: {ex.Message}");
                }

                return client ?? new ElasticClient(settings);
            });

            services.AddSingleton<IElasticsearchService, ElasticsearchService>();

            //Sendgrid Configuration

            services.AddSingleton<ISendGridClient, SendGridClient>(provider =>
            {
                var apiKey = configuration["SendGrid:ApiKey"];
                return new SendGridClient(apiKey);
            });

            //Email Configuration
            services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));

            //Prevent CSRF
            services.AddAntiforgery(options =>
            {

                options.Cookie.Name = "AntiForgeryCookie";
                options.HeaderName = "X-XSRF-TOKEN";
            });


            //redis Configuration
            services.Configure<RedisConfig>(configuration.GetSection("RedisConfig"));
            var redisConfigurationSection = services.BuildServiceProvider().GetRequiredService<IOptions<RedisConfig>>().Value;

            var redisConfiguration = new ConfigurationOptions
            {
                EndPoints = { $"{redisConfigurationSection.Host}:{redisConfigurationSection.Port}" },
                Password = redisConfigurationSection.Password,
                Ssl = bool.Parse(redisConfigurationSection.Ssl),
                AbortOnConnectFail = bool.Parse(redisConfigurationSection.AbortOnConnectFail),
                ConnectRetry = 5,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                KeepAlive = 180

            };
            services.AddSingleton(redisConfiguration);
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = sp.GetRequiredService<ConfigurationOptions>();
                return ConnectionMultiplexer.Connect(configuration);
            });

            // Add CORS
            services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigin",
                        policy =>
                        {
                            policy.WithOrigins(
                                configuration["FE:Url"],
                                configuration["FE:Url_Http"],
                                configuration["BE:Local"],
                                 configuration["BE:Publish"]
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                        });
                });

            services.AddSignalR();

            return services;
        }


    }

}

