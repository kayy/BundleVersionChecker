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

public abstract class AbstractBundleVersionGenerator
{
	protected string className;
	
	protected System.Object lastVersionObject;
	
	protected string bundleVersion;
	
	protected string version = "";
	
	protected AbstractBundleVersionGenerator (string className, string bundleVersion) {
		this.className = className;
		this.bundleVersion = bundleVersion;
	}
	
	/// <summary>
	/// Checks if version field can be read from generated class instance. If so the implementing class has to decide 
	/// whether we are running a newer version or not e.g. version strings are equal, version is less than 
	/// bundleVersion, ...
	/// </summary>
	/// <returns>
	/// True indicates that an update has to be performed.
	/// </returns>
	protected abstract bool CheckForUpdatesFromClass ();
	
	/// <summary>
	/// Regenerates the code for ClassName with new bundle version id.
	/// </summary>
	/// <returns>
	/// Code to write to file i.e. something like:
	/// "public class CurrentBundleVersion
	/// {
	///     public string version = "0.8.5";
	/// }"
	/// </returns>
	public abstract string GenerateCode ();

	public bool CheckForUpdates () {
		Assembly assembly = Assembly.Load ("Assembly-CSharp");
		Type type = assembly.GetType (className);
		if (type != null) {
			lastVersionObject = Activator.CreateInstance (type);
			if (lastVersionObject != null) {
				return CheckForUpdatesFromClass ();
			}
		}
		Debug.Log ("Very first call, class file \"" + className + "\".cs" + " not yet generated.");
		return true;
		
	}

	protected string Line (int tabs, string code, int noOfReturns = 1) {
		string indent = "";
		for (int i = 0; i < tabs; i++) {
			indent += "\t";
		}
		string CRs = "";
		for (int i = 0; i < noOfReturns; i++) {
			CRs += "\n";
		}
		return indent + code + CRs;
	}
	
}

