using System;
using System.Collections.Generic;

public class WorldEvents : Feature {
    private readonly Dictionary<int, List<Delegate>> handlers = new();

    public void Publish<T>(T message) {
        var id = EventTypeMgr.GetId<T>();
        if (handlers.TryGetValue(id, out var list)) {
            foreach (var del in list) {
                ((Action<T>)del)?.Invoke(message);
            }
        }
    }

    public void Register<T>(Action<T> action) {
        var id = EventTypeMgr.GetId<T>();
        if (!handlers.ContainsKey(id)) {
            handlers.Add(id, new List<Delegate>());
        }
        var list = handlers[id];
        list.Add(action);
    }

    public void Unregister<T>(Action<T> action) {
        var id = EventTypeMgr.GetId<T>();
        if (!handlers.TryGetValue(id, out var list)) {
            return;
        }
        
        list.Remove(action);
    }
}

public static class EventTypeMgr {
    private static int idAllocator;
    private static Dictionary<Type, int> typeIds = new();

    public static int GetId<T>() {
        var type = typeof(T);
        if (!typeIds.ContainsKey(type)) {
            idAllocator++;
            typeIds.Add(type, idAllocator);
        }

        return typeIds[type];
    }
}