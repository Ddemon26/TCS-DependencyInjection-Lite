using UnityEngine;
namespace TCS.DependencyInjection.Lite.Tests {
    public class ClassA : MonoBehaviour {
        [Inject] public ServiceA ServiceA;
        
        [Inject] public void InjectServiceB(ServiceA service) {
            ServiceA = service;
            Debug.Log(message: $"ClassA.InjectServiceB({service})");
        }
    }
}