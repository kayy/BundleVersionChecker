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
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

public class TrackedBundleVersionGenerator : AbstractBundleVersionGenerator
{
	const string Current = "current";
	const string VersionInfoVersionField = "version";
	const string HistoryField = "history";
	
	public TrackedBundleVersionGenerator (string className, string bundleVersion, string bundleIdentifier) : 
		base (className, bundleVersion, bundleIdentifier) {
	}
	
	protected override bool CheckForUpdatesFromClass () {
		ArrayList history = GetHistoryFromLastVersionObject ();
		if (history != null && history.Count > 0) {
			version = GetVersionFromLastVersionObject ();
			if (version == bundleVersion) {
				return false;
			}
			Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + version + " in class \"" + className + "\"");
			
		} else {
			Debug.LogWarning ("Field \"history\" is null or does not contain any elements in " + lastVersionObject);
		}
		return true;
	}
	
	string FormatVersionConstantNames (string version) {
		string trimmed = version.Trim ();
		string noDots = trimmed.Replace (".", "_");
		string noBlanks = noDots.Replace (" ", "_");
		Regex rgx = new Regex ("[^a-zA-Z0-9_]");
		string alphaNum = rgx.Replace (noBlanks, "");
		return "Version_" + alphaNum;
	}
	
	ArrayList GetHistoryFromLastVersionObject () {
		return GetMember<ArrayList> (lastVersionObject, HistoryField);
	}
	
	string GetVersionFromVersionInfoObject (object o) {
		return GetMember<string> (o, VersionInfoVersionField);
	}
	
	string GetVersionFromLastVersionObject () {
		object versionInfoObject = GetMember<object> (lastVersionObject, Current);
		if (versionInfoObject != null) {
			return GetMember<string> (versionInfoObject, VersionInfoVersionField);
		}
		return null;
	}
	
	public override string GenerateCode () {
		object trackedBundleVersionInfoObject = CreateInstance ("TrackedBundleVersionInfo");
		if (trackedBundleVersionInfoObject == null) {
			// we are creating TrackedBundleVersion.cs the very first time
			if (!BundleVersionChecker.CopyTrackedBundleVersionInfo ()) {
				// doesn't make sense without TrackedBundleVersionInfo
				return null;
			}
		}
		int versionInfoIndex = 0;
		ArrayList history = GetHistoryFromLastVersionObject ();
		string oldVersionsToAdd = "";
		string code = Line (0, "using System.Collections;", 2);
		code += Line (0, "// GENERATED CODE.");
		code += Line (0, "public class " + className);
		code += Line (0, "{");
		code += Line (1, "public static readonly string bundleIdentifier = \"" + bundleIdentifier + "\";", 2);
		string currentVersionInfoName = null;
		int currentVersionInfoIndex = -1;
		if (history != null) {
			foreach (object versionObject in history) {
				string trackedVersion = GetVersionFromVersionInfoObject (versionObject);
				string f = FormatVersionConstantNames (trackedVersion);
				code += Line (1, "public static readonly TrackedBundleVersionInfo " + f + 
					" =  new TrackedBundleVersionInfo (\"" + trackedVersion + "\", " + versionInfoIndex + ");");
				oldVersionsToAdd += Line (2, "history.Add (" + f + ");");
				if (trackedVersion == bundleVersion) {
					currentVersionInfoName = f;
					currentVersionInfoIndex = versionInfoIndex;
				}
				versionInfoIndex++;
			}
		}
		if (currentVersionInfoIndex < 0 || currentVersionInfoName == null) {
			// Add a constant for the current label too:
			currentVersionInfoName = FormatVersionConstantNames (bundleVersion);
			code += Line (1, "public static readonly TrackedBundleVersionInfo " + currentVersionInfoName + 
				" =  new TrackedBundleVersionInfo (\"" + bundleVersion + "\", " + versionInfoIndex + ");");
			currentVersionInfoIndex = versionInfoIndex;
		}
		code += Line (1, "");
		code += Line (1, "public static readonly TrackedBundleVersion Instance = new TrackedBundleVersion ();", 2);
		code += Line (1, "public static TrackedBundleVersionInfo Current { get { return Instance.current; } }", 2);
		code += Line (1, "public ArrayList history = new ArrayList ();", 2);
		code += Line (1, "public TrackedBundleVersionInfo " + Current + " = " + currentVersionInfoName + ";", 2);
		code += Line (1, "public  " + className + "() {");
		code += oldVersionsToAdd;
		if (currentVersionInfoIndex == versionInfoIndex) {
			// default case: current version is the most recent so it is not yet contained in history; 
			// after a version number rollback the current entry is already contained in history
			code += Line (2, "history.Add (" + Current + ");");
		}
		code += Line (1, "}", 2);
		code += "}";
		return code;
	}

}
