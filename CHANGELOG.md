# CHANGELOG

## v0.1.8

- Feat: click to shoot.
- Fix: cloud will have consistent behaviour among all clients.

## v0.1.7

- Feat: add responsive Model.
- Feat: add blinking help in battle scene.
- Feat: set non-local player to gray.
- Feat: better welcome scene.
- Feat: add cloud.

## v0.1.6

Apply `UniStart` and `UniUtils`. Optimize code.

## v0.1.5

- Feat: show error message.
- Fix: canvas scale with screen size.

## v0.1.4

Use TryDeserialize to catch errors.

## v0.1.3

- Set `C++ compiler configuration` to `Release` instead of `Debug`.
- Fix: Player can't shoot.

## v0.1.2

- Fix: WebSocket client send empty object since JsonUtility can't serialize anonymous class.
- Fix: Server event should be deserialized twice.
- Fix: Make structures serializable.
- Fix: Set `C++ compiler configuration` to `Debug` instead of `Release` to avoid `Not implemented: Class::FromIl2CppType`. Deep dive this issue later.

## v0.1.1

- Add server url hint.
- Support copy/paste in WebGL.
- Allow run in background.

## v0.1.0

The initial release.
