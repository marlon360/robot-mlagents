using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResultLogger
{

    private static int tries = 0;
    private static int successes = 0;

    public static void LogRatio() {
        if (tries > 0) {
            Debug.Log("Current Ratio "+ ((float) successes / (float) tries));
        }
    }

    public static void AddTry() {
        if (tries > 200) {
            tries = 0;
            successes = 0;
        }
        tries++;
    }

    public static void AddSuccess() {
        successes++;
    }

}
