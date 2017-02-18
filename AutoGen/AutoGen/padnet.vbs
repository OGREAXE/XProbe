'--------------------------------------------------------'
' Description:
'   This script intend for print net name to pins
'--------------------------------------------------------'
Option Explicit

' Add any type libraries to be used. 
Scripting.AddTypeLibrary("MGCPCB.ExpeditionPCBApplication")
Scripting.AddTypeLibrary("MGCSDD.CommandBarsEx")

' Execute!
'CALL InitMenuItem
CALL InitMenuItem
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Sub addMenuItem(barControlsColl, caption, description, method)
    Dim btnObj
    Set btnObj = Nothing

    Dim bb
    For Each bb In barControlsColl
        If bb.Caption = caption Then
            Set btnObj = bb
            Exit For
        End If
    Next

    If btnObj is Nothing Then
        Set btnObj = barControlsColl.Add(cmdControlButton,,,-1)
    Else

    End If

    ' Configure the new button
    btnObj.Caption = caption
    btnObj.DescriptionText = description
    btnObj.Target = ScriptEngine
    btnObj.ExecuteMethod = method
End Sub

'Local functions
Function InitMenuItem()
    ' Get the application object 
    Dim pcbApp
    Set pcbApp = Application

    ' Get the document menu bar.
    Dim docMenuBarObj
    Set docMenuBarObj = pcbApp.Gui.CommandBars("Document Menu Bar")

    ' Get the collection of controls for the menu 
    '(i.e. menu popup buttons, File, Edit, View, etc...)
    Dim docMenuBarCtrlColl
    Set docMenuBarCtrlColl = docMenuBarObj.Controls

    ' Find the View menu control
    Dim targetMenuObj
    Set targetMenuObj = Nothing
    Set targetMenuObj = docMenuBarCtrlColl.Item(3)

    'Get the control collection for View
    Dim barControlsColl
    Set barControlsColl = targetMenuObj.Controls

    ' Create the new button by adding to the control collection
    CALL addMenuItem(barControlsColl, "&PadNet", "add pad net to user layer 'padnet'", "OnGeneratePartNet2UL")
    CALL addMenuItem(barControlsColl, "&RemovePadNet", "remove 'padnet' user layer", "OnRemovePartNetUL")

    MsgBox("Install menu item OK. At:[View->PadNet]&[View->RemovePadNet]")
    Scripting.DontExit = True
End Function

Function OnGeneratePartNet2UL(nId)
    ' generate part values for bottom layer
	CALL GenerateContentLayer("PadNet_bot", epcbSideBottom)
    CALL GenerateContentLayer("PadNet_top", epcbSideTop)
End Function

Function OnRemovePartNetUL(nId)
    ' generate part values for bottom layer
    CALL RemoveUserLayer("PadNet")
	CALL RemoveUserLayer("PadNet_top")
	CALL RemoveUserLayer("PadNet_bot")
End Function

Function RemoveUserLayer(userLayerName)
    Dim pcbDoc
    Set pcbDoc = RetainActiveDoc()

    ' Get User Layer "PartValue"
    ' If not Exist create it
    ' See if the user layer already exists.
    Dim usrLyrObj
    Set usrLyrObj = pcbDoc.FindUserLayer(userLayerName)
    If usrLyrObj Is Nothing Then
    Else
        ' It does exist, remove any gfx on the user layer
        pcbDoc.UserLayerTexts(, userLayerName).Delete
        usrLyrObj.Delete
    End If
End Function

