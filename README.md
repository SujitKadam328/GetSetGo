Game Flow Overview :
1. Splash Screen
	The game starts with a splash screen managed by SplashScreenManager which:
	-> Displays "Get Set Go" with animated dots
	-> Automatically transitions to main menu after a delay
	
2. Main Menu
	MainMenuManager handles the main menu interactions:
	-> Play button: Opens grid selection panel
	-> Exit button: Quits application

3. Grid Selection
	GridSelection provides three game modes:
	-> 2x2 (4 cards)
	-> 2x3 (6 cards)
	-> 5x6 (30 cards)
	Each selection initializes the game with different grid configurations.

4. Game Manager
	-> The core game logic is handled by GameManager which:
	-> Creates and positions cards based on selected grid size
	-> Manages card interactions and matching logic
	-> Handles scoring system
	-> Controls game state (save/load)
	-> Manages audio feedback
	
5. Card System
	Individual cards are managed by the Card class which:
	-> Handles card flipping animations
	-> Manages card states (matched, flipped)
	-> Processes player interactions
	-> Controls visual elements (front/back faces)
	
6. Audio Management
	AudioManager provides sound effects for:
	-> Card flipping
	-> Successful matches
	-> Mismatches
	-> Game completion
	
7. Save/Load System
	The game implements persistence through:
	-> Save functionality to store current game state
	-> Load functionality to restore previous game
	-> Automatic save button state management