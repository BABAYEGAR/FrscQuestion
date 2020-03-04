using System.Collections.Generic;
using System.Linq;

namespace FrscQuestion.Models.Services
{
    public class ApplicationCalculator
    {
  public decimal CalculateDiscount(decimal discount, decimal price)
        {
            long amount = 0;
            var discounted
                = discount * price / 100;
            amount = (long) (price - discounted);
            return amount;
        }

        /// <summary>
        ///     Split the image list into a specified part concurrently
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
        {
            var i = 0;
            var enumerable = list as IList<T> ?? list.ToList();
            var splits = from item in enumerable
                group item by i++ % enumerable.ToList().Count / 2
                into part
                select part.AsEnumerable();
            return splits;
        }
    }
}