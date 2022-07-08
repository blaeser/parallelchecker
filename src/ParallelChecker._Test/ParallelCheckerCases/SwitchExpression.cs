using System;
using System.Threading.Tasks;

public enum Rainbow {
  Red,
  Orange,
  Yellow,
  Green,
  Blue,
  Indigo,
  Violet,
  None
}

namespace NewCSharp8Features {
  class Program {
    static void Main() {
      Console.WriteLine(FromRainbow(Rainbow.Blue));
      if (FromRainbow(Rainbow.Blue).Blue == 0xFF) {
        var x = 1;
        Task.Run(() => x++);
        Console.Write(x);
      }
      try {
        FromRainbow(Rainbow.None);
      } catch (ArgumentException) {
        var y = 1;
        Task.Run(() => y++);
        Console.Write(y);
      }
    }

    public static RGBColor FromRainbow(Rainbow colorBand) =>
    colorBand switch
    {
      Rainbow.Red => new RGBColor(0xFF, 0x00, 0x00),
      Rainbow.Orange => new RGBColor(0xFF, 0x7F, 0x00),
      Rainbow.Yellow => new RGBColor(0xFF, 0xFF, 0x00),
      Rainbow.Green => new RGBColor(0x00, 0xFF, 0x00),
      Rainbow.Blue => new RGBColor(0x00, 0x00, 0xFF),
      Rainbow.Indigo => new RGBColor(0x4B, 0x00, 0x82),
      Rainbow.Violet => new RGBColor(0x94, 0x00, 0xD3),
      _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(colorBand)),
    };

    internal class RGBColor {
      private int r;
      private int g;
      private int b;

      public RGBColor(int r, int g, int b) {
        this.r = r;
        this.g = g;
        this.b = b;
      }

      public int Blue => b;
    }
  }
}
