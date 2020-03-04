using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrscQuestion.Models.Entities;

namespace FrscQuestion.Models.Services
{
    public class ServiceGenerator
    {
        private readonly Random _random = new Random();

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

    }
}