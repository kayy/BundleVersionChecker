// The MIT License (MIT)
// 
//    Copyright 2013 by Kay Bothfeld, Germany
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

/// <summary>
/// Editor class for generating a class containing bundle version information to be accessed at runtime.
/// </summary>
[InitializeOnLoad]
public class BundleVersionChecker
{
	/// <summary>
	/// If set, version with history tracking will be generated.
	/// </summary>
	static bool trackedMode = true;

	/// <summary>
	/// Target dir for file [ClassName].cs.
	/// </summary>
	const string TargetDir = "Assets/Scripts/Config/BundleVersionCheck";
	
	/// <summary>
	/// Class name to use when tracking history is disabled.
	/// </summary>
	const string SimpleClassName = "CurrentBundleVersion";
	/// <summary>
	/// Class name when history tracking is enabled.
	/// </summary>
	const string TrackedClassName = "TrackedBundleVersion";
	
	static string ClassName {
		get { return (trackedMode ? TrackedClassName : SimpleClassName); }
	}
	
	static string TargetCodeFile { 
		get { return TargetDir + "/" + ClassName + ".cs"; }
	}
	
	static AbstractBundleVersionGenerator generator;
	
	static BundleVersionChecker () {
		string bundleVersion = PlayerSettings.bundleVersion;
		if (trackedMode) {
			generator = new TrackedBundleVersionGenerator (ClassName, bundleVersion);
		} else {
			generator = new SimpleBundleVersionGenerator (ClassName, bundleVersion);
		}
		if (generator.CheckForUpdates ()) {
			CreateNewBuildVersionClassFile (bundleVersion);
		}
	}
	
	/// <summary>
	/// Creates the new class file for ClassName.cs regardless if there is an existing one or not.
	/// </summary>
	/// <param name='bundleVersion'>
	/// New bundle version to write into code.
	/// </param>
	static void CreateNewBuildVersionClassFile (string bundleVersion) {
		if (File.Exists (TargetDir)) {
			Debug.LogWarning (TargetDir + " is a file instead of a directory !");
			return;
		} else if (!Directory.Exists (TargetDir)) {
			try {
				Directory.CreateDirectory (TargetDir);
			} catch (System.Exception ex) {
				Debug.LogWarning (ex.Message);
				throw ex;
			}
		}
		using (StreamWriter writer = new StreamWriter (TargetCodeFile, false)) {
			try {
				string code = generator.GenerateCode ();
				writer.WriteLine ("{0}", code);
			} catch (System.Exception ex) {
				string msg = " \n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog ("Error when trying to regenerate file " + TargetCodeFile, msg, "OK");
			}
		}
	}
	
}
