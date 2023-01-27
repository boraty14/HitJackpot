using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Spin;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

[TestFixture]
public class SpinTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void SpinTestsSimplePasses()
    {
        var builder = new ContainerBuilder();
        builder.Register<SpinDataHolder>(Lifetime.Transient);
        // Use the Assert class to test conditions

        using (var container = builder.Build())
        {
            var spinGenerator = container.Resolve<SpinDataHolder>();
        }
    }
}
