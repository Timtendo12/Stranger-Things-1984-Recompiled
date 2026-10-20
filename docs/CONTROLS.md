# Controls and display

The original touch and mouse controls still work: click to move, click a unit to attack or
interact. On top of that the port adds keyboard and gamepad support (`PCInput.cs`) and window
handling (`PCDisplay.cs`).

## In the game world

| Action | Keyboard | Gamepad (Xbox layout) |
|---|---|---|
| Move (tile steps; hold to keep walking) | WASD / arrow keys | left stick, d-pad |
| Attack / interact with the nearest unit | Space, Enter, Z, J | A |
| Special ability (Dustin's pudding, Mike's bike) | X, K, Left Ctrl | X |
| Switch to the previous character | Q, E, Tab | LB, RB |
| Pick unlocked character 1–3 | 1 / 2 / 3 | – |
| Bag | I, B | Y |
| Map | M | Back |
| Pause | Esc, P | Start |
| Advance dialogue | Space, Enter, Z, J, Q, E | A |

Ranged weapons fire in the direction you are facing when no target is nearby.

## Menus

The arrow keys, WASD, stick or d-pad move a yellow focus ring between buttons. It hides again
once you use the mouse.
- **Confirm:** Space, Enter, Z, J or A.
- **Cancel:** Backspace, X, Esc or B. Cancel closes the pause menu; on other pages it presses a
  close/cancel/back button. P or Start resumes from pause.

## Window

| Action | Keys |
|---|---|
| Toggle fullscreen | F11, Alt+Enter, or Ctrl+Cmd+F on macOS |

- The game starts windowed at 1280×720, and the window can be resized.
- Fullscreen is a borderless window at desktop resolution.
- The fullscreen state and window size are remembered between launches.
- On a screen or window that is not 16:9, the game renders pixel-exact into a centered 16:9 area
  with black bars.
- Command-line options `-screen-width`, `-screen-height` and `-screen-fullscreen` override the
  remembered settings for one launch.

## Known gaps

- **Gamepad.** Only tested with the Windows (XInput) button layout. On macOS and Linux, Unity's
  legacy input manager numbers buttons and d-pad axes differently, so some buttons may be
  mapped wrong there. Keyboard and mouse work on every platform.
- **Scrolling lists** cannot be scrolled with the keyboard or gamepad; use the mouse.
