using A2UI.Sample.BlazorServer.Components;
using A2UI.Sample.BlazorServer.Services;
using A2UI.Core.Processing;
using A2UI.Theming;
using A2UI.Blazor.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add A2UI services - Use Scoped for per-circuit (per-user-session) state in Blazor Server
builder.Services.AddScoped<MessageProcessor>();
builder.Services.AddScoped<DataBindingResolver>(sp => 
    new DataBindingResolver(sp.GetRequiredService<MessageProcessor>()));
builder.Services.AddScoped<EventDispatcher>();
builder.Services.AddSingleton<ThemeService>(); // Theme service can be singleton as it's read-only
builder.Services.AddSingleton<MarkdownRenderer>(); // Markdown renderer is stateless and can be singleton

// Add A2A services (Agent and Client)
builder.Services.AddScoped<MockA2AAgent>();
builder.Services.AddScoped<A2AClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
