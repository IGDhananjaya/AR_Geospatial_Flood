using UnityEngine;
using TMPro;

public class FloodDropdownColour : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Color lowColor = new Color(0.6f, 1f, 0.6f);  // Hijau muda
    public Color mediumColor = new Color(1f, 1f, 0.6f);   // Kuning muda
    public Color highColor = new Color(1f, 0.6f, 0.6f);  // Merah muda

    void Start()
    {
        if (dropdown == null)
        {
            Debug.LogWarning("[FloodDropdownColour] Dropdown belum diassign.");
            return;
        }

        ApplyColorsToOptions();
        dropdown.onValueChanged.AddListener(UpdateSelectedColor);
        UpdateSelectedColor(dropdown.value);
    }

    void ApplyColorsToOptions()
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            var option = dropdown.options[i];
            switch (option.text.ToLower())
            {
                case "low":
                    option.image = MakeColorSprite(lowColor);
                    break;
                case "medium":
                    option.image = MakeColorSprite(mediumColor);
                    break;
                case "high":
                    option.image = MakeColorSprite(highColor);
                    break;
            }
        }
    }

    void UpdateSelectedColor(int index)
    {
        string selected = dropdown.options[index].text.ToLower();
        Color color = Color.white;

        switch (selected)
        {
            case "low": color = lowColor; break;
            case "medium": color = mediumColor; break;
            case "high": color = highColor; break;
        }

        if (dropdown.captionImage != null)
            dropdown.captionImage.color = color;
    }

    Sprite MakeColorSprite(Color color)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
    }
}
