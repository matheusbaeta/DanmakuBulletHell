# DanmakuBulletHell

DanmakuBulletHell is a bullet hell (danmaku) shooter game framework made with Unity. Bullet hell games are known for their visually stunning, challenging gameplay where players dodge intricate patterns of projectiles while battling enemies and bosses. This project provides a foundation for developing such games in Unity, including reusable systems for bullet patterns, enemy behaviors, collision detection, and more.

## Features

- **Flexible Bullet Pattern System:** Easily create complex bullet arrangements such as waves, spirals, and custom shapes.
- **Enemy and Boss Framework:** Manage enemies and multi-phase boss fights with customizable AI.
- **Collision Detection:** Precise collision handling for bullets, enemies, and the player ship.
- **Player Controls:** Responsive, fine-tuned movement and shooting mechanics.
- **Extensible Architecture:** Add new patterns, enemies, or features effortlessly thanks to a modular design.
- **Difficulty Scaling:** Customize bullet density, speed, and improve the challenge for all skill levels.

## Controls

- **Arrow Keys / WASD:** Move the player.
- **Z / Space:** Shoot.
- **Esc / P:** Pause or exit.

## Project Structure

```
DanmakuBulletHell/
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Sprites/
│   └── ...
├── ProjectSettings/
└── README.md
```

- `Scenes/`: Unity scene files.
- `Scripts/`: C# scripts for gameplay logic.
- `Prefabs/`: Reusable game objects.
- `Sprites/`: Art assets.

## Building the Game

1. In Unity, go to **File > Build Settings**.
2. Add the main scene to the build.
3. Choose your target platform (Windows, macOS, Linux, WebGL, etc.).
4. Click **Build** and select your output folder.

The built executable/app can now be distributed or played independently of the Unity editor.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

