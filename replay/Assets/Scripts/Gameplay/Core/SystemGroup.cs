using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gameplay.Core {
    public class SystemGroup : ISystem {
        private readonly List<ISystem> _systems = new();
        private SystemState[] _states = new SystemState[8];
        private int _count;
        public IReadOnlyList<ISystem> Systems => _systems;

        private readonly World myWorld;

        public SystemGroup(World world) {
            myWorld = world;
        }

        public void AddSystem(ISystem sys) {
            if (sys == null) throw new ArgumentNullException(nameof(sys));

            EnsureCapacity(_count + 1);

            var state = new SystemState() {
                World = myWorld,
            };
            sys.OnCreate(ref state);

            _systems.Add(sys);
            _states[_count++] = state;
        }

        public void OnCreate() { }

        public void Update(ref SystemState state) {
            for (int i = 0; i < _count; i++) {
                ref var st = ref _states[i];
                st.DeltaTime = state.DeltaTime;
                _systems[i].Update(ref st);
            }
        }

        public void OnDestroy() {
            for (int i = 0; i < _count; i++) {
                _systems[i].OnDestroy();
            }
        }

        private void EnsureCapacity(int size) {
            if (_states.Length < size) {
                int newCap = _states.Length * 2;
                if (newCap < size) newCap = size;
                Array.Resize(ref _states, newCap);
            }
        }

        /// <summary>
        /// 按照 [Order] / [UpdateBefore] / [UpdateAfter] 排序
        /// </summary>
        public virtual void SortSystems() {
            // 把 systems + states 打包成 entries
            var entries = _systems.Select((sys, i) => (sys, state: _states[i])).ToList();

            var typeMap = entries.ToDictionary(s => s.sys, s => s.sys.GetType());

            // Order.First
            var first = entries
                .Where(p => typeMap[p.sys].GetCustomAttribute<OrderAttribute>()?.Order == SystemOrder.First)
                .ToList();

            // Order.Last
            var last = entries
                .Where(p => typeMap[p.sys].GetCustomAttribute<OrderAttribute>()?.Order == SystemOrder.Last)
                .ToList();

            // 中间的系统
            var middle = entries.Except(first).Except(last).ToList();

            // ========= 构建依赖图 =========
            var graph = new Dictionary<Type, List<Type>>();
            var indegree = new Dictionary<Type, int>();

            void Ensure(Type t) {
                if (!graph.ContainsKey(t)) graph[t] = new();
                if (!indegree.ContainsKey(t)) indegree[t] = 0;
            }

            foreach (var (sys, _) in middle) {
                var t = sys.GetType();
                Ensure(t);

                foreach (var before in t.GetCustomAttributes<UpdateBeforeAttribute>()) {
                    Ensure(before.TargetType);
                    graph[t].Add(before.TargetType);
                    indegree[before.TargetType]++;
                }

                foreach (var after in t.GetCustomAttributes<UpdateAfterAttribute>()) {
                    Ensure(after.TargetType);
                    graph[after.TargetType].Add(t);
                    indegree[t]++;
                }
            }

            // ========= Kahn 拓扑排序 =========
            var sortedMiddle = new List<(ISystem, SystemState)>();
            var q = new Queue<Type>(indegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));

            while (q.Count > 0) {
                var t = q.Dequeue();
                var sysTuple = middle.FirstOrDefault(p => p.sys.GetType() == t);
                if (sysTuple.sys != null)
                    sortedMiddle.Add(sysTuple);

                if (graph.TryGetValue(t, out var edges)) {
                    foreach (var nxt in edges) {
                        indegree[nxt]--;
                        if (indegree[nxt] == 0) q.Enqueue(nxt);
                    }
                }
            }

            // ========= 如果有环，直接附加 =========
            if (sortedMiddle.Count < middle.Count) {
                var remaining = middle.Except(sortedMiddle).ToList();
                sortedMiddle.AddRange(remaining);
            }

            // ========= rebuild =========
            _systems.Clear();
            _count = 0;
            EnsureCapacity(entries.Count);

            foreach (var (sys, state) in first.Concat(sortedMiddle).Concat(last)) {
                _systems.Add(sys);
                _states[_count++] = state;
            }
        }
    }
}