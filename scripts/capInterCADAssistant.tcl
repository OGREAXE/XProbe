#/////////////////////////////////////////////////////////////////////////////////
#  WARRANTY: NONE. THIS PROGRAM WAS WRITTEN AS "SHAREWARE" AND IS AVAILABLE AS IS
#            AND MAY NOT WORK AS ADVERTISED IN ALL ENVIRONMENTS. THERE IS NO
#            SUPPORT FOR THIS PROGRAM
#  TCL file: capInterCADAssistant.tcl
#
#/////////////////////////////////////////////////////////////////////////////////


package require Tcl 8.4
package provide capInterCADAssistant 1.0

namespace eval ::capInterCADAssistant {

}

proc ::capInterCADAssistant::pickPoint { pCommandName } {    

	set lSelectedObjects [GetSelectedObjects]
	#set lStatus [DboState]
	#set lIdsString ""
	
	set lRefDesStr [DboTclHelper_sMakeCString]
	set valueTemp ""
	set valueTempParts ""
	set valueTempNets ""
	set valueTempPins ""
	
	#DboState_WriteToSessionLog [DboTclHelper_sMakeCString "begin"]
	
	foreach lObject $lSelectedObjects { 
		#set lObjectId [$lObject GetId $lStatus]
		#object type enum
		#parts :  ::DboBaseObject_PLACED_INSTANCE
			
		#set lIdsString "$lIdsString,$lObjectId"
		
		set lObjectType [$lObject GetObjectType]
		
        DboState_WriteToSessionLog [DboTclHelper_sMakeCString "type is $lObjectType"]
		
		switch -exact -- $lObjectType {
		    13 {
			    set lObject [DboBaseObjectToDboGraphicInstance $lObject]
			    set lObject [DboGraphicInstanceToDboPartInst $lObject]
			    set scmOcc [GetInstanceOccurrence]
			    set lState [DboState]
			    set curOcc [$scmOcc GetInstOccurrence $lObject $lState]

			    if {$curOcc != "NULL" && $curOcc != 0} {
				    $curOcc GetReferenceDesignator $lRefDesStr
				    set valueTemp "[DboTclHelper_sGetConstCharPtr $lRefDesStr]"
				    set valueTempParts "$valueTempParts,$valueTemp"
			    }
		    }
		    20 -
		    21 {
			    set lObject [DboBaseObjectToDboWire $lObject]
			
			    $lObject GetNetName $lRefDesStr

			    set valueTemp "[DboTclHelper_sGetConstCharPtr $lRefDesStr]"
			    set valueTempNets "$valueTempNets,$valueTemp"
		    }
		    16 -
		    39 -
		    49 {
			    set lParent [$lObject GetOwner]
			    set valueTemp [::capAppInterCAD::ixPartReference $lParent]

				$lObject GetPinName $lRefDesStr
				
				set pinName "[DboTclHelper_sGetConstCharPtr $lRefDesStr]"
				DboState_WriteToSessionLog [DboTclHelper_sMakeCString "pin name is $pinName"]
								
				set valueTempPins "$valueTempPins,$valueTemp-$pinName"
		    }
		    23 -
		    37 -
		    38 {
			    #set lObject [DboBaseObjectToDboGraphicInstance $lObject]
			    #set lObject [DboGraphicInstanceToDboNetSymbolInstance $lObject]
			
			    #$lObject GetName $lRefDesStr
			    #set valueTemp "N[DboTclHelper_sGetConstCharPtr $lRefDesStr]"
		    }
		    default {
		    }
	    }

	}
	
	set nValueLength [string length $valueTempParts]
	set valueTempParts [string range $valueTempParts 1 $nValueLength]	
	
	set nValueLength [string length $valueTempNets]
	set valueTempNets [string range $valueTempNets 1 $nValueLength]
	
	set nValueLength [string length $valueTempPins]
	set valueTempPins [string range $valueTempPins 1 $nValueLength]
	
	# register selected reference to registry
	set keyName {HKEY_CURRENT_USER\Software\VB and VBA Program Settings\XProbe\OrCAD Capture}
	
	set valueName "Parts"
	exec reg add $keyName /v $valueName /t REG_SZ /d $valueTempParts /f
	
	set valueName "Nets"
	exec reg add $keyName /v $valueName /t REG_SZ /d $valueTempNets /f
	
	set valueName "Pins"
	exec reg add $keyName /v $valueName /t REG_SZ /d $valueTempPins /f
	
	DboTclHelper_sReleaseAllCreatedPtrs
	
	return
}

proc ::capInterCADAssistant::needsCallback { pCommandName } {
	set ret 0
	
	if {$pCommandName=="OnLButtonUp"} {
		set ret 1
	} 
	
	return $ret
}

proc ::capInterCADAssistant::registerCommandListener {} {
    RegisterAction "_cdnOrSchViewCmdComplete" "::capInterCADAssistant::needsCallback" "" "::capInterCADAssistant::pickPoint" ""
}

::capInterCADAssistant::registerCommandListener
