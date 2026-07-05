using System.Diagnostics;
using System.Net.Http;
using Microsoft.Playwright;
using Xunit;

namespace VetAmb.Tests;

public sealed class VetAmbEndToEndTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;
    private Process? _appProcess;
    private string _baseUrl = string.Empty;

    private static string ProjectDirectory =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "VetAmb"));

    public async Task InitializeAsync()
    {
        _baseUrl = await StartAppAsync();

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });

        _page = await _context.NewPageAsync();
        _page.SetDefaultTimeout(30000);
    }

    public async Task DisposeAsync()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();

        if (_appProcess is not null)
        {
            try
            {
                if (!_appProcess.HasExited)
                {
                    _appProcess.Kill(entireProcessTree: true);
                    _appProcess.WaitForExit(5000);
                }
            }
            finally
            {
                _appProcess.Dispose();
            }
        }
    }

    [Fact]
    public async Task TenStepWorkflow_ShouldCompleteWithResilientTestIdsOnly()
    {
        await EnsureLoggedInAsAdminAsync();

        // Step 1: Landing on the dashboard.
        await _page.GotoAsync($"{_baseUrl}/");
        await _page.GetByTestId("dashboard-title").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        // Step 2: Searching for a patient using global search bar.
        await _page.GetByTestId("global-search-input").FillAsync("Rex");
        await _page.GetByTestId("global-search-dropdown").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        // Step 3: Clicking the patient from dropdown and navigating to details.
        var patientResult = _page.Locator("[data-testid^='global-search-result-pacijent-']").First;
        await patientResult.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
        await patientResult.ClickAsync();

        // Step 4: Viewing medical records on patient details page.
        await _page.GetByTestId("patient-medical-records-table").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        // Step 5: Navigating to appointments page.
        await _page.GetByTestId("nav-appointments").ClickAsync();
        await _page.WaitForURLAsync("**/appointments");

        // Step 6: Ensuring appointments list is rendered.
        var appointmentCards = _page.Locator("[data-testid^='appointment-card-']");
        var appointmentCount = await appointmentCards.CountAsync();
        Assert.True(appointmentCount > 0, "Expected at least one appointment card on appointments page.");

        // Step 7: Navigating to vets list.
        await _page.GetByTestId("nav-vets").ClickAsync();
        await _page.WaitForURLAsync("**/vets");

        // Step 8: Filtering/searching vets for a specific veterinarian.
        await _page.GetByTestId("vets-filter-input").FillAsync("Novak");

        var visibleVetCards = _page.Locator("[data-testid^='vet-card-']:visible");
        var visibleCount = await visibleVetCards.CountAsync();
        Assert.True(visibleCount > 0, "Expected at least one visible vet card after filtering.");

        for (var i = 0; i < visibleCount; i++)
        {
            var text = await visibleVetCards.Nth(i).InnerTextAsync();
            Assert.Contains("Novak", text, StringComparison.OrdinalIgnoreCase);
        }
    }

    private async Task EnsureLoggedInAsAdminAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/Identity/Account/Login");

        await _page.GetByTestId("login-email").FillAsync("admin@vetamb.com");
        await _page.GetByTestId("login-password").FillAsync("AdminPassword123!");
        await _page.GetByTestId("login-submit").ClickAsync();

        await _page.WaitForURLAsync("**/");
        await _page.GetByTestId("dashboard-title").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
    }

    private async Task<string> StartAppAsync()
    {
        var port = GetAvailablePort();
        var baseUrl = $"http://127.0.0.1:{port}";
        var projectFile = Path.Combine(ProjectDirectory, "VetAmb.csproj");

        _appProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectFile}\" --no-build --urls \"{baseUrl}\"",
                WorkingDirectory = ProjectDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        var startupOutput = new StringWriter();
        _appProcess.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                startupOutput.WriteLine(args.Data);
            }
        };

        _appProcess.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                startupOutput.WriteLine(args.Data);
            }
        };

        if (!_appProcess.Start())
        {
            throw new InvalidOperationException("Failed to start VetAmb for end-to-end testing.");
        }

        _appProcess.BeginOutputReadLine();
        _appProcess.BeginErrorReadLine();

        using var httpClient = new HttpClient();
        var deadline = DateTimeOffset.UtcNow.AddSeconds(90);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (_appProcess.HasExited)
            {
                throw new InvalidOperationException($"VetAmb exited before becoming ready.\n{startupOutput}");
            }

            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/Identity/Account/Login");
                if (response.IsSuccessStatusCode || ((int)response.StatusCode >= 300 && (int)response.StatusCode < 500))
                {
                    return baseUrl;
                }
            }
            catch (HttpRequestException)
            {
                // Not ready yet; keep polling.
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"VetAmb did not become ready in time.\n{startupOutput}");
    }

    private static int GetAvailablePort()
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

}
