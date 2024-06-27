 
using UnityEngine;
using System.Collections;
using TMPro;
 
public class ReadInstructions : MonoBehaviour {
 
   private UdpReceive udpRec;
   public GameObject Texte_debutX;
 
   void Start ()
   {
       Application.runInBackground = true;
       //using find //
       // THIS FIND A SPECIFIC GAME OBJECT THAT THE RECEIVE SCRIPT IS ATTACHED TO BUT THE ACTUAL MANIPULATION HAPPENS
       // TO AN OBJECT THAT THE READ SCRIPT IS ATTACHED TO..
       udpRec = GameObject.Find("MaxSendRevInstructions").GetComponent(typeof(UdpReceive)) as UdpReceive;
 
       // FIND THE RELEVANT GAMEOBJECTS..
       Texte_debutX = GameObject.Find ("Texte_debut");
   }
 
   // Update is called once per frame
   void Update ()
   {
       if (udpRec.maxValues.Length <= 0) {
           return;
       }

       TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
       int valeur = (int) udpRec.MaxValue(0);
       switch(valeur)
       {
        case 0:
        textmeshPro.SetText("");
        break;
        case 1:
        textmeshPro.SetText("Appuyer sur VALIDER pour commencer le prétest");
        break;
        case 2:
        textmeshPro.SetText("Appuyer sur VALIDER pour commencer le test");
        break;
        case 3:
        textmeshPro.SetText("Prétest terminé. Appuyer sur VALIDER pour commencer le test");
        break;
        case 4:
        textmeshPro.SetText("Le test est terminé, MERCI");
        break;
       }
   }
}