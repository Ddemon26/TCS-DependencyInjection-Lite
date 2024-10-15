using UnityEditor;
using UnityEngine;
namespace TCS.DependencyInjection.Lite {
    public static class InjectorSpawner  {
#if UNITY_EDITOR
        [MenuItem("GameObject/Tent City Studio/Global Managers/Add Injector", false, 1000)]
        public static void CreateInjector() {
            if (Object.FindAnyObjectByType<Injector>(FindObjectsInactive.Include)) {
                Logger.LogWarning("An Injector already exists in the scene.");
                return;
            }
            
            var injectorObject = new GameObject("[Injector]");
            injectorObject.AddComponent<Injector>();
            Logger.Log("Injector spawned successfully.");
        }
#endif
    }
}