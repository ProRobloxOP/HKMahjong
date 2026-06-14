using UnityEngine;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{

    private void OnEnable() => TileCreator.CreatedTilesEvent += SetupHand;
    private void OnDisable() => TileCreator.CreatedTilesEvent -= SetupHand;
    private PlayerHand hand = PlayerHand.CreateInstance<PlayerHand>();

    

    private void SetupHand()
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

        hand.SetupPlayerHand(Tiles);
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
