using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

// -quit -batchmode -nographics -executeMethod Jenkins.PerformWindowsBuild
public class Jenkins
{
	static readonly string[] Levels;
	static Jenkins()
	{
		List<string> scenes = new List<string>();

		foreach (var scene in EditorBuildSettings.scenes)
			if (scene != null && scene.enabled)
				scenes.Add(scene.path);

		Levels = scenes.ToArray();
	}

	static void WebPlayer()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebPlayer);
		BuildPipeline.BuildPlayer(Levels, "output", BuildTarget.WebPlayer, BuildOptions.Development);
	}
}