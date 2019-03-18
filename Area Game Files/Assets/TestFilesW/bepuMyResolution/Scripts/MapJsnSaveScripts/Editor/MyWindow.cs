using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Dim
{
    public class MyWindow : EditorWindow
    {

        //string collisionLevelName = "mapCol.txt";

        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("DimensionArena/JSON and ...")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(MyWindow));
        }

        void OnGUI()
        {
            GUILayout.Label("Dimension Arena unity extension", EditorStyles.boldLabel);
            //myString = EditorGUILayout.TextField("Text Field", myString);

            //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            //myBool = EditorGUILayout.Toggle("Toggle", myBool);
            //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);

            if (GUILayout.Button("Save JSON from scene colliders2D", EditorStyles.miniButton))
            {
                //Debug.Log("Save");


                Collider2D[] col = SceneAsset.FindObjectsOfType<Collider2D>();


               // List<CircleCollider2D> circleList = new List<CircleCollider2D>();
                //List<BoxCollider2D> boxList = new List<BoxCollider2D>();


                SphereCollider[] circleCol = FindObjectsOfType<SphereCollider>();
                BoxCollider[] boxCol = FindObjectsOfType<BoxCollider>();


                SaveJSONColliders jsonColl = new SaveJSONColliders(circleCol, boxCol);


                //string serialized = EditorJsonUtility.ToJson(jsonColl);// " ";//= Js//Json.JsonParser.Serialize(jsonColl);//  JsonConvert.SerializeObject(jsonColl);

               // Debug.Log(serialized);
                //write string to file
                // System.IO.File.WriteAllText(Application.dataPath+"/" + collisionLevelName, serialized);

                string ser2 = Newtonsoft.Json.JsonConvert.SerializeObject(jsonColl);

                System.IO.File.WriteAllText(Application.dataPath + "/" + ServerNetworkStats.collisionLevelName, ser2);
              //  Debug.Log(ser2);



                //foreach (Collider2D co in col)
                //{


                //}

                //foreach (Collider2D co in circleList)
                //{


                //}

            }








            // EditorGUILayout.EndToggleGroup();
        }
    }
}