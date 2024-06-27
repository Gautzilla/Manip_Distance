 
using UnityEngine;
using System.Collections;
 
public class ReadHP : MonoBehaviour {
 
   private UdpReceive udpRec;
   public GameObject HPX;
 
   void Start ()
   {
       Application.runInBackground = true;
       //using find //
       // THIS FIND A SPECIFIC GAME OBJECT THAT THE RECEIVE SCRIPT IS ATTACHED TO BUT THE ACTUAL MANIPULATION HAPPENS
       // TO AN OBJECT THAT THE READ SCRIPT IS ATTACHED TO..
       udpRec = GameObject.Find("MaxSendRevHP").GetComponent(typeof(UdpReceive)) as UdpReceive;
 
       // FIND THE RELEVANT GAMEOBJECTS..
       HPX = GameObject.Find ("HP");
   }
 
   // Update is called once per frame
   void Update ()
   {
       if (udpRec.maxValues.Length <= 0) {
           return;
       }
 
       // COORDS TO TRANSLATE THE LOUDSPEAKER..
       float posZ = udpRec.MaxValue(0)+0.15f; //15 cm : écart entre le centre du HP et la membrane
 
       // TRANSLATE THE LS..
       HPX.transform.position = new Vector3(0.0f, -0.047f, posZ);
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