/* ​
* Copyright © 2022 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections.Generic;


namespace Interhaptics.Platforms.Mobile.Tools
{

    public static class iOSUtilities
    {

        public static string BufferToAHAP(float[] buffer, float[] frequencies, float[] TransientTimer, float[] TransientGain, float duration, float timestep)
        {
            if (buffer == null)
            {
                return null;
            }

            if (frequencies == null)
            {
                UnityEngine.Debug.Log("freq null");
            }

            if (TransientTimer == null || TransientTimer.Length == 0)
            {
                UnityEngine.Debug.Log("trans timer null");
            }

            if (TransientGain == null)
            {
                UnityEngine.Debug.Log("trans gain null");
            }

            List<ParameterCurve> Pattern = new List<ParameterCurve>();

            List<Event> Transients = new List<Event>();

            string json = "";

            Event e = new Event();
            e.Time = 0;
            e.eventType = EventType.HapticContinuous;
            e.eventDuration = duration;
            EventParameter intensity, sharpness;
            intensity.parameterID = ParameterID.HapticIntensity;
            intensity.parameterValue = 1;
            sharpness.parameterID = ParameterID.HapticSharpness;
            sharpness.parameterValue = 0;
            e.eventParameters = new EventParameter[2];
            e.eventParameters[0] = intensity;
            e.eventParameters[1] = sharpness;

            //Pattern.Add(e);

            int i = 0;

            if (TransientTimer != null && TransientGain != null)
            {
                while ((i < TransientTimer.Length) && (i < TransientGain.Length))
                {
                    Event CurrentTransient = new Event();
                    CurrentTransient.Time = TransientTimer[i];
                    CurrentTransient.eventType = EventType.HapticTransient;
                    EventParameter intensityTrans, sharpnessTrans;
                    intensityTrans.parameterID = ParameterID.HapticIntensity;
                    intensityTrans.parameterValue = TransientGain[i];
                    sharpnessTrans.parameterID = ParameterID.HapticSharpness;
                    sharpnessTrans.parameterValue = 0.5f;
                    CurrentTransient.eventParameters = new EventParameter[2];
                    CurrentTransient.eventParameters[0] = intensityTrans;
                    CurrentTransient.eventParameters[1] = sharpnessTrans;

                    Transients.Add(CurrentTransient);
                    UnityEngine.Debug.Log("transient added" + CurrentTransient.ToJson());

                    i++;
                }
            }

            i = 0;

            //Amplitude pattern
            while (i < buffer.Length)
            {
                ParameterCurve p1 = new ParameterCurve();
                p1.parameterID = ParameterID.HapticIntensityControl;
                p1.Time = i * timestep;
                p1.parameterCurveControlPoints = new List<ParameterCurveControlPoint>();

                int k = 0;
                for (k = 0; (k < 16) && (i + k) < buffer.Length; k++)
                {
                    ParameterCurveControlPoint point;
                    point.Time = k * timestep;
                    point.ParameterValue = buffer[i + k];
                    p1.parameterCurveControlPoints.Add(point);
                }

                Pattern.Add(p1);

                i += k;
            }

            i = 0;

            //Frequency pattern
            while (i < frequencies.Length)
            {
                ParameterCurve p2 = new ParameterCurve();
                p2.parameterID = ParameterID.HapticSharpnessControl;
                p2.Time = i * timestep;
                p2.parameterCurveControlPoints = new List<ParameterCurveControlPoint>();

                int k = 0;
                for (k = 0; (k < 16) && (i + k) < frequencies.Length; k++)
                {
                    ParameterCurveControlPoint point;
                    point.Time = k * timestep;
                    float freq = UnityEngine.Mathf.Clamp(frequencies[i + k], 65, 300);
                    point.ParameterValue = (freq - 300.0f) / 235.0f + 1;
                    p2.parameterCurveControlPoints.Add(point);
                }

                Pattern.Add(p2);

                i += k;
            }

            json += "{" +
                        "\"Version\": " + 1 + "," +
                        "\"Metadata\": {" +
                            "\"Project\": \"\"," +
                            "\"Created\": \"\"" +
                           "},";
            json += "\"Pattern\": [" +
                            e.ToJson() + ",";
            if (Transients.Count > 0)
            {
                for (int j = 0; j < Transients.Count; j++)
                {
                    if (j > 0)
                        json += ",";

                    json += ((Event)Transients[j]).ToJsonTransient();
                }
                json += ",";
            }
            if (Pattern.Count > 0)
            {
                for (int j = 0; j < Pattern.Count; j++)
                {
                    if (j > 0)
                    {
                        json += ",";
                    }

                    json += ((ParameterCurve)Pattern[j]).ToJson();
                }
            }

            json += "]}";
            return json;
        }
    }

    public class MetaData
    {
        public string Project = "";
        public string Created = "";
    }


    public abstract class PatternObject
    {
        public abstract string ToJson();
    }

    public class Event
    {

        public string ToJson()
        {
            string json = "";

            string eventParametersStr = "\"EventParameters\": [";

            if (eventParameters.Length > 0)
            {
                for (int i = 0; i < eventParameters.Length; i++)
                {
                    if (i > 0)
                    {
                        eventParametersStr += ", ";
                    }

                    eventParametersStr += "{" + "" +
                                            "\"ParameterID\": \"" + eventParameters[i].parameterID.ToString() + "\"," +
                                            "\"ParameterValue\": " + eventParameters[i].parameterValue +
                                          "}";
                }
            }
            eventParametersStr += "]";

            json += "{" +
                        "\"Event\": {" +
                            "\"Time\": " + Time + "," +
                            "\"EventType\": \"" + eventType.ToString() + "\"," +
                            "\"EventDuration\": " + eventDuration + "," +
                            eventParametersStr +
                        "}" +
                    "}";

            return json;
        }

        public string ToJsonTransient()
        {
            string json = "";

            string eventParametersStr = "\"EventParameters\": [";

            if (eventParameters.Length > 0)
            {
                for (int i = 0; i < eventParameters.Length; i++)
                {
                    if (i > 0)
                    {
                        eventParametersStr += ", ";
                    }

                    eventParametersStr += "{" + "" +
                                            "\"ParameterID\": \"" + eventParameters[i].parameterID.ToString() + "\"," +
                                            "\"ParameterValue\": " + eventParameters[i].parameterValue +
                                          "}";
                }
            }
            eventParametersStr += "]";

            json += "{" +
                        "\"Event\": {" +
                            "\"Time\": " + Time + "," +
                            "\"EventType\": \"" + EventType.HapticTransient.ToString() + "\"," +
                            eventParametersStr +
                        "}" +
                    "}";

            return json;
        }

        public float Time = 0;
        public EventType eventType = EventType.HapticContinuous;
        public float eventDuration = 0;
        public EventParameter[] eventParameters;

    }

    public class ParameterCurve
    {

        public string ToJson()
        {
            string json = "";

            string pccp = "\"ParameterCurveControlPoints\": [";

            if (parameterCurveControlPoints.Count > 0)
            {
                for (int i = 0; i < parameterCurveControlPoints.Count; i++)
                {
                    if (i > 0)
                    {
                        pccp += ", ";
                    }

                    pccp += "{" + "" +
                                "\"Time\": " + parameterCurveControlPoints[i].Time + "," +
                                "\"ParameterValue\": " + parameterCurveControlPoints[i].ParameterValue +
                            "}";
                }
            }
            pccp += "]";

            json += "{" +
                        "\"ParameterCurve\": {" +
                            "\"ParameterID\": \"" + parameterID.ToString() + "\"," +
                            "\"Time\": " + Time + "," +
                            pccp +
                        "}" +
                    "}";

            return json;
        }

        public ParameterID parameterID = ParameterID.HapticIntensityControl;
        public float Time = 0;
        public List<ParameterCurveControlPoint> parameterCurveControlPoints;

    }

    public enum EventType
    {
        HapticContinuous,
        HapticTransient
    }

    public enum ParameterID
    {
        HapticIntensity,
        HapticIntensityControl,
        HapticSharpness,
        HapticSharpnessControl
    }

    public struct EventParameter
    {
        public ParameterID parameterID;
        public float parameterValue;
    }

    public struct ParameterCurveControlPoint
    {
        public float Time;
        public float ParameterValue;
    }

}