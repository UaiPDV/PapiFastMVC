using BixWeb.Models;
using System.Drawing;

namespace BixWeb.ViewModel
{
    public class Anuncio
    {
        public int largura { get; set; }
        public int altura { get; set; }
        public Bitmap? logo { get; set; }
        public Bitmap? produto { get; set; }
        public int fontValor { get; set; }
        public int fontProd { get; set; }
        public string? caminho { get; set; }
    }
    public static class Extensions
    {
        public static List<List<T>> partition<T>(this List<T> values, int chunkSize)
        {
            return values.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
