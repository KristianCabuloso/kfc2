using System;
using System.Collections.Generic;

[System.Serializable]
public class NextPlayersAnalyticsGroup
{
    public List<Health> players = new List<Health>();
    public float totalTime;
}