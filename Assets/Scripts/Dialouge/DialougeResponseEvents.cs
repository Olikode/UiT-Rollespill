using UnityEngine;
using System; 

public class DialougeResponseEvents : MonoBehaviour
{
    [SerializeField] private DialougeObject dialougeObject;
    [SerializeField] private ResponseEvent[] events;

    public DialougeObject DialougeObject => dialougeObject;

    public ResponseEvent[] Events => events;

    public void OnValidate()
    {
        if (dialougeObject == null) return;
        if (dialougeObject.Responses == null) return;
        if (events != null && events.Length == dialougeObject.Responses.Length) return;

        if (events == null)
        {
            events = new ResponseEvent[dialougeObject.Responses.Length];
        }
        else
        {
            Array.Resize(ref events, dialougeObject.Responses.Length);
        }

        for (int i = 0; i < dialougeObject.Responses.Length; i++)
        {
            Response response = dialougeObject.Responses[i];

            if (events[i] != null)
            {
                events[i].name = response.ResponseText;
                continue;
            }

            events[i] = new ResponseEvent() {name = response.ResponseText};


        }
    }
}
