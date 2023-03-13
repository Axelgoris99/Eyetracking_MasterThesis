using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;

public class WordRecognizer : MonoBehaviour
{
    // Speech inputs
    [SerializeField]
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Events to throw when recognizing words
    #region events
    public delegate void GrabAction();
    public static event GrabAction onGrab;

    public delegate void ReleaseAction();
    public static event ReleaseAction onRelease;

    public delegate void RotationAxis(Vector3 axis);
    public static event RotationAxis onRotationAxisChanged;

    public delegate void RotationDirection();
    public static event RotationDirection onRotationDirectionChanged;

    public delegate void RotationMode();
    public static event RotationMode onRotationEnabled;

    public delegate void TranslationMode();
    public static event TranslationMode onTranslationEnabled;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Keywords added and actions as well
        #region action and keywords
        keywords.Add("attrape", () =>
        {
            Debug.Log("Grabbed");
            if (onGrab != null)
            {
                onGrab();
            }
        });
        keywords.Add("lache", () =>
        {
            Debug.Log("Release");
            if (onRelease != null)
            {
                onRelease();
            }
        });

        keywords.Add("X", () =>
        {
            Debug.Log("X");
            if (onRotationAxisChanged != null)
            {
                onRotationAxisChanged(new Vector3(1.0f, 0.0f, 0.0f));
            }
        });
        keywords.Add("Y", () =>
        {
            Debug.Log("Y");
            if (onRotationAxisChanged != null)
            {
                onRotationAxisChanged(new Vector3(0.0f, 1.0f, 0.0f));
            }
        });
        keywords.Add("Z", () =>
        {
            Debug.Log("Z");
            if (onRotationAxisChanged != null)
            {
                onRotationAxisChanged(new Vector3(0.0f, 0.0f, 1.0f));
            }
        });

        keywords.Add("positif", () =>
        {
            Debug.Log("positif");
            if (onRotationDirectionChanged != null)
            {
                onRotationDirectionChanged();
            }
        });

        keywords.Add("negatif", () =>
        {
            Debug.Log("négatif");
            if (onRotationDirectionChanged != null)
            {
                onRotationDirectionChanged();
            }
        });

        keywords.Add("rotation", () =>
        {
            Debug.Log("rotation");
            if (onRotationEnabled != null)
            {
                onRotationEnabled();
            }
        });

        keywords.Add("translation", () =>
        {
            Debug.Log("translation");
            if (onTranslationEnabled != null)
            {
                onTranslationEnabled();
            }
        });
        #endregion

        // Start the recognizer
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
    
    // If we recognize a speech input, we throw the associated callback
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        // if the keyword recognized is in our dictionary, call that Action.
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
