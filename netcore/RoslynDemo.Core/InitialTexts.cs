namespace RoslynDemo.Core
{
    public static class InitialTexts
    {
        public const string HelloWorld =
@"using System;

public class Hest
{
  public event EventHandler OnKnegg;
  public string Navn { get; set; }

  public void Knegg()
  {
    Console.WriteLine(""Vrinsk!"");
    OnKnegg?.Invoke(this, EventArgs.Empty);
  }
  
  public static void Main(string[] kneggs)
  {
    var hest = new Hest { Navn = ""Klaus Knegg"", OnKnegg = HarKnegga};
    hest.Knegg();
    hest.OnKnegg -= HarKnegga;
  }

  private static void HarKnegga(object sender, EventArgs e)
  {
    var hest = (Hest) sender;
    Console.WriteLine($""{hest.Navn} har knegga"");
  }
}
";
    }
}
