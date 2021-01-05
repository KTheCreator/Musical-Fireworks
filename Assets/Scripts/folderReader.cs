using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TagLib;
public class folderReader:MonoBehaviour
{
    public AudioClip tfile;
    public Text titleText;
    public GameObject buttonPrefab;
    public Transform menuList;
    // Start is called before the first frame update
    
   public void Start()
    {
        AudioSource tempMM = GameObject.Find("menuMusic").GetComponent<AudioSource>();
        string myPath = @"C:\Users\kwalk\MusicLal Firework v2\Assets\TestAudio";
        DirectoryInfo dir = new DirectoryInfo(myPath);
        FileInfo[] info = dir.GetFiles("*.mp3");
        Debug.Log(info.Length);
        foreach(FileInfo file in info)
        {
            GameObject temp = Instantiate(buttonPrefab,menuList);
            buttonPrefab.GetComponentInChildren<Text>().text = file.Name;
            buttonPrefab.GetComponent<songButton>().objectSong = file.FullName;
            buttonPrefab.GetComponent<songButton>().menuMusic = tempMM;
            //Debug.Log(file);
        }
    }
}
