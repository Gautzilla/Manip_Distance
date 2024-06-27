using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMovieVP : MonoBehaviour
{
    //public UnityEngine.Video.VideoClip test_vp8_v2;
    public UnityEngine.Video.VideoClip[] videoClips;
    public UdpReceiveInt udpRecPlayStop;
    public UdpReceive udpRecVideoClip;
    public GameObject videoPlayer;

   void Start()
   {
        Application.runInBackground = true;
       //using find //
       // THIS FIND A SPECIFIC GAME OBJECT THAT THE RECEIVE SCRIPT IS ATTACHED TO BUT THE ACTUAL MANIPULATION HAPPENS
       // TO AN OBJECT THAT THE READ SCRIPT IS ATTACHED TO..
       udpRecPlayStop = GameObject.Find("MaxSendRevVideoPlayStop").GetComponent(typeof(UdpReceiveInt)) as UdpReceiveInt;
       udpRecVideoClip = GameObject.Find("MaxSendRevVideoClip").GetComponent(typeof(UdpReceive)) as UdpReceive;
 
       // FIND THE RELEVANT GAMEOBJECTS..
       videoPlayer = GameObject.Find ("Video Player");
       var vp = GetComponent<UnityEngine.Video.VideoPlayer>();
       vp.Stop();
   }
   /*{
        var videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.playOnAwake = true;

   }*/

    void Update()
    {


        if (udpRecPlayStop.MaxValue() == -1) {
           return;
       }
        
        //Debug.Log("Hello : " + udpRec.maxValue);
        var vp = GetComponent<UnityEngine.Video.VideoPlayer>();
        switch(udpRecPlayStop.MaxValue())
        {
            case 1: // Stop
                vp.Stop();
            break;
            case 2: // Charge unc clip puis le joue
                int clipVideo = (int) udpRecVideoClip.MaxValue(0);
                // vp.clip = videoClips[clipVideo - 1]; // Quand les 9 vidéos seront placées dans "Video Clips" (dans le script depuis Unity), utiliser cette ligne plutôt que le switch qui suit, et envoyer l'entier correspondant à l'indice de la vidéo.
                switch(clipVideo) // Sélectionne le clip vidéo à jouer. 
                {
                  case 1:
                  vp.clip = videoClips[0];
                  break;
                  case 2:
                  vp.clip = videoClips[0];
                  break;
                  case 3:
                  vp.clip = videoClips[0];
                  break;
                  case 4:
                  vp.clip = videoClips[0];
                  break;
                  case 5:
                  vp.clip = videoClips[1];
                  break;
                  case 6:
                  vp.clip = videoClips[1];
                  break;
                  case 7:
                  vp.clip = videoClips[1];
                  break;
                  case 8:
                  vp.clip = videoClips[1];
                  break;
                  case 9:
                  vp.clip = videoClips[1];
                  break;
                }
            vp.Stop();
            vp.Play();
            break;
          case 3: // Rejoue le clip
          vp.Stop();
          vp.Play();
          break;
        }

    }
}