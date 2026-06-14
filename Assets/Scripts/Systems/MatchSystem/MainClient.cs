using UnityEngine;
using UnityEngine.SceneManagement;

public class MainClient : MonoBehaviour
{
    private void OnEnable() => TileCreator.CreatedTilesEvent += SetupClientHand;
    private void OnDisable() => TileCreator.CreatedTilesEvent -= SetupClientHand;
    private PlayerHand clientHand = PlayerHand.CreateInstance<PlayerHand>();

    private void SetupClientHand()
    {
        GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject Tiles = null;

        foreach (GameObject gameObject in rootObjs)
        {
            if (gameObject.name.Equals("Tiles"))
            {
                Tiles = gameObject;
                break;
            }
        }

        clientHand.SetupPlayerHand(Tiles);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
