using Game.Core;
using Game.Domain;
using Game.Infrastructure;
using Game.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Composition
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);

            builder.RegisterInstance(new GridSessionSettings(GridNavigator.DefaultWindowSize));
            builder.RegisterInstance<IStartCoordProvider>(new RandomStartCoordProvider());

            builder.Register<IGridSource, FileGridSource>(Lifetime.Singleton);
            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Singleton);
            builder.Register<GameControls>(Lifetime.Singleton);
            builder.Register<IMoveInput, InputSystemMoveInput>(Lifetime.Singleton);
            builder.Register<IErrorPresenter, ErrorOverlayPresenter>(Lifetime.Singleton);

            builder.Register<CubePaletteLoader>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<GridView>().As<IGridView>();

            builder.Register<GridSessionController>(Lifetime.Singleton);

            builder.RegisterEntryPoint<QuitOnEscape>();
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
