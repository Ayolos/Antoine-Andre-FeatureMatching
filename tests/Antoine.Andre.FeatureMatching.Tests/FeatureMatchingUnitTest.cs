namespace Antoine.Andre.FeatureMatching.Tests;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class FeatureMatchingUnitTest
{
    [Fact]
    public async Task ObjectShouldBeDetectedCorrectly() {
        var executingPath = GetExecutingPath();
        var imageScenesData = new List<byte[]>();
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath,
                     "Scenes")))
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            imageScenesData.Add(imageBytes);
        }
        var objectImageData = await File.ReadAllBytesAsync(Path.Combine(executingPath, "Andre-Antoine-object.jpg"));
        var detectObjectInScenesResults = await new ObjectDetection().DetectObjectInScenesAsyncMock(objectImageData, imageScenesData);
        Assert.Equal("[{\"X\":1,\"Y\":2}]",JsonSerializer.Serialize(detectObjectInScenesResults[0].Points));
        Assert.Equal("[{\"X\":1,\"Y\":2}]",JsonSerializer.Serialize(detectObjectInScenesResults[1].Points));
    }
    private static string GetExecutingPath() {
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location; var executingPath = Path.GetDirectoryName(executingAssemblyPath); return executingPath;
    } }