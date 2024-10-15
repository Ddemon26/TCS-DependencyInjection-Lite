using System;
using UnityEngine;

namespace TCS.DependencyInjection.Lite.Tests {
    public class ProviderExample : MonoBehaviour, IDependencyProvider {
        [Provide] public ServiceA ProvideServiceA() => new();
        [Provide] public ServiceB ProvideServiceB() => new();
        [Provide] public FactoryA ProvideFactoryA() => new();
    }
    public class ServiceA {
        public void Initialize(string message = null) {
            Debug.Log(message: $"ServiceA.Initialize({message})");
        }
    }
    public class ServiceB {
        public void Initialize(string message = null) {
            Debug.Log(message: $"ServiceB.Initialize({message})");
        }
    }
    public class FactoryA {
        ServiceA m_cachedServiceA;

        public ServiceA CreateServiceA() => m_cachedServiceA ??= new ServiceA();
    }

}