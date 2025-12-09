using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour, IDataPersistence
{
    public static string lastUsedPortalID = null;

    public static SequenceManager instance;

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #region Variables
    private bool isResting = false;
    private bool canRest = true;
    [SerializeField] private AudioClip bonfireTheme;
    [SerializeField][Range(0f, 1f)] private float bonfireVolume = 0.5f;
    private Vector3 bonfireLocation = Vector3.zero;
    public Vector3 spawnPoint = Vector3.zero;
    private string currentScene;
    private bool visitedBefore;
    private GameObject firstSpawnObj;
    private Vector3 sceneFirstSpawn;
    private GameObject lastSpawnObj;
    private Vector3 sceneLastSpawn;
    IEnumerable<EnemyCombat> enemyObjects;

    private GameObject player;
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private UIManager uiM;
    public bool IsResting { get => isResting; set => isResting = value; }
    #endregion

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        anim = player.GetComponent<Animator>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        uiM = FindAnyObjectByType<UIManager>();

        enemyObjects = FindAllEnemies();

        if (spawnPoint != Vector3.zero && playerCombat != null)
        {
            playerCombat.transform.position = spawnPoint;
        }
        
    }

    public IEnumerator Respawn()
    {
        if (playerCombat.IsRespawning)
        {
            yield break;
        }

        playerCombat.IsRespawning = true;
        playerCombat.IsImmortal = true;
        playerMovement.Speed = 0;
        playerMovement.Rig.linearVelocity = Vector2.zero;
        uiM.fade.SetActive(true);
        yield return StartCoroutine(uiM.Fade(0, 1, 0.65f));
        yield return new WaitForSeconds(0.1f);
        player.transform.position = spawnPoint;
        playerMovement.Rig.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(uiM.Fade(1, 0, 0.65f));
        uiM.fade.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        playerMovement.Speed = playerMovement.InitialSpeed;
        playerCombat.IsRespawning = false;
        playerCombat.IsImmortal = false;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(Respawn());
    }

    #region Rest Sequence
    public IEnumerator RestSequence()
    {
        if (!canRest) yield break;
        canRest = false;

        if (!IsResting)
        {
            IsResting = true;
            playerCombat.IsImmortal = true;
            playerMovement.Speed = 0f;
            playerCombat.ReceiveHealing(playerCombat.MaxHp);
            uiM.fade.SetActive(true);
            yield return StartCoroutine(uiM.Fade(0, 1));

            MusicManager.instance.PlayTemporaryBGM(bonfireTheme, bonfireVolume);

            if (playerCombat.Bonfire != null)
            {
                bonfireLocation = playerCombat.Bonfire.transform.position;
                playerCombat.transform.position = bonfireLocation;
            }

            playerCombat.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            anim.SetBool("isResting", true);

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(uiM.Fade(1, 0));
            uiM.fade.SetActive(false);

            uiM.PauseState = true;
            DataPersistenceManager.instance.SaveGame();
        }
        else
        {
            foreach (EnemyCombat enemy in enemyObjects)
            {
                if (enemy != null)
                {
                    enemy.Respawn();
                }
            }

            uiM.fade.SetActive(true);
            yield return StartCoroutine(uiM.Fade(0, 1));

            anim.SetBool("isResting", false);
            uiM.PauseState = false;
            yield return new WaitForSeconds(0.5f);
            playerMovement.Speed = playerMovement.InitialSpeed;
            MusicManager.instance.RestorePreviousBGM();

            yield return StartCoroutine(uiM.Fade(1, 0));
            uiM.fade.SetActive(false);

            IsResting = false;
            playerCombat.IsImmortal = false;
        }

        yield return new WaitForSeconds(1f);
        canRest = true;
    }
    #endregion

    #region OnSceneLoad/Destroy
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = sceneFirstSpawn;
        Time.timeScale = 1f;
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerCombat.IsCharging = false;
        if (playerCombat.IsDead)
        {
            playerCombat.CurrentHp = playerCombat.MaxHp;
            playerCombat.IsDead = false;
            uiM.UpdateHearts();
        }
        playerCombat.IsRespawning = false;
        playerCombat.IsImmortal = false;
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerMovement.Speed = playerMovement.InitialSpeed;
        playerAnim = FindAnyObjectByType<PlayerAnim>();

        anim = playerAnim.GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        uiM.PauseState = false;
        

        firstSpawnObj = GameObject.FindGameObjectWithTag("FirstSpawn");
        lastSpawnObj = GameObject.FindGameObjectWithTag("LastSpawn");
        if (firstSpawnObj != null) sceneFirstSpawn = firstSpawnObj.transform.position;
        if (lastSpawnObj != null) sceneLastSpawn = lastSpawnObj.transform.position;

        PositionPlayerAtSpawn();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Player Spawn
    public void PositionPlayerAtSpawn()
    {
        if (playerCombat == null) return;

        if (!string.IsNullOrEmpty(lastUsedPortalID))
        {
            var portals = FindObjectsByType<ScenePortal>(FindObjectsSortMode.None);
            ScenePortal destination = portals.FirstOrDefault(p => p.portalID == lastUsedPortalID);

            if (destination != null)
            {
                playerCombat.transform.position = destination.transform.position;
                StartCoroutine(destination.WaitTeleport());
                //Debug.Log($"Player spawnado na saída '{lastUsedPortalID}' em {destination.transform.position}");
                lastUsedPortalID = null;
                return;
            }
        }

        Vector3 finalPosition = spawnPoint != Vector3.zero ? spawnPoint : sceneFirstSpawn;
        playerCombat.transform.position = finalPosition;
        //Debug.Log($"Player posicionado em {finalPosition}");
    }
    #endregion

    #region Save/Load
    public void SaveData(ref GameData data)
    {
        if (bonfireLocation != Vector3.zero)
        {
            data.firstBonfire = true;
            data.playerPosition = bonfireLocation;
            data.lastSceneName = SceneManager.GetActiveScene().name;
        }

        currentScene = SceneManager.GetActiveScene().name;
        if (!data.visitedScenes.ContainsKey(currentScene))
        {
            data.visitedScenes[currentScene] = true;
        }
    }

    public void LoadData(GameData data)
    {
        currentScene = SceneManager.GetActiveScene().name;
        firstSpawnObj = GameObject.FindGameObjectWithTag("FirstSpawn");
        lastSpawnObj = GameObject.FindGameObjectWithTag("LastSpawn");

        if (firstSpawnObj != null)
            sceneFirstSpawn = firstSpawnObj.transform.position;
        if (lastSpawnObj != null)
            sceneLastSpawn = lastSpawnObj.transform.position;

        visitedBefore = data.visitedScenes.ContainsKey(currentScene) && data.visitedScenes[currentScene];
        spawnPoint = visitedBefore ? sceneLastSpawn : sceneFirstSpawn;

        if (data.lastSceneName == currentScene && data.firstBonfire)
        {
            Transform nearestBonfire = FindClosestBonfire(data.playerPosition);
            if (nearestBonfire != null && Vector3.Distance(nearestBonfire.position, data.playerPosition) < 5f)
            {
                spawnPoint = data.playerPosition;
            }
            else
            {
                spawnPoint = sceneFirstSpawn;
                data.firstBonfire = false;
            }
        }
    }
    #endregion

    #region Auxiliar
    private Transform FindClosestBonfire(Vector3 position)
    {
        GameObject[] bonfires = GameObject.FindGameObjectsWithTag("Bonfire");
        if (bonfires.Length == 0) return null;

        return bonfires
            .OrderBy(b => Vector3.Distance(b.transform.position, position))
            .FirstOrDefault()?.transform;
    }

    private List<EnemyCombat> FindAllEnemies()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<EnemyCombat>()
            .ToList();
    }
    #endregion
}
