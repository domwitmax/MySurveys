using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using MySurveys.Server.Data;
using MySurveys.Server.Services;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Questions;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Server.Repositores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<MySurveysDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<MySurveysDbContext>();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
    };
});

builder.Services.Configure<HttpJsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapGet("api/survey/{id:int}",([FromRoute] int id) =>
{
    Survey survey = new Survey();
    survey.Headers = new HeaderQuestion[]
    {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>"""),
            new HeaderQuestion("Pytanie wielowierszowe",2),
            new HeaderQuestion("Pytanie jednego wyboru",3),
            new HeaderQuestion("Pytanie wielokrotnego wyboru",4),
            new HeaderQuestion("Pytanie z formatowaniem tekstu",5),
            new HeaderQuestion("Pytanie z listy rozwijanej",6),
            new HeaderQuestion("Pytanie z listy",7),
            new HeaderQuestion("Pytanie z gwiazdkami",8),
            new HeaderQuestion("Pytanie w wyborze z tabeli",9),
            new HeaderQuestion("Pytanie z klikniêciem obrazka",10),
            new HeaderQuestion("Pytanie z uk³adaniem opcji",11),
            new HeaderQuestion("Pytanie z ocen¹ w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
    };
    IEnumerable<string> choices = new string[] { "wybór 1", "wybór 2", "wybór 3", "wybór 4" };
    IEnumerable<string> icons = new string[] { "Sentiment_Satisfied", "Mood", "Sentiment_Very_Satisfied", "Mood_Bad" };
    string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
    survey.Options = new OptionQuestion[]
    {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines),
            new OptionQuestion(QuestionEnum.OneChoice, choices),
            new OptionQuestion(QuestionEnum.MultipleChoice, choices),
            new OptionQuestion(QuestionEnum.HTMLEditor),
            new OptionQuestion(QuestionEnum.DropDown, choices),
            new OptionQuestion(QuestionEnum.ListBox, choices),
            new OptionQuestion(QuestionEnum.Stars),
            new OptionQuestion(QuestionEnum.SelectOptionsInTable, choices),
            new OptionQuestion(QuestionEnum.Image, path, 600, 300),
            new OptionQuestion(QuestionEnum.OrganizingItemList, choices),
            new OptionQuestion(QuestionEnum.SelectRatingScale10),
            new OptionQuestion(QuestionEnum.SelectIcon, icons)

    };
    return Results.Json(survey);
});
app.MapPost("api/survey", ([FromBody] SurveyAnswer answer) =>
{
    return Results.Ok(answer.Id + " " + string.Join(" | ", answer.Answers));
});
app.MapFallbackToFile("index.html");

app.Run();
