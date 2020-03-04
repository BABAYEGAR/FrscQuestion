using System.Collections.Generic;
using System.Security.Cryptography;

namespace FrscQuestion.Models.Services
{
    public static class ListShuffle
    {
        public static List<T> Shuffle<T>(this IList<T> list)
        {
            var images = new List<T>();
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                var k = box[0] % n;
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
                images.Add(value);
            }

            return images;
        }
    }
}