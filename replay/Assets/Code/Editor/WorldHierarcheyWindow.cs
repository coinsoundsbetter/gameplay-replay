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

        private enum WorldPick {
            Client,
            Server
        }

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
                    Repaint();
                }
            }

            var rect = position;
            rect.y = 20;
            rect.height -= 20;

            // 左右分栏
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
            selectedActor = actor;
            Repaint();
        }

        private void DrawRightPane() {
            var world = GetWorld();
            if (world == null) {
                EditorGUILayout.HelpBox("未找到世界实例。请在 bootstrap 中注册 BattleWorldDebugRegistry。", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"World: {world.Name}   Flags: {world.Flags}",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"FrameTickΔ: {world.FrameTickDelta}   LogicTickΔ: {world.LogicTickDelta}");
            EditorGUILayout.Space();

            if (selectedActor == null) {
                EditorGUILayout.HelpBox("在左侧选择一个 Actor 查看详情。", MessageType.None);
                return;
            }

            using (var scroll = new EditorGUILayout.ScrollViewScope(rightScroll)) {
                rightScroll = scroll.scrollPosition;

                var info = world.GetActorInfo(selectedActor);
                
                // Group
                EditorGUILayout.LabelField("Group", info.Group.ToString());
                
                // Features
                List<string> featureList = new List<string>();
                foreach (var kvp in info.Features) {
                    featureList.Add(kvp.Key);
                }
                DrawFoldList("Features", featureList);
                
                // Managed Data
                var managedDataList = new List<string>();
                foreach (var kvp in info.DataManagedSet) {
                    managedDataList.Add(kvp.Key.Name);
                }   
                DrawFoldList("Managed Data", managedDataList);

                // Group
                /*if (!world.TryGetGroup(selectedActor, out var group))
                    EditorGUILayout.LabelField("Group", "Unknown");
                else
                    EditorGUILayout.LabelField("Group", group.ToString());

                EditorGUILayout.Space();

                // Features
                if (world.TryGetFeatures(selectedActor, out var features) && features != null) {
                    DrawFoldList("Features", features.OrderBy(kv => kv.Key).Select(kv =>
                        $"{kv.Key}  —  {kv.Value?.GetType().Name ?? "null"}"));
                } else {
                    DrawFoldList("Features", Array.Empty<string>());
                }

                // Managed Data
                if (world.getman(selectedActor, out var managed) && managed != null) {
                    DrawFoldList("Managed Data", managed.OrderBy(kv => kv.Key.Name).Select(kv =>
                        $"{kv.Key.Name}  —  {(kv.Value == null ? "null" : kv.Value.GetType().Name)}"));
                } else {
                    DrawFoldList("Managed Data", Array.Empty<string>());
                }

                // Unmanaged Data / Buffers
                if (world.TryGetActorDataManaged(selectedActor, out var unmg, out var buf)) {
                    DrawFoldList("Unmanaged Data", (unmg ?? new Dictionary<Type, RefStorageBase>())
                        .OrderBy(kv => kv.Key.Name).Select(kv => $"{kv.Key.Name}  —  {kv.Value?.GetType().Name}"));

                    DrawFoldList("Unmanaged Buffers", (buf ?? new Dictionary<Type, RefStorageBase>())
                        .OrderBy(kv => kv.Key.Name).Select(kv => $"{kv.Key.Name}  —  {kv.Value?.GetType().Name}"));
                } else {
                    DrawFoldList("Unmanaged Data", Array.Empty<string>());
                    DrawFoldList("Unmanaged Buffers", Array.Empty<string>());
                }*/
            }
        }

        private void DrawFoldList(string title, IEnumerable<string> lines) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            foreach (var line in lines)
                EditorGUILayout.LabelField("• " + line);
            EditorGUILayout.Space(4);
        }

        // ---------------- Tree ----------------
        private class HierarchyTree : TreeView {
            private readonly Func<BattleWorld> getWorld;
            private readonly Action<GameplayActor> onSelectActor;

            public HierarchyTree(TreeViewState state, Func<BattleWorld> getWorld, Action<GameplayActor> onSelectActor)
                : base(state, new MultiColumnHeader(new MultiColumnHeaderState(new[] {
                    new MultiColumnHeaderState.Column()
                        { headerContent = new GUIContent("Actor / Group"), width = 220, autoResize = true },
                    new MultiColumnHeaderState.Column()
                        { headerContent = new GUIContent("Type/Info"), width = 160, autoResize = true },
                }))) {
                this.getWorld = getWorld;
                this.onSelectActor = onSelectActor;
                rowHeight = 20f;
                showBorder = true;
                Reload();
            }

            protected override TreeViewItem BuildRoot() {
                var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
                var world = getWorld();
                if (world == null) {
                    root.AddChild(new TreeViewItem(1, 0, "<No World>"));
                    return root;
                }

                int nextId = 1;

                // 按 Group → Actors
                /*foreach (var kv in world.EnumerateGroups()) {
                    var group = kv.Key;
                    var actors = kv.Value ?? new List<GameplayActor>();

                    var groupItem = new GroupItem(nextId++, 0, group.ToString());
                    root.AddChild(groupItem);

                    foreach (var a in actors) {
                        var name = a?.ToString() ?? "<null>";
                        var actorItem = new ActorItem(nextId++, 1, name) { Actor = a };
                        groupItem.AddChild(actorItem);
                    }
                }

                // 如果某些 actor 不在 group 里（理论上不会），也加进来
                var set = new HashSet<GameplayActor>(world.EnumerateGroups().SelectMany(p => p.Value));
                foreach (var a in world.EnumerateAllActors()) {
                    if (a == null || set.Contains(a)) continue;
                    var orphan = new GroupItem(nextId++, 0, "<Ungrouped>");
                    root.AddChild(orphan);
                    orphan.AddChild(new ActorItem(nextId++, 1, a.ToString()) { Actor = a });
                }*/

                SetupDepthsFromParentsAndChildren(root);
                return root;
            }

            protected override void RowGUI(RowGUIArgs args) {
                var colRect0 = args.GetCellRect(0);
                var colRect1 = args.GetCellRect(1);

                if (args.item is ActorItem ai) {
                    EditorGUI.LabelField(colRect0, ai.displayName);
                    // 右侧列：可展示类型或其他信息
                    EditorGUI.LabelField(colRect1, ai.Actor?.GetType().Name ?? "");
                } else {
                    EditorGUI.LabelField(colRect0, args.item.displayName);
                }
            }

            protected override void SingleClickedItem(int id) {
                if (FindItem(id, rootItem) is ActorItem ai)
                    onSelectActor?.Invoke(ai.Actor);
                else
                    onSelectActor?.Invoke(null);
            }

            private class GroupItem : TreeViewItem {
                public GroupItem(int id, int depth, string name) : base(id, depth, name) {
                }
            }

            private class ActorItem : TreeViewItem {
                public GameplayActor Actor;

                public ActorItem(int id, int depth, string name) : base(id, depth, name) {
                }
            }
        }
    }
}