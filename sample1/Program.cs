using Newtonsoft.Json;

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
        var personne = new Personne { Nom = "Gabriel", Age = 22 };
        string json = JsonConvert.SerializeObject(personne, Formatting.Indented);
        Console.WriteLine(json);
    }
}
