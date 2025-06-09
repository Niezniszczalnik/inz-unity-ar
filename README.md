# inz-unity-ar

This repository demonstrates a basic Unity script that connects to a WebSocket server and displays received messages in a UI Text element. The example works on mobile platforms.

## Setup

1. Create a new Unity project.
2. Copy `Assets/Scripts/WebSocketClient.cs` into your project's `Assets/Scripts` folder.
3. Install the `websocket-sharp` library (place `websocket-sharp.dll` in `Assets/Plugins`).
4. In your scene, create a UI Text object and assign it to the script's `outputText` field.
5. Set the `websocketUrl` field to your server's address (e.g., `ws://example.com:12345`).
6. Run the scene on a mobile device or in the editor to see incoming messages.
