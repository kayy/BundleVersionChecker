BundleVersionChecker
====================

Smart workaround to get Unity's bundle version information from PlayerSettings.bundleVersion in your source code by automatic code generation from a Unity editor class.

There is a blog postings in 3 parts describing the details
Usage:
http://www.scio.de/en/blog-a-news/scio-development-blog-en/entry/accessing-bundle-version-in-unity-ios-runtime-3

Concept:
http://www.scio.de/en/blog-a-news/scio-development-blog-en/entry/accessing-bundle-version-in-unity-ios-runtime-2

Problem description and basic solution:
http://www.scio.de/en/blog-a-news/scio-development-blog-en/entry/accessing-bundle-version-in-unity-at-runtime-1


Quick Start:

- Ensure that you have a bundle version defined in Unity's Player Settings: In Unity go to menu Edit / Project Settings / Player. In Inspector choose "Settings for iOS" under "Per-Platform Settings" and look in section other settings
- Download BundleVersionChecker from GitHub
- Put it in a new directory BundleVersionChecker under Assets/Editor; The code within the project has no sub-directories so it's easy to create a submodule if you are using git as version control system
- Bring Unity to the foreground (or start it if you haven't it running)
- In console you should see 2 messages after a few seconds:
     - Very first call, class file "TrackedBundleVersion".cs will be generated for the first tracked version 0.8.5 (or whatever version)
     - Successfully copied template for class TrackedBundleVersionInfo to Assets/Scripts/Config/Generated/TrackedBundleVersionInfo.cs

Now you find 2 new classes in a new directory under Assets/Scripts/Config/Generated TrackedBundleVersion.cs and TrackedBundleVersionInfo.cs.

 
