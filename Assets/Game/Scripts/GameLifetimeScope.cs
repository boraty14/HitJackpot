using Game.Scripts.Core;
using Game.Scripts.SlotElement;
using Game.Scripts.Spin;
using Game.Scripts.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private SpinDataHolder _spinDataHolder;
    [SerializeField] private SpinSettings _spinSettings;
    [SerializeField] private SpinResultList _spinResultList;
    [SerializeField] private SlotMachine _slotMachine;
    [SerializeField] private SaveHandler _saveHandler;
    [SerializeField] private SpinButton _spinButton;
    
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_spinDataHolder);
        builder.RegisterComponent(_spinSettings);
        builder.RegisterComponent(_spinResultList);
        builder.RegisterComponent(_slotMachine);
        builder.RegisterComponent(_saveHandler);
        builder.RegisterComponent(_spinButton);
        builder.RegisterEntryPoint<SaveHandlerSystem>();
        builder.RegisterEntryPoint<SpinButtonSystem>();
        builder.Register<SpinGenerator>(Lifetime.Singleton);
        builder.Register<SlotController>(Lifetime.Singleton);
    }
}