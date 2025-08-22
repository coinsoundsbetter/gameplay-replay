namespace KillCam.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public static class UnmanagedInspectorUtil {
        // ====== 在你的窗口里维护：每种类型一个 foldout 状态 ======
        private static readonly Dictionary<Type, bool> s_FoldStates = new();

        // ====== 字段缓存，避免频繁反射 ======
        private sealed class FieldMeta {
            public string Name;
            public Type FieldType;
            public FieldInfo Info;
        }

        private static readonly Dictionary<Type, List<FieldMeta>> s_FieldCache = new();

        private static List<FieldMeta> GetSerializableFields(Type t) {
            if (s_FieldCache.TryGetValue(t, out var cached))
                return cached;

            const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = t.GetFields(FLAGS)
                .Where(f =>
                    !f.IsStatic &&
                    f.GetCustomAttribute<NonSerializedAttribute>() == null &&
                    (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null))
                .Select(f => new FieldMeta { Name = f.Name, FieldType = f.FieldType, Info = f })
                .ToList();

            s_FieldCache[t] = fields;
            return fields;
        }

        /// <summary>
        /// 在 Inspector/自定义窗口中绘制：每个类型一组折叠项，内部列出字段与（可选）字段值
        /// dataByType: key=组件Type；value=该组件的“装箱实例”（object）或 null（只显示字段类型）
        /// </summary>
        public static void DrawUnmanagedData(string title, IDictionary<Type, object> dataByType, int maxDepth = 3) {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            // 排序：按类型名
            foreach (var kvp in dataByType.OrderBy(k => k.Key.Name)) {
                var type = kvp.Key;
                var boxedInstance = kvp.Value; // 可能为 null（无实例，仅 schema）

                if (!s_FoldStates.ContainsKey(type)) s_FoldStates[type] = false;
                s_FoldStates[type] = EditorGUILayout.Foldout(s_FoldStates[type], type.Name, true);

                if (!s_FoldStates[type]) continue;

                EditorGUI.indentLevel++;
                var fields = GetSerializableFields(type);

                if (fields.Count == 0) {
                    EditorGUILayout.LabelField("<no serializable fields>");
                }
                else {
                    foreach (var f in fields) {
                        // 没有实例：显示“字段名 : 字段类型”
                        if (boxedInstance == null) {
                            EditorGUILayout.LabelField($"• {f.Name} : {FriendlyTypeName(f.FieldType)}");
                            continue;
                        }

                        // 有实例：尝试读取值
                        object value;
                        try {
                            value = f.Info.GetValue(boxedInstance);
                        }
                        catch (Exception e) {
                            EditorGUILayout.LabelField($"• {f.Name} : <read error> ({e.GetType().Name})");
                            continue;
                        }

                        DrawFieldLine(f.Name, f.FieldType, value, maxDepth);
                    }
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
        }

        private static void DrawFieldLine(string fieldName, Type fieldType, object value, int maxDepth) {
            // 基础类型/枚举/字符串 直接一行
            if (IsLeaf(fieldType)) {
                var text = value == null ? "null" : value.ToString();
                EditorGUILayout.LabelField($"• {fieldName} : {FriendlyTypeName(fieldType)} = {text}");
                return;
            }

            // 数组/集合：首行展示类型与计数，下面缩进列元素
            if (TryAsEnumerable(value, out var enumerable, out int count)) {
                EditorGUILayout.LabelField($"• {fieldName} : {FriendlyTypeName(fieldType)} (Count={count})");
                if (value == null) return;

                EditorGUI.indentLevel++;
                int i = 0;
                foreach (var elem in enumerable) {
                    if (elem == null) {
                        EditorGUILayout.LabelField($"[{i}] = null");
                    }
                    else {
                        var et = elem.GetType();
                        if (IsLeaf(et)) {
                            EditorGUILayout.LabelField($"[{i}] ({FriendlyTypeName(et)}) = {elem}");
                        }
                        else {
                            DrawComplex($"{i}", elem, et, maxDepth - 1);
                        }
                    }

                    i++;
                }

                EditorGUI.indentLevel--;
                return;
            }

            // 复杂类型（struct/class）递归
            DrawComplex(fieldName, value, fieldType, maxDepth - 1);
        }

        private static void DrawComplex(string label, object value, Type type, int remainDepth) {
            if (value == null) {
                EditorGUILayout.LabelField($"• {label} : {FriendlyTypeName(type)} = null");
                return;
            }

            if (remainDepth < 0) {
                EditorGUILayout.LabelField($"• {label} : {FriendlyTypeName(type)} (…depth limit)");
                return;
            }

            EditorGUILayout.LabelField($"• {label} : {FriendlyTypeName(type)}");
            EditorGUI.indentLevel++;
            foreach (var f in GetSerializableFields(type)) {
                object v;
                try {
                    v = f.Info.GetValue(value);
                }
                catch {
                    EditorGUILayout.LabelField($"{f.Name} : <read error>");
                    continue;
                }

                DrawFieldLine(f.Name, f.FieldType, v, remainDepth);
            }

            EditorGUI.indentLevel--;
        }

        private static bool IsLeaf(Type t) {
            return t.IsPrimitive
                   || t.IsEnum
                   || t == typeof(string)
                   || t == typeof(decimal)
                   || t == typeof(DateTime)
                   || t == typeof(Guid)
                   // 常用 Unity 向量/颜色等，可按需扩展
                   || t.FullName == "UnityEngine.Vector2"
                   || t.FullName == "UnityEngine.Vector3"
                   || t.FullName == "UnityEngine.Vector4"
                   || t.FullName == "UnityEngine.Quaternion"
                   || t.FullName == "UnityEngine.Color"
                   || t.FullName == "Unity.Mathematics.float2"
                   || t.FullName == "Unity.Mathematics.float3"
                   || t.FullName == "Unity.Mathematics.float4"
                   || t.FullName == "Unity.Mathematics.quaternion";
        }

        private static bool TryAsEnumerable(object value, out IEnumerable enumerable, out int count) {
            enumerable = null;
            count = 0;
            if (value == null) return false;

            var t = value.GetType();
            if (t.IsArray) {
                var arr = (Array)value;
                enumerable = arr;
                count = arr.Length;
                return true;
            }

            if (typeof(IList).IsAssignableFrom(t)) {
                var list = (IList)value;
                enumerable = list.Cast<object>();
                count = list.Count;
                return true;
            }

            if (typeof(IEnumerable).IsAssignableFrom(t)) {
                // 泛型 IEnumerable<T> 的计数尽量用 ICollection<T>/IReadOnlyCollection<T>
                enumerable = (IEnumerable)value;
                if (t.GetInterface("ICollection`1") != null) {
                    // 反射拿 Count
                    var prop = t.GetProperty("Count");
                    if (prop != null && prop.PropertyType == typeof(int)) {
                        count = (int)prop.GetValue(value);
                        return true;
                    }
                }

                // 无法快速取 Count 时，保守不统计
                count = -1;
                return true;
            }

            return false;
        }

        private static string FriendlyTypeName(Type t) {
            if (!t.IsGenericType) return t.Name;
            var def = t.GetGenericTypeDefinition().Name;
            def = def.Substring(0, def.IndexOf('`'));
            var args = string.Join(", ", t.GetGenericArguments().Select(FriendlyTypeName));
            return $"{def}<{args}>";
        }
    }
}