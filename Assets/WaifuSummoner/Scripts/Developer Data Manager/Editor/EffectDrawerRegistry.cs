using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

[InitializeOnLoad]
public static class EffectDrawerRegistry
{
    static readonly Dictionary<EffectType, IEffectDrawer> drawerMap;

    static EffectDrawerRegistry()
    {
        drawerMap = new Dictionary<EffectType, IEffectDrawer>();

        // Busca en todos los assemblies las clases que implementan IEffectDrawer y tienen el atributo
        var drawerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IEffectDrawer).IsAssignableFrom(t) &&
                !t.IsInterface && !t.IsAbstract &&
                t.GetCustomAttribute<EffectDrawerAttribute>() != null
            );

        // Instancia y guarda en el diccionario
        foreach (var type in drawerTypes)
        {
            var attr = type.GetCustomAttribute<EffectDrawerAttribute>();
            var instance = (IEffectDrawer)Activator.CreateInstance(type);
            drawerMap[attr.EffectType] = instance;
        }
    }

    public static bool TryGetDrawer(EffectType type, out IEffectDrawer drawer)
        => drawerMap.TryGetValue(type, out drawer);
}
