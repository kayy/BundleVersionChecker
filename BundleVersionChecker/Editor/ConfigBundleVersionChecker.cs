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

/// <summary>
/// Configuration constants for BundleVersionChecker and generator classes.
/// </summary>
public static class ConfigBundleVersionChecker
{
	/// <summary>
	/// If set, version with history tracking will be generated.
	/// </summary>
	public static bool trackedMode = true;

	/// <summary>
	/// Dir where templates are found.
	/// </summary>
	public const string TemplateFileDirectorySearchPattern = "/BundleVersionChecker/Editor/";
	
	/// <summary>
	/// Dir where templates are found.
	/// </summary>
	public const string TemplateFileSearchPattern = TrackedBundleVersionInfoName + ".txt";
	
	/// <summary>
	/// Class name to use when tracking history is disabled.
	/// </summary>
	public const string SimpleClassName = "CurrentBundleVersion";
	
	/// <summary>
	/// Class name when history tracking is enabled.
	/// </summary>
	public const string TrackedClassName = "TrackedBundleVersion";
	
	/// <summary>
	/// Class name used to copy TrackedBundleVersionInfo template to target directory as .cs file. 
	/// NOTE that Unity converts extensions like .cs.txt to .cs automatically. Therefore template name ends with .txt.
	/// </summary>
	public const string TrackedBundleVersionInfoName = "TrackedBundleVersionInfo";
	
	/// <summary>
	/// Target dir for file [ClassName].cs and helper classes (if needed). Change this to the location where you want 
	/// the class(es) to be generated.
	/// </summary>
	public static string TargetDir {
		get { return PlayerPrefs.GetString ("BundleVersionChecker.TargetDir"); }
	}
	
	public static string TrackedBundleVersionInfoTemplate {
		get { 
			string templateDir = PlayerPrefs.GetString ("BundleVersionChecker.TemplateDir");
			return templateDir + "/" + TrackedBundleVersionInfoName + ".txt";
		}
	}
	public static string TrackedBundleVersionInfoTarget {
		get { 
			return TargetDir + "/" + TrackedBundleVersionInfoName + ".cs";
		}
	}

	static ConfigBundleVersionChecker ()
	{
	}
}

