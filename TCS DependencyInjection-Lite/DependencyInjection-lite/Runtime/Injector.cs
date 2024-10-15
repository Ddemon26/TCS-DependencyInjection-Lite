using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TCS.DependencyInjection.Lite {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : PropertyAttribute { }
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : PropertyAttribute { }

    public interface IDependencyProvider { }

    [DefaultExecutionOrder(-1000)]
    public class Injector : InjectorSingleton<Injector> {
        const BindingFlags K_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        readonly Dictionary<Type, object> m_registry = new();

        protected override void Awake() {
            base.Awake();
            MonoBehaviour[] monoBehaviours = FindMonoBehaviours();
            
            // Find all modules implementing IDependencyProvider and register the dependencies they provide
            IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
            foreach (var provider in providers) {
                Register(provider);
            }
            
            // Find all injectable objects and inject their dependencies
            IEnumerable<MonoBehaviour> injectables = monoBehaviours.Where(IsInjectable);
            foreach (var injectable in injectables) {
                Inject(injectable);
            }
        }

        // Register an instance of a type outside the normal dependency injection process
        public void Register<T>(T i) {
            m_registry[typeof(T)] = i;
        }
        
        void Inject(object i) {
            var type = i.GetType();
            
            // Inject into fields
            IEnumerable<FieldInfo> injectableFields = type.GetFields(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields) {
                if (injectableField.GetValue(i) != null) {
                    InjectorLogger.LogWarning($"Field '{injectableField.Name}' of class '{type.Name}' is already set.");
                    continue;
                }
                var fieldType = injectableField.FieldType;
                object resolvedInstance = Resolve(fieldType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into field '{injectableField.Name}' of class '{type.Name}'.");
                }
                
                injectableField.SetValue(i, resolvedInstance);
            }
            
            // Inject into methods
            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods) {
                Type[] requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();
                object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null)) {
                    throw new Exception($"Failed to inject dependencies into method '{injectableMethod.Name}' of class '{type.Name}'.");
                }
                
                injectableMethod.Invoke(i, resolvedInstances);
            }
            
            // Inject into properties
            IEnumerable<PropertyInfo> injectableProperties = type.GetProperties(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
            foreach (var injectableProperty in injectableProperties) {
                var propertyType = injectableProperty.PropertyType;
                object resolvedInstance = Resolve(propertyType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into property '{injectableProperty.Name}' of class '{type.Name}'.");
                }

                injectableProperty.SetValue(i, resolvedInstance);
            }
        }

        void Register(IDependencyProvider provider) {
            MethodInfo[] methods = provider.GetType().GetMethods(K_BINDING_FLAGS);

            foreach (var method in methods) {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;
                
                var returnType = method.ReturnType;
                object providedInstance = method.Invoke(provider, null);
                if (providedInstance != null) {
                    m_registry.Add(returnType, providedInstance);
                } else {
                    throw new Exception($"Provider method '{method.Name}' in class '{provider.GetType().Name}' returned null when providing type '{returnType.Name}'.");
                }
            }
        }

        public void ValidateDependencies() {
            MonoBehaviour[] monoBehaviours = FindMonoBehaviours();
            IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
            HashSet<Type> providedDependencies = GetProvidedDependencies(providers);

            IEnumerable<string> invalidDependencies = monoBehaviours
                .SelectMany(mb => mb.GetType().GetFields(K_BINDING_FLAGS), (mb, field) => new {mb, field})
                .Where(t => Attribute.IsDefined(t.field, typeof(InjectAttribute)))
                .Where(t => !providedDependencies.Contains(t.field.FieldType) && t.field.GetValue(t.mb) == null)
                .Select(t => $"[Validation] {t.mb.GetType().Name} is missing dependency {t.field.FieldType.Name} on GameObject {t.mb.gameObject.name}");
            
            List<string> invalidDependencyList = invalidDependencies.ToList();
            
            if (!invalidDependencyList.Any()) {
                InjectorLogger.Log("All dependencies are valid.");
            } else {
                InjectorLogger.LogError($"{invalidDependencyList.Count} dependencies are invalid:");
                foreach (string invalidDependency in invalidDependencyList) {
                    InjectorLogger.LogError(invalidDependency);
                }
            }
        }
        
        HashSet<Type> GetProvidedDependencies(IEnumerable<IDependencyProvider> providers) {
            HashSet<Type> providedDependencies = new();
            foreach (var provider in providers) {
                MethodInfo[] methods = provider.GetType().GetMethods(K_BINDING_FLAGS);
                
                foreach (var method in methods) {
                    if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;
                    
                    var returnType = method.ReturnType;
                    providedDependencies.Add(returnType);
                }
            }

            return providedDependencies;
        }

        public void ClearDependencies() {
            foreach (var monoBehaviour in FindMonoBehaviours()) {
                var type = monoBehaviour.GetType();
                IEnumerable<FieldInfo> injectableFields = type.GetFields(K_BINDING_FLAGS)
                    .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

                foreach (var injectableField in injectableFields) {
                    injectableField.SetValue(monoBehaviour, null);
                }
            }
            
            InjectorLogger.Log("All injectable fields cleared.");
        }

        object Resolve(Type type) {
            m_registry.TryGetValue(type, out object resolvedInstance);
            return resolvedInstance;
        }

        static MonoBehaviour[] FindMonoBehaviours()
            => FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);

        static bool IsInjectable(MonoBehaviour obj) {
            MemberInfo[] members = obj.GetType().GetMembers(K_BINDING_FLAGS);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }
    }
}