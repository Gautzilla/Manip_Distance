using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
 
public class UdpReceiveInt : MonoBehaviour
{
   public int port = 2006;
   private UdpClient client;
   private IPEndPoint RemoteIpEndPoint;
   private Thread t_udp;
   public int maxValue =-1; // Initialisation 
 
 
   void Start()
   {
       client = new UdpClient(port);
       RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
       t_udp = new Thread(new ThreadStart(UDPRead));
       t_udp.Name = "UDP thread";
       t_udp.Start();
   }
 
   public void UDPRead()
   {
       while (true)
       {
           try
           {
               //Debug.Log("listening UDP port " + port);
               byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
               maxValue=receiveBytes[receiveBytes.Length-1]; // La dernière valeur de receiveBytes est l'int envoyé par MaxMSP.
           }
           catch (Exception e)
           {
               Debug.Log("Not so good " + e.ToString());
           }
           Thread.Sleep(20);
           maxValue=-1; // Evite que PlayMovieVP entre dans la boucle de vérification si maxValue n'a pas changé
       }
   }
 
   void OnDisable()
   {
       if (t_udp != null) t_udp.Abort();
       client.Close();
   }
 
   public int MaxValue()
   {
      return maxValue;
   }
}