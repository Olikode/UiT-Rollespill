using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Collidable
{
    public string[] sceneNames;

    [SerializeField] int levelRequired;
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            int playerLevel = coll.gameObject.GetComponent<UnitList>().GetPlayerUnit().Level;
            if(checkRequirements(playerLevel))
            {
                // Teleport the player
                GameManager.instance.SaveState();
                string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    bool checkRequirements(int characterLevel)
    {
        if(levelRequired > characterLevel)
            return false;

        return true;
    }
}
