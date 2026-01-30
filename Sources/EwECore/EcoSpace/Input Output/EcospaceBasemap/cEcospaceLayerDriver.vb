' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



''' <summary>
''' Layer providing access to Ecospace external driving data.
''' </summary>
Public Class cEcospaceLayerDriver
    Inherits cEcospaceLayerSingle

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceLayerDriver)()

#Region " Constructor "

    Sub New(core As cCore, manager As cEcospaceBasemap, iIndex As Integer)

        ' Layer has no name, because this layer is user-defined and users 
        ' are responsible for naming a layer of this type. There will be no default name.
        MyBase.New(core, manager, "", eVarNameFlags.LayerDriver, iIndex)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc As Char()

        Me.AllowValidation = False

        Try
            Me.m_dataType = eDataTypes.EcospaceLayerDriver
            Me.m_coreComponent = eCoreComponentType.Ecospace
            Me.DBID = core.m_EcospaceData.EnvironmentalLayerDBID(iIndex)

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Description
            meta = New cVariableMetaData(60000)
            val = New cValue(core, New String(desc), eVarNameFlags.Description, eStatusFlags.OK, eValueTypes.Str, meta)
            Me.m_values.Add(val.varName, val)

            meta = New cVariableMetaData(50)
            val = New cValue(core, New String(desc), eVarNameFlags.UnitEnvDriver, eStatusFlags.OK, eValueTypes.Str, meta)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, True, eVarNameFlags.EcospaceCapacityEnabled, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'set status flags to default values
            Me.ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceLayerDriver.")
            m_logger.LogError(ex, ".New(..) Error creating new cEcospaceLayerDriver. Error: " & ex.Message)
        End Try

        ' Use local metadata for distributing per-layer units
        Me.m_metadata = cVariableMetaData.Default(eValueTypes.Sng)

    End Sub

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_DRIVER, Me.Index)
    End Function

#End Region ' Constructor

#Region " Overrides "

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = cCore.NULL_VALUE) As Object
        Get
            Try
                If Me.ValidateCellPosition(iRow, iCol) Then
                    Return DirectCast(Me.Data, Single()(,))(Me.Index)(iRow, iCol)
                End If
            Catch ex As Exception

            End Try

            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    DirectCast(Me.Data, Single()(,))(Me.Index)(iRow, iCol) = CSng(value)
                End If
            End If
        End Set
    End Property

#End Region ' Overrides

#Region " Properties by dot (.) operator "

    Public Overrides Property Description() As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Description))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Description, value)
        End Set
    End Property

    Public Overrides Property Units(Optional varName As eVarNameFlags = eVarNameFlags.Name) As String
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.UnitEnvDriver))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.UnitEnvDriver, value)
        End Set
    End Property

    ''' <summary>
    ''' This used to be cEcospaceLayer.IsActive()
    ''' </summary>
    ''' <returns></returns>
    Public Property IsCapacityEnabled As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceCapacityEnabled))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceCapacityEnabled, value)
        End Set
    End Property

    Public Property IsCapacityEnabledStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceCapacityEnabled)
        End Get
        Friend Set(ByVal value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceCapacityEnabled, value)
        End Set
    End Property

#End Region ' Properties by dot (.) operator

End Class
