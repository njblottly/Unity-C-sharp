# Baseline project exploration
Captured before extension; asset listings below exclude Assets/_Agent.
Unity 6000.3.9f1. Original scene: Assets/Scenes/LevelOne.unity. Original hierarchy: 15 root objects.
Legacy Input Manager only; activeInputHandler=0. No Input System package was installed. New code uses UnityEngine.Input only.

## Existing scripts (all 16 read before implementation)
- Assets/Scripts/Managers/RPGCameraManager.cs
- Assets/Scripts/Managers/RPGGameManager.cs
- Assets/Scripts/MonoBehaviours/Character.cs
- Assets/Scripts/MonoBehaviours/Consumable.cs
- Assets/Scripts/MonoBehaviours/DuckFollower.cs
- Assets/Scripts/MonoBehaviours/EnemyAnimationDriver.cs
- Assets/Scripts/MonoBehaviours/EnemyHitPoints.cs
- Assets/Scripts/MonoBehaviours/HealthBar.cs
- Assets/Scripts/MonoBehaviours/Inventory/Inventory.cs
- Assets/Scripts/MonoBehaviours/MovementController.cs
- Assets/Scripts/MonoBehaviours/Player.cs
- Assets/Scripts/MonoBehaviours/Slot.cs
- Assets/Scripts/MonoBehaviours/SpawnPoint.cs
- Assets/Scripts/RoundCameraPos.cs
- Assets/Scripts/ScriptableObjects/HitPoints.cs
- Assets/Scripts/ScriptableObjects/Item.cs

## Sprite sources and imported sprite counts
- Assets/Sprites/Companions/duck 1.png — 20 imported sprites
- Assets/Sprites/Enemies/EnemyIdle_1.png — 5 imported sprites
- Assets/Sprites/Enemies/EnemyWalk_1.png — 5 imported sprites
- Assets/Sprites/Health Bar/Topdown RPG 32x32 - UI Bars.PNG — 217 imported sprites
- Assets/Sprites/Inventory/Topdown RPG 32x32 - UI Bars_83.asset — 1 imported sprites
- Assets/Sprites/Inventory/Topdown RPG 32x32 - UI Bars_87.asset — 1 imported sprites
- Assets/Sprites/Objects/heart_animation.png — 9 imported sprites
- Assets/Sprites/Objects/hearts-and-coins32x32.png — 12 imported sprites
- Assets/Sprites/Objects/wheat-bounce.png — 4 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Bushes 1.1.PNG — 120 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Ground Tileset 1.1.png — 133 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Mushrooms.png — 24 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Nature Details.png — 48 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Rocks 1.1.PNG — 24 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Tree Stumps and Logs.png — 24 imported sprites
- Assets/Sprites/Outdoors/Topdown RPG 32x32 - Trees 1.2.PNG — 105 imported sprites
- Assets/Sprites/Player/player.png — 14 imported sprites
- Assets/TextMesh Pro/Sprites/EmojiOne.png — 0 imported sprites

## Prefabs
- Assets/Prefabs/CoinObject.prefab
- Assets/Prefabs/EnemyObject.prefab
- Assets/Prefabs/HealthBarObject.prefab
- Assets/Prefabs/HeartObject.prefab
- Assets/Prefabs/InventoryObject.prefab
- Assets/Prefabs/Manager Prefabs/RPGCameraManager.prefab
- Assets/Prefabs/PlayerObject.prefab
- Assets/Prefabs/PlayerSpawnPoint.prefab
- Assets/Prefabs/Slot.prefab
- Assets/TilePalettes/Outdoor Tiles.prefab

## Scenes
- Assets/Scenes/LevelOne.unity

## Data and animations
- Assets/Animations/Animations/coin-spin.anim
- Assets/Animations/Animations/duck-walk-east.anim
- Assets/Animations/Animations/duck-walk-idle.anim
- Assets/Animations/Animations/duck-walk-north.anim
- Assets/Animations/Animations/duck-walk-south.anim
- Assets/Animations/Animations/duck-walk-west.anim
- Assets/Animations/Animations/enemy-idle-1.anim
- Assets/Animations/Animations/enemy-walk-1.anim
- Assets/Animations/Animations/heart-animation.anim
- Assets/Animations/Animations/player-idle.anim
- Assets/Animations/Animations/player-walk-east.anim
- Assets/Animations/Animations/player-walk-north.anim
- Assets/Animations/Animations/player-walk-south.anim
- Assets/Animations/Animations/player-walk-west.anim
- Assets/Animations/Controllers/CoinController.controller
- Assets/Animations/Controllers/DuckController.controller
- Assets/Animations/Controllers/EnemyController.controller
- Assets/Animations/Controllers/HeartController.controller
- Assets/Animations/Controllers/PlayerController.controller
- Assets/Scripts/ScriptableObjects/Coin.asset
- Assets/Scripts/ScriptableObjects/Heart.asset
- Assets/Scripts/ScriptableObjects/HitPoints.asset

## Fonts
- Assets/Fonts/Adventurer.ttf
- Assets/Fonts/slkscr.ttf
- Assets/Fonts/slkscrb.ttf
- Assets/Fonts/slkscre.ttf
- Assets/Fonts/slkscreb.ttf
- Assets/TextMesh Pro/Fonts/LiberationSans.ttf

## Packages
{
  "dependencies": {
    "com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main",
    "com.unity.cinemachine": "3.1.5",
    "com.unity.feature.2d": "2.0.2",
    "com.unity.ide.rider": "3.0.39",
    "com.unity.ide.visualstudio": "2.0.26",
    "com.unity.multiplayer.center": "1.0.1",
    "com.unity.test-framework": "1.6.0",
    "com.unity.timeline": "1.8.10",
    "com.unity.ugui": "2.0.0",
    "com.unity.visualscripting": "1.9.9",
    "com.unity.modules.accessibility": "1.0.0",
    "com.unity.modules.adaptiveperformance": "1.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.androidjni": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.cloth": "1.0.0",
    "com.unity.modules.director": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.terrain": "1.0.0",
    "com.unity.modules.terrainphysics": "1.0.0",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.umbra": "1.0.0",
    "com.unity.modules.unityanalytics": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestassetbundle": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.unitywebrequestwww": "1.0.0",
    "com.unity.modules.vectorgraphics": "1.0.0",
    "com.unity.modules.vehicles": "1.0.0",
    "com.unity.modules.video": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.wind": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
  }
}


## Findings and decisions
- Reused Player, MovementController, DuckFollower, Inventory, HealthBar, RPGGameManager, RPGCameraManager, SpawnPoint, Consumable and enemy components.
- Added a KillCharacter override so Player damage reaches the existing Die/RespawnAt flow.
- Explicitly wired serialized references and removed indexed child lookups from Inventory.
- No baseline AudioClip assets; sound omitted.
- Original scene/assets retained. New placeholder geometry, material and coin icon are replaceable.
