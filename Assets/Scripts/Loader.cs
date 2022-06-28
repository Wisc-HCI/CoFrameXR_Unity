using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEditor;
using System.Linq;

using RosPosVector3 = Unity.Robotics.ROSTCPConnector.ROSGeometry.Vector3<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;


public class Loader : MonoBehaviour
{
    // Start is called before the first frame update

    // "package://ur_description/meshes/ur3/visual/base.dae" : Ur3Base,
    // "package://ur_description/meshes/ur3/visual/forearm.dae": Ur3Forearm,
    // "package://ur_description/meshes/ur3/visual/shoulder.dae" : Ur3Shoulder,
    // "package://ur_description/meshes/ur3/visual/upperarm.dae" : Ur3Upperarm,
    // "package://ur_description/meshes/ur3/visual/wrist1.dae" : Ur3Wrist1, 
    // "package://ur_description/meshes/ur3/visual/wrist2.dae" : Ur3Wrist2,
    // "package://ur_description/meshes/ur3/visual/wrist3.dae" : Ur3Wrist3,


    public DataParser parser;
    public List<GameObject> meshList = new List<GameObject>();


    private const string Ur3Base = "Assets/Meshes/Ur3/visual/base.glb";
    private const string Ur3Forearm = "Assets/Meshes/Ur3/visual/forearm.glb";
    private const string Ur3Shoulder = "Assets/Meshes/Ur3/visual/shoulder.glb";
    private const string Ur3Upperarm = "Assets/Meshes/Ur3/visual/upperarm.glb";
    private const string Ur3Wrist1 = "Assets/Meshes/Ur3/visual/wrist1.glb";
    private const string Ur3Wrist2 = "Assets/Meshes/Ur3/visual/wrist2.glb";
    //private const string Ur3Wrist3 = "Assets/Meshes/Ur3/visual/wrist3.glb";
    private const string Ur3Wrist3 = "Assets/Meshes/Ur3/visual/newWrist3.glb";

    private Quaternion unityQuat;
    private Vector3 unityPos;

    public List<GameObject> Tree = new List<GameObject>();

    private Dictionary<string, string> meshLookupTable = new Dictionary<string, string>();
    void Start()
    {
        // Automatically makes the tree based on tfs parsed from DataParser
        MakeTree();

        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/base.dae", Ur3Base);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/forearm.dae", Ur3Forearm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/shoulder.dae", Ur3Shoulder);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/upperarm.dae", Ur3Upperarm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist1.dae", Ur3Wrist1);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist2.dae", Ur3Wrist2);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist3.dae", Ur3Wrist3);
        foreach (DataParser.Item item in parser.currentItemList.items)
        {
            if (meshLookupTable.ContainsKey(item.shape))
            {
                foreach (DataParser.Tf tf in parser.currentTfsList.tfs)
                {
                    if (item.id == tf.id)
                    {

                        //GameObject tfGameObject = new GameObject(tf.id);
                        GameObject tfGameObject = GameObject.Find(tf.id);
                        //GameObject mesh = ImportGLTF(meshLookupTable[item.shape]);
                        GameObject mesh = Importer.LoadFromFile(meshLookupTable[item.shape]);
                        //mesh.transform.parent = tfGameObject.transform;

                        mesh.name = item.name;
                        //Vector3<FLU> rosTFPos = new Vector3<FLU>(tf.position.x, tf.position.y, tf.position.z);
                        Vector3 rosTFPos = new Vector3(tf.position.x, tf.position.y, tf.position.z);


                        //Vector3<FLU> rosItemPos = new Vector3<FLU>(item.position.x, item.position.y, item.position.z);
                        Vector3 rosItemPos = new Vector3(item.position.x, item.position.y, item.position.z);


                        //Vector3 unityTFPos = rosTFPos.toUnity;
                        Vector3 unityTFPos;
                        //Vector3 unityTFPos = Ros2UnityVec(rosTFPos);
                        //Vector3 unityItemPos = rosItemPos.toUnity;
                        Vector3 unityItemPos;
                        //Vector3 unityItemPos = Ros2UnityVec(rosItemPos);

                        //Quaternion<FLU> rosTFQuat = new Quaternion<FLU>(tf.rotation.w, tf.rotation.x, tf.rotation.y, tf.rotation.z);
                        Quaternion rosTFQuat = new Quaternion(tf.rotation.x, tf.rotation.y, tf.rotation.z, tf.rotation.w);
                        //Quaternion<FLU> rosItemQuat = new Quaternion<FLU>(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w);
                        Quaternion rosItemQuat = new Quaternion(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w);



                        //Quaternion unityTFQuat = rosTFQuat.toUnity;
                        //Quaternion unityItemQuat = rosItemQuat.toUnity;
                        Quaternion unityTFQuat;
                        //Quaternion unityTFQuat = Ros2UnityQuat(rosTFQuat);
                        Quaternion unityItemQuat;
                        //Quaternion unityItemQuat = Ros2UnityQuat(rosItemQuat);


                        Vector3 itemScale = new Vector3(item.scale.x, item.scale.y, item.scale.z);

                        // Set the translation, rotation and scale parameters.
                        Matrix4x4 item_m = Matrix4x4.TRS(rosItemPos, rosItemQuat, itemScale);
                        (unityItemPos, unityItemQuat) = toLeftHanded(item_m);
                        Matrix4x4 rosTF_m = Matrix4x4.TRS(rosTFPos, rosTFQuat, itemScale);
                        (unityTFPos, unityTFQuat) = toLeftHanded(rosTF_m);

                        mesh.transform.parent = tfGameObject.transform;
                       

                        tfGameObject.transform.position = rosTFPos;
                        tfGameObject.transform.rotation = rosTFQuat;
                         
                         


                        
                        mesh.transform.localPosition = rosItemPos;
                        mesh.transform.localRotation = rosItemQuat;

                        mesh.GetComponent<Transform>().localScale = itemScale;

                        meshList.Add(mesh);
                       
                    }

                }



            }
        }

