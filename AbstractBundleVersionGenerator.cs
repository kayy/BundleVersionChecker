// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
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
			return true;
		} else {
			Debug.Log ("Very first call, class file \"" + className + "\".cs" + " not yet generated.");
			return true;
		}
		
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

