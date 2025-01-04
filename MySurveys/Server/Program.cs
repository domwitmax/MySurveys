using Microsoft.AspNetCore.ResponseCompression;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MySurveys.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HttpJsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
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

app.MapGet("api/survey/{id:int}",async ([FromRoute] int id) =>
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
            new HeaderQuestion("Pytanie z klikni�ciem obrazka",10),
            new HeaderQuestion("Pytanie z uk�adaniem opcji",11),
            new HeaderQuestion("Pytanie z ocen� w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
    };
    IEnumerable<string> choices = new string[] { "wyb�r 1", "wyb�r 2", "wyb�r 3", "wyb�r 4" };
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
app.MapPost("api/survey", async ([FromBody] SurveyAnswer answer) =>
{
    return Results.Ok(answer.Id + " " + string.Join(" | ", answer.Answers));
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
