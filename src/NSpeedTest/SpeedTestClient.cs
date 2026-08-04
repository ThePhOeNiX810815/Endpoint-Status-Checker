using EndpointChecker;
using NSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NSpeedTest
{
    public class SpeedTestClient : ISpeedTestClient
    {
        private List<string> ConfigUrls = new List<string>()
        {
            "https://www.speedtest.net/speedtest-config.php",
            "https://c.speedtest.net/speedtest-config.php",
            "http://www.speedtest.net/speedtest-config.php"
        };
        private const int MinimumDownloadConcurrency = 12;
        private const int MinimumUploadConcurrency = 4;
        private static readonly TimeSpan DownloadWarmupDuration = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan DownloadBenchmarkDuration = TimeSpan.FromSeconds(12);
        private static readonly TimeSpan UploadWarmupDuration = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan UploadBenchmarkDuration = TimeSpan.FromSeconds(6);
        private List<string> ServersUrlsList = new List<string>()
        {
            { "https://www.speedtest.net/speedtest-servers-static.php" },
            { "https://c.speedtest.net/speedtest-servers.php" },
            { "https://c.speedtest.net/speedtest-servers-static.php" },
            { "http://www.speedtest.net/speedtest-servers-static.php" },
            { "http://c.speedtest.net/speedtest-servers.php" }
        };

        private readonly int[] downloadSizes = { 500, 750, 1000, 1500, 2000, 2500, 3000, 4000, 5000, 6000, 7000, 8000 };
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int MaxUploadSize = 12; // 2.4 MB max chunk size in 200 KB steps

        #region ISpeedTestClient

        /// <summary>
        /// Download speedtest.net settings
        /// </summary>
        /// <returns>speedtest.net settings</returns>
        public Settings GetSettings()
        {
            try
            {
                using (var client = new SpeedTestWebClient())
                {
                    return GetSettingsCore(client);
                }
            }
            catch (Exception exception) when (IsTlsFailure(exception))
            {
                using (var client = new SpeedTestWebClient(bypassTlsCertificateValidation: true))
                {
                    return GetSettingsCore(client);
                }
            }
        }

        private Settings GetSettingsCore(SpeedTestWebClient client)
        {
            Settings settings = null;
            Exception lastConfigException = null;

            foreach (string configUrl in ConfigUrls)
            {
                try
                {
                    settings = client.GetConfig<Settings>(configUrl);
                    if (settings != null)
                    {
                        break;
                    }
                }
                catch (Exception exception)
                {
                    lastConfigException = exception;
                }
            }

            if (settings == null)
            {
                throw lastConfigException ?? new InvalidOperationException("Unable to download SpeedTest settings configuration.");
            }

            ServersList serversConfig = new ServersList();

            foreach (string serverURL in ServersUrlsList)
            {
                ServersList serverConfig = new ServersList();

                try
                {
                    serverConfig = client.GetConfig<ServersList>(serverURL);
                }
                catch
                {
                }

                foreach (Server server in serverConfig.Servers)
                {
                    if (serversConfig.Servers.Where(srv =>
                            srv.Name == server.Name &&
                            srv.Country == server.Country &&
                            srv.Sponsor == server.Sponsor)
                                .Count() == 0)
                    {
                        serversConfig.Servers.Add(server);
                    }
                }
            }

            serversConfig.CalculateDistances(settings.Client.GeoCoordinate);
            settings.Servers = serversConfig.Servers.OrderBy(s => s.Distance).ToList();

            return settings;
        }

        /// <summary>
        /// Test latency (ping) to server
        /// </summary>
        /// <returns>Latency in milliseconds (ms)</returns>
        public int TestServerLatency(Server server, int retryCount = 3)
        {
            try
            {
                using (var client = new SpeedTestWebClient())
                {
                    return TestServerLatencyCore(client, server, retryCount);
                }
            }
            catch (Exception exception) when (IsTlsFailure(exception))
            {
                using (var client = new SpeedTestWebClient(bypassTlsCertificateValidation: true))
                {
                    return TestServerLatencyCore(client, server, retryCount);
                }
            }
        }

        private static int TestServerLatencyCore(SpeedTestWebClient client, Server server, int retryCount)
        {
            var latencyUri = CreateTestUrl(server, "latency.txt");
            long totalElapsedMilliseconds = 0;
            int successfulAttempts = 0;

            for (var i = 0; i < retryCount; i++)
            {
                var timer = Stopwatch.StartNew();
                string testString;
                try
                {
                    testString = client.DownloadString(latencyUri);
                }
                catch (WebException)
                {
                    timer.Stop();
                    continue;
                }

                timer.Stop();

                if (!testString.StartsWith("test=test", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Server returned incorrect test string for latency.txt");
                }

                totalElapsedMilliseconds += timer.ElapsedMilliseconds;
                successfulAttempts++;
            }

            if (successfulAttempts == 0)
            {
                throw new WebException("Server latency check failed for all retry attempts.");
            }

            return (int)(totalElapsedMilliseconds / successfulAttempts);
        }

        private static bool IsTlsFailure(Exception exception)
        {
            Exception current = exception;
            while (current != null)
            {
                if (current is AuthenticationException)
                {
                    return true;
                }

                if (current is WebException webException &&
                    (webException.Status == WebExceptionStatus.TrustFailure ||
                     webException.Status == WebExceptionStatus.SecureChannelFailure))
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(current.Message) &&
                    current.Message.IndexOf("SSL connection could not be established", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                current = current.InnerException;
            }

            return false;
        }

        /// <summary>
        /// Test download speed to server
        /// </summary>
        /// <returns>Download speed in Kbps</returns>
        public double TestDownloadSpeed(Server server, int simultaniousDownloads = 2, int retryCount = 2)
        {
            return TestDownloadSpeed(server, simultaniousDownloads, retryCount, null, null);
        }

        public double TestDownloadSpeed(Server server, int simultaniousDownloads, int retryCount, Action<double> progressCallback)
        {
            return TestDownloadSpeed(server, simultaniousDownloads, retryCount, null, null, progressCallback);
        }

        public double TestDownloadSpeed(
            Server server,
            int simultaniousDownloads,
            int retryCount,
            TimeSpan? warmupDuration,
            TimeSpan? benchmarkDuration,
            Action<double> progressCallback = null)
        {
            var testData = GenerateDownloadUrls(server, retryCount);
            int effectiveConcurrency = Math.Max(simultaniousDownloads, MinimumDownloadConcurrency);
            TimeSpan effectiveWarmupDuration = warmupDuration ?? DownloadWarmupDuration;
            TimeSpan effectiveBenchmarkDuration = benchmarkDuration ?? DownloadBenchmarkDuration;

            WarmupDownload(testData.ToArray(), Math.Max(4, effectiveConcurrency / 2), effectiveWarmupDuration);

            return MeasureDownloadSpeed(testData.ToArray(), effectiveConcurrency, effectiveBenchmarkDuration, countTransferredBytes: true, progressCallback)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Test upload speed to server
        /// </summary>
        /// <returns>Upload speed in Kbps</returns>
        public double TestUploadSpeed(Server server, int simultaniousUploads = 2, int retryCount = 2)
        {
            var testData = GenerateUploadPayloads(retryCount);
            int effectiveConcurrency = Math.Max(simultaniousUploads, MinimumUploadConcurrency);

            WarmupUpload(server, testData, Math.Max(2, effectiveConcurrency / 2), UploadWarmupDuration);

            return MeasureUploadSpeed(server, testData, effectiveConcurrency, UploadBenchmarkDuration)
                .GetAwaiter()
                .GetResult();
        }

        public double TestUploadSpeed(Server server, int simultaniousUploads, int retryCount, Action<double> progressCallback)
        {
            var testData = GenerateUploadPayloads(retryCount);
            int effectiveConcurrency = Math.Max(simultaniousUploads, MinimumUploadConcurrency);

            WarmupUpload(server, testData, Math.Max(2, effectiveConcurrency / 2), UploadWarmupDuration);

            return MeasureUploadSpeed(server, testData, effectiveConcurrency, UploadBenchmarkDuration, countTransferredBytes: true, progressCallback)
                .GetAwaiter()
                .GetResult();
        }

        #endregion

        #region Helpers

        private static HttpClient CreateHttpClient(int connectionLimit)
        {
            SocketsHttpHandler handler = new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = Math.Max(16, connectionLimit),
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                UseCookies = false,
                UseProxy = WebRequest.DefaultWebProxy != null,
                Proxy = WebRequest.DefaultWebProxy,
            };

            if (handler.Proxy != null)
            {
                handler.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            }

            HttpClient client = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };

            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Program.http_UserAgent);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");

            return client;
        }

        private static void WarmupDownload(IReadOnlyList<string> urls, int concurrencyCount, TimeSpan duration)
        {
            _ = MeasureDownloadSpeed(urls, concurrencyCount, duration, countTransferredBytes: false)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task<double> MeasureDownloadSpeed(
            IReadOnlyList<string> urls,
            int concurrencyCount,
            TimeSpan duration,
            bool countTransferredBytes,
            Action<double> progressCallback = null)
        {
            if (urls.Count == 0)
            {
                throw new InvalidOperationException("No download URLs were generated for the speed test.");
            }

            long totalBytes = 0;
            int urlIndex = -1;
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(duration);
            using HttpClient client = CreateHttpClient(concurrencyCount);
            Stopwatch stopwatch = Stopwatch.StartNew();

            Task progressReporter = Task.CompletedTask;
            if (countTransferredBytes && progressCallback != null)
            {
                progressReporter = Task.Run(async () =>
                {
                    try
                    {
                        while (!cancellationTokenSource.IsCancellationRequested)
                        {
                            await Task.Delay(250, cancellationTokenSource.Token).ConfigureAwait(false);
                            long sampledBytes = Interlocked.Read(ref totalBytes);
                            double sampledSpeed = ConvertBytesToKilobitsPerSecond(sampledBytes, stopwatch.Elapsed);
                            try
                            {
                                progressCallback(sampledSpeed);
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            }

            Task[] workers = Enumerable.Range(0, concurrencyCount)
                .Select(async _ =>
                {
                    byte[] buffer = new byte[128 * 1024];

                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        string url = urls[(Interlocked.Increment(ref urlIndex) & int.MaxValue) % urls.Count];

                        try
                        {
                            using HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token).ConfigureAwait(false);
                            response.EnsureSuccessStatusCode();

                            using var responseStream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                            while (!cancellationTokenSource.IsCancellationRequested)
                            {
                                int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token).ConfigureAwait(false);
                                if (bytesRead <= 0)
                                {
                                    break;
                                }

                                if (countTransferredBytes)
                                {
                                    Interlocked.Add(ref totalBytes, bytesRead);
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (HttpRequestException)
                        {
                        }
                    }
                })
                .ToArray();

            await Task.WhenAll(workers).ConfigureAwait(false);
            stopwatch.Stop();

            cancellationTokenSource.Cancel();
            await progressReporter.ConfigureAwait(false);

            double finalSpeed = ConvertBytesToKilobitsPerSecond(totalBytes, stopwatch.Elapsed);
            if (countTransferredBytes && progressCallback != null)
            {
                try
                {
                    progressCallback(finalSpeed);
                }
                catch
                {
                }
            }

            return finalSpeed;
        }

        private static void WarmupUpload(Server server, IReadOnlyList<UploadPayload> payloads, int concurrencyCount, TimeSpan duration)
        {
            _ = MeasureUploadSpeed(server, payloads, concurrencyCount, duration, countTransferredBytes: false)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task<double> MeasureUploadSpeed(Server server, IReadOnlyList<UploadPayload> payloads, int concurrencyCount, TimeSpan duration)
        {
            return await MeasureUploadSpeed(server, payloads, concurrencyCount, duration, countTransferredBytes: true).ConfigureAwait(false);
        }

        private static async Task<double> MeasureUploadSpeed(
            Server server,
            IReadOnlyList<UploadPayload> payloads,
            int concurrencyCount,
            TimeSpan duration,
            bool countTransferredBytes,
            Action<double> progressCallback = null)
        {
            if (payloads.Count == 0)
            {
                throw new InvalidOperationException("No upload payloads were generated for the speed test.");
            }

            long totalBytes = 0;
            int payloadIndex = -1;
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(duration);
            using HttpClient client = CreateHttpClient(concurrencyCount);
            Stopwatch stopwatch = Stopwatch.StartNew();

            Task progressReporter = Task.CompletedTask;
            if (countTransferredBytes && progressCallback != null)
            {
                progressReporter = Task.Run(async () =>
                {
                    try
                    {
                        while (!cancellationTokenSource.IsCancellationRequested)
                        {
                            await Task.Delay(250, cancellationTokenSource.Token).ConfigureAwait(false);
                            long sampledBytes = Interlocked.Read(ref totalBytes);
                            double sampledSpeed = ConvertBytesToKilobitsPerSecond(sampledBytes, stopwatch.Elapsed);
                            try
                            {
                                progressCallback(sampledSpeed);
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            }

            Task[] workers = Enumerable.Range(0, concurrencyCount)
                .Select(async _ =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        UploadPayload payload = payloads[(Interlocked.Increment(ref payloadIndex) & int.MaxValue) % payloads.Count];

                        try
                        {
                            using FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>(payload.FieldName, payload.Content)
                            });

                            using HttpResponseMessage response = await client.PostAsync(server.Url, content, cancellationTokenSource.Token).ConfigureAwait(false);
                            response.EnsureSuccessStatusCode();
                            if (countTransferredBytes)
                            {
                                Interlocked.Add(ref totalBytes, payload.ByteLength);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (HttpRequestException)
                        {
                        }
                    }
                })
                .ToArray();

            await Task.WhenAll(workers).ConfigureAwait(false);
            stopwatch.Stop();

            cancellationTokenSource.Cancel();
            await progressReporter.ConfigureAwait(false);

            double finalSpeed = ConvertBytesToKilobitsPerSecond(totalBytes, stopwatch.Elapsed);
            if (countTransferredBytes && progressCallback != null)
            {
                try
                {
                    progressCallback(finalSpeed);
                }
                catch
                {
                }
            }

            return finalSpeed;
        }

        private static double ConvertBytesToKilobitsPerSecond(long totalBytes, TimeSpan elapsed)
        {
            if (elapsed.TotalSeconds <= 0)
            {
                return 0;
            }

            return (totalBytes * 8d / 1024d) / elapsed.TotalSeconds;
        }

        private static IReadOnlyList<UploadPayload> GenerateUploadPayloads(int retryCount)
        {
            var random = new Random();
            var result = new List<UploadPayload>();

            for (var sizeCounter = 1; sizeCounter < MaxUploadSize + 1; sizeCounter++)
            {
                var size = sizeCounter * 200 * 1024;
                var builder = new StringBuilder(size);

                for (var i = 0; i < size; ++i)
                    builder.Append(Chars[random.Next(Chars.Length)]);

                string payloadContent = builder.ToString();
                int payloadLength = Encoding.UTF8.GetByteCount(payloadContent);

                for (var i = 0; i < retryCount; i++)
                {
                    result.Add(new UploadPayload(string.Format("content{0}", sizeCounter), payloadContent, payloadLength));
                }
            }

            return result;
        }

        private static string CreateTestUrl(Server server, string file)
        {
            return new Uri(new Uri(server.Url), ".").OriginalString + file;
        }

        private IEnumerable<string> GenerateDownloadUrls(Server server, int retryCount)
        {
            var downloadUriBase = CreateTestUrl(server, "random{0}x{0}.jpg?r={1}");
            foreach (var downloadSize in downloadSizes)
            {
                for (var i = 0; i < retryCount; i++)
                {
                    yield return string.Format(downloadUriBase, downloadSize, i);
                }
            }
        }

        #endregion

        private sealed class UploadPayload
        {
            public UploadPayload(string fieldName, string content, int byteLength)
            {
                FieldName = fieldName;
                Content = content;
                ByteLength = byteLength;
            }

            public string FieldName { get; }

            public string Content { get; }

            public int ByteLength { get; }
        }
    }
}
