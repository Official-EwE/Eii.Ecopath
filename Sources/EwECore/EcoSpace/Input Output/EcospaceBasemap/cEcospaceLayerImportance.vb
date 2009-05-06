'==============================================================================
'
' $Log: cEcospaceLayerImportance.vb,v $
' Revision 1.3  2009/05/06 12:33:00  jeroens
' Added meaningful datatypes
'
' Revision 1.2  2009/01/16 18:30:23  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/08/13 17:37:08  jeroens
' Renamed LayerImportanceWeight to ImportanceWeight
'
' Revision 1.2  2008/08/11 18:35:50  jeroens
' Uses real Index, not a home-made version
'
' Revision 1.1  2008/08/11 02:00:35  jeroens
' Simplified class names
'
' Revision 1.2  2008/08/09 00:06:59  jeroens
' Simplified
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports directive

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class cEcospaceLayerImportance
    Inherits cEcospaceLayerSingleNxM

#Region " Constructor "

    Sub New(ByRef theCore As cCore, ByVal idBID As Integer, ByRef manager As cEcospaceBasemap, ByVal iIndex As Integer)

        MyBase.New(theCore, idBID, manager, eVarNameFlags.LayerImportance, iIndex)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc As Char()

        Me.AllowValidation = False

        Try
            m_dataType = eDataTypes.EcospaceLayerImportance
            m_coreComponent = eCoreComponentType.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimGroupInput, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            ' Weight
            meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThan), cOperatorManager.getOperator(eOperators.LessThan))
            val = New cValue(0, eVarNameFlags.ImportanceWeight, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            ' Description
            meta = New cVariableMetaData(60000)
            val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceBasemap.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceBasemap. Error: " & ex.Message)
        End Try

    End Sub

#End Region ' Constructor

#Region " Properties by dot (.) operator "

    Public Property Weight() As Single

        Get
            Return CSng(GetVariable(eVarNameFlags.ImportanceWeight))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.ImportanceWeight, value)
        End Set

    End Property

    Public Property Description() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Description))
        End Get
        Set(ByVal value As String)
            SetVariable(eVarNameFlags.Description, value)
        End Set
    End Property

#End Region ' Properties by dot (.) operator

End Class