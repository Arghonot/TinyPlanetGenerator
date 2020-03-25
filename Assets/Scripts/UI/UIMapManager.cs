using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UIMapManager : Singleton<UIMapManager>
{
    [DllImport("__Internal")]
    private static extern void ImageDownloader(string str, string fn);

    public GameObject UIMapPrefab;

    //RectTransform _trans;

    Dictionary<string, UIMap> uimaps;

    private void Start()
    {
        uimaps = new Dictionary<string, UIMap>();
        //_trans = GetComponent<RectTransform>();
    }

    public void AddMap(Texture2D tex, string name)
    {
        if (uimaps.ContainsKey(name))
        {
            return;
        }

        var newmap = Instantiate(UIMapPrefab);

        uimaps.Add(name, newmap.GetComponent<UIMap>());
        newmap.transform.SetParent(transform);
        uimaps[name].Init(tex, name);
    }

    // TODO use a object pool
    public void FlushMaps()
    {
        foreach (var map in uimaps)
        {
            Destroy(map.Value.gameObject);
        }

        uimaps.Clear();
    }

    public void DownloadImg(Texture2D tex, string name)
    {
        print("Downloading");

        #if UNITY_WEBGL	
            ImageDownloader(System.Convert.ToBase64String(tex.EncodeToPNG()), name);
        #endif
    }
}
