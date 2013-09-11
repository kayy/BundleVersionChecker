// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

public class SimpleBundleVersionGenerator : AbstractBundleVersionGenerator
{
	
	public SimpleBundleVersionGenerator (string className, string bundleVersion) : base (className, bundleVersion) {
		
	}
	
	public override string GenerateCode () {
		string code = Line (0, "public class " + className);
		code += Line (0, "{");
		code += Line (1, "\tpublic string version = \"" + bundleVersion + "\";");
		code += Line (0, "}");
		return code;
	}

	protected override bool CheckForUpdatesFromClass () {
		FieldInfo fieldInfo = lastVersionObject.GetType ().GetField ("version");
		version = (string)fieldInfo.GetValue (lastVersionObject);
		if (version != bundleVersion) {
			Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + version + " in class \"" + className + "\"");
			return true;
		}
		return false;
	}
}
