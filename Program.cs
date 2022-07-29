using System.CommandLine;
using System.IO.Compression;

var urlArg = new Argument<string>
    ("url", "URL");

var destinationArg = new Argument<string>
    ("destination", "Destination");

var extractOption = new Option<bool>("--extract", "Extract");

var rootCommand = new RootCommand("Downloader")
{
    urlArg,
    destinationArg,
    extractOption
};
rootCommand.SetHandler(async (url, destination, extract) =>
{
    if (string.IsNullOrEmpty(url))
    {
        Console.WriteLine("No url provided");
        return;
    }
                
    if (string.IsNullOrEmpty(destination))
    {
        Console.WriteLine("No destination provided");
        return;
    }

    var httpClient = new HttpClient();
    
    var stream = await httpClient.GetStreamAsync(url);

    var newDest = extract ? Path.GetTempFileName() : destination;

    Console.WriteLine($"Downloading {url} to {newDest}");
    await using (var fileStream = File.OpenWrite(newDest))
    {
        await stream.CopyToAsync(fileStream);
    }
    Console.WriteLine($"Downloaded {url} to {newDest}");

                
    if (extract)
    {
        Console.WriteLine($"Extracting {newDest} to {destination}");
        ZipFile.ExtractToDirectory(newDest, destination, true);
        Console.WriteLine($"Extracted {newDest} to {destination}");
    }
}, urlArg, destinationArg, extractOption);


await rootCommand.InvokeAsync(args);