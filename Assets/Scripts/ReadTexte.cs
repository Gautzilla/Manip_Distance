 
using UnityEngine;
using System.Collections;
using TMPro;
 
public class ReadTexte : MonoBehaviour {
 
   private UdpReceive udpRec;
   public GameObject TextX;
 
   void Start ()
   {
       Application.runInBackground = true;
       //using find //
       // THIS FIND A SPECIFIC GAME OBJECT THAT THE RECEIVE SCRIPT IS ATTACHED TO BUT THE ACTUAL MANIPULATION HAPPENS
       // TO AN OBJECT THAT THE READ SCRIPT IS ATTACHED TO..
       udpRec = GameObject.Find("MaxSendRevTexte").GetComponent(typeof(UdpReceive)) as UdpReceive;
 
       // FIND THE RELEVANT GAMEOBJECTS..
       TextX = GameObject.Find ("Texte");
   }
 
   // Update is called once per frame
   void Update ()
   {
       if (udpRec.maxValues.Length <= 0) {
           return;
       }
		
		// Valeur donnée par le sujet
		float valeur = udpRec.MaxValue(0);
		
       TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
       //textmeshPro.text = "Sensation : " + valeur.ToString ();
       textmeshPro.text = "Puissance : " + valeur.ToString ();
	   //textmeshPro.text = "Distance : " + valeur.ToString () + "m";
  
		// textmeshPro.SetText("Niveau : {0:2}", valeur );
   }
}