using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Database.Methods;

namespace Database.Tests;

public sealed class Play
{
    [Test]
    public async Task Do()
    {
        Console.WriteLine("Do play!");

        TransmittanceSumMethod myTransmittanceSumInstance = new TransmittanceSumMethod();
        myTransmittanceSumInstance.PrintDummyLine();

        await Task.FromResult(0);
    }
}