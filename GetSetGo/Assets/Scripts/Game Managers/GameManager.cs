using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager
    /// </summary>
    public static GameManager Instance;
    [Header("Game Settings")]
    [SerializeField] GameObject m_gamePlayPanel = null;        // Panel containing gameplay UI elements
    [SerializeField] GameObject m_mainMenuPanel = null;        // Panel containing main menu UI elements
    [SerializeField] bool m_isInputEnabled = false;            // Flag to control player input
    [SerializeField] private Camera m_mainCamera;              // Reference to main game camera
    [Header("Card Data")]
    [SerializeField] private Card m_cardPrefab;                // Prefab for card objects
    [SerializeField] private Transform m_cardContainer;         // Parent transform for spawned cards
    [SerializeField] private List<Card> m_cards = new List<Card>();        // List of all active cards
    [SerializeField] private List<Sprite> m_cardSprites;       // Available card face sprites
   [Header("Game Data - Active Cards")]
    private List<Card> m_firstSelectedCards = new List<Card>();     // Cards selected as first choice
    private List<Card> m_secondSelectedCards = new List<Card>();    // Cards selected as second choice
    [Space]
    [Header("Audio Settings")]
    [SerializeField] private AudioManager m_audioManager;      // Reference to audio management system
    [Header("Score Settings")]
    private int m_score = 0;                                   // Current game score
    private int m_turns = 0;                                   // Number of turns taken
    [SerializeField] private TextMeshProUGUI m_scoreText;      // UI text for score display
    [SerializeField] private TextMeshProUGUI m_turnsText;      // UI text for turns display
    [SerializeField] private TextMeshProUGUI m_gameOverText;   // UI text for game over message
    [Header("Card Grid Settings")]
    [SerializeField] int m_numOfCardInRow = 2;                // Number of cards per row
    [SerializeField] int m_numOfCardInColumn = 3;             // Number of cards per column
    [Space(6)]
    [SerializeField] float m_cardWidth = 3.0f;                // Width of each card
    [SerializeField] float m_cardHeight = 4.0f;               // Height of each card
    [Space(6)]
    [SerializeField] float m_horizontalCardSpacing = 0.5f;    // Horizontal spacing between cards
    [SerializeField] float m_verticalCardSpacing = 0.5f;      // Vertical spacing between cards
    [Header("UI Elements")]
    [SerializeField] private GameObject m_saveButton;          // Button to save game state
    [SerializeField] private GameObject m_loadButton;          // Button to load saved game
    [SerializeField] private GameObject m_scorePanel;          // Panel displaying score information

    // Singleton pattern: Ensures only one instance of GameManager exists
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UpdateLoadButtonState();
    }

    /// <summary>
    /// Initializes a new game with specified grid dimensions
    /// </summary>
    /// <param name="a_rows">Number of rows in the card grid</param>
    /// <param name="a_columns">Number of columns in the card grid</param>
    public void InitializeGame(int a_rows, int a_columns)
    {
        m_gameOverText.gameObject.SetActive(false);
        m_saveButton.SetActive(true);
        UpdateLoadButtonState();
        m_scorePanel.SetActive(true);
        m_gamePlayPanel.SetActive(true);
        ClearBoard();
        CreateBoard(a_rows, a_columns);

    }

    /// <summary>
    /// Creates and positions cards in a grid layout and adjusts camera position
    /// </summary>
    /// <param name="a_rows">Number of rows in the grid</param>
    /// <param name="a_columns">Number of columns in the grid</param>
    private void CreateBoard(int a_rows, int a_columns)
    {
        int l_pairsCount = (a_rows * a_columns) / 2;
        List<int> cardIds = new List<int>();
        // Create pairs of cards
        for (int i = 0; i < l_pairsCount; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }
        // Shuffle the cards
        cardIds = cardIds.OrderBy(x => Random.value).ToList();
        // Create and position cards
        float cardWidth = m_cardWidth;
        float cardHeight = m_cardHeight;
        for (int row = 0; row < a_rows; row++)
        {
            for (int col = 0; col < a_columns; col++)
            {
                int index = row * a_columns + col;
                Vector3 position = new Vector3(
                    col * cardWidth + col * m_horizontalCardSpacing - (a_columns - 1) * cardWidth * 0.5f,
                    row * -cardHeight + row * -m_verticalCardSpacing + (a_rows - 1) * cardHeight * 0.5f,
                    0
                );
                Card card = Instantiate(m_cardPrefab, position, Quaternion.identity, m_cardContainer);
                card.Initialize(cardIds[index], m_cardSprites[cardIds[index]]);
                m_cards.Add(card);
            }
        }

        // Calculate grid dimensions
        float gridWidth = a_columns * (m_cardWidth + m_horizontalCardSpacing) - m_horizontalCardSpacing;
        float gridHeight = a_rows * (m_cardHeight + m_verticalCardSpacing) - m_verticalCardSpacing;

        // Calculate required camera distance for 60 degree FOV
        float fov = 60f * Mathf.Deg2Rad;
        float aspect = m_mainCamera.aspect;

        // Calculate distance needed to fit both width and height
        float distanceWidth = gridWidth / (2f * Mathf.Tan(fov * 0.5f * aspect));
        float distanceHeight = gridHeight / (2f * Mathf.Tan(fov * 0.5f));
        float requiredDistance = Mathf.Max(distanceWidth, distanceHeight);

        // Calculate the center of the grid
        Vector3 firstCardPosition = m_cards[0].transform.position;
        Vector3 lastCardPosition = m_cards[m_cards.Count - 1].transform.position;
        Vector3 center = (firstCardPosition + lastCardPosition) * 0.5f;

        // Set camera position with the calculated distance
        m_mainCamera.transform.DOMove(new Vector3(center.x, center.y, -requiredDistance), 1f);
        StartCoroutine(PreviewCard());
    }
    /// <summary>
    /// Handles card click events and manages card selection logic
    /// </summary>
    /// <param name="card">The card that was clicked</param>
    public void OnCardClicked(Card card)
    {
        if (!m_isInputEnabled) return;

        if (card.m_isMatched || card.m_isFlipped) return;

        m_audioManager.PlayFlipSound();
        card.Flip();

        if (m_firstSelectedCards.Count <= m_secondSelectedCards.Count)
        {
            m_firstSelectedCards.Add(card);
        }
        else
        {
            m_secondSelectedCards.Add(card);
            StartCoroutine(CheckMatch());
        }
    }
    /// <summary>
    /// Checks if selected cards match and updates game state accordingly
    /// </summary>
    private IEnumerator CheckMatch()
    {
        m_turns++;
        UpdateScoreAndTurns();

        // Get the last cards from both lists
        Card firstCard = m_firstSelectedCards[m_firstSelectedCards.Count - 1];
        Card secondCard = m_secondSelectedCards[m_secondSelectedCards.Count - 1];

        if (firstCard.m_cardId == secondCard.m_cardId)
        {
            m_audioManager.PlayMatchSound();
            firstCard.SetMatched();
            secondCard.SetMatched();
            m_score++;
            UpdateScoreAndTurns();
            
            if (IsGameComplete())
            {
                yield return new WaitForSeconds(1f);
                ShowGameOver();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            m_audioManager.PlayMismatchSound();
            yield return new WaitForSeconds(0.5f);
            firstCard.Flip();
            secondCard.Flip();
        }

        m_isInputEnabled = true;
    }
    /// <summary>
    /// Checks if all cards have been matched
    /// </summary>
    /// <returns>True if game is complete, false otherwise</returns>
    private bool IsGameComplete()
    {
        return m_cards.All(card => card.m_isMatched);
    }
    /// <summary>
    /// Displays game over screen with final score and cleanup
    /// </summary>
    private void ShowGameOver()
    {
        m_audioManager.PlayWinSound();
        m_gameOverText.gameObject.SetActive(true);
        m_saveButton.SetActive(false);
        m_loadButton.SetActive(false);
        m_scorePanel.SetActive(false);
        
        m_gameOverText.text = $"Game Over!\nScore: {m_score}\nTurns: {m_turns}\n\nClick Home \nfor Main Menu";

        foreach (Card card in m_cards)
        {
            Destroy(card.gameObject);
        }
        m_cards.Clear();
    }
    /// <summary>
    /// Resets the game board and all game state variables
    /// </summary>
    private void ClearBoard()
    {
        m_gameOverText.gameObject.SetActive(false);
        m_saveButton.SetActive(true);
        m_loadButton.SetActive(true);
        m_scorePanel.SetActive(true);
        foreach (Card card in m_cards)
        {
            Destroy(card.gameObject);
        }
        m_cards.Clear();
        m_score = 0;
        m_turns = 0;
        m_scoreText.text = "0";     
        m_turnsText.text = "0";
        m_firstSelectedCards.Clear();
        m_secondSelectedCards.Clear();
    }

    /// <summary>
    /// Returns to main menu and clears the game board
    /// </summary>
    public void OnClickHome()
    {
        m_gamePlayPanel.SetActive(false);
        ClearBoard();
        m_mainMenuPanel.SetActive(true);
    }
    /// <summary>
    /// Saves current game state to PlayerPrefs in JSON format
    /// </summary>
    public void OnClickSave()
    {
        GameData saveData = new GameData
        {
            score = this.m_score,
            turns = this.m_turns,
            cardStates = m_cards.Select(c => new CardState
            {
                id = c.m_cardId,
                isFlipped = c.m_isFlipped,
                isMatched = c.m_isMatched
            }).ToList()
        };
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SavedGame", json);
        PlayerPrefs.Save();
        
        UpdateLoadButtonState();
    }
    /// <summary>
    /// Loads and reconstructs game state from saved JSON data
    /// </summary>
    public void OnClickLoad()
    {
        if (!PlayerPrefs.HasKey("SavedGame")) return;

        string json = PlayerPrefs.GetString("SavedGame");
        GameData loadedData = JsonUtility.FromJson<GameData>(json);

        // Clear existing board
        ClearBoard();       

        int totalCards = loadedData.cardStates.Count;
        
        // Determine grid size based on total cards
        int rows, columns;
        if (totalCards == 4) // 2x2 grid
        {
            rows = 2;
            columns = 2;
        }
        else if (totalCards == 6) // 2x3 grid
        {
            rows = 2;
            columns = 3;
        }
        else if (totalCards == 30) // 5x6 grid
        {
            rows = 5;
            columns = 6;
        }
        else
        {
            Debug.LogError("Invalid number of cards in saved game");
            return;
        }

        // Update class variables to match current grid size
        m_numOfCardInRow = rows;
        m_numOfCardInColumn = columns;

        // Calculate spacing and offsets
        float totalWidth = columns * m_cardWidth + (columns - 1) * m_horizontalCardSpacing;
        float totalHeight = rows * m_cardHeight + (rows - 1) * m_verticalCardSpacing;
        float startX = -totalWidth / 2f + m_cardWidth / 2f;
        float startY = totalHeight / 2f - m_cardHeight / 2f;

        // Create and position cards
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= totalCards) break;

                // Calculate position
                float xPos = startX + col * (m_cardWidth + m_horizontalCardSpacing);
                float yPos = startY - row * (m_cardHeight + m_verticalCardSpacing);
                Vector3 position = new Vector3(xPos, yPos, 0);

                // Create card
                CardState savedState = loadedData.cardStates[index];
                Card card = Instantiate(m_cardPrefab, position, Quaternion.identity, m_cardContainer);
                card.Initialize(savedState.id, m_cardSprites[savedState.id]);
                
                if (savedState.isMatched)
                {
                    card.SetMatched();
                }
                
                if (savedState.isFlipped)
                {
                    card.FlipOnLoad();
                }

                m_cards.Add(card);
            }
        }

        // Calculate camera position
        float padding = 1.1f; // Add some padding around the grid
        float fov = 60f * Mathf.Deg2Rad;
        float aspect = m_mainCamera.aspect;

        // Calculate required distance to fit both width and height
        float distanceWidth = (totalWidth * padding) / (2f * Mathf.Tan(fov * 0.5f * aspect));
        float distanceHeight = (totalHeight * padding) / (2f * Mathf.Tan(fov * 0.5f));
        float requiredDistance = Mathf.Max(distanceWidth, distanceHeight);

        // Set camera position
        m_mainCamera.transform.DOMove(new Vector3(0, 0, -requiredDistance), 1f);
        
        // Restore game state
        m_score = loadedData.score;
        m_turns = loadedData.turns;
        UpdateScoreAndTurns();
        
        // Ensure game panel is visible
        m_gamePlayPanel.SetActive(true);
        m_mainMenuPanel.SetActive(false);

        // Clear the saved game data
        PlayerPrefs.DeleteKey("SavedGame");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Updates the score and turns display in the UI
    /// </summary>
    void UpdateScoreAndTurns()
    {
        m_scoreText.text = m_score.ToString();
        m_turnsText.text = m_turns.ToString();
    }

    /// <summary>
    /// Updates the visibility of the load button based on saved game data
    /// </summary>
    private void UpdateLoadButtonState()
    {
        if (m_loadButton != null)
        {
            m_loadButton.SetActive(PlayerPrefs.HasKey("SavedGame"));
        }
    }
    
    /// <summary>
    /// Shows cards briefly at start of game for player preview
    /// </summary>
    IEnumerator PreviewCard()
    {
        m_isInputEnabled = false;
        yield return new WaitForSeconds(0.5f);
        foreach (Card card in m_cards){
            card.Flip();
            yield return new WaitForSeconds(0.06f);
        }
        yield return new WaitForSeconds(1f);

        foreach (Card card in m_cards){
            card.Flip();
            yield return new WaitForSeconds(0.06f);
        }
        m_isInputEnabled = true;
    }
}
/// <summary>
/// Data structure for saving game state
/// </summary>
[System.Serializable]
public class GameData
{
    public int score;              // Current game score
    public int turns;              // Number of turns taken
    public List<CardState> cardStates;  // State of each card in the game
}
/// <summary>
/// Data structure for individual card state
/// </summary>
[System.Serializable]
public class CardState
{
    public int id;                 // Card identifier
    public bool isFlipped;         // Whether card is face up
    public bool isMatched;         // Whether card has been matched
}