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
using System.Collections;

/// <summary>
/// Generates a simple class CurrentBundleVersion when bundleVersion has changed:
/// public class CurrentBundleVersion
/// {
/// 	public string version = "0.8.5";
/// }
/// </summary>
public class SimpleBundleVersionGenerator : AbstractBundleVersionGenerator
{
	const string VersionField = "version";
	
	public SimpleBundleVersionGenerator (string className, string bundleVersion, string bundleIdentifier) : 
		base (className, bundleVersion, bundleIdentifier) {
	}
	
	protected override bool CheckForUpdatesFromClass () {
		version = GetMember<string> (lastVersionObject, VersionField);
		if (version != null) {
			if (version == bundleVersion) {
				return false;
			}
			Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + version + " in class \"" + className + "\"");
		} else {
			Debug.LogWarning ("Could not access field \"version\" in " + lastVersionObject);
		}
		return true;
	}

	public override string GenerateCode () {
		string code = Line (0, "public class " + className);
		code += Line (0, "{");
		code += Line (1, "public static readonly string bundleIdentifier = \"" + bundleIdentifier + "\";", 2);
		code += Line (1, "\tpublic string version = \"" + bundleVersion + "\";");
		code += Line (0, "}");
		return code;
	}

}
