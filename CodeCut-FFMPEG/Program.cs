using CodeCut_FFMPEG;
using System.Diagnostics;

static string FindFirstFileWithExtension(string folderPath, string extension)
{
    // Search for the files with the specified extension
    string[] files = Directory.GetFiles(folderPath, $"*.{extension}");

    // Return the first file name found, or null if no files are found
    return files.Length > 0 ? Path.GetFileName(files[0]) : null;
}

static void ExitIfFileNotFound(string mp4FileName, string txtFileName)
{
    // Print the results for .mp4 file
    if (!string.IsNullOrEmpty(mp4FileName))
    {
        Console.WriteLine("MP4 file found: " + mp4FileName);
        if (!string.IsNullOrEmpty(txtFileName))
        {
            Console.WriteLine("TXT file found: " + txtFileName);
        }
        else
        {
            Console.WriteLine("No TXT files found in the application folder. Any key to exit.");
            Console.ReadLine();
            Environment.Exit(1); // Exit the program if no MP4 file is found
        }
    }
    else
    {
        Console.WriteLine("No MP4 files found in the application folder. Any key to exit.");
        Console.ReadLine();
        Environment.Exit(1); // Exit the program if no MP4 file is found
    }
}
// Get the application folder (current directory)
string appFolder = AppDomain.CurrentDomain.BaseDirectory;

// Search for .mp4 and .txt files
string mp4FileName = FindFirstFileWithExtension(appFolder, "mp4");
string txtFileName = FindFirstFileWithExtension(appFolder, "txt");
ExitIfFileNotFound(mp4FileName, txtFileName);

//LOAD COLLECTION OF TIMEFRAGMENTS FROM TEXT FILE

List<TimeFragment> timeFragments = new List<TimeFragment>();
string[] lines = File.ReadAllLines(txtFileName);

// Load values to collection
for (int i = 0; i < lines.Length; i++)
{
    try
    {
        string line = lines[i].Trim();
        if (string.IsNullOrEmpty(line)) continue; // Skip empty lines

        // Split the line by space to get the time and the song name
        string[] parts = line.Split(new[] { "   " }, StringSplitOptions.None);
        TimeSpan startTime = TimeSpan.Parse(parts[0]);
        string songName = parts[1];

        // Determine the end time for the current song (if it's not the last one)
        TimeSpan? endTime = (i < lines.Length - 1) ? TimeSpan.Parse(lines[i + 1].Split(new[] { "   " }, StringSplitOptions.None)[0]) : (TimeSpan?)null;

        // Create a new SongFragment and add it to the list
        TimeFragment fragment = new TimeFragment(startTime, songName, endTime);
        timeFragments.Add(fragment);

    }

    catch (Exception ex)
    {
        // Log the exception message and exit the program
        Console.WriteLine($"Error processing line {i + 1}: {ex.Message}. Press any key to exit");
        Console.ReadLine();
        Environment.Exit(1); // Exit the program if no MP4 file is found
    }
}

// Print all values located
foreach (var fragment in timeFragments)
{
    string command = fragment.GetFFMpegCommandLine(mp4FileName);
    Console.WriteLine(command);

    // Execute the command
    ExecuteCommand(command);
}

void ExecuteCommand(string command)
{

    // Split the command and its arguments
    var commandParts = command.Split(' ', 2);
    string processName = commandParts[0];  // This is usually "ffmpeg" or similar
    string arguments = commandParts.Length > 1 ? commandParts[1] : string.Empty;

    // Start the process and wait for it to exit
    Process process = new Process();
    process.StartInfo.FileName = processName;
    process.StartInfo.Arguments = arguments;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;  // Prevent creating a window for the process

    process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
    process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    // Wait for the process to complete before starting the next one
    process.WaitForExit();
}

//EXECUTE FFMPEG COMMANDS