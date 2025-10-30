using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SequenceManager : MonoBehaviour, IDataPersistence
{
    #region Singleton
    public static SequenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one Sequence Manager in the scene!");
        }

        instance = this;
    }
    #endregion

    #region Variables
    private bool isResting = false;
    private bool canRest = true;
    [SerializeField] private AudioClip bonfireTheme;
    [SerializeField][Range(0f, 1f)] private float bonfireVolume = 0.5f;
    private Vector3 bonfireLocation = Vector3.zero;
    public Vector3 spawnPoint;
    private string currentScene;
    private GameObject firstSpawnObj;
    private Vector3 sceneFirstSpawn;
    IEnumerable<EnemyCombat> enemyObjects;

    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private PlayerAnim playerAnim;
    private Animator anim;
    private UIManager uiM;
    

    public bool IsResting { get => isResting; set => isResting = value; }
    #endregion

    void Start()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerAnim = FindAnyObjectByType<PlayerAnim>();
        anim = playerAnim.GetComponent<Animator>();
        uiM = FindAnyObjectByType<UIManager>();
        enemyObjects = FindAllEnemies();

        if (spawnPoint != Vector3.zero && playerCombat != null)
        {
            playerCombat.transform.position = spawnPoint;
        }
    }

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
            //playerCombat.RechargeItems();
            uiM.fade.SetActive(true);
            yield return StartCoroutine(uiM.Fade(0, 1));

            AudioManager.instance.PlayTemporaryBGM(bonfireTheme, bonfireVolume);

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
                enemy.Respawn();
            }
            uiM.fade.SetActive(true);
            yield return StartCoroutine(uiM.Fade(0, 1));

            anim.SetBool("isResting", false);
            uiM.PauseState = false;
            yield return new WaitForSeconds(0.5f);
            playerMovement.Speed = playerMovement.InitialSpeed;
            AudioManager.instance.RestorePreviousBGM();

            yield return StartCoroutine(uiM.Fade(1, 0));
            uiM.fade.SetActive(false);

            IsResting = false;
            playerCombat.IsImmortal = false;
        }

        yield return new WaitForSeconds(1f);
        canRest = true;
    }

    public void SaveData(ref GameData data)
    {
        if (bonfireLocation != Vector3.zero)
        {
            data.firstBonfire = true;
            data.playerPosition = bonfireLocation;
            data.lastSceneName = SceneManager.GetActiveScene().name;
        }
    }

    public void LoadData(GameData data)
    {
        currentScene = SceneManager.GetActiveScene().name;

        firstSpawnObj = GameObject.FindGameObjectWithTag("FirstSpawn");

        if (firstSpawnObj != null)
        {
            sceneFirstSpawn = firstSpawnObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Spawnpoint padrão não encontrado!");
        }

        if (data.lastSceneName == currentScene && data.firstBonfire)
        {
            Transform nearestBonfire = FindClosestBonfire(data.playerPosition);
            if (nearestBonfire != null && Vector3.Distance(nearestBonfire.position, data.playerPosition) < 5f)
            {
                spawnPoint = data.playerPosition;
            }
            else
            {
                Debug.LogWarning("Bonfire não encontrada, resetando spawnpoint padrão.");
                spawnPoint = sceneFirstSpawn;
                data.firstBonfire = false;
            }
        }
        else
        {
            spawnPoint = sceneFirstSpawn;
        }
    }
    
    public void PositionPlayerAtSpawn()
    {
        if (playerCombat != null)
        {
            playerCombat.transform.position = spawnPoint;
            Debug.Log($"Player posicionado em {spawnPoint}");
        }
    }

    private Transform FindClosestBonfire(Vector3 position)
    {
        GameObject[] bonfires = GameObject.FindGameObjectsWithTag("Bonfire");

        if (bonfires.Length == 0)
        {
            return null;
        }

        GameObject nearest = bonfires.OrderBy(bonfire => Vector3.Distance(bonfire.transform.position, position)).FirstOrDefault();
        if (nearest != null)
        {
            return nearest.transform;
        }
        return null;
    }
    
    private List<EnemyCombat> FindAllEnemies()
    {
        IEnumerable<EnemyCombat> enemyObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<EnemyCombat>();
        return new List<EnemyCombat>(enemyObjects);
    }
}
