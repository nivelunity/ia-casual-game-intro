using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

using HuggingFace.API;


public class GameManager: MonoBehaviour {
    [Header("Topics")]
    string[] topics = { "nice", "mean", "information" };
    public string currentTopic = "";

    [Header("Player Variables")]
    public static int playerScore = 0;
    public int highestScore = 0;

    [Header("Game UI")]
    public TMP_Text topic;
    public RawImage aiEmotion;
    public TMP_Text feedbackText;
    public TMP_InputField inputField;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text highestScoreText;
    public GameObject gameOverPanel;

    [Header("AI feedback face textures")]
    public Texture positive_1;
    public Texture positive_2;
    public Texture neutral;
    public Texture thinking_1;
    public Texture thinking_2;
    public Texture negative_1;
    public Texture negative_2;

    public Texture[] positive;
    public Texture[] thinking;
    public Texture[] negative;

    [Header("Game Settings")]
    public float timeLimit = 60f;
    public float timer = 60f;
    public int minTimeAddition = 3; // Minimum time added to the timer
    public float scoreClassificationThreshold = 0.50f; // Minimum label score to be considered as a good answer
    public int minLength = 10; // Minimum length of the input text to be accepted

    // Define an event for the end of the game
    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;

    // GAME MANAGEMENT

    /// <summary>
    /// Set a new turn
    /// - Remove the input text
    /// - Display Topic
    /// </summary>
    void NewTurn()
    {
        // Remove the input text
        inputField.text = "";
        DisplayTopic(topics);
    }

    // Method to trigger the Game Over event
    void TriggerGameOverEvent()
    {
        OnGameOver?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        positive = new Texture[] { positive_1, positive_2 };
        thinking = new Texture[] { thinking_1, thinking_2 };
        negative = new Texture[] { negative_1, negative_2 };
        highestScoreText.text = "0";
        NewGame();
    }

    /// <summary>
    /// Launch a new game
    /// </summary>
    public void NewGame()
    {
        // Focus input field
        inputField.Select();
        inputField.ActivateInputField();

        // Hide the Game Over Panel
        gameOverPanel.SetActive(false);

        // Empty the texts
        scoreText.text = "0";
        feedbackText.text = "";

        // Display the topic
        DisplayTopic(topics);

        // Start the countdown timer
        StartTimer();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
            inputField.Select();
            inputField.ActivateInputField();
        }

