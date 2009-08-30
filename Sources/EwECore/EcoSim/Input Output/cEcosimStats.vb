'==============================================================================
'
' $Log: cEcosimStats.vb,v $
' Revision 1.4  2009/01/29 17:41:45  jeroens
' Fixed another copy/paste bug
'
' Revision 1.3  2009/01/29 16:11:43  jeroens
' Fixed copy/paste bugs
' Uses new datatype
'
' Revision 1.2  2009/01/16 18:30:17  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:20  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcosimStats
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing

        Me.DBID = iDBID
        Me.m_dataType = eDataTypes.EcoSimStatistics
        Me.m_coreComponent = eCoreComponentType.EcoSim

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimStatistics, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            'SS
            val = New cValue(New Single, eVarNameFlags.EcosimSS, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'SSGroup
            val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcosimSSGroup, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups, AddressOf m_core.GetCoreCounter)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcosimStats.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcosimStats. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray, eValueTypes.LayerArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function


    Public Property SS() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSS))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSS, value)
        End Set
    End Property


    Public Property SSGroup(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcosimSSGroup, iGroup))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcosimSSGroup, value, iGroup)
        End Set
    End Property

    Public Property SSStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimSS)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimSS, value)
        End Set
    End Property


    Public Property SSGroupStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcosimSS, iGroup)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcosimSS, value, iGroup)
        End Set
    End Property
End Class
