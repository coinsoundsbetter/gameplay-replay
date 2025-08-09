using System;
using System.Collections.Generic;
using Unity.Collections;

namespace KillCam {
    public partial class BattleWorld {
        public WorldFlag Flags { get; private set; }
        public FixedString32Bytes Name { get; private set; }
        public float FrameTickDelta { get; private set; }
        public double LogicTickDelta { get; private set; }

        private GameEntry boostrap;
        private GameplayActor worldActor;
        private readonly Dictionary<TickGroup, List<Feature>> tickGroups = new();
        private readonly Dictionary<ActorGroup, List<GameplayActor>> groupActors = new();
        private readonly Dictionary<GameplayActor, ActorGroup> actorGroupMap = new();
        private readonly Dictionary<GameplayActor, Dictionary<string, Feature>> actorFeatureSet = new();
        private readonly Dictionary<GameplayActor, Dictionary<Type, object>> actorDataManagedSet = new();
        private readonly Dictionary<GameplayActor, Dictionary<Type, RefStorageBase>> actorDataUnmanagedSet = new();

        private readonly TickGroup[] logicTickOrders = {
            TickGroup.NetworkReceive,
            TickGroup.PreSimulation,
            TickGroup.Input,
            TickGroup.Prediction,
            TickGroup.Simulation,
            TickGroup.CollisionAndHits,
            TickGroup.PostSimulation
        };

        private readonly TickGroup[] frameTickOrders = {
            TickGroup.Visual,
            TickGroup.PostVisual,
        };

        private readonly ActorGroup[] actorRemoveOrders = {
            ActorGroup.Default,
            ActorGroup.Player,
            ActorGroup.World,
        };

        public void Init(WorldFlag flag, GameEntry bs, FixedString32Bytes worldName) {
            Flags = flag;
            Name = worldName;
            boostrap = bs;
            foreach (var actorGroupType in actorRemoveOrders) {
                groupActors.Add(actorGroupType, new List<GameplayActor>());
            }
            foreach (var logicTickGroupType in logicTickOrders) {
                tickGroups.Add(logicTickGroupType, new List<Feature>());
            }
            foreach (var frameTickGroupType in frameTickOrders) {
                tickGroups.Add(frameTickGroupType, new List<Feature>());
            }

            worldActor = CreateActor(ActorGroup.World);
            boostrap.Start(this);
        }
        
        public void Destroy() {
            foreach (var actorGroup in actorRemoveOrders) {
                DestroyGroupAllActors(actorGroup);    
            }
            boostrap.End();
        }

        private void DestroyGroupAllActors(ActorGroup actorGroup) {
            if (groupActors.TryGetValue(actorGroup, out var actorList)) {
                int len = actorList.Count;
                for (int i = len - 1; i >= 0; i--) {
                    DestroyActor(actorList[i]);
                }
                
                groupActors.Remove(actorGroup);
            }
        }

        public void FrameUpdate(float delta) {
            FrameTickDelta = delta;
            foreach (var t in frameTickOrders) {
                Tick(t, delta);
            }
        }

        public void LogicUpdate(double delta) {
            LogicTickDelta = delta;
            foreach (var t in logicTickOrders) {
                Tick(t, delta);
            }
        }

        private void Tick(TickGroup group, double deltaTime) {
            foreach (var c in tickGroups[group]) {
                if (!c.IsActive && c.OnShouldActivate()) {
                    c.Activate();
                }else if (c.IsActive && !c.OnShouldActivate()) {
                    c.Deactivate();
                }
                
                if (c.IsActive) {
                    c.TickActive(deltaTime);
                }
            }
        }
    }
}