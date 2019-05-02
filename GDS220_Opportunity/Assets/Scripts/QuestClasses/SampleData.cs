using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SampleType { Aluminium, Iron, Magnesium, Potassium };

public class SampleData : QuestTarget
{
    public SampleType sampleType;
    public float sampleTime;
    public bool sampled;
}
