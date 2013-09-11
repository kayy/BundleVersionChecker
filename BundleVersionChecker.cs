// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

[InitializeOnLoad]
public class BundleVersionChecker
{
	/// <summary>
	/// Class name to use when referencing from code.
	/// </summary>
	const string ClassName = "CurrentBundleVersion";
	
	const string TargetCodeFile = "Assets/Scripts/Config/" + ClassName + ".cs";
	
	static BundleVersionChecker () {
		string bundleVersion = PlayerSettings.bundleVersion;
		Assembly assembly = Assembly.Load ("Assembly-CSharp");
		Type type = assembly.GetType (ClassName);
		if (type != null) {
			System.Object lastVersionObject = Activator.CreateInstance (type);
			FieldInfo fieldInfo = type.GetField ("version");
			string lastVersion = (string)fieldInfo.GetValue (lastVersionObject);
			if (lastVersion != bundleVersion) {
				Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + lastVersion +" in file \"" + TargetCodeFile + "\"");
				CreateNewBuildVersionClassFile (bundleVersion);
			}
		} else {
			Debug.Log ("Very first call creating file \"" + TargetCodeFile + "\"" + " for bundle version " + bundleVersion);
			CreateNewBuildVersionClassFile (bundleVersion);
		}
	}

	static void CreateNewBuildVersionClassFile (string bundleVersion) {
		using (StreamWriter writer = new StreamWriter (TargetCodeFile, false)) {
			try {
				string code = GenerateCode (bundleVersion);
				writer.WriteLine ("{0}", code);
			} catch (System.Exception ex) {
				string msg = " \n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog ("Error when trying to regenerate class", msg, "OK");
			}
		}
	}
	
	/// <summary>
	/// Regenerates (and replaces) the code for ClassName with new bundle version id.
	/// </summary>
	/// <returns>
	/// Code to write to file.
	/// </returns>
	/// <param name='bundleVersion'>
	/// New bundle version.
	/// </param>
	static string GenerateCode (string bundleVersion) {
		string code = "public class " + ClassName + "\n{\n";
		code += System.String.Format ("\tpublic string version = \"{0}\";", bundleVersion);
		code += "\n}\n";
		return code;
	}
}
