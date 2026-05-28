# GameLogic — Class Documentation

## Enumerations

### `TeamColor`
Identifies the team a character or player belongs to. Used as a key discriminator throughout the game logic.

### `CharacterType`
Enumerates every character in the game. Acts as the primary identifier for character-specific behavior and resolver selection.

### `CharacterCard`
Enumerates the recruitable character cards. Differs from `CharacterType` in that some cards represent grouped characters (e.g. `HermitAndCub`).

### `GameMode`
Distinguishes game modes, each defining a different turn phase sequence and recruitment ruleset.

### `GameActionType`
Categorizes every possible game action. Used by `IGameAction` to identify action types without relying solely on polymorphism.

### `TransitionType`
Defines whether a transition action marks the beginning or the end of a segment.

### `TransitionTarget`
Identifies the segment targeted by a `TransitionAction`. Covers all `ISegment` implementations.

### `Direction`
Represents the six possible directions on the hexagonal board. Used in movement resolution and neighbor queries.

### `AbilityType`
Categorizes the nature of a character's ability. Determines how and when an ability can be triggered.

---

## Entities

### `Position`
Represents a coordinate on the board. Serves as a lightweight value used by `Cell` and action targets.

### `Cell`
Represents a single cell on the board. Holds its position and optionally the character currently occupying it. Used as well to convey a character alongside its position to external callers.

### `Character`
Represents a character piece on the board. Identifies a character by its type, team color, and a unique `Guid` assigned at creation. The `Guid` ensures stable identity across transformations and temporary removals from the board.

### `Board`
Represents the game board as a collection of cells. Serves as the spatial data structure queried by `BoardQuery` and mutated by action handlers.

### `Player`
Represents a participant in the game. Holds identity, team color, and warning count.

### `Player`
Represents a participant in the game. Holds identity, team color, warning count, the list of character cards the player has banished, and the list of characters explicitly recruited by the player via `RecruitmentAction`. Characters created through other means such as transformations or in-game spawning are not tracked here. Exposes a `GetCharacterCards()` method to derive the list of character cards currently controlled by the player from their recruited characters.

### `GameConfig`
Captures all information that defines a game before the first action is played. Holds the player list, the first player, the game mode, and the initial board placements as a list of `RecruitmentAction`. Persisted alongside `GameHistory` to allow full reconstruction of any game state.

### `GameHistory`
The single source of truth for a game session. Holds a `GameConfig` describing the initial state and an ordered list of `IHistoryEntry` representing everything that occurred during the game. Owned by `GameHandler`.

---

## History Structure

### `IHistoryEntry`
Defines the contract for top-level entries in `GameHistory`. Implemented by `Turn` and `BanishmentPhase`.

### `ISegment`
Defines the contract for any element that has an explicit start and end, represented as `IGameAction` markers. Implemented by `Turn` and `IPhase`.

### `IPhase`
Extends `ISegment` with an ordered list of `IGameAction`. Represents a discrete phase of play with a defined beginning, body, and end.

### `Turn`
Represents a full turn of play. Implements `IHistoryEntry` and `ISegment`. Holds the active team color and four ordered phases: `TurnStartPhase`, `ActionsPhase`, `RecruitmentPhase`, and `TurnEndPhase`.

### `TurnStartPhase`
Represents the opening phase of a turn. Implements `IPhase`. Holds actions triggered automatically at the start of a turn.

### `ActionsPhase`
Represents the main action phase of a turn. Implements `IPhase`. Holds the character actions played by the active player during their turn.

### `RecruitmentPhase`
Represents the recruitment phase of a turn. Implements `IPhase`. Holds the recruitment actions played during the turn.

### `TurnEndPhase`
Represents the closing phase of a turn. Implements `IPhase`. Holds actions triggered automatically at the end of a turn.

### `BanishmentPhase`
Represents a banishment phase. Implements both `IHistoryEntry` and `IPhase`. Intercalates between turns and holds the banishment actions played by a specific team color. May appear multiple times consecutively and is mandatory in certain game modes.

---

## Actions

