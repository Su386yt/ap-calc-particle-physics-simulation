using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityGraph : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject velParent;
    public GameObject pixel;
    public GameObject canvas;

    public float height = 150;
    public float width = 550;
    public float startX = 15;
    public float startY = -125;
    // Start is called before the first frame update
    void Start() {

    }

    public void clearVelFunction() {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void graphVelFunc() {
        var pixelsNeeded = getPixelsNeeded();
        var xScale = getXScale();
        var yScale = getYScale();

        var tInterval = (gameManager.maxT - gameManager.minT) / pixelsNeeded;

        for (int i = 0; i < pixelsNeeded; i++) {
            var t = tInterval * i + gameManager.minT;
            var vel = (float)gameManager.compiledVelocFunc.Call(t).Real;

            if (!double.IsFinite(vel)) {
                continue;
            }

            var x = (t - gameManager.minT) * xScale + startX;
            var y = (vel - gameManager.minVel) * yScale + startY;

            Debug.Log((t - gameManager.minT) * xScale + startX + ", " + xScale);
            Debug.Log(x);

            var instantiatedPixel = Instantiate(pixel);
            instantiatedPixel.transform.SetParent(canvas.transform, true);
            instantiatedPixel.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            instantiatedPixel.name = "t=" + t + ", vel=" + vel;
            instantiatedPixel.transform.SetParent(velParent.transform, true);
        }

    }

    float getYScale() {
        var velRange = (gameManager.maxVel - gameManager.minVel);

        if (velRange == 0) {
            return 0;
        }

        return (height) / (gameManager.maxVel - gameManager.minVel);

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
    void Update() {

    }
}
