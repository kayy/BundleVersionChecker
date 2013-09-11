// Created by Kay
// Copyright 2013 by SCIO System-Consulting GmbH & Co. KG. All rights reserved.
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

public class TrackedBundleVersionGenerator : AbstractBundleVersionGenerator
{
	
	public TrackedBundleVersionGenerator (string className, string bundleVersion) : base (className, bundleVersion) {
		
	}

	public override string GenerateCode () {
		string code = "";
		return code;
	}

	protected override bool CheckForUpdatesFromClass () {
		throw new System.NotImplementedException ();
	}
	
}
