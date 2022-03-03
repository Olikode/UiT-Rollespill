using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }

        //Used  temporary if you need to clear all player data 
        //PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Resources 
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public GameObject hud;
    public GameObject menu; 

    //Logic 
    public int gold;
    public int experience;

    //Floating text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    //Upgrade weapon
    public bool TryUpgradeWeapon()
    {
        //is the weapon max level rihght now?
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        if (gold >= weaponPrices[weapon.weaponLevel])
        {
            gold -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }
        return false;
    }

    //Hitpoint bar 
    public void OnHitPointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    //Experience system
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) //Max level
                return r;
        }

        return r;    
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;

        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }

        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp();

    }
    public void OnLevelUp()
    {
        Debug.Log("Level up!");
        player.OnLevelUp();
        OnHitPointChange();
    }

    //On scene loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        //setting player on spawnpoint when entering a new scene
        //player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    //Death menu and respawn 
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
        Debug.Log("Respawn virke beibi");
    }

    //Save the state of the game
    /* 
     *  INT preferedSkin
     *  INT gold
     *  INT experience
     *  INT weaponLevel
     * 
     */
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";
        s += gold.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString(); 

        PlayerPrefs.SetString("SaveState", s);
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveSate"))
            return; 

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //Change player skin
        gold = int.Parse(data[1]);

        //Experience
        experience = int.Parse(data[2]);
        if(GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());

        //Change the weapon level
        weapon.SetWeaponLevel(int.Parse(data[3]));
        
    }
}
