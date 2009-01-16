'==============================================================================
'
' $Log: cEcotracerScenario.vb,v $
' Revision 1.2  2009/01/16 18:30:25  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/06/06 15:55:55  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.5  2008/02/13 03:53:41  jeroens
' Added IsLoaded()
'
' Revision 1.4  2007/12/07 14:41:24  jeroens
' * Uses new baseclass ;)
'
' Revision 1.3  2007/11/26 02:07:46  jeroens
' + Moved logic to EcotracerModelParameters
'
' Revision 1.2  2007/11/25 02:15:07  jeroens
' * Set correct message datatype
'
' Revision 1.1  2007/11/25 00:33:08  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcotracerScenario
    Inherits cEwEScenario

#Region " Constructor "

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)
        m_dataType = eDataTypes.EcotracerScenario
    End Sub

#End Region ' Constructor

    Public Overrides Function IsLoaded() As Boolean
        Return (Me.m_core.ActiveEcotracerScenarioIndex = Me.Index)
    End Function

End Class
