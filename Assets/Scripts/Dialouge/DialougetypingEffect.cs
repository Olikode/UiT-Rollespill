using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialougetypingEffect : MonoBehaviour
{
    [SerializeField] private float dialougeSpeed = 50f;

    public bool IsRunning { get; private set; }

    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>()
    {
        {new HashSet<char>() {'.', '!', '?'}, 0.6f },
        {new HashSet<char>() {',', ';', ':'}, 0.3f },
    };

    private Coroutine typingCororoutine;

    public void Run(string textToType, TMP_Text textLabel)
    {
        typingCororoutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    public void Stop()
    {
        StopCoroutine(typingCororoutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true; 
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * dialougeSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i > textToType.Length - 1; 

                textLabel.text = textToType.Substring(0, i + 1);

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            

            yield return null;
        }
        IsRunning = false; 
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations)
        {
            if (punctuationCategory.Key.Contains(character))
            {
                waitTime = punctuationCategory.Value;
                return true;
            }
        }
        waitTime = default;
        return false; 
    }
}
