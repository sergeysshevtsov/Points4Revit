!define PRODUCT_NAME "Points4Revit"
!define RVT_BUNDLE_NAME "${PRODUCT_NAME}.RVT.bundle"
!define ACD_BUNDLE_NAME "${PRODUCT_NAME}.ACD.bundle"
!define BUNDLE_VERSION "1.0.0.1"
!define PUBLISHER "Sergey Shevtsov"

Name "${PRODUCT_NAME}_${BUNDLE_VERSION}"
OutFile "InstallFiles\${PRODUCT_NAME}_${BUNDLE_VERSION}.exe"
Caption "${PRODUCT_NAME}_${BUNDLE_VERSION}"

!include "nsProcess.nsh"
!include "Sections.nsh"
!include "MUI.nsh"
!include "MUI2.nsh"
!include "x64.nsh"
!define MUI_ICON "Logo.ico"

RequestExecutionLevel admin

!define MUI_HEADERIMAGE_BITMAP "Logo.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "Logo.bmp"

Unicode true

!define MUI_FINISHPAGE_LINK 'Click here to visit GitHub repository.'
!define MUI_FINISHPAGE_LINK_LOCATION https://github.com/sergeysshevtsov/Points4Revit
!define MUI_FINISHPAGE_TEXT_LARGE
!define MUI_FINISHPAGE_TEXT "${PRODUCT_NAME} successfully installed on your PC."

!define MUI_LANGDLL_REGISTRY_ROOT "HKCU" 
!define MUI_LANGDLL_REGISTRY_KEY "Software\${BUNDLE_NAME}" 
!define MUI_LANGDLL_REGISTRY_VALUENAME "Installer Language"
!define MUI_LANGDLL_ALWAYSSHOW

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_RESERVEFILE_LANGDLL
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_LANGUAGE "English" ;1033

VIProductVersion ${BUNDLE_VERSION}
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "${PRODUCT_NAME} plugin."
VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "${BUNDLE_VERSION}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "${BUNDLE_VERSION}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "${PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments" "This installation file will install ${PRODUCT_NAME}."
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "${PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "InternalName" "${PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "${PUBLISHER}"
VIAddVersionKey /LANG=${LANG_ENGLISH} "OriginalFilename" "${PRODUCT_NAME}.exe"

Function un.onInit
	!insertmacro MUI_UNGETLANGUAGE
FunctionEnd

Section "${PRODUCT_NAME}.Revit" Section1
	SectionIn RO
	SetShellVarContext "all"
	SetOverwrite on

	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "DisplayIcon" "$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}\Logo.ico"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "DisplayName" "Points4Revit plugin"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "InstallLocation" "$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "UninstallString" "$\"$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}\Uninstall.exe$\"" 
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "Publisher" "${PUBLISHER}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "DisplayVersion" "${BUNDLE_VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "HelpLink" "https://github.com/sergeysshevtsov/Points4Revit"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "NoModify" "1"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}" "NoRepair" "1"

	RMDir /r "$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}"	
	SetOutPath "$APPDATA\Autodesk\ApplicationPlugins\"
		File /nonfatal /a /r "${RVT_BUNDLE_NAME}"

	WriteUninstaller "$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}\Uninstall.exe"
SectionEnd

Section "${PRODUCT_NAME}.AutoCAD" Section2
	SectionIn RO
	SetShellVarContext "all"
	SetOverwrite on
	RMDir /r "$APPDATA\Autodesk\ApplicationPlugins\${ACD_BUNDLE_NAME}"	
	SetOutPath "$APPDATA\Autodesk\ApplicationPlugins\"
		File /nonfatal /a /r "${ACD_BUNDLE_NAME}"
SectionEnd

LangString DESC_Section1 ${LANG_ENGLISH} "Plugin Revit, ${RVT_BUNDLE_NAME}"
LangString DESC_Section1 ${LANG_ENGLISH} "AutoCAD Revit, ${ACD_BUNDLE_NAME}"

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${Section1} $(DESC_Section1)
  !insertmacro MUI_DESCRIPTION_TEXT ${Section2} $(DESC_Section2)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Section "Uninstall"
	SetShellVarContext "all"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${BUNDLE_NAME}"
	RMDir /r "$APPDATA\Autodesk\ApplicationPlugins\${RVT_BUNDLE_NAME}"
	RMDir /r "$APPDATA\Autodesk\ApplicationPlugins\${ACD_BUNDLE_NAME}"	
SectionEnd
