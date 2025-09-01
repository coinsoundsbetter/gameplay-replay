/*using System.Linq;
using System.Reflection;
using Gameplay.Core;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Editor {
    public class WorldSystemViewer : EditorWindow{
        private Vector2 _scroll;
        private World _world;

        [MenuItem("Gameplay/World System Viewer")]
        public static void Open() {
            var wnd = GetWindow<WorldSystemViewer>("World Systems");
            wnd.Show();
        }

        private int _selectedWorldIndex = 0;

        private void OnGUI()
        {
            var worlds = WorldRegistry.GetAllWorlds().AsReadOnlyList();
            if (worlds.Count == 0)
            {
                EditorGUILayout.HelpBox("没有正在运行的 World", MessageType.Info);
                return;
            }

            string[] names = worlds.Select(w => w.WorldName).ToArray();
            _selectedWorldIndex = EditorGUILayout.Popup("World", _selectedWorldIndex, names);

            var targetWorld = worlds[_selectedWorldIndex];
            DrawSystemGroup("LogicRoot", targetWorld.LogicRoot);
            DrawSystemGroup("FrameRoot", targetWorld.FrameRoot);
        }

        private void DrawSystemGroup(string title, SystemGroup group) {
            if (group == null) return;

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            var systems = group.Systems;
            for (int i = 0; i < systems.Count; i++) {
                var sys = systems[i];
                var type = sys.GetType();

                // 显示系统名字 + 顺序属性
                var orderAttr = type.GetCustomAttribute<OrderAttribute>();
                string orderStr = orderAttr != null ? $"[{orderAttr.Order}]" : "";

                EditorGUILayout.LabelField($"{i + 1}. {type.Name} {orderStr}");
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}*/