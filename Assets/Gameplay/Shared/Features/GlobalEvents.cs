using System;
using System.Collections.Generic;

public class WorldEvents : Feature {
    private readonly Dictionary<int, List<Delegate>> handlers = new();

    public void Publish<T>(T message) {
        var id = Event<T>.GetId();
        if (handlers.TryGetValue(id, out var list)) {
            foreach (var del in list) {
                ((Action<T>)del)?.Invoke(message);
            }
        }
    }

    public void Register<T>(Action<T> action) {
        var id = Event<T>.GetId();
        if (!handlers.ContainsKey(id)) {
            handlers.Add(id, new List<Delegate>());
        }
        var list = handlers[id];
        list.Add(action);
    }

    public void Unregister<T>(Action<T> action) {
        var id = Event<T>.GetId();
        if (!handlers.TryGetValue(id, out var list)) {
            return;
        }
        
        list.Remove(action);
    }
}

public static class Event<T> {
    private static int idPool;
    private static Dictionary<Type, int> eventIds = new();

    public static int GetId() {
        var type = typeof(T);
        if (!eventIds.ContainsKey(type)) {
            eventIds.Add(type, ++idPool);
        }
        
        return eventIds[type];
    }
}