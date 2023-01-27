using System.Collections.Generic;
using System.IO;
using Game.Scripts.Spin;
using NUnit.Framework;
using UnityEditor;
using VContainer;
using VContainer.Unity;

[TestFixture]
public class SpinTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void SpinTestsSimplePasses()
    {
        string[] pathList = { "Assets", "Game", "Data", @"Spin Data Holder.asset" };
        SpinDataHolder spinDataHolder = AssetDatabase.LoadAssetAtPath<SpinDataHolder>(Path.Combine(pathList));

        var builder = new ContainerBuilder();
        builder.RegisterComponent(spinDataHolder);
        builder.Register<SpinGenerator>(Lifetime.Scoped);

        using (var container = builder.Build())
        {
            var spinGenerator = container.Resolve<SpinGenerator>();
            spinGenerator.GenerateSpinListNew();
            
            var intervalList = spinGenerator.GetIntervalListDictionary();
            Dictionary<string, List<int>> resultDictionary = new Dictionary<string, List<int>>();
            foreach (var spinData in spinDataHolder.spinDataList)
            {
                var keyName = GetKeyName(spinData.spinResult);
                resultDictionary.Add(keyName, new List<int>());
            }
            for (int i = 0; i < 100; i++)
            {
                resultDictionary[GetKeyName(spinDataHolder.spinResultList.Value[i])].Add(i);
            }

            foreach (var spinData in spinDataHolder.spinDataList)
            {
                for (int i = 0; i < intervalList[spinData].Count; i++)
                {
                    var keyName = GetKeyName(spinData.spinResult);
                    var interval = intervalList[spinData][i];
                    var spinIndex = resultDictionary[keyName][i];
                    bool isTestStillSuccessful = spinIndex >= interval.x && spinIndex <= interval.y;
                    if (!isTestStillSuccessful)
                    {
                        Assert.Fail();
                    }
                }
            }
            
            Assert.Pass();
        }
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }
}