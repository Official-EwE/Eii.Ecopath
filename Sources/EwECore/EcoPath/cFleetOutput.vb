'==============================================================================
'
' $Log: cFleetOutput.vb,v $
' Revision 1.1  2008/09/26 07:30:18  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2007/03/08 03:24:59  jeroens
' * Dropped iFleet property, replaced its use by generic Index property
'
' Revision 1.3  2006/08/18 15:11:45  joeb
' Renamed ICoreInputOutput.CurrentStatus to ValidationStatus
'
' Revision 1.2  2006/07/13 19:10:32  joeb
' ICoreInputOutputBase uses a reference to the core instead of a delegates to communicate with the core.
'
' Revision 1.1  2006/07/07 11:32:31  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports EwECore.ValueWrapper

''' <summary>
''' Class to encapsulate all variables for a single Fishing Fleet Output
''' </summary>
''' <remarks></remarks>
Public Class cFleetOutput
    Inherits cCoreInputOutputBase

    Private m_nGroups As Integer
    Private m_nDetritusGroups As Integer

#Region "Construction and Intialization"

    Friend Sub New(ByRef TheCore As cCore, ByVal DBID As Integer)
        MyBase.New(TheCore)

        'No data validation for output classes
        Me.AllowValidation = False
        'No messages?
        'Me.m_messageSource = eMessageSource.EcoPath
        Me.m_DataType = eDataTypes.FleetOutput

        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.FleetOutput, eMessageSource.EcoPath, Index, cCore.NULL_VALUE)

        Me.DBID = DBID

        Dim val As cValue = Nothing

    End Sub

#End Region

#Region "Variables via dot (.) operator"

#Region "Indexed Variables"
#End Region

#End Region


End Class
