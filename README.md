# MO's IV Quest

A tiny top-down, turn-based clinical simulation. **MO** is a junior nurse on a night
shift at St. Pixel General. Each level = one patient, one IV cannulation, done in the
correct 12-step order. **Dr. Olayinka** (an AI preceptor) reviews the attempt and gives
feedback.

> Engine: **Unity 6000.4.0f1** (Unity 6.4 LTS) · 2D · C# · legacy Input.

---

## 1. Open the project

1. Open **Unity Hub** → **Add** → **Add project from disk**.
2. Select this folder: `Mo's IV Quest`.
3. Hub matches it to editor `6000.4.0f1`. Open it.
4. First open takes a few minutes — Unity rebuilds the `Library/` folder and imports
   the 2D packages listed in `Packages/manifest.json`. This is normal.

If Hub asks about a different editor version, install/use `6000.4.0f1`.

---

## 2. What's already built (the code)

All scripts live in `Assets/Scripts/`. They compile as-is and are wired together by
dragging references in the Inspector (see section 3).

| Script | Job |
|---|---|
| `ClinicalStep.cs` | The 12 steps as an enum (order = correct order) + names + which are mini-games |
| `GameManager.cs` | Singleton: score, sequence, errors, mini-game scores |
| `TurnManager.cs` | State machine + validates each action against the correct order |
| `ActionButton.cs` | Put on each of the 12 action buttons; fires its step |
| `MiniGameController.cs` | Runs vein-select / insertion-timing / sharps-drag mini-games |
| `VeinOption.cs` | Put on each vein choice button (set a quality 0-100) |
| `SharpsDraggable.cs` | Drag-the-needle-into-the-bin handler |
| `UIManager.cs` | Score text, Dr. Olayinka speech bubble, outcome screen |
| `PlayerController.cs` | MO's WASD/arrow-key movement |
| `AIFeedback.cs` | Builds the JSON summary; offline fallback now, Claude later |

The clinical sequence modelled (correct order):

1. Introduce & consent → 2. Hand hygiene → 3. Gather equipment → 4. Apply tourniquet →
5. **Vein selection (mini-game)** → 6. Don gloves → 7. Clean site → 8. **Insert cannula
(mini-game)** → 9. Confirm flashback → 10. Advance & release tourniquet → 11. Secure &
flush → 12. **Dispose sharps (mini-game)**.

---

## 3. Wire up the scene (one-time, ~30 min)

> You do this once inside the Unity Editor. Nothing here needs more coding.

1. **Scene**: `File → New Scene → Basic 2D`. Save it as `Assets/Scenes/Main.unity`.
2. **Managers**: Create empty GameObjects and add components:
   - `GameManager` (GameManager.cs)
   - `TurnManager` (TurnManager.cs)
   - `MiniGameController` (MiniGameController.cs)
   - `AIFeedback` (AIFeedback.cs)
   - `UIManager` (UIManager.cs)
3. **Canvas UI** (`GameObject → UI → Canvas`):
   - A **Text** for score → drag into `UIManager.scoreText`.
   - A **Panel** "SpeechBubble" with a child **Text** → drag into
     `UIManager.speechBubble` / `speechText`. (Optional `Image` for the portrait.)
   - 12 **Buttons** (one per step). On each: add `ActionButton`, pick its `step`,
     drag in `TurnManager`, and (optional) drag the button's label `Text`.
   - Three panels: **VeinPanel**, **InsertionPanel**, **SharpsPanel** → drag into the
     matching fields on `MiniGameController`. Start them disabled.
     - VeinPanel: 3 buttons, each with `VeinOption` (set quality, e.g. 90/55/20) and a
       reference to `MiniGameController`.
     - InsertionPanel: a track + a moving handle (`RectTransform`) → set
       `insertionHandle`. Player presses **Space** to lock it in.
     - SharpsPanel: a needle `Image` with `SharpsDraggable` + a bin `RectTransform`.
   - An **OutcomePanel** with a `Text` and `Image` → drag into `UIManager`
     (`outcomePanel`, `outcomeText`, `outcomeFace`, and the 3 face sprites).
4. **Wire TurnManager**: drag `UIManager`, `MiniGameController`, `AIFeedback` into it.
5. **MO**: drop MO's sprite in the scene, add `PlayerController`. (Optional `Rigidbody2D`
   set to *Gravity Scale 0* for physics movement.)
6. Press **Play**. Click the action buttons in order and watch Dr. Olayinka react.

---

## 4. Art & audio (free, CC0)

Drop sprites into `Assets/Sprites/`, audio into `Assets/Audio/`.

- **Kenney.nl** — RPG Urban Pack (indoor tiles), Game Icons (action buttons), UI audio.
- **itch.io** — search "Hospital Mini Pack", medical/nurse sprites.
- **Game-icons.net** — syringe/gloves/etc. button icons.
- **Freesound.org** — heart-monitor beep, success chime (filter CC0).
- **Dr. Olayinka portrait** — any free nurse/doctor sprite or an AI-generated face.

---

## 5. Wiring Dr. Olayinka to your Claude API (later)

Works **offline right now** via a rule-based fallback, so your demo never breaks.

When you're ready to use your Claude key from Genspark:

1. Copy `Assets/Resources/ai_config.sample.json` → `Assets/Resources/ai_config.json`.
2. Fill in your `apiKey`, and adjust `endpoint`/`model` if Genspark gives you a custom URL.
3. In the Inspector, tick **`AIFeedback.useLiveApi`**.
4. The request is built for Anthropic's Messages API. If Genspark hands you an
   OpenAI-compatible endpoint instead, tweak the headers/body in the clearly marked
   `WIRE YOUR CLAUDE API HERE` region of `AIFeedback.cs` (only a few lines).

`ai_config.json` is in `.gitignore` — **your key never gets committed.**

---

## 6. Build a playable demo

`File → Build Settings` → add the `Main` scene → choose **Windows** or **WebGL**
(both build modules are installed) → **Build**.

---

## 7. Learning outcomes (put these on the title screen)

After playing, the player can:
- Sequence the 12 core steps of peripheral IV cannulation.
- Identify suitable veins by visibility/palpability/location.
- Recognise common errors (skipping hand hygiene, contaminating site, poor sharps disposal).
- Recall the role of flashback confirmation and saline flush.
- Reflect on performance via AI-generated feedback.

---

## 8. If you fall behind (cut order)

✅ MO moves · ✅ 12-step sequence + scoring · ✅ outcome screen · ✅ AI feedback (your
differentiator — keep it) · ⚠️ mini-games (replace with button presses) · ⚠️ SFX ·
❌ multiple levels · ❌ animations.
