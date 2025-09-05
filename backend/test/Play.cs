using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Database.Tests;

public sealed class Play
{
    [Test]
    public async Task Do()
    {
        Console.WriteLine("Do play!");
        await Task.FromResult(0);
    }
}