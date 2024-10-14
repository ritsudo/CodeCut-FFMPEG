using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCut_FFMPEG
{
    using System;

    public class TimeFragment
    {
        // Start time (required)
        public DateTime StartTime { get; set; }

        // End time (nullable)
        public DateTime? EndTime { get; set; }

        // Fragment name (required)
        public string FragmentName { get; set; }

        // Constructor
        public TimeFragment(DateTime startTime, string fragmentName, DateTime? endTime = null)
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
    }

}
