#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class CardImageProcessor
{
    private static readonly Vector2Int cardSize = new Vector2Int(628, 1103);
    private static readonly Vector2Int artworkSize = new Vector2Int(606, 1070);

    public static Sprite SaveAdjustedImage(Texture2D source, Vector2 offset, Vector2 scale, string savePath, string fileName, Sprite frameSprite)
    {
        RenderTexture rt = new RenderTexture(cardSize.x, cardSize.y, 24);
        RenderTexture.active = rt;

        GL.Clear(true, true, Color.clear);

        // Ajusta posición y tamaño del gráfico
        float scaledWidth = artworkSize.x * scale.x;
        float scaledHeight = artworkSize.y * scale.y;

        Rect imageRect = new Rect(
            (cardSize.x - artworkSize.x) / 2f + offset.x + (artworkSize.x - scaledWidth) / 2f,
            (cardSize.y - artworkSize.y) / 2f + offset.y + (artworkSize.y - scaledHeight) / 2f,
            scaledWidth,
            scaledHeight);

        Graphics.DrawTexture(imageRect, source);
        Graphics.DrawTexture(new Rect(0, 0, cardSize.x, cardSize.y), frameSprite.texture);

        Texture2D finalTexture = new Texture2D(cardSize.x, cardSize.y, TextureFormat.RGBA32, false);
        finalTexture.ReadPixels(new Rect(0, 0, cardSize.x, cardSize.y), 0, 0);
        finalTexture.Apply();

        RenderTexture.active = null;
        Object.DestroyImmediate(rt);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string fullPath = Path.Combine(savePath, fileName + ".png");
        File.WriteAllBytes(fullPath, finalTexture.EncodeToPNG());
        AssetDatabase.Refresh();

        string relativePath = fullPath.Replace(Application.dataPath, "Assets");
        return AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
    }
}
#endif
