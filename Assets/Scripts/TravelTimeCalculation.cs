using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


public class TravelTimeCalculation: MonoBehaviour
{

    MqttClient client;
    String message = "";
    String response = "";
   // String timestamp = "";

    // Start is called before the first frame update
    void Start()
    {
        client = new MqttClient("192.168.120.4"); // router 2 
        byte code = client.Connect(Guid.NewGuid().ToString());

        client.Subscribe(new string[] { "HoloVision/TravelTime/3" }, new byte[] { 0 });
        client.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        message = Encoding.UTF8.GetString(e.Message);
        //Debug.Log("Message Received: " + message);
        sendMessage(message);

    }

    private void OnDestroy()
    {
        client.MqttMsgPublishReceived -= _mqttClient_MqttMsgPublishReceived;
        //Debug.Log("bye bye: ");
    }

    private void sendMessage(String s)
    {
        ///When it receives a published mqtt message - "red ball"...
        /// sends another mqtt message 
        
        //send another mqtt message
        client.Publish("HoloVision/TravelTime/4", Encoding.UTF8.GetBytes(s));
        //Debug.Log("Message published: " + s);
        
    }
}
