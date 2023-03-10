using System.Collections.Generic;
using System.IO;
using Game.Scripts.Spin;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

[TestFixture]
public class SpinTests
{
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

            bool isTestStillSuccessful = true;
            foreach (var spinData in spinDataHolder.spinDataList)
            {
                for (int i = 0; i < intervalList[spinData].Count; i++)
                {
                    var keyName = GetKeyName(spinData.spinResult);
                    var interval = intervalList[spinData][i];
                    var spinIndex = resultDictionary[keyName][i]; 
                    isTestStillSuccessful = spinIndex >= interval.x && spinIndex <= interval.y;
                    if (!isTestStillSuccessful)
                    {
                        Debug.LogError($"Test failed! " +
                                       $"spin interval: {interval.x} - {interval.y} spin index : {spinIndex} " +
                                       $"spin percentage: {spinData.percentage} " + 
                                       $"spin result: {spinData.spinResult.firstSpin} {spinData.spinResult.secondSpin} {spinData.spinResult.thirdSpin}");
                    }
                }
            }
            
            Assert.IsTrue(isTestStillSuccessful);
        }
    }

    private string GetKeyName(SpinResult spinResult)
    {
        return $"{spinResult.firstSpin}, {spinResult.secondSpin}, {spinResult.thirdSpin}";
    }
}