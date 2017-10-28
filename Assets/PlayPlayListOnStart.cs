using UnityEngine;
using System.Collections;

public class PlayPlayListOnStart : MonoBehaviour {

    void Start()
    {
        AudioController.PlayMusicPlaylist();
    }
}
