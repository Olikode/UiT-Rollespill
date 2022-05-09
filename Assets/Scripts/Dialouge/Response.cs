using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private DialougeObject dialougeObject;

    public string ResponseText => responseText;

    public DialougeObject DialougeObject => dialougeObject;
}
