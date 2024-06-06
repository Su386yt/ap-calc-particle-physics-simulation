using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class PositionGraph : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject posParent;
    public GameObject pixel;
    public GameObject canvas;

    public float height = 150;
    public float width = 550;
    public float startX = 15;
    public float startY = 25;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void clearPosFunction() {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void graphPosFunc() {
        var pixelsNeeded = getPixelsNeeded();
        var xScale = getXScale();
        var yScale = getYScale();

        var tInterval = (gameManager.maxT - gameManager.minT) / pixelsNeeded;

        for (int i = 0; i < pixelsNeeded; i++) {
            var t = tInterval * i + gameManager.minT;
            var pos = (float) gameManager.compiledPosFunc.Call(t).Real;

            if (!double.IsFinite(pos)) {
                continue;
            }

            var x = (t - gameManager.minT) * xScale + startX;
            var y = (pos - gameManager.minPos) * yScale + startY;

            Debug.Log((t - gameManager.minT) * xScale + startX + ", " + xScale);
            Debug.Log(x);

            var instantiatedPixel = Instantiate(pixel);
            instantiatedPixel.transform.SetParent(canvas.transform, true);
            instantiatedPixel.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            instantiatedPixel.name = "t=" + t + ", pos=" + pos;
            instantiatedPixel.transform.SetParent(posParent.transform, true);



        }

    }

    float getYScale() {
        var posRange = (gameManager.maxPos - gameManager.minPos);
        if (posRange == 0) {
            return 0;
        }
        return (height) / (gameManager.maxPos - gameManager.minPos);

    }

    float getXScale() {
        var tRange = (gameManager.maxT - gameManager.minT);

        if (tRange == 0) {
            return 0;
        }

        return (width) / (gameManager.maxT - gameManager.minT);
    }

    int getPixelsNeeded() {
        return Mathf.CeilToInt(width / pixel.GetComponent<RectTransform>().rect.width);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
