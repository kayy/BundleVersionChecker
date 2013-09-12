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
/// Base class for bundle version generators providing interfaces to:
/// 1.) load and look up version info -> CheckForUpdates ().
/// 2.) generate code if necessary i.e. newer bundle version or not found on very first start i.e. there is type 
///     "className" in assembly
/// This and all derived classes have to take care to never reference its generated types in code directly. This is 
/// because at very first start after installation, there are several classes not defined as they are not yet generated.
/// So when dealing with generated classes always use reflection and native types.
/// </summary>
public abstract class AbstractBundleVersionGenerator
{
	/// <summary>
	/// The name of the version class to look up its version info by reflection.
	/// </summary>
	protected string className;
	/// <summary>
	/// The last version object of type "className" instantiated at start. Used to look up "version".
	/// </summary>
	protected object lastVersionObject;
	/// <summary>
	/// Current bundle version read from PlayerSettings.bundleVersion.
	/// </summary>
	protected string bundleVersion;
	/// <summary>
	/// The version found in className instance.
	/// </summary>
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
	
	/// <summary>
	/// Retrieve member called "name" from object via reflection.
	/// </summary>
	/// <returns>
	/// Member or default (T) i.e. null or 0.
	/// </returns>
	/// <param name='o'>
	/// Object to inspect.
	/// </param>
	/// <param name='name'>
	/// Name of the field.
	/// </param>
	/// <typeparam name='T'>
	/// Type of member.
	/// </typeparam>
	protected T GetMember<T> (object o, string name) {
		if (o != null) {
			FieldInfo fieldInfo = o.GetType ().GetField (name);
			if (fieldInfo != null) {
				return (T)fieldInfo.GetValue (o);
			}
		}
		return default (T);
	}
	
	/// <summary>
	/// Creates an instance of "name" provided that "name" has a public constructor with no arguments.
	/// </summary>
	/// <returns>
	/// The instance or null.
	/// </returns>
	/// <param name='name'>
	/// Class name.
	/// </param>
	/// <param name='assemblyName'>
	/// "Assembly-CSharp" if not specified
	/// </param>
	protected object CreateInstance (string name, string assemblyName = "Assembly-CSharp") {
		Assembly assembly = Assembly.Load (assemblyName);
		Type type = assembly.GetType (name);
		if (type != null) {
			return Activator.CreateInstance (type);
		}
		return null;
	}
	
	/// <summary>
	/// Checks if an update of bundle version class is required.
	/// </summary>
	/// <returns>
	/// true if regeneration of version class is necessary.
	/// </returns>
	public bool CheckForUpdates () {
		lastVersionObject = CreateInstance (className);
		if (lastVersionObject != null) {
			return CheckForUpdatesFromClass ();
		}
		Debug.Log ("Very first call, class file \"" + className + "\".cs" + " will be generated for the first tracked version " + bundleVersion);
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

