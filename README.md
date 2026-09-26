# Realtime Movement Client

**A Unity Transport UDP client for a small 2D realtime movement prototype.** Local keyboard input moves a player object immediately; the client reports positions to the server and draws remote players as procedurally created circle proxies from relayed updates.

**Paired repository:** [realtime-movement-server](https://github.com/PapiChulllo/realtime-movement-server)

---

## How it works

1. `PlayerMovement` reads W / A / S / D, updates the local transform, and sends `PlayerMoved,<x>,<y>` on the reliable sequenced pipeline.
2. The server stores the position under the connection ID and broadcasts `PlayerMoved,<playerId>,<x>,<y>`.
3. `GameLogic` creates a circle `SpriteRenderer` proxy the first time it sees a player ID, then applies later positions.

**Status / limitations:** educational sample. Movement is **client-authoritative**. Destination IP is hardcoded to `10.0.0.82` in `NetworkClient.cs` — change it to `127.0.0.1` (localhost) or a LAN host before Play. No reconnect, interpolation, prediction, or proxy cleanup on disconnect. Same Unicode CSV / culture-sensitive float caveats as the server. No automated tests; Unity unavailable for re-verification in this documentation pass.

## Tech stack

| Area | What it uses |
|---|---|
| Engine | **Unity** `2022.3.5f1` |
| Networking | **Unity Transport** `1.3.4` |
| Endpoint | UDP port **9001**; IPv4 const currently `10.0.0.82` |
| Encoding | `Encoding.Unicode` + `int` length prefix |
| Input | Legacy `KeyCode` W/A/S/D in `PlayerMovement` |
| Scene | `Assets/Scenes/SampleScene.unity` |

Pipelines match the server (reliable sequenced used; fire-and-forget defined but unused).

## What's in the project

| System | Key files |
|---|---|
| Driver, endpoint, pipelines, framing, connect/send/teardown | `Assets/Scripts/NetworkClient.cs` |
| Server message parse → game logic | `Assets/Scripts/NetworkClientProcessing.cs` |
| Keyboard move + position report | `Assets/Scripts/PlayerMovement.cs` |
| Runtime circle sprites and per-ID proxy updates | `Assets/Scripts/GameLogic.cs` |
| Editor scene (Client, Player, camera) | `Assets/Scenes/SampleScene.unity` |

Four authored C# scripts (~9 KB). No external art packs; proxies are generated at runtime.

### Code / system highlights

- **`PlayerMovement`:** while a movement key is held, updates `transform.position` and sends a reliable `PlayerMoved` CSV through `NetworkClientProcessing`.
- **`GameLogic`:** `CreateCircleSprite()` builds a simple circle texture/sprite; first sighting of an ID instantiates a proxy GameObject; later messages move it.
- **`NetworkClient`:** `DontDestroyOnLoad`-style lifecycle pattern via static registration; connect events are logged; no automatic retry after disconnect.

## Scenes

| Scene | Purpose |
|---|---|
| `Assets/Scenes/SampleScene.unity` | Client Editor scene — Play only after the server is listening |

## Run (Editor)

1. Start [realtime-movement-server](https://github.com/PapiChulllo/realtime-movement-server) in Play mode first.
2. Edit `const string IPAddress` in `Assets/Scripts/NetworkClient.cs` (`127.0.0.1` or server LAN IP).
3. Open this repo in Unity **2022.3.5f1**, open `SampleScene`, enter Play mode, focus the Game view, use W/A/S/D.

No verified standalone build is claimed here.

## Third-party assets

Unity packages only (Transport, 2D, TextMesh Pro, etc.). Authored work is the four scripts above.

## About this repository

Public educational / portfolio showcase for Unity Transport client input and proxy sync under **PapiChulllo**. Pair with [realtime-movement-server](https://github.com/PapiChulllo/realtime-movement-server). Claims are limited to what the committed source shows.
