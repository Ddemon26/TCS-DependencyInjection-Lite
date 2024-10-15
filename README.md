
# Dependency Injection Lite for Unity

![License](https://img.shields.io/github/license/Ddemon26/DependencyInjection-Lite)
![Issues](https://img.shields.io/github/issues/Ddemon26/DependencyInjection-Lite)
![Stars](https://img.shields.io/github/stars/Ddemon26/DependencyInjection-Lite)
![Forks](https://img.shields.io/github/forks/Ddemon26/DependencyInjection-Lite)

## Overview

**Dependency Injection Lite** is a lightweight, reflection-based dependency injection framework for Unity, designed to simplify the process of managing and injecting dependencies across various MonoBehaviour components.

The system leverages attributes like `[Inject]` and `[Provide]` to automatically handle dependency injection without manual setup, following your project's defined services and factories.

## Features

- **Field and Method Injection**: Automatically inject services into fields or methods using the `[Inject]` attribute.
- **Service Registration with Providers**: Register services globally using `[Provide]` within classes that implement `IDependencyProvider`.
- **Factory Support**: Factories can be injected and used to create instances dynamically.

## Installation

1. Clone or download the repository.
2. Add the `DependencyInjection-Lite` folder to your Unity project's `Assets` directory.
3. Ensure that your service and factory scripts are properly integrated.

## Usage

### 1. Defining and Registering Services

Create a class that implements `IDependencyProvider` and uses the `[Provide]` attribute to register services.

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class ProviderExample : MonoBehaviour, IDependencyProvider {
    [Provide]
    public ServiceA ProvideServiceA() => new ServiceA();
    
    [Provide]
    public ServiceB ProvideServiceB() => new ServiceB();
    
    [Provide]
    public FactoryA ProvideFactoryA() => new FactoryA();
}
```

In this example, services like `ServiceA`, `ServiceB`, and `FactoryA` are registered globally via the `[Provide]` attribute, making them available for injection.

### 2. Injecting Services into Components

You can inject these registered services into other MonoBehaviours using the `[Inject]` attribute.

#### Field Injection

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class ClassA : MonoBehaviour {
    [Inject]
    public ServiceA ServiceA;

    [Inject]
    public void InjectServiceB(ServiceA service) {
        ServiceA = service;
        Debug.Log(message: $"ClassA.InjectServiceB({service})");
    }
}
```

In this example, `ServiceA` is injected into the field, and an additional method `InjectServiceB` shows method injection for more flexible injection use cases.

#### Injecting Multiple Services and Factories

```csharp
using UnityEngine;
using TCS.DependencyInjection.Lite;

public class ClassB : MonoBehaviour {
    [Inject] private ServiceA m_serviceA;
    [Inject] private ServiceB m_serviceB;

    private FactoryA m_factoryA;

    [Inject]
    public void Init(FactoryA factory) {
        m_factoryA = factory;
        Debug.Log("Initialized FactoryA in ClassB");
    }
}
```

In `ClassB`, `ServiceA`, `ServiceB`, and `FactoryA` are injected into fields and methods. This allows the class to dynamically interact with the services and factories provided.

### 3. Initializing the Injector

The injector should be initialized early in the application's lifecycle, typically in a bootstrap class or in `Awake`.

```csharp
using TCS.DependencyInjection.Lite;
using UnityEngine;

public class Bootstrap : MonoBehaviour {
    private void Awake() {
        Injector.Instance.Awake();  // Initialize the injector to start injecting services
    }
}
```

By calling `Injector.Instance.Awake()`, the system is ready to inject services wherever the `[Inject]` attribute is used.

## Example Project

The repository includes examples such as `ClassA`, `ClassB`, and `ProviderExample` that demonstrate how to:
- Register services with `[Provide]`.
- Inject services into fields and methods using `[Inject]`.
- Use factories to create instances dynamically.

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
