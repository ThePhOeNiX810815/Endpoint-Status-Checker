using System;
using System.IO;
using NAudio.Wave;

namespace EndpointChecker
{
    /// <summary>
    /// Plays one of the About screen's two MP3s via NAudio. Lives independently of any
    /// AboutDialog instance so playback can keep going after the dialog that started it is
    /// closed.
    /// </summary>
    /// <remarks>
    /// This used to go through the legacy MCI API (winmm.dll, "type mpegvideo"), but that
    /// driver is a thin DirectShow wrapper that turned out to be unreliable in practice: it
    /// flatly refused to open one of the two tracks (a large embedded-art ID3 tag tripped it
    /// up) and got flakier still under repeated rapid open/close. NAudio decodes MP3 frames
    /// directly in managed code and plays through WaveOut, with no such legacy baggage.
    /// </remarks>
    internal static class AboutDialogMusicPlayer
    {
        private static readonly string[] TrackFileNames =
        {
            "Endpoint Checker.mp3",
            "Endpoint Checker2.mp3"
        };

        private static readonly object SyncRoot = new object();
        private static readonly Random Rng = new Random();
        private static string lastPlayedFileName;

        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;

        public static bool IsPlaying
        {
            get
            {
                lock (SyncRoot)
                {
                    return outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing;
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
                if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }

                string fileName = PickNextTrack();
                string path = Path.Combine(AppContext.BaseDirectory, "media", fileName);
                if (!File.Exists(path))
                {
                    return;
                }

                DisposePlayback();

                try
                {
                    audioFile = new AudioFileReader(path);
                    outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    lastPlayedFileName = fileName;
                }
                catch
                {
                    // Corrupt/unsupported file, no audio device available, etc. — stay silent
                    // rather than let a playback failure take down the About screen.
                    DisposePlayback();
                }
            }
        }

        public static void Stop()
        {
            lock (SyncRoot)
            {
                DisposePlayback();
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

        private static void DisposePlayback()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;

            audioFile?.Dispose();
            audioFile = null;
        }
    }
}
