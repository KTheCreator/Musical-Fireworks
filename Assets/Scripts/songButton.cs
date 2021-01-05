using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class songButton : MonoBehaviour
{
    public string objectSong;
    private songSelection sSelection;
    public AudioSource menuMusic;
    private void Awake()
    {
        //sSelection = GameObject.Find("songManager").GetComponent<songSelection>();
        menuMusic = GameObject.Find("menuMusic").GetComponent<AudioSource>();
    }
    public void setSong()
    {
        sSelection.pickedSong = menuMusic.clip;
        SceneManager.LoadScene("Scenes/SampleScene", LoadSceneMode.Single);
    }

    private IEnumerator loadSongTest()
    {
        UnityWebRequest audioFile = UnityWebRequest.Get(this.objectSong);
        yield return audioFile.SendWebRequest();
        if (audioFile.isNetworkError || audioFile.isHttpError)
            Debug.Log(audioFile.error);
        else
        {
            Debug.Log(this.objectSong + "was found");
            byte[] results = audioFile.downloadHandler.data;
            var memStream = new System.IO.MemoryStream(results);
            var mpgFiles = new NLayer.MpegFile(memStream);
            var samples = new float[mpgFiles.Length];
            mpgFiles.ReadSamples(samples, 0, (int)mpgFiles.Length);
            var clip = AudioClip.Create("convertedFile", samples.Length, mpgFiles.Channels, mpgFiles.SampleRate, false);
            clip.SetData(samples, 0);
            menuMusic.clip = clip;
        }
        /*using (UnityWebRequest audioFile = UnityWebRequestMultimedia.GetAudioClip(this.objectSong, AudioType.MPEG))
        {
            yield return audioFile.SendWebRequest();
            if (audioFile.result == UnityWebRequest.Result.ConnectionError)
                Debug.Log(audioFile.error);
            else
            {
                byte[] results = audioFile.downloadHandler.data;
                var memStream = new System.IO.MemoryStream(results);
                var mpgFiles = new NLayer.MpegFile(memStream);
                var samples = new float[mpgFiles.Length];
                mpgFiles.ReadSamples(samples, 0, (int)mpgFiles.Length);
                var clip = AudioClip.Create("convertedFile", samples.Length, mpgFiles.Channels, mpgFiles.SampleRate, false);
                clip.SetData(samples, 0);
                menuMusic.clip = clip;
            }
        }*/
    }

    public void printSongDebug()
    {
        StartCoroutine(loadSongTest());
        Debug.Log(this.objectSong);
    }

    
}
