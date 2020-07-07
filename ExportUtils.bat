@echo off

SETLOCAL

set ROOT_FOLDER_NAME=HTC.UPMRegistryTool
set OUTPUT_PATH=Builds\UPMRegistryTool
set LOG_FILE_PATH=Logs\ExportUtils.log

if exist %LOG_FILE_PATH% (
	del %LOG_FILE_PATH%
)

>> %LOG_FILE_PATH% (
	if exist %OUTPUT_PATH% (
		rmdir /s /q %OUTPUT_PATH%
	)
	
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Resources" "%OUTPUT_PATH%/Editor/Resources" "LICENSE.txt*"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Resources" "%OUTPUT_PATH%/Editor/Resources" "RegistrySettings.json*"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Scripts/Configs" "%OUTPUT_PATH%/Editor/Scripts/Configs" "RegistrySettings.cs*"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Scripts/Utils" "%OUTPUT_PATH%/Editor/Scripts/Utils"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Scripts" "%OUTPUT_PATH%/Editor/Scripts" "Configs.meta"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Scripts" "%OUTPUT_PATH%/Editor/Scripts" "Utils.meta"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor/Scripts" "%OUTPUT_PATH%/Editor/Scripts" "HTC.UPMRegistryTool.Editor.asmdef*"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor" "%OUTPUT_PATH%/Editor" "Resources.meta"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%/Editor" "%OUTPUT_PATH%/Editor" "Scripts.meta"
	robocopy /np /njh /njs /s "Assets/%ROOT_FOLDER_NAME%/Plugins" "%OUTPUT_PATH%/Plugins"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%" "%OUTPUT_PATH%" "VERSION*"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%" "%OUTPUT_PATH%" "Editor.meta"
	robocopy /np /njh /njs "Assets/%ROOT_FOLDER_NAME%" "%OUTPUT_PATH%" "Plugins.meta"
)

ENDLOCAL