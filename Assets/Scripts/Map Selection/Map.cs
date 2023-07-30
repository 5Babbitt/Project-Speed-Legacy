using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Map Selection Item")]
public class Map : ScriptableObject
{
    //name of the scen that will be loaded
    public string sceneName;
    //Name to be displayed
    public string trackName;
    //thumbnail of track
    public Sprite thumbnail;


    public void LoadMap() 
    {
        SceneManager.LoadScene(sceneName);
    }
}
