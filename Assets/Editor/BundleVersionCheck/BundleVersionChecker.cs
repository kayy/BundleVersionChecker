// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System.IO;

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
		string lastVersion = CurrentBundleVersion.version;
		if (lastVersion != bundleVersion) {
			Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + lastVersion +" in file \"" + TargetCodeFile + "\"");
			CreateNewBuildVersionClassFile (bundleVersion);
		}
	}

	static string CreateNewBuildVersionClassFile (string bundleVersion) {
		using (StreamWriter writer = new StreamWriter (TargetCodeFile, false)) {
			try {
				string code = GenerateCode (bundleVersion);
				writer.WriteLine ("{0}", code);
			} catch (System.Exception ex) {
				string msg = " threw:\n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog ("Error when trying to regenrate class", msg, "OK");
			}
		}
		return TargetCodeFile;
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
		string code = "public static class " + ClassName + "\n{\n";
		code += System.String.Format ("\tpublic static readonly string version = \"{0}\";", bundleVersion);
		code += "\n}\n";
		return code;
	}
}
