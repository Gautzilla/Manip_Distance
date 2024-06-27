using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
 
public class UdpReceive : MonoBehaviour
{
   public int port = 2002;
   private UdpClient client;
   private IPEndPoint RemoteIpEndPoint;
   private Thread t_udp;
   public float[] maxValues;
 
 
   void Start()
   {
       client = new UdpClient(port);
       RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
       t_udp = new Thread(new ThreadStart(UDPRead));
       t_udp.Name = "UDP thread";
       t_udp.Start();
 
 
       //FilterData(test);
   }
 
   public void UDPRead()
   {
       while (true)
       {
           try
           {
               // Debug.Log("listening UDP port " + port);
               byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
               string returnData = Encoding.ASCII.GetString(receiveBytes);
               // parsing //
               FilterData(returnData);
           }
           catch (Exception e)
           {
               Debug.Log("Not so good " + e.ToString());
           }
           Thread.Sleep(20);
       }
   }
 
   void OnDisable()
   {
       if (t_udp != null) t_udp.Abort();
       client.Close();
   }
 
   public float MaxValue(int index)
   {
       return maxValues[index];
   }
 
   public void FilterData(string dataString)
   {
       string[] splitString = dataString.Split(":"[0]);
       maxValues = new float[splitString.Length];
 
       for( int i=0; i<maxValues.Length; i++ ) {
           maxValues[i] = float.Parse(splitString[i]);
       }      
   }
}