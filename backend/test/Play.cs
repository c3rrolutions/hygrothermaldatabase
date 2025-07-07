using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Database.Methods;
using Database.GraphQl.MethodAsService;
using System.Collections.Generic;

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