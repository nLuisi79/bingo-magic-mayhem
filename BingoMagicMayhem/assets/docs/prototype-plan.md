# Bingo Magic Mayhem Prototype Plan

## Direction

- Engine: Cocos Creator 3.8.8
- Language: TypeScript
- Target: mobile landscape first, with web build support later
- First playable Realm: Whispering Fen

## First Playable Goal

Build a small, testable bingo match before adding the larger live-service systems.

1. Generate one bingo card.
2. Call numbers from B-1 through O-75 without repeats.
3. Mark matching card cells.
4. Detect a standard bingo win: rows, columns, and diagonals.
5. Show a simple status/reward screen.

## Early Systems To Keep Data-Driven

- Realms
- Rooms
- Cards and album sets
- Rewards
- Currencies
- Ingredients
- Power-ups
- Events

## Next Editor Steps

1. Create a new scene named `Prototype`.
2. Add a Canvas for a landscape layout.
3. Add a Label for status text.
4. Add a Button for calling the next number.
5. Attach `BingoPrototypeController` to an empty node.
6. Connect the Label to the controller's `Status Label` property.
7. Connect the Button click to `BingoPrototypeController.callNextNumber`.
