using System;
using UnityEngine;
namespace TCS.DependencyInjection.Lite {
    [DefaultExecutionOrder(-99999)]
    public class InjectionBehaviour : MonoBehaviour, IDependencyProvider {
        protected bool IsAddedScene {get; set;}
        
        protected virtual void Awake() {
            if (IsAddedScene) {
                Injector.Instance.ReInject();
            }
        }
        protected virtual void Start() {
            if (IsAddedScene) {
                Injector.Instance.InvokeListeners();
            }
            
            Injector.Instance.ClearRegistry();
            Destroy(gameObject);
        }
        
        protected static bool TryGetFirstObjectByType<T>(out T result) where T : MonoBehaviour {
            result = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            return result;
        }
    }
}