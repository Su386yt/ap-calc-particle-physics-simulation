using AngouriMath;
using AngouriMath.Extensions;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using AngouriMath.Core;


public class GameManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI functionEntry;
    public TMPro.TextMeshProUGUI functionInformation;
    public TMPro.TextMeshProUGUI timeText;

    public RectTransform particle;

    public PositionGraph positionGraph;
    public VelocityGraph velocityGraph;
    public AccelerationGraph accelerationGraph;

    public float startTime = 0;
    public float maxT = 4;
    public float minT = -4;
    public float cycleInterval = 3.5f;

    // The min and max values of the position function
    public float minPos = 0;
    public float maxPos = 0;

    // The min and max values of the velocity function
    public float minVel = 0;
    public float maxVel = 0;

    // The min and max values of the accelerations function
    public float minAccel = 0;
    public float maxAccel = 0;

    public float minUIPosX = 30;
    public float maxUIPosX = 550;
    public float UIPosY = 30;

    public List<float> criticalVelocities = new List<float>();

    public float t = 0;


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.fixedTime;
        UpdateFunction();
    }

    // Update is called once per frame
    void Update()
    {
        var posX = calculatePosition();
        timeText.text = "t = " + getT(); 
        particle.anchoredPosition = new Vector2(posX, UIPosY);
    }

    float getT()
    {
        var timeInCycle = ((Time.fixedTime - startTime) % (cycleInterval));
        var timeInDoubleCycle = (Time.fixedTime - startTime) % (cycleInterval * 2);
         //if its on the way back
        if (timeInCycle != timeInDoubleCycle)
        {

            return (maxT - minT) - ((maxT - minT) * (timeInCycle / cycleInterval)) + minT;
        }
        else
        {
            return ((maxT - minT) * (timeInCycle / cycleInterval)) + minT;

        }
    }

    public Entity posFunc = "t";
    public Entity velocFunc = "1";
    public Entity accelFunc = "0";

    public FastExpression compiledPosFunc = "t".Compile("t");
    public FastExpression compiledVelocFunc = "1".Compile("t");
    public FastExpression compiledAccelFunc = "0".Compile("t");

    void UpdateFromPosFunc()
    {
        var rawValue = functionEntry.text.Trim();

        rawValue = Regex.Replace(rawValue, @"[^\x00-\x7F]+", string.Empty);

        var functionPrefix = "x(t) = ";
        var functionStartIndex = rawValue.IndexOf(functionPrefix);

        posFunc = rawValue.Substring(functionPrefix.Length + functionStartIndex).Simplify();
        velocFunc = posFunc.Differentiate("t").Simplify();
        accelFunc = velocFunc.Differentiate("t").Simplify();
    }

    void UpdateFromVelocFunc()
    {
        var rawValue = functionEntry.text.Trim();

        rawValue = Regex.Replace(rawValue, @"[^\x00-\x7F]+", string.Empty);

        var functionPrefix = "v(t) = ";
        var functionStartIndex = rawValue.IndexOf(functionPrefix);

        velocFunc = rawValue.Substring(functionPrefix.Length + functionStartIndex).Simplify();
        posFunc = velocFunc.Integrate("t").Simplify();
        accelFunc = velocFunc.Differentiate("t").Simplify();
    }

    void UpdateFromAccelFunc()
    {
        var rawValue = functionEntry.text.Trim();

        rawValue = Regex.Replace(rawValue, @"[^\x00-\x7F]+", string.Empty);

        var functionPrefix = "a(t) = ";

        var functionStartIndex = rawValue.IndexOf(functionPrefix);

        accelFunc = rawValue.Substring(functionPrefix.Length + functionStartIndex).Simplify();
        velocFunc = accelFunc.Integrate("t").Simplify();
        posFunc = velocFunc.Integrate("t").Simplify();
    }

    public void UpdateFunction()
    {
        if (functionEntry.text.StartsWith("x(t) = "))
        {
            UpdateFromPosFunc();
        }
        else if (functionEntry.text.StartsWith("v(t) = "))
        {
            UpdateFromVelocFunc();
        }
        else if (functionEntry.text.StartsWith("a(t) = "))
        {
            UpdateFromAccelFunc();
        }

        compiledPosFunc = posFunc.Compile("t");
        compiledVelocFunc = velocFunc.Compile("t");
        compiledAccelFunc = accelFunc.Compile("t");

        criticalVelocities = FindCriticalPoints(posFunc);
        var minPosT = GetMinT(compiledPosFunc, criticalVelocities, minT, maxT);
        minPos = (float)compiledPosFunc.Call(minPosT).Real;
        
        var maxPosT = GetMaxT(compiledPosFunc, criticalVelocities, minT, maxT);
        maxPos = (float)compiledPosFunc.Call(maxPosT).Real;


        var criticalAccelerations = FindCriticalPoints(velocFunc);
        var minVelT = GetMinT(compiledVelocFunc, criticalAccelerations, minT, maxT);
        minVel = (float) compiledVelocFunc.Call(minVelT).Real;

        var maxVelT = GetMaxT(compiledVelocFunc, criticalAccelerations, minT, maxT);
        maxVel = (float) compiledVelocFunc.Call(maxVelT).Real;


        var criticalJerks = FindCriticalPoints(accelFunc);
        var minAccelT = GetMinT(compiledAccelFunc, criticalJerks, minT, maxT);
        minAccel = (float) compiledAccelFunc.Call(minAccelT).Real;

        var maxAccelT = GetMaxT(compiledAccelFunc, criticalJerks, minT, maxT);
        maxAccel = (float) compiledAccelFunc.Call(maxAccelT).Real;

        var functionsString = "";
        functionsString += "Position:\nx(t) = " + posFunc.ToString() + "\n";
        functionsString += "Velocity:\nv(t) = " + velocFunc.ToString() + "\n";
        functionsString += "Acceleration:\na(t) = " + accelFunc.ToString() + "\n\n";


        functionsString += "On the interval t = [" + minT + ", " + maxT + "]:\n";
        functionsString += "Minimum Position: (" + minPosT + ", " + minPos + ")\n";
        functionsString += "Maximum Position: (" + maxPosT + ", " + maxPos + ")\n\n";
        functionsString += "Minimum Velocity: (" + minVelT + ", " + minVel + ")\n";
        functionsString += "Maximum Velocity: (" + maxVelT + ", " + maxVel + ")\n\n";
        functionsString += "Minimum Acceleration: (" + minAccelT + ", " + minAccel + ")\n";
        functionsString += "Maximum Acceleration: (" + maxAccelT + ", " + maxAccel + ")\n\n";

        functionsString += "Critical Times (Position):\n";
        foreach (var point in criticalVelocities) {
            functionsString += point.ToString() + "\n";
        }
        functionsString += "\n";

        functionsString += "Critical Times (Velocity):\n";
        foreach (var point in criticalAccelerations) {
            functionsString += point.ToString() + "\n";
        }
        functionsString += "\n";

        functionsString += "Critical Times (Acceleration):\n";
        foreach (var point in criticalAccelerations) {
            functionsString += point.ToString() + "\n";
        }
        functionsString += "\n";


        functionInformation.text = functionsString;

        positionGraph.clearPosFunction();
        positionGraph.graphPosFunc();

        velocityGraph.clearVelFunction();
        velocityGraph.graphVelFunc();

        accelerationGraph.clearAccelFunction();
        accelerationGraph.graphAccelFunc();
    }


    public float GetMinT(FastExpression compiledFunction, List<float> criticalPoints, float minT, float maxT)
    {
        var pointsToTest = new List<float>(criticalPoints);
        pointsToTest.Add(minT);
        pointsToTest.Add(maxT);

        var results = new List<float>();

        var smallestNumber = compiledFunction.Call(pointsToTest[0]).Real;
        var smallestPoint = pointsToTest[0];

        foreach (var point in pointsToTest)
        {
            if (!(point <= maxT && point >= minT)) {
                continue; 
            }

            var result = (float) compiledFunction.Call(point).Real;

            if (smallestNumber > result)
            {
                smallestNumber = result;
                smallestPoint = point;
            }
        }

        return smallestPoint;
        
    }

    public float GetMaxT(FastExpression compiledFunction, List<float> criticalPoints, float minT, float maxT)
    {
        var pointsToTest = new List<float>(criticalPoints);
        pointsToTest.Add(minT);
        pointsToTest.Add(maxT);

        var results = new List<float>();

        var largestNumber = compiledFunction.Call(pointsToTest[pointsToTest.Count - 1]).Real;
        var largestPoint = pointsToTest[pointsToTest.Count - 1];

        foreach (var point in pointsToTest)
        {
            if (!(point <= maxT && point >= minT)) {
                continue;
            }

            var result = (float) compiledFunction.Call(point).Real;
            if (largestNumber <= result)
            {
                largestNumber = result;
                largestPoint = point;
            }
        }

        return largestPoint;
    }

    public float calculatePosition()
    {
        var scale = (maxUIPosX - minUIPosX) / (maxPos - minPos);

        t = getT();
        return (float) (compiledPosFunc.Call(t).Real - minPos) * scale + minUIPosX;
    }


    public List<float> FindCriticalPoints(Entity function)
    {
        var list = new List<float>();
        var solutions = (function.Differentiate("t").ToString() + " = 0").Solve("t");
        if (solutions is Entity.Set.FiniteSet finiteSet)
        {
            foreach(var t in finiteSet)
            {
                if (!t.EvaluableNumerical) {
                    continue;
                }
                list.Add((float) t.EvalNumerical().RealPart);
            }
        }

        return list;
    }
}
