'==============================================================================
'
' $Log: cEcoSimScenario.vb,v $
' Revision 1.1  2008/09/26 07:30:19  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/06/06 15:56:01  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.5  2008/02/13 03:53:44  jeroens
' Added IsLoaded()
'
' Revision 1.4  2007/12/07 14:41:24  jeroens
' * Uses new baseclass ;)
'
' Revision 1.3  2007/10/30 18:40:36  jeroens
' + Added author, contact
'
' Revision 1.2  2007/05/04 15:32:06  jeroens
' + Uses ecopath as message source
'
' Revision 1.1  2007/05/01 19:02:31  joeb
' Changed directory structure
'
' Revision 1.12  2007/03/28 01:16:33  jeroens
' * Changed all status modification access from Public to Friend
'
' Revision 1.11  2006/12/14 23:32:59  jeroens
' * Updated variable definitions to metadata default value
'
' Revision 1.10  2006/08/18 15:12:04  joeb
' Renamed ICoreInputOutput.CurrentStatus to ValidationStatus
'
' Revision 1.9  2006/07/20 14:09:28  joeb
' Validation using MetaData and Operator classes.
'
' Revision 1.8  2006/07/13 19:10:48  joeb
' ICoreInputOutputBase uses a reference to the core instead of a delegates to communicate with the core.
'
' Revision 1.7  2006/07/07 02:12:07  jeroens
' * Messed with eDataTypes again
'
' Revision 1.6  2006/07/06 05:02:17  jeroens
' * Renamed key eDataTypes
'
' Revision 1.5  2006/06/30 04:44:45  jeroens
' + Added Description
'
' Revision 1.4  2006/06/28 13:59:26  jeroens
' * Renamed iGroup member vars, properties to Index
' * Renamed GroupName vartype and usage to Name where applicable
' * Merged usage of varName Name (fleet) with GroupName
'
' Revision 1.3  2006/06/19 16:09:11  joeb
' jb: Changed validation to not use delegates
'
' Revision 1.2  2006/06/07 03:40:41  jeroens
' + Added cCore.EWE5_NULL_MAX
'
' Revision 1.1  2006/06/02 14:16:05  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Class to encapsulate scenario parameters for a single scenario in the cEcoSim Model
''' </summary>
Public Class cEcoSimScenario
    Inherits cEwEScenario

#Region "Constructor"

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)
        m_DataType = eDataTypes.EcoSimScenario
    End Sub

#End Region

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcosimScenarioIndex = Me.Index)
    End Function

End Class
