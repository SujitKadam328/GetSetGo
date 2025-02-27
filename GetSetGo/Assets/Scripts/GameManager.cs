using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Game Settings")]
    [SerializeField] GameObject m_gamePlayPanel = null;
    [SerializeField] GameObject m_mainMenuPanel = null;
    [SerializeField] bool m_isGameStarted = false;
    [SerializeField] bool m_isInputEnabled = false;
    [SerializeField] private Camera m_mainCamera;
    [Header("Card Data")]
    [SerializeField] private Card m_cardPrefab;
    [SerializeField] private Transform m_cardContainer;
    [SerializeField] private List<Card> m_cards = new List<Card>();
    [SerializeField] private List<Sprite> m_cardSprites;
    [Header("Game Data - Active Cards")]
    private Card m_firstSelectedCard;
    private Card m_secondSelectedCard;
    [Space]
    [Header("Audio Settings")]
    [SerializeField] private AudioManager m_audioManager;
    [Header("Score Settings")]
    private int m_score = 0;
    private int m_turns = 0;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_turnsText;
    [SerializeField] private TextMeshProUGUI m_gameOverText;
    [Header("Card Grid Settings")]
    [SerializeField] int m_numOfCardInRow = 2;
    [SerializeField] int m_numOfCardInColumn = 3;
    [Space(6)]
    [SerializeField] float m_cardWidth = 3.0f;
    [SerializeField] float m_cardHeight = 4.0f;
    [Space(6)]
    [SerializeField] float m_horizontalCardSpacing = 0.5f;
    [SerializeField] float m_verticalCardSpacing = 0.5f;
    [Header("UI Elements")]
    [SerializeField] private GameObject m_saveButton;
    [SerializeField] private GameObject m_loadButton;
    [SerializeField] private GameObject m_scorePanel;

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
        m_isGameStarted = true;
        // m_isInputEnabled = true;
    }

    public void InitializeGame(int a_rows, int a_columns)
    {
        m_gameOverText.gameObject.SetActive(false);
        m_saveButton.SetActive(true);
        m_loadButton.SetActive(true);
        m_scorePanel.SetActive(true);
        m_gamePlayPanel.SetActive(true);
        ClearBoard();
        CreateBoard(a_rows, a_columns);
    }

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
    }
    public void OnCardClicked(Card card)
    {
        // if (!m_isInputEnabled) return;

        if (card.m_isMatched || card.m_isFlipped) return;

        m_audioManager.PlayFlipSound();

        card.Flip();

        if (m_firstSelectedCard == null)
        {
            m_firstSelectedCard = card;
        }
        else
        {
            m_secondSelectedCard = card;

            StartCoroutine(CheckMatch());
        }
    }
    private IEnumerator CheckMatch()
    {
        m_turns++;
        UpdateScoreAndTurns();

        if (m_firstSelectedCard.m_cardId == m_secondSelectedCard.m_cardId)
        {
            m_firstSelectedCard.SetMatched();
            m_secondSelectedCard.SetMatched();
            m_score++;
            UpdateScoreAndTurns();
            
            // Check for game over
            if (IsGameComplete())
            {
                yield return new WaitForSeconds(1f); // Wait a moment before showing game over
                ShowGameOver();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            m_firstSelectedCard.Flip();
            m_secondSelectedCard.Flip();
        }

        m_firstSelectedCard = null;
        m_secondSelectedCard = null;
        m_isInputEnabled = true;
    }
    private bool IsGameComplete()
    {
        return m_cards.All(card => card.m_isMatched);
    }
    private void ShowGameOver()
    {
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
        m_firstSelectedCard = null;
        m_secondSelectedCard = null;
    }

    public void OnClickHome()
    {
        m_gamePlayPanel.SetActive(false);
        ClearBoard();
        m_mainMenuPanel.SetActive(true);
    }
    // Save/Load functionality
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
    }
    public void OnClickLoad()
    {
        // if (!PlayerPrefs.HasKey("SavedGame")) return;
        // string json = PlayerPrefs.GetString("SavedGame");
        // GameData loadedData = JsonUtility.FromJson<GameData>(json);
        // // Implement loading logic here
        // m_score = loadedData.score;
        // m_turns = loadedData.turns;

        if (!PlayerPrefs.HasKey("SavedGame")) return;
    
        string json = PlayerPrefs.GetString("SavedGame");
        GameData loadedData = JsonUtility.FromJson<GameData>(json);
    
        // Clear existing board
        ClearBoard();       
    
        // Calculate rows and columns from saved data
        int totalCards = loadedData.cardStates.Count;
        int rows = m_numOfCardInRow;
        int columns = m_numOfCardInColumn;
    
        // Recreate the board with saved state
        float cardWidth = m_cardWidth;
        float cardHeight = m_cardHeight;
    
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int index = row * columns + col;
                if (index >= totalCards) break;
                
                Vector3 position = new Vector3(
                    col * cardWidth + col * m_horizontalCardSpacing - (columns - 1) * cardWidth * 0.5f,
                    row * -cardHeight + row * -m_verticalCardSpacing + (rows - 1) * cardHeight * 0.5f,
                    0
                );
                
                CardState savedState = loadedData.cardStates[index];
                Card card = Instantiate(m_cardPrefab, position, Quaternion.identity, m_cardContainer);
                card.Initialize(savedState.id, m_cardSprites[savedState.id]);
                
                // Restore card state
                if (savedState.isMatched)
                {
                    card.SetMatched();
                }
                else if (savedState.isFlipped)
                {
                    card.Flip();
                }

                m_cards.Add(card);
            }
        }
        
        // Restore game state
        m_score = loadedData.score;
        m_turns = loadedData.turns;
        UpdateScoreAndTurns();
        
        // Ensure game panel is visible
        m_gamePlayPanel.SetActive(true);
        m_mainMenuPanel.SetActive(false);

        
    }

    void UpdateScoreAndTurns()
    {
        m_scoreText.text = m_score.ToString();
        m_turnsText.text = m_turns.ToString();
    }
}
[System.Serializable]
public class GameData
{
    public int score;
    public int turns;
    public List<CardState> cardStates;
}
[System.Serializable]
public class CardState
{
    public int id;
    public bool isFlipped;
    public bool isMatched;
}