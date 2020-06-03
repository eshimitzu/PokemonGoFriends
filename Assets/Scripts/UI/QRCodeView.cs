using QRCoder;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QRCodeView : MonoBehaviour
{
    [SerializeField] private int pixelsPerModule = 20;

    private Image image;
    private Texture2D texture;

    
    private void Awake()
    {
        image = GetComponent<Image>();

        int size = 580;
        texture = new Texture2D(size, size);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, size, size), Vector2.one * 0.5f);
    }

    private void OnDestroy()
    {
        if (texture)
        {
            Destroy(texture);
        }
    }

    public void Encode(string data)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        GetGraphic(qrCodeData, pixelsPerModule, Color.black, Color.white);
    }
    

    private void GetGraphic(QRCodeData qrCodeData, int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
    {
        var size = (qrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
        var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

        texture.Resize(size, size);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, size, size), Vector2.one * 0.5f);

        for (var x = 0; x < size + offset; x = x + pixelsPerModule)
        {
            for (var y = 0; y < size + offset; y = y + pixelsPerModule)
            {
                var module = qrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                if (module)
                {
                    FillRectangle(darkColor, new Rect(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                }
                else
                {
                    FillRectangle(lightColor, new Rect(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                }
            }
        }
        
        texture.Apply();
    }

    private void FillRectangle(Color color, Rect rect)
    {
        for (int i = 0; i < rect.size.x; i++)
        {
            for (int j = 0; j < rect.size.y; j++)
            {
                int x = (int)(rect.position.x + i);
                int y = (int)(rect.position.y + j);
                texture.SetPixel(x, y, color);
            }
        }
    }
}