using System;
using System.Collections.Generic;
using FOIA.Cards.Systems;
using FOIA.GameLoop.State;
using FOIA.GameLoop.Systems;
using FOIA.Officers.Runtime;
using FOIA.Officers.Systems;
using FOIA.Presentation;
using FOIA.Presentation.Presenters;
using FOIA.Requests.Sources;
using FOIA.Requests.State;
using FOIA.Requests.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FOIA.App
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private List<ViewBase> _views = new();
        [SerializeField] private RequestBoardTestBootstrapper _requestBoardTestBootstrapper;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterGameLoop(builder);
            RegisterRequestBoard(builder);
            RegisterPresentation(builder);
        }

        private static void RegisterGameLoop(IContainerBuilder builder)
        {
            builder.Register<GameCalendarState>(Lifetime.Singleton);

            builder.Register<TimeAdvanceSystem>(Lifetime.Singleton)
                .As<ITimeAdvanceSystem>()
                .AsSelf();

            builder.Register<DayEndSystem>(Lifetime.Singleton)
                .As<IDayEndSystem>()
                .AsSelf();

            builder.Register<GameLoopSystem>(Lifetime.Singleton)
                .As<IGameLoopSystem>()
                .AsSelf();
        }

        private void RegisterRequestBoard(IContainerBuilder builder)
        {
            builder.Register<OfficerStore>(Lifetime.Singleton);
            builder.Register<OfficerStressSystem>(Lifetime.Singleton);

            builder.Register<RequestCaseStore>(Lifetime.Singleton);
            builder.Register<RequestSource>(Lifetime.Singleton);
            builder.Register<RequestSpawnSystem>(Lifetime.Singleton);
            builder.Register<RequestAssignmentSystem>(Lifetime.Singleton);
            builder.Register<CardPlacementSystem>(Lifetime.Singleton);

            if (_requestBoardTestBootstrapper != null)
            {
                builder.RegisterComponent(_requestBoardTestBootstrapper);
            }
        }

        private void RegisterPresentation(IContainerBuilder builder)
        {
            RegisterViews(builder);

            builder.RegisterEntryPoint<PresenterInitializer>();

            builder.Register<TimeControlPresenter>(Lifetime.Singleton)
                .As<IPresenter>()
                .AsSelf();

            builder.Register<CardBoardPresenter>(Lifetime.Singleton)
                .As<IPresenter>()
                .AsSelf();
        }

        private void RegisterViews(IContainerBuilder builder)
        {
            HashSet<Type> registeredTypes = new();

            foreach (ViewBase view in _views)
            {
                if (view == null)
                {
                    Debug.LogError($"{nameof(GameLifetimeScope)} has a null view reference.", this);
                    continue;
                }

                Type viewType = view.GetType();

                if (!registeredTypes.Add(viewType))
                {
                    Debug.LogError($"{nameof(GameLifetimeScope)} has duplicate view type: {viewType.Name}", view);
                    continue;
                }

                builder.RegisterComponent(view)
                    .As(viewType)
                    .As<ViewBase>();
            }
        }
    }
}
