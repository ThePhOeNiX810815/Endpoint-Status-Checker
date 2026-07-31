using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EndpointChecker
{
    public enum CloudflareBypassMethod
    {
        Disabled     = 0,
        FlareSolverr = 1,
        Playwright   = 2
    }

    public class CloudflareBypassResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string MethodUsed { get; set; }
    }

    public static class CloudflareBypassChecker
    {
        // Playwright stores its browsers in %LOCALAPPDATA%\ms-playwright\.
        // We detect it by looking for a chromium-* subfolder there.
        public static bool IsPlaywrightAvailable()
        {
            string msPlaywrightDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ms-playwright");

            if (!Directory.Exists(msPlaywrightDir))
                return false;

            return Directory.GetDirectories(msPlaywrightDir, "chromium-*").Length > 0;
        }

        // Returns the path to the Chromium executable inside ms-playwright, or null.
        public static string FindPlaywrightChromium()
        {
            string msPlaywrightDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ms-playwright");

            foreach (string dir in Directory.GetDirectories(msPlaywrightDir, "chromium-*"))
            {
                string exe = Path.Combine(dir, "chrome-win", "chrome.exe");
                if (File.Exists(exe))
                    return exe;
            }
            return null;
        }

        public static CloudflareBypassResult Check(
            string url,
            CloudflareBypassMethod method,
            string flareSolverrBaseUrl = "http://localhost:8191")
        {
            if (method == CloudflareBypassMethod.FlareSolverr)
                return CheckViaFlareSolverr(url, flareSolverrBaseUrl);

            if (method == CloudflareBypassMethod.Playwright)
                return CheckViaPlaywright(url);

            return null;
        }

        // ── FlareSolverr ─────────────────────────────────────────────────────────

        private static CloudflareBypassResult CheckViaFlareSolverr(string url, string flareSolverrBaseUrl)
        {
            try
            {
                return Task.Run(() => CheckViaFlareSolverrAsync(url, flareSolverrBaseUrl)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return new CloudflareBypassResult
                {
                    Success = false,
                    StatusCode = 0,
                    StatusMessage = "FlareSolverr error: " + ex.Message,
                    MethodUsed = "FlareSolverr"
                };
            }
        }

        private static async Task<CloudflareBypassResult> CheckViaFlareSolverrAsync(string url, string flareSolverrBaseUrl)
        {
            string apiUrl = flareSolverrBaseUrl.TrimEnd('/') + "/v1";

            string json = JsonConvert.SerializeObject(new
            {
                cmd = "request.get",
                url,
                maxTimeout = 60000
            });

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(75) };
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await httpClient.PostAsync(apiUrl, content);
            }
            catch (Exception ex)
            {
                return new CloudflareBypassResult
                {
                    Success = false,
                    StatusCode = 0,
                    StatusMessage = "FlareSolverr not reachable: " + ex.Message,
                    MethodUsed = "FlareSolverr"
                };
            }

            string responseBody = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);

            if (result != null && result.status == "ok")
            {
                int statusCode = result.solution?.status != null ? (int)result.solution.status : 200;
                bool success = statusCode >= 200 && statusCode < 400;
                return new CloudflareBypassResult
                {
                    Success = success,
                    StatusCode = statusCode,
                    StatusMessage = success ? "OK (FlareSolverr bypass)" : "Error " + statusCode + " (FlareSolverr)",
                    MethodUsed = "FlareSolverr"
                };
            }
            else
            {
                string message = result?.message != null ? (string)result.message : "unknown error";
                return new CloudflareBypassResult
                {
                    Success = false,
                    StatusCode = 0,
                    StatusMessage = "FlareSolverr: " + message,
                    MethodUsed = "FlareSolverr"
                };
            }
        }

        // ── Playwright ───────────────────────────────────────────────────────────
        // Launches the Playwright-managed Chromium directly (no Node.js needed).
        // Chromium is invoked in headless mode with --dump-dom so we can read
        // the final HTTP status via the DevTools remote debugging port.
        // We use a lightweight approach: launch Chrome with remote-debugging,
        // send a CDP navigate command, read the response status, then kill Chrome.

        private static CloudflareBypassResult CheckViaPlaywright(string url)
        {
            try
            {
                string chromiumExe = FindPlaywrightChromium();
                if (chromiumExe == null)
                {
                    return new CloudflareBypassResult
                    {
                        Success = false,
                        StatusCode = 0,
                        StatusMessage = "Playwright Chromium not found in %LOCALAPPDATA%\\ms-playwright",
                        MethodUsed = "Playwright"
                    };
                }

                return Task.Run(() => CheckViaPlaywrightAsync(url, chromiumExe)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return new CloudflareBypassResult
                {
                    Success = false,
                    StatusCode = 0,
                    StatusMessage = "Playwright error: " + ex.Message,
                    MethodUsed = "Playwright"
                };
            }
        }

        private static async Task<CloudflareBypassResult> CheckViaPlaywrightAsync(string url, string chromiumExe)
        {
            // Pick a random free port for remote-debugging
            int debugPort = new Random().Next(19222, 19999);
            string userDataDir = Path.Combine(Path.GetTempPath(), "pw_bypass_" + Guid.NewGuid().ToString("N"));

            var psi = new ProcessStartInfo
            {
                FileName = chromiumExe,
                Arguments = string.Join(" ",
                    "--headless=new",
                    "--no-sandbox",
                    "--disable-gpu",
                    "--disable-dev-shm-usage",
                    $"--remote-debugging-port={debugPort}",
                    $"--user-data-dir=\"{userDataDir}\"",
                    "about:blank"),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            using var chrome = Process.Start(psi);
            if (chrome == null)
                return new CloudflareBypassResult { Success = false, StatusCode = 0, StatusMessage = "Playwright: Chrome failed to start", MethodUsed = "Playwright" };

            try
            {
                // Give Chrome a moment to start the DevTools server
                await Task.Delay(1500);

                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

                // Get the list of targets (pages) from DevTools
                string targetsJson = await client.GetStringAsync($"http://localhost:{debugPort}/json/list");
                dynamic targets = JsonConvert.DeserializeObject(targetsJson);
                string wsUrl = targets[0].webSocketDebuggerUrl;

                // Use CDP over WebSocket to navigate and capture the response
                using var ws = new System.Net.WebSockets.ClientWebSocket();
                await ws.ConnectAsync(new Uri((string)wsUrl), default);

                // Enable Network domain
                await SendCdpCommand(ws, 1, "Network.enable", "{}");

                // Navigate and wait for response
                int? statusCode = null;
                string finalUrl = url;

                await SendCdpCommand(ws, 2, "Page.navigate", JsonConvert.SerializeObject(new { url }));

                // Poll for a Network.responseReceived event for up to 20 seconds
                var deadline = DateTime.UtcNow.AddSeconds(20);
                var recvBuf = new byte[65536];

                while (DateTime.UtcNow < deadline && statusCode == null)
                {
                    var seg = new ArraySegment<byte>(recvBuf);
                    var recvResult = await ws.ReceiveAsync(seg, default);
                    string msg = Encoding.UTF8.GetString(recvBuf, 0, recvResult.Count);
                    dynamic evt = JsonConvert.DeserializeObject(msg);

                    if (evt?.method == "Network.responseReceived")
                    {
                        string evtUrl = (string)evt.@params?.response?.url;
                        if (evtUrl != null && evtUrl.TrimEnd('/').Equals(url.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                        {
                            statusCode = (int?)evt.@params?.response?.status;
                            finalUrl = evtUrl ?? url;
                        }
                    }
                }

                if (statusCode == null)
                {
                    return new CloudflareBypassResult
                    {
                        Success = false,
                        StatusCode = 0,
                        StatusMessage = "Playwright: no response captured within timeout",
                        MethodUsed = "Playwright"
                    };
                }

                bool success = statusCode >= 200 && statusCode < 400;
                return new CloudflareBypassResult
                {
                    Success = success,
                    StatusCode = statusCode.Value,
                    StatusMessage = success
                        ? $"OK {statusCode} (Playwright bypass)"
                        : $"Error {statusCode} (Playwright bypass)",
                    MethodUsed = "Playwright"
                };
            }
            finally
            {
                try { chrome.Kill(entireProcessTree: true); } catch { }
                try { Directory.Delete(userDataDir, true); } catch { }
            }
        }

        private static async Task SendCdpCommand(System.Net.WebSockets.ClientWebSocket ws, int id, string method, string paramsJson)
        {
            string cmd = $"{{\"id\":{id},\"method\":\"{method}\",\"params\":{paramsJson}}}";
            var bytes = Encoding.UTF8.GetBytes(cmd);
            await ws.SendAsync(new ArraySegment<byte>(bytes), System.Net.WebSockets.WebSocketMessageType.Text, true, default);
        }
    }
}
