 
using UnityEngine;
using System.Collections;
 
public class ReadPanneau : MonoBehaviour {
 
   private UdpReceive udpRec;
   public GameObject PanneauX;
 
   void Start ()
   {
       Application.runInBackground = true;
       //using find //
       // THIS FIND A SPECIFIC GAME OBJECT THAT THE RECEIVE SCRIPT IS ATTACHED TO BUT THE ACTUAL MANIPULATION HAPPENS
       // TO AN OBJECT THAT THE READ SCRIPT IS ATTACHED TO..
       udpRec = GameObject.Find("MaxSendRevPanneau").GetComponent(typeof(UdpReceive)) as UdpReceive;
 
       // FIND THE RELEVANT GAMEOBJECTS..
       PanneauX = GameObject.Find ("Panneau");
   }
 
   // Update is called once per frame
   void Update ()
   {
       if (udpRec.maxValues.Length <= 0) {
           return;
       }
 
       // COORDS TO TRANSLATE THE LOUDSPEAKER..
       float posX = udpRec.MaxValue(0);
 
       // TRANSLATE THE LS..
       PanneauX.transform.position = new Vector3(posX, 1.004f, 0.9f);
   }
 
   /*public static float Position(float angle)
   {
       if(angle < -360f)
       angle += 360f;
       if(angle > 360)
       angle -= 360;
 
       return Mathf.Clamp(angle, -360f, 360f);
   } */
}