' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcospaceHabitat
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceHabitat)()

#Region "Constructor"

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Me.DBID = DBID
        Me.m_dataType = eDataTypes.EcospaceHabitat
        Me.m_coreComponent = eCoreComponentType.Ecospace

        Dim val As cValue

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' HabAreaProportion
            val = New cValue(core, New Single, eVarNameFlags.HabAreaProportion, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceHabitat.")
            m_logger.LogError(ex, Me.ToString & ".New(nGroups) Error creating new cEcospaceHabitat. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Properties by dot (.) operator "

    Public Property HabAreaProportion() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.HabAreaProportion))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

#Region "Status by dot (.) operator"

    Public Property HabAreaProportionStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.HabAreaProportion)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.HabAreaProportion, value)
        End Set
    End Property

#End Region

End Class
