// See https://aka.ms/new-console-template for more information

namespace Antoine.Andre.FeatureMatching.Console;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run <objectImagePath> <scenesDirectory>");
            return;
        }

        string objectImagePath = args[0];
        string scenesDirectory = args[1];

        if (!File.Exists(objectImagePath) || !Directory.Exists(scenesDirectory))
        {
            Console.WriteLine($"Usage: dotnet run {objectImagePath} {scenesDirectory}");
            Console.WriteLine("Invalid file or directory path.");
            return;
        }

        var objectImageData = await File.ReadAllBytesAsync(objectImagePath);
        var sceneImagesData = Directory.EnumerateFiles(scenesDirectory)
            .Select(file => File.ReadAllBytesAsync(file).Result)
            .ToList();

        var objectDetection = new ObjectDetection();
        var detectObjectInScenesResults = await objectDetection.DetectObjectInScenesAsyncMock(objectImageData, sceneImagesData);

        foreach (var result in detectObjectInScenesResults)
        {
            Console.WriteLine($"Points: {JsonSerializer.Serialize(result.Points)}");
        }
    }
}