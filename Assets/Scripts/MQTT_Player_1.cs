using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using System.IO;
using System.Linq;
using System.Windows;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
#endif


public class MQTT_Player_1 : MonoBehaviour {
    // Use this for initialization
    MqttClient client;
    GameObject _capsule;
    String message = "";
    String label = "";
    String timestamp0 = "";
    String timestamp1 = "";

    private GameObject tree;
    private GameObject redBall;
    private GameObject blueBall;
    ///private Transform body;
    ///private Transform handLeft;
    ///private Transform handRight;

    ///////////////////// 
    //define filePath
#if WINDOWS_UWP
    Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
    Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
#endif

    //private string saved line;
    private string saveInformation = "";

    //private static string timeStamp = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
    private static string timeStamp2 = "";
    //private static string fileName = timeStamp + ".txt";
    private static string fileName = "Timestamps.txt";

    //private save counter
    private static bool firstSave = true;

    //Hashtable declaration
    // private static Dictionary<string, IconProperty> iconCollection = new Dictionary<string, IconProperty>();


#if WINDOWS_UWP
    async void WriteData()
    {
        if (firstSave){
        //StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
        await FileIO.AppendTextAsync(sampleFile, saveInformation + "\r\n");
        firstSave = false;
        }
    else{
        StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
        await FileIO.AppendTextAsync(sampleFile, saveInformation + "\r\n");
    }
    }
#endif


    //public void addToCounter(string iconName, IconProperty iconproperty)
    //{
    //  iconCollection[iconName] = iconproperty;
    //  Debug.Log(iconName);
    //  iconCollection[iconName].getIconProperty(iconName);
    //  saveInformation = iconCollection[iconName].iconPropertyOutput(iconName);
    //}

    /////////////////////////////

    void Start() {
        // the hololens application creates an mqtt client and listens for identification of 
        // an ornament. When it receives a published mqtt message - "red ball"...
        /// it asks the user to turn around and look at the tree, a hologram of a red ball appears 
        // on the appropriate location on a holographic tree. 

        //client = new MqttClient("dveiot.cs.vt.edu");
        client = new MqttClient("192.168.120.4"); // router 2 
        byte code = client.Connect(Guid.NewGuid().ToString());

        /*ushort msgId = client.Publish("test", // topic
                              Encoding.UTF8.GetBytes("MyMessageBody"), // message body
                              MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                              false); // retained*/
        // client.Subscribe(new string[] { "Kinect/ICU" }, new byte[] { 0 });
        
        //// NEW CODE - Jan 02, 2020, Archi Dasgupta.
        client.Subscribe(new string[] { "HoloVision/Ornaments" }, new byte[] { 0 });
        client.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;

        ///body = GameObject.Find("Body_Player_1").transform;
        ///handLeft = GameObject.Find("LeftHand_Player_1").transform;
        ///handRight = GameObject.Find("RightHand_Player_1").transform;

        //// NEW CODE - Jan 02, 2020, Archi Dasgupta.
        tree = GameObject.Find("Christmas_Tree_V1_00");
        tree.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        //tree.SetActive(false);
        redBall = GameObject.Find("Christmas_Ball_01");
        redBall.SetActive(false);
        blueBall = GameObject.Find("Christmas_Ball_07");
        blueBall.SetActive(false);
        //blueBall = GameObject.Find("Christmas_Ball_07");       
    }

    // Update is called once per frame
    void Update() {
        ///parseString(message);
        //// NEW CODE - Jan 02, 2020, Archi Dasgupta.
          // to avoid calling this function unnecessarily, we put it in publish received
                              //Write to file 
        parseMessage(label);
        label = "";
    }

    private void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        message = Encoding.UTF8.GetString(e.Message);
        Debug.Log("Message Received: " + message);
        String[] msg = message.Split(',');  // redball,t0=111,t1=4556 
        label = msg[0];

        //parseMessage(label);

        timestamp0 = msg[1];
        timestamp1 = msg[2];

        //Debug.Log("label: " + label);
        //Debug.Log("timestamp: " + timestamp);
    }

    private void OnDestroy()
    {
        client.MqttMsgPublishReceived -= _mqttClient_MqttMsgPublishReceived;
        //Debug.Log("MQTT bye bye: ");

    }


    private void parseMessage(String s)
    {
        ///When it receives a published mqtt message - "red ball"...
        /// it asks the user to turn around and look at the tree, hologram of the tree appears, 
        /// hologram of the red ball appears on the appropriate location. Once the user places the actual red ball
        // in the expected location they say "task completed". then the virtual red ball disapperars again.

        // mqtt topic = "HoloVision/Ornaments"
        // mqtt message = "red ball,1" // label and timestamp


        //// NEW CODE - Jan 02, 2020, Archi Dasgupta.

        ///
        if (s == "tree")
        {
            // tree hologram appear
            // make tree hologram stop 
            tree.SetActive(true);
            tree.transform.localScale = new Vector3(1,1,1);
           // tree.setActive, makes the tree visible at some random location
        }

        if (s == "red ball") {
            // make red ball visible on tree
            redBall.SetActive(true);
        }

        else if (s == "blue ball")
        {
            // make blue ball visible on tree
            blueBall.SetActive(true);
        }
        else
        {
            return;
        }

        long seconds = DateTimeOffset.Now.ToUnixTimeSeconds();

        //timeStamp2 = System.DateTime.Now.ToString();
        //saveInformation = "timestamp1: " + timestamp + "," + "timestamp2: " + timestamp2;
        saveInformation = "timestamp0: " + timestamp0 + ", " + "timestamp1: "+ timestamp1 + ", " + "timestamp2: " + seconds;
        Debug.Log("saveInformation: " + saveInformation);
#if WINDOWS_UWP
        WriteData();
#endif

    }


    /*
    private void parseString(String s)
    {
        String[] coords = s.Split(';');
        Quaternion newQuaternion = new Quaternion();
        if (coords.Length == 8)
        {
            if (coords[0] == "Head")
            {
                body.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]+5f));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                body.rotation = newQuaternion;
            }
            else if (coords[0] == "HandLeft")
            {
                handLeft.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]+5f));
                //Vector4 headRotation = new Vector4(float.Parse(coords[3]), float.Parse(coords[4]), float.Parse(coords[5]), float.Parse(coords[6]));
                //newQuaternion = new Quaternion(float.Parse(coords[5]), float.Parse(coords[6]), float.Parse(coords[7]), float.Parse(coords[4]));
                //handLeft.rotation = newQuaternion;
            }
            else if (coords[0] == "HandRight")
            {
                handRight.position = new Vector3(float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]+5f));
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
    }*/
}
