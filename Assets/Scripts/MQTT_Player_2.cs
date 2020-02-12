using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MQTT_Player_2 : MonoBehaviour
{


    // Use this for initialization
    MqttClient client;
    GameObject _capsule;
    String message = "0.0";
    private Transform body;
    private Transform handLeft;
    private Transform handRight;

    void Start()
    {
        //client = new MqttClient("dveiot.cs.vt.edu");
        client = new MqttClient("192.168.120.4");
        byte code = client.Connect(Guid.NewGuid().ToString());

        /*ushort msgId = client.Publish("test", // topic
                              Encoding.UTF8.GetBytes("MyMessageBody"), // message body
                              MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                              false); // retained*/

        client.Subscribe(new string[] { "test2" }, new byte[] { 0 });
        client.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;
        //body = GameObject.Find("Zombie").transform;
        body = GameObject.Find("Body_Player_2").transform;
        handLeft = GameObject.Find("LeftHand_Player_2").transform;
        handRight = GameObject.Find("RightHand_Player_2").transform;


    }

    // Update is called once per frame
    void Update()
    {

        parseString(message);


    }

    private void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        message = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Message Received: " + message);

    }

    private void OnDestroy()
    {
        client.MqttMsgPublishReceived -= _mqttClient_MqttMsgPublishReceived;

    }

    private void parseString(String s)
    {
        String[] coords = s.Split(';');
        Quaternion newQuaternion = new Quaternion();
        if (coords.Length == 8)
        {
            if (coords[0] == "Head")
            {
                body.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                body.rotation = newQuaternion;
            }
            else if (coords[0] == "HandLeft")
            {
                handLeft.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                //newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                //handLeft.rotation = newQuaternion;
            }
            else if (coords[0] == "HandRight")
            {
                handRight.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                //newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                //handRight.rotation = newQuaternion;
            }
        }

        else if (coords.Length == 4)
        {
            if (coords[0] == "HandLeft")
            {
                handLeft.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                //newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                //handLeft.rotation = newQuaternion;
            }
            else if (coords[0] == "HandRight")
            {
                handRight.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                //newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                //handRight.rotation = newQuaternion;
            }
        }


    }
}