        // Check if the timer has reached zero
        if (timer > 0)
        {
            // Update the timer every frame
            UpdateTimer();
        }
        else
        {
            // Trigger the Game Over event
            TriggerGameOverEvent();
        }
    }


    // TIME MANAGEMENT

    /// <summary>
    /// Start the timer
    /// </summary>
    void StartTimer()
    {
        timer = timeLimit;
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Update the timer
    /// </summary>
    void UpdateTimer()
    {
        timer -= Time.deltaTime;

        // Display the remaining time on the UI
        timerText.text = Mathf.Round(timer).ToString();
    }

    // Coroutine for the countdown
    IEnumerator Countdown()
    {
        while (timer > 0)
        {
            yield return null;
        }

        // Trigger the Game Over event when the countdown is complete
        TriggerGameOverEvent();
    }

    /// <summary>
    /// Add additional time to the timer
    /// </summary>
    /// <param name="time"></param>
    void AddAdditionalTime(int time)
    {
        timer += time;
    }

    // SCORE MANAGEMENT

    /// <summary>
    /// Calculate the score
    /// The calculation of the score is simple
    /// score = +1 + Mathf.CeilToInt((length - 10) / 10);
    /// The idea is to give more points for longer answers
    /// The minimum length is 10 characters
    /// For example, an answer of 10 characters will give 1 point
    /// An answer of 20 characters will give 2 points
    /// </summary>
    void CalculateScore()
    {
        int score = 1 + Mathf.CeilToInt((inputField.text.Length - 10) / 10);
        AddScore(score);
    }

    /// <summary>
    /// Calculate the additional given when the answer is good
    /// The calculation of additional time is simple
    /// time = +3 + Mathf.CeilToInt((length - 10) / 10);
    /// The idea is to give more time for longer answers
    /// The minimum length is 10 characters
    /// For example, an answer of 10 characters will give +1s
    /// An answer of 20 characters will give +2s
    /// </summary>
    void CalculateAdditionalTime()
    {
        int additionalTime = minTimeAddition + Mathf.CeilToInt((inputField.text.Length - 10) / 10);
        AddAdditionalTime(additionalTime);
    }

    /// <summary>
    /// Add score to the player
    /// </summary>
    /// <param name="score"></param>
    void AddScore(int score)
    {
        playerScore += score;
        scoreText.text = playerScore.ToString();
        HighestScore(playerScore);
    }

    /// <summary>
    /// Update the highest score
    /// </summary>
    /// <param name="score"></param>
    void HighestScore(int score)
    {
        if (score > highestScore)
        {
            highestScore = score;
            highestScoreText.text = highestScore.ToString();
        }
    }


    // TOPIC MANAGEMENT

    /// <summary>
    /// Choose randomly from a list of topics
    /// </summary>
    /// <param name="topics"></param>
    /// <returns></returns>
    public string ChooseTopicRandomly(string[] topics) {
        int randomIndex = Random.Range(0, topics.Length);
        return topics[randomIndex];
    }

    /// <summary>
    /// Display the topic on the UI
    /// </summary>
    /// <param name="topics"></param>
    public void DisplayTopic(string[] topics) {
        string topic = ChooseTopicRandomly(topics);
        currentTopic = topic;

        if (currentTopic == "mean") {
            topic = "Bad <size=120> <sprite=15> </size>!";
        }
        else if (currentTopic == "nice") {
            topic = "Nice <size=120> <sprite=14> </size> !";

        }
        else if (currentTopic == "information") {
            topic = "Interesting!";
        }
        this.topic.text = topic;
    }

    // ROBOT FACE EMOTION MANAGEMENT

    /// <summary>
    /// Set the AI Emoji emotion based on the current topic
    /// </summary>
    /// <param name="currentTopic"></param>
    void SetAIEmojiEmotion(string currentTopic)
    {
        if (currentTopic == "nice")
        {
            DisplayEmotion(ChooseTextureRandomly(positive));
            DisplayFeedback("Nice! Keep going!");
        }

        else if (currentTopic == "mean")
        {
            DisplayEmotion(ChooseTextureRandomly(negative));
            DisplayFeedback("Oh!");
        }

        else if (currentTopic == "information")
        {
            DisplayEmotion(neutral);
            DisplayFeedback("Okay");
        }
    }

    /// <summary>
    /// When there's multiple versions of the same emotion texture, choose one randomly
    /// </summary>
    /// <param name="textures"></param>
    /// <returns></returns>
    public Texture ChooseTextureRandomly(Texture[] textures)
    {
        int randomIndex = Random.Range(0, textures.Length);
        Texture texture = textures[randomIndex];
        return texture;
    }

    /// <summary>
    /// Display the robot face emotion
    /// </summary>
    /// <param name="texture"></param>
    void DisplayEmotion(Texture texture)
    {
        aiEmotion.texture = texture;
    }


    // FEEDBACK MANAGEMENT

    /// <summary>
    /// Display text feedback
    /// </summary>
    /// <param name="feedback"></param>
    void DisplayFeedback(string feedback)
    {
        feedbackText.text = feedback;
    }


    // PLAYER INPUT MANAGEMENT

    /// <summary>
    /// Check the Input Length (input must be > 10 characters)
    /// </summary>
    /// <param name="userInput"></param>
    void CheckInputLength(string userInput) {
        if (userInput.Length < minLength) {
            Debug.Log("Your phrase is too short!");
            DisplayFeedback("Your phrase is too short! Write more than 10 characters!");
        }
        else {
            CheckUserResponse(userInput);
        }
    }

    /// <summary>
    /// Check input from input text field directly
    /// </summary>
    void CheckInput()
    {
        CheckInputLength(inputField.text);
    }

    /// <summary>
    /// Check the user response with the API Call
    /// </summary>
    /// <param name="playerInput"></param>
    void CheckUserResponse(string playerInput) {
        Query(playerInput);
    }

    /// <summary>
    /// Check if the topic is the same as the output classification and if the score is high enough
    /// </summary>
    /// <param name="outputLabel"></param>
    /// <param name="outputScore"></param>
    void CheckIfTopicEnough(string outputLabel, float outputScore) {
        // Check if the current topic is the same as the output classification
        if (outputLabel == currentTopic) {
            Debug.Log("OUTPUT SCORE" + outputScore);
            Debug.Log(scoreClassificationThreshold);
            // Check if the current topic score is high enough
            if (outputScore > scoreClassificationThreshold) {

                // Add the score and additional time
                CalculateScore();
                CalculateAdditionalTime();

                // Display the emotion
                SetAIEmojiEmotion(currentTopic);
                NewTurn();
            }
            else {
                DisplayEmotion(ChooseTextureRandomly(thinking));
                DisplayFeedback("Not enough!");
            }
        }
        else {
            DisplayEmotion(ChooseTextureRandomly(thinking));
            DisplayFeedback("Are you sure it's on the topic I asked?");
        }
    }

    /// <summary>
    /// API Call
    /// </summary>
    /// <param name="playerInput"></param>
    void Query(string playerInput) {
        Debug.Log("PLAYER INPUT  : " + playerInput);
        string inputText = playerInput;
        HuggingFaceAPI.ZeroShotTextClassification(inputText, outputClassification => {
            // On Success
            // The JSON always output the most probable label first
            CheckIfTopicEnough(outputClassification.classifications[0].label, outputClassification.classifications[0].score);
            Debug.Log(outputClassification.classifications[0].label + outputClassification.classifications[0].score);
            Debug.Log(outputClassification.classifications[1].label + outputClassification.classifications[1].score);
            Debug.Log(outputClassification.classifications[2].label + outputClassification.classifications[2].score);
        }, error => {
            // On Error
            Debug.Log("Error");
        }, topics);
    }
}
