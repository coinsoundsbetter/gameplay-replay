using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace KillCam.Editor {
    public class BattleWorldHierarchyWindow : EditorWindow {
        [MenuItem("KillCam/WorldHierarchy")]
        public static void Open() {
            var w = GetWindow<BattleWorldHierarchyWindow>("WorldHierarchy");
            w.Show();
        }

        private enum WorldPick { Client, Server }
        private WorldPick worldPick = WorldPick.Client;

        private TreeViewState treeState;
        private HierarchyTree tree;
        private SearchField searchField;

        private GameplayActor selectedActor;
        private Vector2 rightScroll;
        private bool autoRefresh = true;
        private double lastRefreshTime;
        private const double REFRESH_INTERVAL = 0.5; // 秒

        private void OnEnable() {
            treeState = treeState ?? new TreeViewState();
            tree = new HierarchyTree(treeState, GetWorld, OnSelectActor);
            searchField = new SearchField();
            searchField.downOrUpArrowKeyPressed += tree.SetFocusAndEnsureSelectedItem;
        }

        private void Update() {
            if (autoRefresh && EditorApplication.timeSinceStartup - lastRefreshTime > REFRESH_INTERVAL) {
                tree?.Reload();
                tree?.RestoreSelection(selectedActor);
                Repaint();
                lastRefreshTime = EditorApplication.timeSinceStartup;
            }
        }

        private BattleWorld GetWorld() {
            return worldPick == WorldPick.Client ? BattleWorldDebugRegistry.Client : BattleWorldDebugRegistry.Server;
        }

        private void OnGUI() {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                worldPick = (WorldPick)EditorGUILayout.EnumPopup(worldPick, GUILayout.Width(100));
                GUILayout.FlexibleSpace();

                tree.searchString = searchField.OnToolbarGUI(tree.searchString);
                autoRefresh = GUILayout.Toggle(autoRefresh, "Auto Refresh", EditorStyles.toolbarButton);
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70))) {
                    tree?.Reload();
                    tree?.RestoreSelection(selectedActor);
                    Repaint();
                }
            }

            var rect = position;
            rect.y = 20; rect.height -= 20;

            var left = new Rect(0, 20, Mathf.Max(250, rect.width * 0.35f), rect.height - 20);
            var right = new Rect(left.width + 2, 20, rect.width - left.width - 2, rect.height - 20);

            GUILayout.BeginArea(left);
            var treeRect = new Rect(0, 0, left.width, left.height);
            tree?.OnGUI(treeRect);
            GUILayout.EndArea();

            GUILayout.BeginArea(right);
            DrawRightPane();
            GUILayout.EndArea();
        }

        private void OnSelectActor(GameplayActor actor) {
            if (actor != null) {
                selectedActor = actor;
                Repaint();
            }
        }

        private void DrawRightPane() {
            var world = GetWorld();
            if (world == null) {
                EditorGUILayout.HelpBox("未找到世界实例。请在 bootstrap 中注册 BattleWorldDebugRegistry。", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"World: {world.Name}   Flags: {world.Flags}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"FrameTickΔ: {world.FrameTickDelta}   LogicTickΔ: {world.LogicTickDelta}");
            EditorGUILayout.Space();

            if (selectedActor == null) {
                EditorGUILayout.HelpBox("在左侧选择一个 Actor 查看详情。", MessageType.None);
                return;
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(rightScroll)) {
                rightScroll = scroll.scrollPosition;

                var info = world.GetActorInfo(selectedActor);

                // (按你的要求) 暂时不显示 Group
                // EditorGUILayout.LabelField("Group", info.Group.ToString());

                // Features
                var featureList = new List<string>();
                foreach (var kvp in info.Features) featureList.Add(kvp.Key);
                DrawFoldList("Features", featureList);

                // Managed Data
                var managedDataList = new List<string>();
                foreach (var kvp in info.DataManagedSet) managedDataList.Add(kvp.Key.Name);
                DrawFoldList("Managed Data", managedDataList);
            }
        }

        private void DrawFoldList(string title, IEnumerable<string> lines) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            foreach (var line in lines) EditorGUILayout.LabelField("• " + line);
            EditorGUILayout.Space(4);
        }

        // ---------------- Tree ----------------
        private class HierarchyTree : TreeView {
            private readonly Func<BattleWorld> getWorld;
            private readonly Action<GameplayActor> onSelectActor;

            // id -> actor 映射，避免依赖 FindItem
            private readonly Dictionary<int, GameplayActor> idToActor = new Dictionary<int, GameplayActor>();

            public HierarchyTree(TreeViewState state, Func<BattleWorld> getWorld, Action<GameplayActor> onSelectActor)
                : base(state, new MultiColumnHeader(new MultiColumnHeaderState(new[] {
                    new MultiColumnHeaderState.Column(){ headerContent = new GUIContent("Actor"), width = 260, autoResize = true },
                    new MultiColumnHeaderState.Column(){ headerContent = new GUIContent("Type"),  width = 160, autoResize = true },
                }))) {
                this.getWorld = getWorld;
                this.onSelectActor = onSelectActor;
                rowHeight = 20f;
                showBorder = true;
                Reload();
            }

            protected override TreeViewItem BuildRoot() {
                idToActor.Clear();

                var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
                var world = getWorld();
                if (world == null) {
                    root.AddChild(new TreeViewItem(1, 0, "<No World>"));
                    return root;
                }

                int nextId = 1;

                // 扁平：把所有组里的 Actor 展平成一个列表（过滤 null）
                IEnumerable<GameplayActor> allActors = Enumerable.Empty<GameplayActor>();
                foreach (var kv in world.GetActorEnumerator()) {
                    if (kv.Value != null)
                        allActors = allActors.Concat(kv.Value);
                }

                // 搜索过滤（使用 TreeView 自带的 searchString）
                string filter = searchString?.Trim();
                bool useFilter = !string.IsNullOrEmpty(filter);

                foreach (var a in allActors) {
                    if (a == null) continue;
                    var name = a.ToString();
                    if (useFilter && (name?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) ?? -1) < 0)
                        continue;

                    int id = nextId++;
                    idToActor[id] = a;
                    // depth=0，直接挂在 root 下
                    var actorItem = new ActorItem(id, 0, name) { Actor = a };
                    root.AddChild(actorItem);
                }

                SetupDepthsFromParentsAndChildren(root);
                return root;
            }

            protected override void RowGUI(RowGUIArgs args) {
                var col0 = args.GetCellRect(0);
                var col1 = args.GetCellRect(1);

                if (args.item is ActorItem ai) {
                    EditorGUI.LabelField(col0, ai.displayName);
                    EditorGUI.LabelField(col1, ai.Actor?.GetType().Name ?? "");
                } else {
                    EditorGUI.LabelField(col0, args.item.displayName);
                }
            }

            // 稳定拿选中
            protected override void SelectionChanged(IList<int> selectedIds) {
                if (selectedIds == null || selectedIds.Count == 0) return;
                int id = selectedIds[0];
                if (idToActor.TryGetValue(id, out var actor) && actor != null) {
                    onSelectActor?.Invoke(actor);
                }
            }

            // Reload 后恢复选中
            public void RestoreSelection(GameplayActor actor) {
                if (actor == null) return;
                foreach (var kv in idToActor) {
                    if (ReferenceEquals(kv.Value, actor)) {
                        SetSelection(new List<int> { kv.Key }, TreeViewSelectionOptions.RevealAndFrame);
                        break;
                    }
                }
            }

            private class ActorItem : TreeViewItem {
                public GameplayActor Actor;
                public ActorItem(int id, int depth, string name) : base(id, depth, name) { }
            }
        }
    }
}
