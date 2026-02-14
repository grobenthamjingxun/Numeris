/*
* Author: Cheang Wei Cheng
* Date: 03/02/2026
* Description: This script is responsible for generating math questions and answers for the player to solve in each level.
* It supports different types of questions (arithmetic, geometry, fractions) based on the current scene name.
* The script also integrates with Firebase to allow for custom question retrieval, while maintaining procedural generation as a fallback.
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MathQuestionGenerator : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public string question;
        public float correctAnswer;
        public float[] wrongAnswers;
    }

    [Header("UI References")]
    [SerializeField] private TMP_Text questionText;

    [Header("Orb References")]
    [SerializeField] private RandomObjectSpawner orbSpawner;

    [Header("Target Selector Reference")]
    [SerializeField] private TargetSelector targetSelector;

    [Header("Firebase Integration")]
    [SerializeField] private FirebaseQuestionManager firebaseQuestionManager; // NEW

    [Header("Orb Tags & Text Components")]
    [SerializeField] private string correctOrbTag = "CorrectOrb";
    [SerializeField] private string wrongOrbTag = "WrongOrb";
    [SerializeField] private string orbTextComponentName = "AnswerText";

    [Header("Scene Configuration")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "BasicScene_SELevel";
    [SerializeField] private string level3SceneName = "WeiChengScene";

    [Header("Question Timing")]
    [SerializeField] private float questionSpawnDelay = 0.1f;

    private QuestionData currentQuestion;
    private string currentSceneName;
    private List<TMP_Text> orbTextComponents = new List<TMP_Text>();
    private bool hasInitializedOrbTexts = false;
    private bool isQuestionActive = false;
    private bool isInitializing = false;

    /// <summary>
    /// In Start(), the script determines the current scene name to decide the type of questions to generate.
    /// It also attempts to auto-assign references for the question text, orb spawner, target selector, and Firebase question manager,
    /// since these variables are in a different scene from the math question generator and will not be assigned in the inspector.
    /// </summary>
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (questionText == null)
        {
            questionText = GameObject.Find("Question").GetComponent<TMP_Text>();
        }
        if (orbSpawner == null)
        {
            orbSpawner = GameObject.Find("OrbSpawner").GetComponent<RandomObjectSpawner>();
        }
        if (targetSelector == null)
        {
            targetSelector = GameObject.Find("XR Origin (XR Rig)").GetComponent<TargetSelector>();
        }
        if (firebaseQuestionManager == null)
        {
            firebaseQuestionManager = GetComponent<FirebaseQuestionManager>();
            if (firebaseQuestionManager == null)
            {
                firebaseQuestionManager = FindFirstObjectByType<FirebaseQuestionManager>();
            }
        }

        if (targetSelector != null)
        {
            targetSelector.OnTargetCleared += ClearCurrentQuestion;
            targetSelector.OnTargetLocked += OnTargetLocked;
        }
    }

    /// <summary>
    /// This method is called when a target is locked by the TargetSelector.
    /// It checks if a question is already active or if the generator is currently initializing.
    /// If a question is active, it clears the current question before proceeding.
    /// It then spawns the orbs using the OrbSpawner and starts a coroutine to initialize the question after a short delay to ensure the orbs are spawned and their text components can be found.
    /// </summary>
    /// <param name="target"></param>
    public void OnTargetLocked(GameObject target)
    {
        if (isInitializing)
        {
            Debug.Log("Already initializing question, ignoring duplicate call");
            return;
        }

        if (isQuestionActive)
        {
            ClearCurrentQuestion();
        }

        if (orbSpawner != null)
        {
            orbSpawner.SpawnObjects();
            StartCoroutine(InitializeQuestionAfterSpawn());
        }
        else
        {
            Debug.LogError("Orb Spawner not assigned in MathQuestionGenerator!");
        }
    }

    /// <summary>
    /// This coroutine waits for a short delay after spawning the orbs to allow them to initialize
    /// It then attempts to find the text components on the orbs and generate a new question.
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeQuestionAfterSpawn()
    {
        isInitializing = true;
        yield return new WaitForSeconds(questionSpawnDelay);

        InitializeOrbTextComponents();

        if (hasInitializedOrbTexts && orbTextComponents.Count >= 4)
        {
            GenerateNewQuestion();
            isQuestionActive = true;
        }
        else
        {
            Debug.LogWarning("Failed to initialize orb texts for question generation");
        }

        isInitializing = false;
    }

    /// <summary>
    /// This method clears the current question and resets the state of the generator.
    /// It clears the question text, tells the orb spawner to clear any spawned orbs, and resets the list of orb text components.
    /// </summary>
    public void ClearCurrentQuestion()
    {
        if (isInitializing)
        {
            Debug.Log("Cannot clear question while initializing");
            return;
        }

        if (questionText != null)
        {
            questionText.text = "";
        }

        if (orbSpawner != null)
        {
            orbSpawner.ClearSpawnedObjects();
        }

        orbTextComponents.Clear();
        hasInitializedOrbTexts = false;
        isQuestionActive = false;

        Debug.Log("Question and orbs cleared");
    }

    public void InitializeOrbTextComponents()
    {
        orbTextComponents.Clear();

        GameObject[] correctOrbs = GameObject.FindGameObjectsWithTag(correctOrbTag);
        GameObject[] wrongOrbs = GameObject.FindGameObjectsWithTag(wrongOrbTag);

        List<GameObject> allOrbs = new List<GameObject>();
        allOrbs.AddRange(correctOrbs);
        allOrbs.AddRange(wrongOrbs);

        foreach (GameObject orb in allOrbs)
        {
            TMP_Text textComponent = orb.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                orbTextComponents.Add(textComponent);
            }
            else
            {
                Transform textTransform = orb.transform.Find(orbTextComponentName);
                if (textTransform != null)
                {
                    TMP_Text foundText = textTransform.GetComponent<TMP_Text>();
                    if (foundText != null)
                    {
                        orbTextComponents.Add(foundText);
                    }
                }
            }
        }

        hasInitializedOrbTexts = orbTextComponents.Count >= 4;

        if (!hasInitializedOrbTexts)
        {
            Debug.LogWarning($"Found only {orbTextComponents.Count} orb text components. Need 4.");
        }
        else
        {
            Debug.Log($"Successfully found {orbTextComponents.Count} orb text components.");
        }
    }

    /// <summary>
    /// Generates a new question - tries Firebase first, falls back to procedural generation
    /// </summary>
    public void GenerateNewQuestion()
    {
        if (!hasInitializedOrbTexts || orbTextComponents.Count < 4)
        {
            InitializeOrbTextComponents();

            if (orbTextComponents.Count < 4)
            {
                Debug.LogError("Cannot generate question: Need at least 4 orb text components.");
                return;
            }
        }

        if (firebaseQuestionManager != null)
        {
            QuestionData firebaseQuestion = firebaseQuestionManager.GetQuestion();
            if (firebaseQuestion != null)
            {
                currentQuestion = firebaseQuestion;
                Debug.Log("Using custom Firebase question");
                DisplayQuestion();
                return;
            }
        }
        // Fallback to procedurally generated questions
        currentQuestion = GenerateQuestionForScene(currentSceneName);
        Debug.Log("Using procedurally generated question");
        DisplayQuestion();
    }

    /// <summary>
    /// Display the current question on orbs
    /// </summary>
    private void DisplayQuestion()
    {
        // Display the question text
        if (questionText != null)
        {
            questionText.text = currentQuestion.question;
        }

        // Prepare answers array
        List<float> allAnswers = new List<float>();
        allAnswers.Add(currentQuestion.correctAnswer);
        allAnswers.AddRange(currentQuestion.wrongAnswers);

        // Find the orb with "CorrectOrb" tag
        GameObject correctOrb = FindCorrectOrb();
        TMP_Text correctOrbText = null;

        if (correctOrb != null)
        {
            correctOrbText = correctOrb.GetComponentInChildren<TMP_Text>();
            if (correctOrbText == null)
            {
                Transform textTransform = correctOrb.transform.Find(orbTextComponentName);
                if (textTransform != null)
                {
                    correctOrbText = textTransform.GetComponent<TMP_Text>();
                }
            }
        }

        if (correctOrbText != null)
        {
            // Assign correct answer to the correct orb
            correctOrbText.text = FormatAnswer(currentQuestion.correctAnswer);
            Debug.Log($"Correct answer assigned to {correctOrb.name}: {currentQuestion.correctAnswer}");

            // Assign wrong answers to the remaining orbs
            int wrongAnswerIndex = 0;
            for (int i = 0; i < Mathf.Min(orbTextComponents.Count, 4); i++)
            {
                if (orbTextComponents[i] == correctOrbText)
                    continue;

                if (wrongAnswerIndex < currentQuestion.wrongAnswers.Length &&
                    orbTextComponents[i] != null)
                {
                    orbTextComponents[i].text = FormatAnswer(currentQuestion.wrongAnswers[wrongAnswerIndex]);
                    wrongAnswerIndex++;
                }
            }
        }
        else
        {
            Debug.LogError("Could not find CorrectOrb or its text component! Using fallback shuffle.");

            ShuffleAnswers(allAnswers);

            for (int i = 0; i < Mathf.Min(orbTextComponents.Count, 4); i++)
            {
                if (orbTextComponents[i] != null)
                {
                    orbTextComponents[i].text = FormatAnswer(allAnswers[i]);
                }
            }
        }

        Debug.Log($"Question generated: {currentQuestion.question}");
        Debug.Log($"Correct answer: {currentQuestion.correctAnswer}");
    }

    /// <summary>
    /// This method finds the GameObject with the "CorrectOrb" tag and returns it. If multiple are found, it logs a warning and returns the first one. If none are found, it logs an error and returns null.
    /// This is used to ensure that the correct answer is assigned to the correct orb, even if the order of orbs in the scene changes.
    /// The method also includes a fallback search for a text component if the standard GetComponentInChildren<TMP_Text>() does not find it, to account for variations in how the orb prefab might be structured.
    /// </summary>
    /// <returns></returns>
    private GameObject FindCorrectOrb()
    {
        GameObject[] correctOrbs = GameObject.FindGameObjectsWithTag(correctOrbTag);

        if (correctOrbs.Length == 0)
        {
            Debug.LogError("No GameObject found with 'CorrectOrb' tag!");
            return null;
        }

        if (correctOrbs.Length > 1)
        {
            Debug.LogWarning($"Found {correctOrbs.Length} objects with 'CorrectOrb' tag. Using the first one.");
        }

        return correctOrbs[0];
    }

    /// <summary>
    /// Depending on the current scene name, this method generates a question of the appropriate type (arithmetic for level 1, geometry for level 2, fractions for level 3).
    /// If the scene name is not recognized, it defaults to generating an arithmetic question.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private QuestionData GenerateQuestionForScene(string sceneName)
    {
        QuestionData question = new QuestionData();
        question.wrongAnswers = new float[3];

        if (sceneName == level1SceneName)
            return GenerateArithmeticQuestion();
        else if (sceneName == level2SceneName)
            return GenerateGeometryQuestion();
        else if (sceneName == level3SceneName)
            return GenerateFractionQuestion();
        else
        {
            Debug.LogWarning($"Scene '{sceneName}' not recognized, defaulting to arithmetic");
            return GenerateArithmeticQuestion();
        }
    }

    /// <summary>
    /// This method generates a random arithmetic question (addition, subtraction, multiplication, or division) with two numbers.
    /// It calculates the correct answer and generates three wrong answers that are close to the correct answer.
    /// </summary>
    /// <returns></returns>
    private QuestionData GenerateArithmeticQuestion()
    {
        QuestionData question = new QuestionData();
        question.wrongAnswers = new float[3];

        int operation = Random.Range(0, 4);
        int num1, num2;
        float correctAnswer;

        switch (operation)
        {
            case 0: // Addition
                num1 = Random.Range(1, 50);
                num2 = Random.Range(1, 50);
                correctAnswer = num1 + num2;
                question.question = $"{num1} + {num2} = ?";
                break;

            case 1: // Subtraction
                num1 = Random.Range(20, 100);
                num2 = Random.Range(1, num1);
                correctAnswer = num1 - num2;
                question.question = $"{num1} - {num2} = ?";
                break;

            case 2: // Multiplication
                num1 = Random.Range(2, 12);
                num2 = Random.Range(2, 12);
                correctAnswer = num1 * num2;
                question.question = $"{num1} × {num2} = ?";
                break;

            case 3: // Division
                num2 = Random.Range(2, 10);
                correctAnswer = Random.Range(2, 10);
                num1 = num2 * (int)correctAnswer;
                question.question = $"{num1} ÷ {num2} = ?";
                break;

            default:
                return GenerateArithmeticQuestion();
        }

        question.correctAnswer = correctAnswer;
        question.wrongAnswers = GenerateWrongAnswers(correctAnswer, 3);
        return question;
    }

    /// <summary>
    /// This method generates a random geometry question (area or perimeter of rectangles/squares/circles, mass of combined boxes, or volume of a box).
    /// It calculates the correct answer and generates three wrong answers that are close to the correct answer.
    /// </summary>
    /// <returns></returns>
    private QuestionData GenerateGeometryQuestion()
    {
        QuestionData question = new QuestionData();
        question.wrongAnswers = new float[3];

        int problemType = Random.Range(0, 3);

        switch (problemType)
        {
            case 0:
                int shape = Random.Range(0, 3);
                if (shape == 0)
                {
                    float rectLength = Random.Range(3, 10);
                    float rectWidth = Random.Range(3, 10);
                    question.correctAnswer = rectLength * rectWidth;
                    question.question = $"Rectangle: length={rectLength}m, width={rectWidth}m. Area?";
                }
                else if (shape == 1)
                {
                    float side = Random.Range(4, 15);
                    question.correctAnswer = side * 4;
                    question.question = $"Square: side={side}cm. Perimeter?";
                }
                else
                {
                    float radius = Random.Range(2, 8);
                    question.correctAnswer = 3.14f * radius * radius;
                    question.question = $"Circle: radius={radius}m. Area? (π=3.14)";
                }
                break;

            case 1:
                float mass1 = Random.Range(100, 500);
                float mass2 = Random.Range(100, 500);
                question.correctAnswer = mass1 + mass2;
                question.question = $"Box A: {mass1}g. Box B: {mass2}g. Total mass?";
                break;

            case 2:
                float boxLength = Random.Range(3, 8);
                float boxWidth = Random.Range(3, 8);
                float height = Random.Range(3, 8);
                question.correctAnswer = boxLength * boxWidth * height;
                question.question = $"Box: {boxLength}cm × {boxWidth}cm × {height}cm. Volume?";
                break;
        }

        question.wrongAnswers = GenerateWrongAnswers(question.correctAnswer, 3);
        return question;
    }

    /// <summary>
    /// This method generates a random fraction question (addition, subtraction, or multiplication of fractions) with two fractions.
    /// It calculates the correct answer and generates three wrong answers that are close to the correct answer.
    /// </summary>
    /// <returns></returns>
    private QuestionData GenerateFractionQuestion()
    {
        QuestionData question = new QuestionData();
        question.wrongAnswers = new float[3];

        int problemType = Random.Range(0, 3);

        switch (problemType)
        {
            case 0:
                int denom = Random.Range(2, 9);
                int num1 = Random.Range(1, denom);
                int num2 = Random.Range(1, denom);
                int totalNum = num1 + num2;

                if (totalNum % 2 == 0 && denom % 2 == 0)
                {
                    question.correctAnswer = (float)(totalNum / 2) / (denom / 2);
                    question.question = $"{num1}/{denom} + {num2}/{denom} = ? (simplified)";
                }
                else
                {
                    question.correctAnswer = (float)totalNum / denom;
                    question.question = $"{num1}/{denom} + {num2}/{denom} = ?";
                }
                break;

            case 1:
                int denom2 = Random.Range(3, 9);
                int num3 = Random.Range(2, denom2);
                int num4 = Random.Range(1, num3);
                question.correctAnswer = (float)(num3 - num4) / denom2;
                question.question = $"{num3}/{denom2} - {num4}/{denom2} = ?";
                break;

            case 2:
                int num5 = Random.Range(1, 5);
                int denom5 = Random.Range(2, 6);
                int num6 = Random.Range(1, 5);
                int denom6 = Random.Range(2, 6);

                int finalNum = num5 * num6;
                int finalDenom = denom5 * denom6;

                if (finalNum % 2 == 0 && finalDenom % 2 == 0)
                {
                    question.correctAnswer = (float)(finalNum / 2) / (finalDenom / 2);
                    question.question = $"{num5}/{denom5} × {num6}/{denom6} = ? (simplified)";
                }
                else
                {
                    question.correctAnswer = (float)finalNum / finalDenom;
                    question.question = $"{num5}/{denom5} × {num6}/{denom6} = ?";
                }
                break;
        }

        question.wrongAnswers = GenerateWrongAnswers(question.correctAnswer, 3);
        return question;
    }

    /// <summary>
    /// This method generates a specified number of wrong answers that are close to the correct answer.
    /// It randomly decides to add or subtract a small offset from the correct answer to create wrong answers, and ensures that they are not approximately equal to the correct answer or to each other.
    /// </summary>
    /// <param name="correctAnswer"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private float[] GenerateWrongAnswers(float correctAnswer, int count)
    {
        float[] wrongAnswers = new float[count];

        for (int i = 0; i < count; i++)
        {
            float offset = Random.Range(1, 5);

            if (Random.value > 0.5f)
            {
                wrongAnswers[i] = correctAnswer + offset;
            }
            else
            {
                wrongAnswers[i] = Mathf.Max(0.1f, correctAnswer - offset);
            }

            while (Mathf.Approximately(wrongAnswers[i], correctAnswer) ||
                   ContainsValue(wrongAnswers, wrongAnswers[i], i))
            {
                wrongAnswers[i] += Random.Range(1, 3);
            }
        }

        return wrongAnswers;
    }

    private bool ContainsValue(float[] array, float value, int excludeIndex)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i != excludeIndex && Mathf.Approximately(array[i], value))
                return true;
        }
        return false;
    }

    /// <summary>
    /// This method shuffles the answers in the provided list using the Fisher-Yates shuffle algorithm.
    /// It is used to ensure that the correct answer is not always in the same position.
    /// </summary>
    /// <param name="answers"></param>
    private void ShuffleAnswers(List<float> answers)
    {
        for (int i = answers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            float temp = answers[i];
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }
    }

    /// <summary>
    /// This method formats the answer for display. If the answer is approximately an integer, it formats it without decimal places. Otherwise, it formats it to two decimal places and trims any trailing zeros.
    /// This ensures that answers are displayed in a clean and readable format on the orbs.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string FormatAnswer(float value)
    {
        if (Mathf.Approximately(value, Mathf.Round(value)))
        {
            return Mathf.Round(value).ToString();
        }
        else
        {
            return value.ToString("F2").TrimEnd('0').TrimEnd('.');
        }
    }

    public void GenerateQuestion()
    {
        GenerateNewQuestion();
    }

    public void OnOrbsSpawned()
    {
        InitializeOrbTextComponents();
        GenerateNewQuestion();
    }

    public string GetOrbText(int orbIndex)
    {
        if (orbIndex >= 0 && orbIndex < orbTextComponents.Count && orbTextComponents[orbIndex] != null)
        {
            return orbTextComponents[orbIndex].text;
        }
        return "";
    }

    public float GetCurrentCorrectAnswer()
    {
        return currentQuestion?.correctAnswer ?? 0f;
    }

    public bool CheckAnswer(float answer)
    {
        if (currentQuestion == null) return false;
        return Mathf.Approximately(answer, currentQuestion.correctAnswer);
    }

    private void OnDestroy()
    {
        if (targetSelector != null)
        {
            targetSelector.OnTargetCleared -= ClearCurrentQuestion;
            targetSelector.OnTargetLocked -= OnTargetLocked;
        }
    }
}