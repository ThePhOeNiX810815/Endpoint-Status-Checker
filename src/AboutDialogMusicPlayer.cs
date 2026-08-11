using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EndpointChecker
{
    /// <summary>
    /// Plays one of the About screen's two MP3s via the legacy MCI API (winmm.dll — no extra
    /// dependency needed for MP3 playback). Lives independently of any AboutDialog instance so
    /// playback can keep going after the dialog that started it is closed.
    /// </summary>
    internal static class AboutDialogMusicPlayer
    {
        private const string Alias = "aboutDialogMusic";

        private static readonly string[] TrackFileNames =
        {
            "Endpoint Checker.mp3",
            "Endpoint Checker2.mp3"
        };

        private static readonly object SyncRoot = new object();
        private static readonly Random Rng = new Random();
        private static string lastPlayedFileName;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr callback);

        public static bool IsPlaying
        {
            get
            {
                lock (SyncRoot)
                {
                    return QueryIsPlaying();
                }
            }
        }

        /// <summary>
        /// Starts a random track only if nothing is currently playing — never overlaps two tracks.
        /// </summary>
        public static void EnsureStarted()
        {
            lock (SyncRoot)
            {
                if (QueryIsPlaying())
                {
                    return;
                }

                string fileName = PickNextTrack();
                string path = Path.Combine(AppContext.BaseDirectory, "media", fileName);
                if (!File.Exists(path))
                {
                    return;
                }

                mciSendString("close " + Alias, null, 0, IntPtr.Zero);

                long openResult = mciSendString(
                    "open \"" + path + "\" type mpegvideo alias " + Alias,
                    null,
                    0,
                    IntPtr.Zero);

                if (openResult == 0)
                {
                    mciSendString("play " + Alias, null, 0, IntPtr.Zero);
                    lastPlayedFileName = fileName;
                }
            }
        }

        public static void Stop()
        {
            lock (SyncRoot)
            {
                mciSendString("stop " + Alias, null, 0, IntPtr.Zero);
                mciSendString("close " + Alias, null, 0, IntPtr.Zero);
            }
        }

        // Picks uniformly among the tracks other than whichever played last, so back-to-back
        // opens of the About screen don't have a 50/50 chance of repeating the same one.
        private static string PickNextTrack()
        {
            string[] candidates = Array.FindAll(TrackFileNames, name => name != lastPlayedFileName);
            if (candidates.Length == 0)
            {
                candidates = TrackFileNames;
            }

            return candidates[Rng.Next(candidates.Length)];
        }

        private static bool QueryIsPlaying()
        {
            StringBuilder status = new StringBuilder(128);
            mciSendString("status " + Alias + " mode", status, status.Capacity, IntPtr.Zero);
            return status.ToString().Trim().Equals("playing", StringComparison.OrdinalIgnoreCase);
        }
    }
}
