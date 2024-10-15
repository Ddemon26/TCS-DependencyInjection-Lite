using UnityEditor;
using UnityEngine;
namespace TCS.DependencyInjection.Lite {
    public static class InjectorSpawner  {
        [MenuItem("Tools/TCS/Managers/Spawn Injector", false)]
        public static void CreateInjector() {
            if (Object.FindAnyObjectByType<Injector>(FindObjectsInactive.Include)) {
                InjectorLogger.LogWarning("An Injector already exists in the scene.");
                return;
            }
            
            var injectorObject = new GameObject("[Injector]");
            injectorObject.AddComponent<Injector>();
            InjectorLogger.Log("Injector spawned.");
        }
    }
}