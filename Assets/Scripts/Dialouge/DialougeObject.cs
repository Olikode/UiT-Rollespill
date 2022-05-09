using UnityEngine;

[CreateAssetMenu(menuName = "Dialouge/DialougeObject")]
public class DialougeObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialouge;
    [SerializeField] private Response[] responses; 

    public string[] Dialouge => dialouge;

    public bool HasResponses => Responses != null && Responses.Length > 0;

    public Response[] Responses => responses;


}
