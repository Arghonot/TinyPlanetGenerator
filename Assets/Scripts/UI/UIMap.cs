using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIMap : MonoBehaviour
{
    public RawImage Preview;
    public Button Button;
    public TextMeshProUGUI MapName;

    public void Init(Texture2D img, string name)
    {
        Preview = GetComponentInChildren<RawImage>();

        if (Preview == null)
        {
            return;
        }

        MapName.text = name;
        Preview.texture = img;

        Button = GetComponentInChildren<Button>();

        Button.onClick.AddListener(() =>
        {
            UIMapManager.Instance.DownloadImg(Preview.texture as Texture2D, MapName.text);
        });
    }
}
