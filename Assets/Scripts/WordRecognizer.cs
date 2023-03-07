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
    
    // Start is called before the first frame update
    void Start()
    {
        keywords.Add("attrape", () =>
        {
            Debug.Log("grabbed");
            if (onGrab != null)
            {
                onGrab();
            }
        });
        keywords.Add("lache", () =>
        {
            Debug.Log("release");
            if (onRelease != null)
            {
                onRelease();
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
