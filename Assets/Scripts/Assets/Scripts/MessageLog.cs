using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class MessageLog : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Text uiText;
#if TMP_PRESENT
    [SerializeField] private TMP_Text tmpText;
#endif
    [SerializeField] private int maxMessages = 100;

    private readonly List<string> messages = new List<string>();

    public void AddMessage(string message)
    {
        messages.Add(message);
        if (maxMessages > 0 && messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
        }

        string joined = string.Join("\n", messages);
        if (uiText != null)
        {
            uiText.text = joined;
        }
#if TMP_PRESENT
        else if (tmpText != null)
        {
            tmpText.text = joined;
        }
#endif
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
