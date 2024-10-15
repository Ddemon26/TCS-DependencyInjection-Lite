using UnityEngine;
namespace TCS.DependencyInjection.Lite.Tests {
    public class ClassB : MonoBehaviour {
        [Inject] ServiceA m_serviceA;
        [Inject] ServiceB m_serviceB;
        
        FactoryA m_factoryA;
        
        [Inject] public void Init(FactoryA factory) {
            m_factoryA = factory;
            Debug.Log("Initialized FactoryA in ClassB");
        }
    }
}