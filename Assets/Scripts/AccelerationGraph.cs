using UnityEngine;

public class AccelerationGraph : MonoBehaviour {
    public GameManager gameManager;
    public GameObject accelParent;
    public GameObject pixel;
    public GameObject canvas;

    public float height = 150;
    public float width = 550;
    public float startX = 15;
    public float startY = -350;
    // Start is called before the first frame update
    void Start() {

    }

    public void clearAccelFunction() {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void graphAccelFunc() {
        var pixelsNeeded = getPixelsNeeded();
        var xScale = getXScale();
        var yScale = getYScale();

        var tInterval = (gameManager.maxT - gameManager.minT) / pixelsNeeded;

        for (int i = 0; i < pixelsNeeded; i++) {
            var t = tInterval * i + gameManager.minT;
            var accel = (float) gameManager.compiledAccelFunc.Call(t).Real;

            if (!double.IsFinite(accel)) {
                continue;
            }

            var x = (t - gameManager.minT) * xScale + startX;
            var y = (accel - gameManager.minAccel) * yScale + startY;

            Debug.Log((t - gameManager.minT) * xScale + startX + ", " + xScale);
            Debug.Log(x);

            var instantiatedPixel = Instantiate(pixel);
            instantiatedPixel.transform.SetParent(canvas.transform, true);
            instantiatedPixel.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            instantiatedPixel.name = "t=" + t + ", accel=" + accel;
            instantiatedPixel.transform.SetParent(accelParent.transform, true);
        }

    }

    float getYScale() {
        var accelRange = (gameManager.maxAccel - gameManager.minAccel);

        if (accelRange == 0) {
            return 0;
        }

        return (height) / (gameManager.maxAccel - gameManager.minAccel);

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
