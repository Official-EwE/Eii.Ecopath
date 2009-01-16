'==============================================================================
'
' $Log: cEcospaceScenario.vb,v $
' Revision 1.2  2009/01/16 18:30:24  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/06/06 15:56:04  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.6  2008/02/13 03:53:42  jeroens
' Added IsLoaded()
'
' Revision 1.5  2007/12/07 14:41:24  jeroens
' * Uses new baseclass ;)
'
' Revision 1.4  2007/10/30 18:40:36  jeroens
' + Added author, contact
'
' Revision 1.3  2007/05/04 15:26:30  jeroens
' + Added description
'
' Revision 1.2  2007/05/04 01:21:59  jeroens
' * s = S
'
' Revision 1.1  2007/05/01 17:12:34  joeb
' Changed directory structure
'
' Revision 1.8  2007/03/28 01:16:33  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.7  2007/02/14 17:16:01  jeroens
' * Renamed Basemap position varnames
'
' Revision 1.6  2007/01/19 04:14:35  jeroens
' * Prepared for split of vars to Basemap and ModelParameters
'
' Revision 1.5  2007/01/18 18:27:07  joeb
' Initialization
'
' Revision 1.4  2007/01/17 16:24:36  jeroens
' - Removed StepSize
'
' Revision 1.3  2007/01/15 14:50:28  jeroens
' * Fixed intial values
'
' Revision 1.2  2007/01/14 21:15:26  jeroens
' + Getting there
'
' Revision 1.1  2006/12/04 14:38:23  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceScenario
    Inherits cEwEScenario

#Region " Constructor "

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)
        m_dataType = eDataTypes.EcoSpaceScenario
    End Sub

#End Region ' Constructor

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcospaceScenarioIndex = Me.Index)
    End Function

End Class
