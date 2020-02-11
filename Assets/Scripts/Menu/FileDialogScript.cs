using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FileDialogScript : MonoBehaviour
{
    public ScrollRect Scrolls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDirectoryList(DirectoryInfo path)
    {

    }

    public List<DirectoryInfo> GetDirectoryList(DirectoryInfo path)
    {
        return path.EnumerateDirectories().ToList();
    }
}
