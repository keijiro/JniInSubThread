using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class JniTest : MonoBehaviour
{
    string text = "(not started)";

    // JNI class and field ID cache.
    IntPtr classUnityPlayer;
    IntPtr fidCurrentActivity;

    void SubThread()
    {
        AndroidJNI.AttachCurrentThread();

        // com.unity3d.player.UnityPlayer.currentActivity
        var currentActivity = AndroidJNI.GetStaticObjectField(classUnityPlayer, fidCurrentActivity);

        // currentActivity.getPackageName()
        var classActivity = AndroidJNI.FindClass("android/app/Activity");
        var midGetPackageName = AndroidJNI.GetMethodID(classActivity, "getPackageName", "()Ljava/lang/String;");
        var packageName = AndroidJNI.CallStringMethod(currentActivity, midGetPackageName, new jvalue[]{});

        text = "PackageName: " + packageName;

        AndroidJNI.DetachCurrentThread();
    }

    void Start()
    {
        // Cache JNI objects which are only available in the main thread.
        classUnityPlayer = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
        fidCurrentActivity = AndroidJNI.GetStaticFieldID(classUnityPlayer, "currentActivity", "Landroid/app/Activity;");

        // Run the subthread.
        var th = new Thread(new ThreadStart(SubThread));
        th.Start();
        th.Join();
    }
    
    void Update()
    {
        GetComponent<GUIText>().text = text;
    }
}
