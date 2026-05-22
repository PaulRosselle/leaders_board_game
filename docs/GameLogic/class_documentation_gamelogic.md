# GameLogic — Class Documentation

## Enumerations

### `PlayPermission`
Defines the play permission of a character during a turn. Used to restrict or enforce character usage, notably during Nemesis interruptions.

### `TeamColor`
Identifies the team a character or player belongs to. Used as a key discriminator throughout the game logic.

### `CharacterType`
Enumerates every character in the game. Acts as the primary identifier for character-specific behavior, resolver selection, and card associations.

### `CharacterCard`
Enumerates the recruitable character cards. Differs from `CharacterType` in that some cards represent grouped characters (e.g. `HermitAndCub`).

### `Direction`
Represents the six possible directions on the hexagonal board. Used in movement resolution and neighbor queries.

### `AbilityType`
Categorizes the nature of a character's ability. Determines how and when an ability can be triggered.

### `GameMode`
Distinguishes game modes, each defining a different turn phase sequence and recruitment ruleset.

### `TurnPhaseType`
Enumerates every phase within a turn. Drives the phase sequencing logic and phase-dependent action availability.

### `GameActionType`
Categorizes every possible game action. Used by `IGameAction` to identify action types without relying solely on polymorphism.

---

## Entities

### `Position`
Represents a coordinate on the board. Serves as a lightweight value used by `Cell`, `CharacterContext`, and action details.

### `Cell`
Represents a single cell on the board. Holds its position and optionally the character currently occupying it.

### `Character`
Represents a character piece on the board. Identifies a character by its type and team color. Intentionally kept minimal — volatile data is held by `CharacterState`.

### `CharacterState`
Holds the runtime state of a character during a game. Tracks play permission, ability blockers, and transformation status. Kept separate from `Character` to isolate identity from mutable game state.

### `CharacterContext`
Aggregates a character, its current state, and its board position into a single object. Provided to external callers that need a complete picture of a character without accessing game internals directly.

### `Board`
Represents the game board as a collection of cells. Serves as the spatial data structure queried by `BoardQuery` and mutated by action handlers.

### `Player`
Represents a participant in the game. Holds identity, team color, and warning count. Intrinsically tied to a game session.

### `TurnPhase`
Represents an active phase within a turn, binding a phase type to the player currently responsible for it.

---

## Actions

### `IGameAction`
Defines the contract for all game actions. Every action carries an `ActionType` to allow identification and dispatch without downcasting.

### `PhaseChangeAction`
Represents a turn phase transition. Records the before and after phases.

### `CharacterActionDetail`
Captures the before and after state of a single character involved in a `CharacterAction`. Holds origin and destination positions along with full state snapshots to enable undo.

### `CharacterAction`
Represents a character performing an action on the board. Composed of a source detail and a list of target details.

### `RecruitmentAction`
Represents a player recruiting a character onto the board. Records the recruited character and its destination cell.

### `BanishmentAction`
Represents a player banishing a character card. Records the targeted card and the player who issued the banishment.

---

## Game

### `Game`
The central state of a game session. Aggregates all runtime data: players, board, character states, phase, recruitable cards, and action history. Mutated exclusively through action handlers.

---

## Handlers

### `IActionHandler`
Defines the contract for all action handlers. Every handler must support both execution and undo to ensure history consistency.

### `PhaseChangeActionHandler`
Handles phase transition actions. Responsible for updating the current phase in `Game` and triggering phase-dependent logic.

### `RecruitmentActionHandler`
Handles recruitment actions. Places a new character on the board and initializes its `CharacterState`.

### `BanishmentActionHandler`
Handles banishment actions. Removes a character card from the recruitable pool.

### `CharacterActionHandler` *(abstract)*
Base class for all character-specific action handlers. Provides the common execution skeleton (validate, apply, update states) via the Template Method pattern. Subclasses override the apply step for character-specific behavior.

---

## Resolvers

### `CharacterActionResolver` *(abstract)*
Base class for all character-specific action resolvers. Computes the list of valid movement and ability actions for a given character. Subclasses override resolution logic for characters with unique movement or ability rules.

---

## Factories

### `CharacterFactory`
Single point of creation for `Character` instances. Ensures each character is assigned a unique identity at creation time.

### `BoardFactory`
Constructs and initializes a `Board`. Responsible for the build of cells adjacent relationships.

### `GameFactory`
Constructs and initializes a `Game` with every subsequent fields ready to use.

### `CharacterActionResolverFactory`
Instantiates the appropriate `CharacterActionResolver` subclass for a given `CharacterType`. Enables per-character resolution logic without conditional branching in the caller.

### `CharacterActionHandlerFactory`
Instantiates the appropriate `CharacterActionHandler` subclass for a given `CharacterType`. Mirrors `CharacterActionResolverFactory` for the execution side.

---

## Queries

### `BoardQuery`
Provides stateless read-only queries over a `Board`. Centralizes board inspection logic shared across handlers, resolvers, and end condition checking. Kept static as it holds no state.

---

## Facade

### `GameHandler`
The single entry point for all external callers (UI, AI, network). Exposes high-level operations to query available actions and execute or undo them, without exposing internal handlers, resolvers, or game logic.