        // rotate root game objects in the tree by -90 degress in x axis
        RotateRoot();

    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject ImportGLTF(string filepath)
    {
        GameObject result = Importer.LoadFromFile(filepath);
        //Debug.Log(result.name);
        return result;
    }

    public (Vector3, Quaternion) toLeftHanded(Matrix4x4 m)
    {
        // this matrix switches y and z
        //Matrix4x4 M = new Matrix4x4(
        //    new Vector4(1, 0, 0, 0),
        //    new Vector4(0, 0, 1, 0),
        //    new Vector4(0, 1, 0, 0),
        //    new Vector4(0, 0, 0, 1)
        //);

        Matrix4x4 M = new Matrix4x4(
            new Vector4(0, -1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 0, 1)
        );

        m = M.inverse * m * M;
        //m = M * m * M.transpose;
        Vector3 t = m.GetPosition();
        Quaternion q = m.rotation;
        return (t, q);
    }

    public (Vector3, Quaternion) toRightHanded(Matrix4x4 m)
    {

        Matrix4x4 M = new Matrix4x4(
            new Vector4(0, -1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 0, 1)
        ).transpose;
        m = M.inverse * m * M;
        Vector3 t = m.GetPosition();
        Quaternion q = m.rotation;
        return (new Vector3(t.x, t.y, t.z), new Quaternion(q.x, q.y, q.z, q.w));
    }

    private Quaternion ConvertToUnity(Quaternion input)
    {
        Matrix4x4 M = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 0, 1)
        );

        Matrix4x4 unity_rot_mat = M * Matrix4x4.Rotate(input) * M.transpose;

        Quaternion output = QuaternionFromMatrix(unity_rot_mat);

        return output;

        //return new Quaternion(
        //     input.y,   // -(  right = -left  )
        //    -input.z,   // -(     up =  up     )
        //     input.x,   // -(forward =  forward)
        //     input.w
        //);
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    public Quaternion Ros2UnityQuat(Quaternion quaternion)
    {
        return new Quaternion(quaternion.y, -quaternion.z, -quaternion.x, quaternion.w);
    }

    public Vector3 Ros2UnityVec(Vector3 vector3)
    {
        return new Vector3(-vector3.y, vector3.z, vector3.x);
    }
    public static Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn)
    {
        return new Vector3(rosIn.z, -rosIn.x, rosIn.y);
    }

    public static Quaternion UnityToRosRotationAxisConversion(Quaternion qIn)
    {
        Quaternion temp = (new Quaternion(-qIn.z, qIn.x, -qIn.y, qIn.w)) * (new Quaternion(0, 0, 0, 1));
        return temp;
    }
    public static Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn)
    {
        return new Vector3(-rosIn.y, rosIn.z, rosIn.x);
    }

    //Convert ROS quaternion to Unity Quaternion
    public static Quaternion RosToUnityQuaternionConversion(Quaternion rosIn)
    {
        return new Quaternion(rosIn.x, -rosIn.z, rosIn.y, rosIn.w);
    }

    void MakeTree()
    {
        foreach (DataParser.Tf tf in parser.currentTfsList.tfs)
        {
            GameObject tfGameObject = new GameObject(tf.id);
            Tree.Add(tfGameObject);

        }
        // assign children to root game objects in Tree
        foreach (DataParser.Tf tf in parser.currentTfsList.tfs)
        {
            // should be root
            if (tf.frame == "world")
            {
 
            }
            // at least nested, not root
            else
            {
                Debug.Log(tf.id);
                for (int i = 0; i < Tree.Count; i++)
                {
                    // return your game object if found, otherwise return null
                    GameObject temp = GameObject.Find(tf.id);

                    if (GameObject.Find(tf.frame))
                    {
                        GameObject parentGameObject = GameObject.Find(tf.frame);
                        temp.gameObject.transform.parent = parentGameObject.gameObject.transform;
                    }


                }

            }
        }

    }

    void RotateRoot()
    { 
        foreach (DataParser.Tf tf in parser.currentTfsList.tfs)
        {
            // should be root
            if (tf.frame == "world")
            {
                //Debug.Log(tf.id + " root");
                GameObject.Find(tf.id).transform.Rotate(-90, 0, 0);
                GameObject.Find(tf.id).transform.localScale = new Vector3(-1,1,1);
            }
        }

    }

}
