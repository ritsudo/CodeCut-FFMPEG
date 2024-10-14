using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCut_FFMPEG
{
    using System;
    using System.Text.RegularExpressions;

    public class TimeFragment
    {
        // Start time (required)
        public TimeSpan StartTime { get; set; }

        // End time (nullable)
        public TimeSpan? EndTime { get; set; }

        // Fragment name (required)
        public string FragmentName { get; set; }

        // Constructor
        public TimeFragment(TimeSpan startTime, string fragmentName, TimeSpan? endTime = null)
        {
            StartTime = startTime;
            EndTime = endTime;
            FragmentName = fragmentName ?? throw new ArgumentNullException(nameof(fragmentName));
        }

        // Method to display information about the time fragment
        public void DisplayInfo()
        {
            Console.WriteLine($"Fragment: {FragmentName}");
            Console.WriteLine($"Start Time: {StartTime}");

            if (EndTime.HasValue)
            {
                Console.WriteLine($"End Time: {EndTime}");
            }
            else
            {
                Console.WriteLine("End Time: Not specified");
            }
        }
        public static string SanitizeFileName(string fileName)
        {
            // Define invalid characters for file names
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            // Create a regular expression that matches invalid characters
            string regexPattern = $"[{Regex.Escape(invalidChars)}]";

            // Replace invalid characters with an underscore or remove them
            return Regex.Replace(fileName, regexPattern, "");
        }


        public string GetFFMpegCommandLine(string mp4FileName)
        {
            string command = "ffmpeg -i \"" + mp4FileName + "\" ";

            command += "-ss " + StartTime.ToString();

            if (!string.IsNullOrEmpty(EndTime?.ToString()))
            {
                command += " -to " + EndTime.ToString();
            }

            command += " -q:a 0 -map 0:v -map 0:a \"" + SanitizeFileName(FragmentName) + ".mp4\"";

            return command;
        }
    }

}
