# 🎵 TCS AudioManager

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black.svg?style=for-the-badge&logo=unity)
![Contributions welcome](https://img.shields.io/badge/Contributions-Welcome-brightgreen.svg?style=for-the-badge)
![GitHub License](https://img.shields.io/github/license/Ddemon26/TCS-AudioManager?style=for-the-badge)

![GitHub Forks](https://img.shields.io/github/forks/Ddemon26/TCS-AudioManager)
![GitHub Stars](https://img.shields.io/github/stars/Ddemon26/TCS-AudioManager)
![GitHub Issues](https://img.shields.io/github/issues/Ddemon26/TCS-AudioManager)
![GitHub Pull Requests](https://img.shields.io/github/issues-pr/Ddemon26/TCS-AudioManager)
![GitHub Last Commit](https://img.shields.io/github/last-commit/Ddemon26/TCS-AudioManager)
![GitHub Repo Size](https://img.shields.io/github/repo-size/Ddemon26/TCS-AudioManager)

[![Join our Discord](https://img.shields.io/badge/Discord-Join%20Us-7289DA?logo=discord&logoColor=white)](https://discord.gg/knwtcq3N2a)
![Discord](https://img.shields.io/discord/1047781241010794506)

---

# 🚀 Overview

**TCS AudioManager** is a sophisticated Unity tool that facilitates efficient and dynamic audio management within Unity projects. Whether you are developing a casual mobile application or a complex, immersive 3D experience, **TCS AudioManager** provides an extensive suite of tools required to manage intricate audio behaviors with ease.

This tool enables developers to seamlessly manage, play, adjust volume, and apply effects to a wide array of audio assets within Unity. Featuring event-driven audio playback, advanced volume control, and versatile integration capabilities, **TCS AudioManager** stands as an essential component in any developer's toolkit for creating immersive soundscapes.

## ✨ Key Features
- **Seamless Audio Management**: Provides a unified interface for managing all audio assets.
- **Volume Control**: Offers comprehensive control over audio channels, including music, sound effects (SFX), and voice.
- **Audio Effects**: Integrate either built-in or custom audio effects to tailor your audio experience.
- **Event-Driven Architecture**: Enable dynamic, responsive audio behaviors through integration with gameplay events.

## 📜 Table of Contents
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Customization](#customization)
- [Contributing](#contributing)
- [License](#license)

## 🏁 Getting Started

To utilize **TCS AudioManager** in your Unity project, follow these straightforward integration steps:

1. **Install Unity**: Ensure that Unity version 2022.3 or later is installed.
2. **Clone the Repository**: Clone or download this repository to your local development environment.
3. **Import to Unity**: Transfer the downloaded folder into the `Assets` directory of your Unity project.
4. **Open AudioManager**: Use the Unity menu `Tools > AudioManager` to access the editor interface and begin configuring your audio settings.

## 🔧 Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/Ddemon26/TCS-AudioManager.git
   ```

2. Import the folder into your Unity project's `Assets` directory.
3. Launch Unity, and **TCS AudioManager** should be accessible under the `Tools` menu.

## 🛠️ Usage

### Extension Methods for AudioClip Management

We employ **Extension Methods** to encapsulate the `AudioClip` component, enabling us to interact with audio functionalities while preserving the integrity of the core component. This methodology guarantees that internal modifications or enhancements to **TCS AudioManager** do not affect the manner in which developers interact with its API. The abstraction provided by extension methods ensures that the system remains intuitive and consistent, irrespective of underlying internal changes.

Below is an example illustrating how to leverage the available extension methods with `AudioClip`:

```csharp
using TCS.AudioManager;

public class ExampleUsage : MonoBehaviour
{
    public AudioClip myClip;

    void Start()
    {
        // Using the extension method to play the audio clip with specified parameters
        myClip.PlaySound(volume: 0.8f, pitch: 1f, loop: false, fadeInDuration: 0.5f, fadeOutDuration: 1f);
        
        // Using the extension method to play spatial sound
        Vector3 soundPosition = new Vector3(0, 0, 0);
        myClip.PlaySpatialSound(soundPosition, volume: 1f, pitch: 1f, loop: false);
        
        // Using the extension method to play music
        myClip.PlayMusic(volume: 0.7f, pitch: 1f, loop: true);
    }
}
```

In this example, methods such as `PlaySound()`, `PlaySpatialSound()`, and `PlayMusic()` extend the default functionality of Unity's audio components, adding capabilities like volume modulation, spatial effects, and fade durations, while preserving a user-friendly API.

### Basic Audio Management
- Utilize extension methods to efficiently manage and manipulate audio clips.
- Leverage the `AudioManager` class to control your project's audio landscape.
- Easily manipulate different categories, such as Music, SFX, and Voice.

### Volume Control
- Adjust volume levels dynamically using `AudioManager.SetVolume(category, level)`.
- Make use of predefined categories, such as *Master*, *Music*, and *SFX*, to maintain consistency.

### Event-Driven Audio
- Link audio playback to gameplay events for a more immersive experience.
- Example: Trigger background music when a player enters a specific area.

### Audio Effects
- Apply audio effects using the `AudioEffects` class, such as echo and reverb.
- Extend the existing classes to create custom effects tailored to your game's aesthetic.

## ✨ Features
- **Data Control for Saving and Loading**: Implements a robust data control system to save and load audio settings, enabling users to retain their preferences across gaming sessions.
- **Centralized Volume Control Hub**: A comprehensive volume control hub that facilitates easy adjustments across different audio categories, ensuring uniformity in volume management.
- **Audio Management**: Efficiently organize and manage a diverse range of audio assets.
- **Volume Control**: Adjust volumes dynamically during gameplay, covering all key aspects such as music, effects, and dialogue.
- **Audio Effects**: Incorporate built-in effects or develop customized logic to enrich the audio experience.
- **Event-Driven System**: Seamlessly integrates with Unity events, allowing for dynamic and context-sensitive audio behaviors.

## ⚙️ Customization

- **Custom Effects**: Extend the `AudioEffects` class to craft unique and personalized effects.
- **Custom Audio Events**: Define bespoke audio events to align precisely with specific gameplay mechanics.
- **Volume Adjustments**: Tailor the `IVolumeControl` logic to accommodate unique volume requirements specific to your game's experience.

## 🤝 Contributing

We welcome contributions from the community! Here is how you can get involved:

1. **Fork this repository**.
2. **Create a new branch** for your feature (`git checkout -b feature/NewFeature`).
3. **Commit your changes** (`git commit -m 'Add new feature'`).
4. **Push to the branch** (`git push origin feature/NewFeature`).
5. **Create a pull request** to merge your feature into the main branch.

Feel free to open an issue if you encounter any bugs or have suggestions for new features.

## 📄 License

This project is licensed under the MIT License. For more information, see the [LICENSE](LICENSE) file.

---

## 🌐 Stay Connected

Join our community on [Discord](https://discord.gg/knwtcq3N2a) to discuss features, report bugs, or share your thoughts!

![Banner Image](https://via.placeholder.com/1000x300.png?text=TCS+AudioManager+for+Unity)
