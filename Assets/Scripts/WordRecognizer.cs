using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;

public class WordRecognizer : MonoBehaviour
{
    [SerializeField]
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public delegate void GrabAction();
    public static event GrabAction onGrab;

    public delegate void ReleaseAction();
    public static event ReleaseAction onRelease;

    public delegate void RotationAxis(Vector3 axis);
    public static event RotationAxis onRotationAxisChanged;

    public delegate void RotationDirection();
    public static event RotationDirection onRotationDirectionChanged;
    
    // Start is called before the first frame update
    void Start()
    {
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
            onRotationAxisChanged(new Vector3(1.0f, 0.0f,0.0f));
        });
        keywords.Add("Y", () =>
        {
            Debug.Log("Y");
            onRotationAxisChanged(new Vector3(0.0f, 1.0f, 0.0f));
        });
        keywords.Add("Z", () =>
        {
            Debug.Log("Z");
            onRotationAxisChanged(new Vector3(0.0f, 0.0f, 1.0f));
        });

        keywords.Add("positif", () =>
        {
            if(onRotationDirectionChanged != null)
            {
                onRotationDirectionChanged();
            }
        });

        keywords.Add("négatif", () =>
        {
            if (onRotationDirectionChanged != null)
            {
                onRotationDirectionChanged();
            }
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }
    
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
