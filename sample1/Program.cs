using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace sample1;

class Personne
{
    public string Nom { get; set; } = "";
    public int Age { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // section 2 - json
        var personne = new Personne { Nom = "Gabriel", Age = 22 };
        string json = JsonConvert.SerializeObject(personne, Formatting.Indented);
        Console.WriteLine(json);

        // section 3 - resize image
        string inputPath = "test.jpg";
        string outputPath = "test-resized.jpg";

        using (var image = Image.Load(inputPath))
        {
            Console.WriteLine($"original: {image.Width}x{image.Height}");
            image.Mutate(x => x.Resize(200, 150));
            image.SaveAsJpeg(outputPath);
            Console.WriteLine($"resized: {image.Width}x{image.Height} -> saved to {outputPath}");
        }
    }
}
