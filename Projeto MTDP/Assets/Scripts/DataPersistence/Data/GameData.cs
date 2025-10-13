using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public int maxHp;
    public int playerDamage;
    public Vector3 playerPosition;
    public Vector3 firstSpawnpoint = new(-39.8f, -3.26f, 1.49f);
    public bool firstBonfire;
    public Dictionary<string, bool> collectables; 

    //Construtor com valores default para novo jogo
    public GameData()
    {
        this.maxHp = 10;
        this.playerDamage = 1;
        this.firstBonfire = false;
        playerPosition = firstSpawnpoint;
        collectables = new Dictionary<string, bool>();
    }
}
