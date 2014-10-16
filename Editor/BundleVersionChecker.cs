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
/// Magic happens in static constructor which is called from Unity automatically when it detects any changes in 
/// Assets/ directory structure.
/// Depending on the flag "trackedMode" (true by default) a tracked or a simple bundle version class is generated.
/// Tracked means that you have access to older version labels as constants to compare with. 
/// See the 2 blog postings for a more detailed discussion:
/// http://www.scio.de/en/blog-a-news/scio-development-blog-en/entry/accessing-bundle-version-in-unity-at-runtime-1
/// </summary>
[InitializeOnLoad]
public class BundleVersionChecker
{
	static string ClassName {
		get { return (ConfigBundleVersionChecker.trackedMode ? ConfigBundleVersionChecker.TrackedClassName : ConfigBundleVersionChecker.SimpleClassName); }
	}
	
	static string TargetCodeFile { 
		get { return ConfigBundleVersionChecker.TargetDir + "/" + ClassName + ".cs"; }
	}
	
	static AbstractBundleVersionGenerator generator;
	
	/// <summary>
	/// Class attribute [InitializeOnLoad] triggers calling the static constructor on every refresh.
	/// </summary>
	static BundleVersionChecker () {
		string bundleVersion = PlayerSettings.bundleVersion;
		string bundleIdentifier = PlayerSettings.bundleIdentifier;
		if (ConfigBundleVersionChecker.trackedMode) {
			generator = new TrackedBundleVersionGenerator (ClassName, bundleVersion, bundleIdentifier);
		} else {
			generator = new SimpleBundleVersionGenerator (ClassName, bundleVersion, bundleIdentifier);
		}
		if (generator.CheckForUpdates ()) {
            CreateNewBuildVersionClassFile(generator);
		}
	}

	/// <summary>
	/// Creates the new class file for ClassName.cs regardless if there is an existing one or not.
	/// </summary>
	/// <param name='generator'>
	/// Generator object to use.
	/// </param>
    public static void CreateNewBuildVersionClassFile(AbstractBundleVersionGenerator generator)
    {
		string code = generator.GenerateCode ();
		if (System.String.IsNullOrEmpty (code)) {
			Debug.Log ("Code generation stopped, no code to write.");
		}
		CheckOrCreateDirectory (ConfigBundleVersionChecker.TargetDir);
		bool success = false;
		using (StreamWriter writer = new StreamWriter (TargetCodeFile, false)) {
			try {
				writer.WriteLine ("{0}", code);
				success = true;
			} catch (System.Exception ex) {
				string msg = " \n" + ex.ToString ();
				Debug.LogError (msg);
				EditorUtility.DisplayDialog ("Error when trying to regenerate file " + TargetCodeFile, msg, "OK");
			}
		}
		if (success) {
			AssetDatabase.Refresh (ImportAssetOptions.Default);
		}
	}
	
	static void CheckOrCreateDirectory (string dir) {
		if (File.Exists (dir)) {
			Debug.LogWarning (dir + " is a file instead of a directory !");
			return;
		} else if (!Directory.Exists (dir)) {
			try {
				Directory.CreateDirectory (dir);
			} catch (System.Exception ex) {
				Debug.LogWarning (ex.Message);
				throw ex;
			}
		}
	}

	public static bool CopyTrackedBundleVersionInfo () {
		if (!File.Exists (ConfigBundleVersionChecker.TrackedBundleVersionInfoTarget)) {
			CheckOrCreateDirectory (ConfigBundleVersionChecker.TargetDir);
			string srcPath = ConfigBundleVersionChecker.TrackedBundleVersionInfoTemplate;
			if (File.Exists (srcPath)) {
				File.Copy (srcPath, ConfigBundleVersionChecker.TrackedBundleVersionInfoTarget, true);
				Debug.Log ("Successfully copied template for class TrackedBundleVersionInfo to " + ConfigBundleVersionChecker.TrackedBundleVersionInfoTarget);
			} else {
				Debug.LogWarning ("File not found " + srcPath);
				return false;
			}
		}
		return true;
	}
}