### `IGameAction`
Defines the contract for all game actions. Every action carries a `GameActionType` to allow identification and dispatch without downcasting.

### `TransitionAction`
Represents a segment boundary marker. Carries a `TransitionType` (Start or End) and a `TransitionTarget` identifying which segment is being opened or closed. Used by the projection to track the current state of the game structure.

### `CharacterActionTarget`
Captures a single character involved as a target in a `CharacterAction`. Holds the character and nullable origin and destination positions, covering four cases: movement (P1 → P2), addition (null → P1), removal (P1 → null), and targeting without movement (null → null).

### `CharacterAction`
Represents a character performing an action on the board. Holds the initiating character as `SourceCharacter` and a list of `CharacterActionTarget`. Can represent either a movement or an active ability activation.

### `RecruitmentAction`
Represents a player recruiting a character onto the board. Records the recruited character and its destination cell.

### `BanishmentAction`
Represents a player banishing a character card. Records the targeted card and the team color of the player who issued the banishment.

---

## Game

### `Game`
The projection of the current game state. Derived entirely from `GameConfig` and `GameHistory`. Holds the board and lists related to the current recruitment state. Mutated exclusively through action handlers in response to changes in `GameHistory`. Never modified directly by external callers.

---

## Handlers

### `IActionHandler`
Defines the contract for all action handlers. Every handler must support both execution (`DoAction`) and undo (`UndoAction`) to ensure consistency between `GameHistory` and the `Game` projection.

### `TransitionActionHandler`
Handles transition actions. Responsible for updating the `Game` projection in response to segment boundary markers.

### `RecruitmentActionHandler`
Handles recruitment actions. Places a new character on the board. Accounts for game mode variations such as drawing from a shuffled subset of cards.

### `BanishmentActionHandler`
Handles banishment actions. Removes a character card from the recruitable pool.

### `CharacterActionHandler`
Handles character actions. Applies or reverts the positional changes described by each `CharacterActionTarget` in a `CharacterAction`. Fully concrete — character-specific behavior is handled at the resolver level, not the handler level.

---

## Resolvers

### `CharacterActionResolver` *(abstract)*
Base class for all character-specific action resolvers. Computes the list of valid movement and active ability actions for a given character. Subclasses override resolution logic for characters with unique movement or ability rules.

### `RecruitmentActionResolver`
Computes the list of valid recruitment actions for a given `CharacterCard`. Generates one `RecruitmentAction` per combination of associated character and available recruitment cell on the board.

---

## Factories

### `CharacterFactory`
Single point of creation for `Character` instances. Assigns a unique `Guid` at creation time. Also exposes a `Transform` method that produces a new instance with the same `Guid` but a different type and color, preserving identity across transformations.

### `BoardFactory`
Constructs and initializes a `Board`. Responsible for building cell adjacency relationships.

### `GameFactory`
Reconstructs a `Game` projection from a `GameConfig` and a `GameHistory`. Replays the history onto the initial state defined by the config to produce the current projection.

### `CharacterActionResolverFactory`
Instantiates the appropriate `CharacterActionResolver` subclass for a given `CharacterType`. Enables per-character resolution logic without conditional branching in the caller.

---

## Queries

### `BoardQuery`
Provides stateless read-only queries over a `Board`. Centralizes board inspection logic shared across handlers and resolvers. Kept static as it holds no state.

### `GameQuery`
Provides stateless read-only queries over a `Game`. Centralizes end condition checking. Kept static as it holds no state.

### `PlayabilityQuery`
Provides stateless read-only queries over a `Game` and `GameHistory` to determine character playability at a given point in time. Exposes whether a character can act, must act, or can use its active ability, by analysing the actions already played in the current turn. Kept static as it holds no state.

---

## Facade

### `GameHandler`
The single entry point for all external callers (UI, AI, network). Owns both the `GameHistory` and the `Game` projection. Exposes high-level operations to query available actions and execute or undo them. Coordinates appending to `GameHistory`, delegating to the appropriate handler, and managing the creation of `IHistoryEntry` and `TransitionAction` markers. Does not expose internal handlers or resolvers.
