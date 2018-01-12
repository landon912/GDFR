; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define VERSION_MAJOR 2
#define VERSION_MINOR 4

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7CBECA7B-460F-43DB-BA3E-6367D49C385C}
AppName=Goblins Drool, Fairies Rule!
AppVersion={#VERSION_MAJOR}.{#VERSION_MINOR}
;AppVerName=Goblins Drool, Fairies Rule! {VERSION}
AppPublisher=Game-O-Gami
AppPublisherURL=http://www.game-o-gami.com/
AppSupportURL=http://www.game-o-gami.com/
AppUpdatesURL=http://www.game-o-gami.com/
DefaultDirName={pf}\Goblins Drool, Fairies Rule!
DisableProgramGroupPage=yes
OutputDir=C:\Users\landon\Desktop\GDFR_Builds\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}_RELEASE
OutputBaseFilename=GDFR_Setup_{#VERSION_MAJOR}.{#VERSION_MINOR}
Compression=lzma
SolidCompression=yes
UninstallFilesDir={app}\Uninstall

[Dirs]
Name: {app}\Uninstall; Attribs: hidden;
Name: {app}\Icon; Attribs: hidden;

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "C:\Users\landon\Desktop\GDFR_Builds\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}\GDFR_Icon.ico"; DestDir: "{app}\Icon"; Flags: ignoreversion
Source: "C:\Users\landon\Desktop\GDFR_Builds\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}\*"; DestDir: "{app}"; Excludes: "GDFR_Icon.ico"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\Goblins Drool, Fairies Rule!"; Filename: "{app}\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}.exe"; IconFilename: "{app}\Icon\GDFR_Icon.ico"
Name: "{commondesktop}\Goblins Drool, Fairies Rule!"; Filename: "{app}\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}.exe"; IconFilename: "{app}\Icon\GDFR_Icon.ico"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Goblins Drool, Fairies Rule!"; Filename: "{app}\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}.exe"; IconFilename: "{app}\Icon\GDFR_Icon.ico"; Tasks: quicklaunchicon
Name: {app}\Uninstall_GDFR; Filename: {app}\Uninstall\unins000.exe; IconFilename: "{app}\Icon\GDFR_Icon.ico"



[Run]
Filename: "{app}\GDFR_Windows_{#VERSION_MAJOR}.{#VERSION_MINOR}.exe"; Description: "{cm:LaunchProgram,GDFR}"; Flags: nowait postinstall skipifsilent
