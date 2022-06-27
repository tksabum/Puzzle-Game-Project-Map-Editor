using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class EditorTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FileBrowser.ShowLoadDialog((paths) => { Debug.Log("Selected: " + paths[0]); },
        						   () => { Debug.Log( "Canceled" ); },
        						   FileBrowser.PickMode.Files, false, null, null, "Select File", "Select" );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
