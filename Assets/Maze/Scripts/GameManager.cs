using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Maze mazePrefab;
    private Maze mazeInstance;
    public GameObject playerPrefab;
    private GameObject playerInstance;

	// Use this for initialization
	void Start () {
        BeginGame();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
	}

    private void BeginGame() {
        //Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        mazeInstance = Instantiate(mazePrefab) as Maze;
        //StartCoroutine(mazeInstance.Generate());
        mazeInstance.Generate();
        playerInstance = Instantiate(playerPrefab) as GameObject;
        playerInstance.GetComponent<Player>().SetLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));

        Vector3 scaleVector = new Vector3(0.1f, 0.1f, 0.1f);
        playerInstance.transform.localScale = scaleVector;
        //playerInstance.GetComponent<Renderer>().transform.localScale = scaleVector;
        //Camera.main.clearFlags = CameraClearFlags.Color;
        Camera.main.SetReplacementShader(Shader.Find("Unlit/Color"), "");
        Camera.main.rect = new Rect(0f, 0f, 0.2f, 0.2f);
    }

    private void RestartGame() {
        //StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        if(playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
        }
        BeginGame();
    }
}
