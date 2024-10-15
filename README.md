
# Dependency Injection Lite for Unity

![License](https://img.shields.io/github/license/Ddemon26/DependencyInjection-Lite) 
![Issues](https://img.shields.io/github/issues/Ddemon26/DependencyInjection-Lite)
![Stars](https://img.shields.io/github/stars/Ddemon26/DependencyInjection-Lite)
![Forks](https://img.shields.io/github/forks/Ddemon26/DependencyInjection-Lite)

## Overview

**Dependency Injection Lite** is a lightweight and simple dependency injection framework for Unity, enabling you to inject global services and components into various classes in your project. It simplifies the management of dependencies through attribute-based injection, providing flexibility and better code organization.

## Features

- **Global Service Registration**: Register components globally and inject them into dependent classes throughout your project.
- **Attribute-Based Injection**: Use `[Inject]` attributes to inject services or dependencies into fields and methods automatically.
- **Factory Support**: Use factories to create instances dynamically and inject them where needed.
- **Reflection-Based System**: Automatically handles the registration and injection of services without requiring explicit dependency management.

## Installation

To integrate **Dependency Injection Lite** into your Unity project:

1. Clone or download the repository from GitHub.
2. Copy the `DependencyInjection-Lite` folder to your Unity project's `Assets` directory.
3. Ensure that your scripts and MonoBehaviours are properly integrated into the Unity project.

## Basic API Usage

### 1. Registering Global Services

Start by creating a class that implements `IDependencyProvider` to register your global services. Use the `[Provide]` attribute to register components, which will be available for injection.

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class GameServices : MonoBehaviour, IDependencyProvider {
    [Provide]
    public ServiceA ProvideServiceA() => new ServiceA();
    
    [Provide]
    public ServiceB ProvideServiceB() => new ServiceB();
    
    [Provide]
    public FactoryA ProvideFactoryA() => new FactoryA();
}
```

In this example, services like `ServiceA`, `ServiceB`, and a factory `FactoryA` are registered for global use.

### 2. Injecting Dependencies

You can inject services into any MonoBehaviour class by using the `[Inject]` attribute on fields or methods. This allows you to automatically receive the registered services.

#### Field Injection

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class PlayerController : MonoBehaviour {
    [Inject]
    private ServiceA _serviceA;

    [Inject]
    private ServiceB _serviceB;

    private void Start() {
        _serviceA.Initialize("PlayerController: ServiceA");
        _serviceB.Initialize("PlayerController: ServiceB");
    }
}
```

#### Method Injection

```csharp
public class ClassA : MonoBehaviour {
    [Inject] public ServiceA ServiceA;
    
    [Inject] public void InjectServiceB(ServiceA service) {
        ServiceA = service;
        Debug.Log(message: $"ClassA.InjectServiceB({service})");
    }
}
```

In `ClassA`, the `ServiceA` service is injected through both field and method injection. The framework will automatically provide the necessary dependencies.

### 3. Factory Injection

Factories can also be injected into your classes, allowing for dynamic creation of objects when needed.

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class ClassB : MonoBehaviour {
    [Inject] private ServiceA _serviceA;
    [Inject] private ServiceB _serviceB;
    
    private FactoryA _factoryA;
    
    [Inject] public void Init(FactoryA factory) {
        _factoryA = factory;
        Debug.Log("Initialized FactoryA in ClassB");
    }
}
```

Here, `ClassB` receives `ServiceA`, `ServiceB`, and a factory `FactoryA`. The factory allows for dynamic creation of additional services or objects at runtime.

### 4. Running the Injector

The injector must be initialized to register all providers and inject dependencies into your classes.

```csharp
using TCS.DependencyInjection.Lite;
using UnityEngine;

public class Bootstrap : MonoBehaviour {
    private void Awake() {
        Injector.Instance.Awake();  // Initialize the injector to start injecting services
    }
}
```

By calling `Injector.Instance.Awake()`, the system will scan for all providers and prepare to inject dependencies where needed.

## Advanced Configuration

You can extend or customize the injection process by subclassing the `Injector` class. This allows you to add custom logic or manage more complex dependency lifecycles.

## Example Project

The repository includes example classes demonstrating how to:
- Register services globally using `[Provide]`.
- Inject dependencies into fields and methods using `[Inject]`.
- Use factories for dynamic service creation.

## Contribution

Contributions are welcome! Please feel free to submit pull requests or open issues for any bugs or feature requests.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push the branch (`git push origin feature-branch`).
5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

Happy Coding!
