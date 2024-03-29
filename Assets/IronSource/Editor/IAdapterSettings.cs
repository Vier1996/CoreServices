﻿using UnityEditor;

namespace ISEditor.IronSource.Editor
{
	public interface IAdapterSettings
	{
		void updateProject(BuildTarget buildTarget, string projectPath);
		void updateProjectPlist(BuildTarget buildTarget, string plistPath);
	}
}