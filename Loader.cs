using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;


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
    public List<GameObject> meshList = new List<GameObject> ();
    
    private const string Ur3Base = "Assets/Meshes/Ur3/visual/base.glb";
    private const string Ur3Forearm = "Assets/Meshes/Ur3/visual/forearm.glb";
    private const string Ur3Shoulder = "Assets/Meshes/Ur3/visual/shoulder.glb";
    private const string Ur3Upperarm = "Assets/Meshes/Ur3/visual/upperarm.glb";
    private const string Ur3Wrist1 = "Assets/Meshes/Ur3/visual/wrist1.glb";
    private const string Ur3Wrist2 = "Assets/Meshes/Ur3/visual/wrist2.glb";
    private const string Ur3Wrist3 = "Assets/Meshes/Ur3/visual/wrist3.glb";

    
    
    private Dictionary<string, string> meshLookupTable = new Dictionary<string,string>();
    void Start()
    {
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/base.dae",Ur3Base);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/forearm.dae",Ur3Forearm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/shoulder.dae",Ur3Shoulder);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/upperarm.dae",Ur3Upperarm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist1.dae",Ur3Wrist1);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist2.dae",Ur3Wrist2);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist3.dae",Ur3Wrist3);
        foreach (DataParser.Item item in parser.currentItemList.items){
            if (meshLookupTable.ContainsKey(item.shape)){
                GameObject mesh = ImportGLTF(meshLookupTable[item.shape]);
                mesh.GetComponent<Transform>().position = new Vector3(item.position.x,item.position.y,item.position.z);
                mesh.GetComponent<Transform>().rotation = new Quaternion(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w);
                meshList.Add(mesh);
                Debug.Log(item.name);
            }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject ImportGLTF(string filepath) {
        GameObject result = Importer.LoadFromFile(filepath);
    return result;
}
}
