' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcosimStats
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcosimStats)()

    Sub New(core As cCore)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Me.DBID = cCore.NULL_VALUE
        Me.m_dataType = eDataTypes.EcoSimStatistics
        Me.m_coreComponent = eCoreComponentType.Ecosim

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'SS
            val = New cValue(core, New Single, eVarNameFlags.EcosimSS, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'SSGroup
            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcosimSSGroup, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcosimStats.")
            m_logger.LogError(ex, Me.ToString & ".New(nGroups) Error creating new cEcosimStats. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.BoolArray
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
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimSS))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimSS, value)
        End Set
    End Property

    Public Property SSGroup(iGroup As Integer) As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcosimSSGroup, iGroup))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcosimSSGroup, value, iGroup)
        End Set
    End Property

    Public Property SSStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimSS)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimSS, value)
        End Set
    End Property

    Public Property SSGroupStatus(iGroup As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcosimSS, iGroup)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcosimSS, value, iGroup)
        End Set
    End Property

End Class
