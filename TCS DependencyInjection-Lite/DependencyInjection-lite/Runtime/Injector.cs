using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TCS.DependencyInjection.Lite {
    /// <summary>
    /// Attribute to mark fields, methods, or properties for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : PropertyAttribute { }

    /// <summary>
    /// Attribute to mark methods that provide dependencies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : PropertyAttribute { }

    /// <summary>
    /// Interface for dependency providers which supply dependency instances via methods.
    /// </summary>
    public interface IDependencyProvider { }

    /// <summary>
    /// Interface for objects that require notification after dependencies have been injected.
    /// This callback occurs after Awake and before Start.
    /// </summary>
    public interface IDependencyListener {
        /// <summary>
        /// Callback invoked after dependency injection is complete.
        /// Use this method to perform any initialization that depends on injected dependencies.
        /// </summary>
        void OnDependenciesInjected();
    }

    /// <summary>
    /// Singleton class responsible for handling dependency injection.
    /// Finds all MonoBehaviour instances in the scene, registers dependencies from providers, 
    /// and injects them into injectable members.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Injector : InjectorSingleton<Injector> {
        const BindingFlags K_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        readonly Dictionary<Type, object> m_registry = new();
        MonoBehaviour[] m_monoBehaviours;

        /// <summary>
        /// Unity Awake method override.
        /// Initializes the Injector singleton and triggers the dependency injection process.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            HandleInjection();
        }

        /// <summary>
        /// Handles the overall injection process.
        /// Finds all MonoBehaviour instances, registers provided dependencies,
        /// and injects dependencies into objects with injectable members.
        /// </summary>
        void HandleInjection() {
            m_monoBehaviours = FindMonoBehaviours();

            // Find and register all modules implementing IDependencyProvider.
            IEnumerable<IDependencyProvider> providers = m_monoBehaviours.OfType<IDependencyProvider>();
            foreach (var provider in providers) {
                Register(provider);
            }

            // Find and inject dependencies into all injectable MonoBehaviours.
            IEnumerable<MonoBehaviour> injectables = m_monoBehaviours.Where(IsInjectable);
            foreach (var injectable in injectables) {
                Inject(injectable);
            }
        }

        /// <summary>
        /// Unity Start method.
        /// Invokes injection on all listeners that implement IDependencyListener.
        /// </summary>
        void Start() => InvokeListeners();

        /// <summary>
        /// Invokes the OnDependenciesInjected callback on all registered dependency listeners.
        /// </summary>
        public void InvokeListeners() {
            IEnumerable<IDependencyListener> listeners = m_monoBehaviours.OfType<IDependencyListener>();
            foreach (var listener in listeners) {
                listener.OnDependenciesInjected();
            }
        }

        /// <summary>
        /// Clears the dependency registry.
        /// All registered dependencies are removed.
        /// </summary>
        public void ClearRegistry() => m_registry.Clear();

        /// <summary>
        /// Re-injects dependencies.
        /// Clears the existing registry, resets MonoBehaviour references, and handles injection from scratch.
        /// </summary>
        public void ReInject() {
            m_monoBehaviours = null;
            m_registry.Clear();
            HandleInjection();
        }

        /// <summary>
        /// Registers an instance of the specified type in the dependency registry.
        /// This allows manual registration outside the normal injection process.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="i">The instance to register.</param>
        public void Register<T>(T i) {
            m_registry[typeof(T)] = i;
        }

        /// <summary>
        /// Injects dependencies into the provided object.
        /// Searches for fields, methods, and properties marked with InjectAttribute,
        /// resolves their dependencies, and assigns the resolved instances.
        /// </summary>
        /// <param name="i">The object into which dependencies will be injected.</param>
        void Inject(object i) {
            var type = i.GetType();

            // Inject into fields.
            IEnumerable<FieldInfo> injectableFields = type.GetFields(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields) {
                if (injectableField.GetValue(i) != null) {
                    Debug.LogWarning($"Field \\{injectableField.Name}\\ of class \\{type.Name}\\ is already assigned.");
                    continue;
                }

                var fieldType = injectableField.FieldType;
                object resolvedInstance = Resolve(fieldType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into field \\{injectableField.Name}\\ of class \\{type.Name}\\.");
                }

                injectableField.SetValue(i, resolvedInstance);
            }

            // Inject into methods.
            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods) {
                Type[] requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();

                object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null)) {
                    throw new Exception($"Failed to inject dependencies into method \\{injectableMethod.Name}\\ of class \\{type.Name}\\.");
                }

                injectableMethod.Invoke(i, resolvedInstances);
            }

            // Inject into properties.
            IEnumerable<PropertyInfo> injectableProperties = type.GetProperties(K_BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
            foreach (var injectableProperty in injectableProperties) {
                var propertyType = injectableProperty.PropertyType;
                object resolvedInstance = Resolve(propertyType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into property \\{injectableProperty.Name}\\ of class \\{type.Name}\\.");
                }

                injectableProperty.SetValue(i, resolvedInstance);
            }
        }

        /// <summary>
        /// Registers dependencies provided by the given dependency provider.
        /// Searches for methods marked with ProvideAttribute and invokes them to register their output.
        /// </summary>
        /// <param name="provider">The dependency provider instance.</param>
        void Register(IDependencyProvider provider) {
            MethodInfo[] methods = provider.GetType().GetMethods(K_BINDING_FLAGS);

            foreach (var method in methods) {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

                var returnType = method.ReturnType;
                object providedInstance = method.Invoke(provider, null);
                if (providedInstance != null) {
                    m_registry.Add(returnType, providedInstance);
                }
                else {
                    throw new Exception($"Provider method \\{method.Name}\\ in class \\{provider.GetType().Name}\\ returned null when providing type \\{returnType.Name}\\.");
                }
            }
        }

        /// <summary>
        /// Validates all dependencies for MonoBehaviour instances in the scene.
        /// Checks that each injectable field has a registered dependency and logs missing dependencies.
        /// </summary>
        public void ValidateDependencies() {
            MonoBehaviour[] monoBehaviours = FindMonoBehaviours();
            IEnumerable<IDependencyProvider> providers = monoBehaviours.OfType<IDependencyProvider>();
            HashSet<Type> providedDependencies = GetProvidedDependencies(providers);

            IEnumerable<string> invalidDependencies = monoBehaviours
                .SelectMany(mb => mb.GetType().GetFields(K_BINDING_FLAGS), (mb, field) => new { mb, field })
                .Where(t => Attribute.IsDefined(t.field, typeof(InjectAttribute)))
                .Where(t => !providedDependencies.Contains(t.field.FieldType) && t.field.GetValue(t.mb) == null)
                .Select(t => $"[Validation] \\{t.mb.GetType().Name}\\ is missing dependency \\{t.field.FieldType.Name}\\ on GameObject \\{t.mb.gameObject.name}\\");

            List<string> invalidDependencyList = invalidDependencies.ToList();

            if (!invalidDependencyList.Any()) {
                Logger.Log("All dependencies are valid.");
            }
            else {
                Logger.LogError($"{invalidDependencyList.Count} dependencies are invalid:");
                foreach (string invalidDependency in invalidDependencyList) {
                    Logger.LogError(invalidDependency);
                }
            }
        }

        /// <summary>
        /// Retrieves the set of dependency types provided by the given dependency providers.
        /// </summary>
        /// <param name="providers">Collection of dependency provider instances.</param>
        /// <returns>A HashSet of types representing provided dependencies.</returns>
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

        /// <summary>
        /// Clears all injectable fields in all MonoBehaviour instances found in the scene.
        /// This resets injected dependencies to null.
        /// </summary>
        public void ClearDependencies() {
            foreach (var monoBehaviour in FindMonoBehaviours()) {
                var type = monoBehaviour.GetType();
                IEnumerable<FieldInfo> injectableFields = type.GetFields(K_BINDING_FLAGS)
                    .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

                foreach (var injectableField in injectableFields) {
                    injectableField.SetValue(monoBehaviour, null);
                }
            }

            Logger.Log("All injectable fields cleared.");
        }

        /// <summary>
        /// Resolves an instance from the registry matching the provided type.
        /// </summary>
        /// <param name="type">The type of the dependency to resolve.</param>
        /// <returns>The resolved instance if found; otherwise, null.</returns>
        object Resolve(Type type) {
            m_registry.TryGetValue(type, out object resolvedInstance);
            return resolvedInstance;
        }

        /// <summary>
        /// Finds all MonoBehaviour instances in the current scene.
        /// </summary>
        /// <returns>An array of MonoBehaviour instances.</returns>
        static MonoBehaviour[] FindMonoBehaviours()
            => FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);

        /// <summary>
        /// Determines whether the specified MonoBehaviour contains any members marked for injection.
        /// </summary>
        /// <param name="obj">The MonoBehaviour instance to check.</param>
        /// <returns>True if at least one member is marked with InjectAttribute; otherwise, false.</returns>
        static bool IsInjectable(MonoBehaviour obj) {
            MemberInfo[] members = obj.GetType().GetMembers(K_BINDING_FLAGS);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }
    }
}