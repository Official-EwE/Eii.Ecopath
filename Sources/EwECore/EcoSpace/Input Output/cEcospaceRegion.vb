'==============================================================================
'
' $Log: cEcospaceRegion.vb,v $
' Revision 1.2  2009/01/16 18:30:24  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/05/29 22:22:45  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.2  2007/09/14 16:54:19  jeroens
' + Added validation status
'
' Revision 1.1  2007/05/01 17:12:33  joeb
' Changed directory structure
'
' Revision 1.3  2007/01/23 16:58:34  jeroens
' + Added DBID to constructor
'
' Revision 1.2  2007/01/18 17:50:14  jeroens
' * Ready to roll
'
' Revision 1.1  2007/01/14 21:18:24  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceRegion
    Inherits cCoreInputOutputBase

#Region "Constructor"

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Try

            Me.m_dataType = eDataTypes.EcospaceRegion
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcospaceHabitat, eCoreComponentType.EcoSpace, Index, cCore.NULL_VALUE)

            ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceRegion.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcospaceRegion. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variables by dot '.' operator "

    ' Haha

#End Region ' Variables by dot '.' operator

End Class