Function GenerateContentLayer(userLayerName,side)
    Dim pcbDoc
    Set pcbDoc = RetainActiveDoc()
	pcbDoc.CurrentUnit =  EPcbUnit.epcbUnitMils
    ' Get User Layer "PartValue"
    ' If not Exist create it
    ' See if the user layer already exists.
    Dim usrLyrObj
    Set usrLyrObj = pcbDoc.FindUserLayer(userLayerName)
    If usrLyrObj Is Nothing Then
    Set usrLyrObj = pcbDoc.SetupParameter.PutUserLayer(userLayerName)
    Else
        ' It does exist, remove any gfx on the user layer
        pcbDoc.UserLayerTexts(, userLayerName).Delete
    End If

	' TOP level
	Dim displCtrlObj
	Set displCtrlObj = pcbDoc.ActiveViewEx.DisplayControl
	Dim dispCtrlGlobalObj
	Set dispCtrlGlobalObj = displCtrlObj.Global
	dispCtrlGlobalObj.UserLayerDisplayLevel(userLayerName) = epcbDisplayLevelTop
	dispCtrlGlobalObj.RaiseUserLayerDisplayLevel(userLayerName)

    ' Get the component collection
    Dim cmpsColl
    Set cmpsColl = pcbDoc.Components
    
    ' filter
    Dim i
    For i = cmpsColl.Count To 1 Step -1
    'Determine if this component is on the size.
    If Not cmpsColl.Item(i).Side = side Then  
        'Component is not on desired side so remove it 
        cmpsColl.Remove(i)
    End If
    Next

    ' Filter collection to remove any component not 
    ' found on bottom side of board.
    Dim part
    Dim v
    Dim vp
    Dim pins
    Dim pin
    Dim ch
    Dim pen
    pen = 1
	'Dim partOrientaion
	Dim textOrientation

    For Each part in cmpsColl
        ch = CalcFontHeightForPartPins(part)
        set pins = part.Pins
		'partOrientaion = part.Orientation
        For Each pin in pins
            
            'MsgBox "N:" & part.RefDes  & ",Net:" & pin.Net.Name & ",CX:" & cx & ",CY:" & cy
            If pin.Net is Nothing Then
			Else
			On Error Resume Next
			textOrientation = CalcFontOrientationForPin(pin)
			'If part.RefDes = "R212" Then
			'	Dim extrema
			'	Dim cx
			'	Dim cy
			'	set extrema = pin.Extrema
			'	cx = extrema.MaxX-extrema.MinX
			'	cy = extrema.MaxY-extrema.MinY				
			'	MsgBox "O:" & textOrientation & ",cx:" & cx & ",cy:" & cy & ",Net:" & pin.Net.Name
			'End If
            ' ch for font height, pen for pen width
            Call pcbDoc.PutUserLayerText(pin.Net.Name,pin.PositionX, pin.PositionY, usrLyrObj,ch,textOrientation,pen,,1,,,part)
			If Err.Number <> 0 Then
				'MsgBox "ERR,N:" & part.RefDes  & ",Net:" & pin.Net.Name & ",CH:" & ch & ",orientation:" & pin.Orientation
				'MsgBox "Src:" & Err.Source & ",Des:" & Err.Description
				Err.Clear
			End If
            'Call pin.PutUserLayerText(pin.Net.Name,pin.PositionX, pin.PositionY, epcbTextPinUserDefined, usrLyrObj)
            'Exit For
			End If
        Next
        'Exit For
    Next

End Function

Function CalcFontOrientationForPin(pin)
    Dim extrema
    Dim cx
    Dim cy
	Dim orientation
	orientation = pin.Orientation

	set extrema = pin.Extrema
	cx = extrema.MaxX-extrema.MinX
	cy = extrema.MaxY-extrema.MinY
	
	Dim dir
	dir = orientation Mod 180

	If dir = 0 And cy > cx Then
		orientation = orientation + 90
	ElseIf dir <> 0 And cx > cy Then
		orientation = orientation - 90
	End If

	CalcFontOrientationForPin = orientation
End Function

Function CalcFontHeightForPartPins(part)
    Dim pins
    Dim pin
    Dim extrema
    Dim cx
    Dim cy
    Dim ch
    Dim netnameSize
    Dim finalCh
    finalCh = 100000
    set pins = part.Pins
    For Each pin in pins
        set extrema = pin.Extrema
        cx = extrema.MaxX-extrema.MinX
        cy = extrema.MaxY-extrema.MinY
        netnameSize = Len(pin.Net.Name)
        ch = cy
        If cx > cy Then
            ch = cx
        End If
        ch = (ch / netnameSize)
        If (finalCh > ch) Then
            finalCh = ch
        End If
    Next

    'If finalCh < 1 Then
    '    finalCh = 6
    'End If

    CalcFontHeightForPartPins = finalCh
End Function

Function RetainActiveDoc()
' Get the application object 
    Dim pcbApp
    Set pcbApp = Application

    ' Get the active document 
    Dim pcbDoc
    Set pcbDoc = pcbApp.ActiveDocument

    ' License the document 
    ValidateServer(pcbDoc)

    Set RetainActiveDoc = pcbDoc
End Function

' Server validation function
Function ValidateServer(docObj)
  
  Dim keyInt
  Dim licenseTokenInt
  Dim licenseServerObj

  ' Ask Expedition’s document for the key
  keyInt = docObj.Validate(0)

  ' Get license server
  Set licenseServerObj = CreateObject("MGCPCBAutomationLicensing.Application")

  ' Ask the license server for the license token
  licenseTokenInt = licenseServerObj.GetToken(keyInt)

  ' Release license server
  Set licenseServerObj = nothing

  ' Turn off error messages (validate may fail if the token is incorrect)
  On Error Resume Next
  Err.Clear

  ' Ask the document to validate the license token
  docObj.Validate(licenseTokenInt)
  If Err Then
	  ValidateServer = 0    
  Else
	  ValidateServer = 1
  End If

End Function
