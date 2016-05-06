' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Layer providing access to Ecospace external driving data.
''' </summary>
Public Class cEcospaceLayerDriver
    Inherits cEcospaceLayerSingle

#Region " Constructor "

    Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal iIndex As Integer)

        ' Layer has no name, because this layer is user-defined and users 
        ' are responsible for naming a layer of this type. There will be no default name.
        MyBase.New(theCore, manager, "", eVarNameFlags.LayerDriver, iIndex)

        Dim val As cValue
        Dim meta As cVariableMetaData
        Dim desc As Char()

        Me.AllowValidation = False

        Try
            m_dataType = eDataTypes.EcospaceLayerDriver
            m_coreComponent = eCoreComponentType.EcoSpace

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            ' Description
            meta = New cVariableMetaData(60000)
            val = New cValue(New String(desc), eVarNameFlags.Description, eStatusFlags.NotEditable Or eStatusFlags.Null, eValueTypes.Str, _
                                meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
            m_values.Add(val.varName, val)

            'set status flags to default values
            ResetStatusFlags()

            Me.AllowValidation = True

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceLayerDriver.")
            cLog.Write(Me.ToString & ".New(..) Error creating new cEcospaceLayerDriver. Error: " & ex.Message)
        End Try

    End Sub

    Protected Overrides Function DefaultName() As String
        Return cStringUtils.Localize(My.Resources.CoreDefaults.CORE_DEFAULT_DRIVER, Me.Index)
    End Function

#End Region ' Constructor

#Region " Overrides "

    Public Overrides Property Cell(ByVal iRow As Integer, ByVal iCol As Integer) As Object
        Get
            Try
                If Me.ValidateCellPosition(iRow, iCol) Then
                    Return DirectCast(Me.Data, Single()(,))(Me.Index)(iRow, iCol)
                End If
            Catch ex As Exception

            End Try

            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Object)
            If Me.ValidateCellValue(value) Then
                If Me.ValidateCellPosition(iRow, iCol) Then
                    DirectCast(Me.Data, Single()(,))(Me.Index)(iRow, iCol) = CSng(value)
                End If
            End If
        End Set
    End Property

    Public Overrides ReadOnly Property CanDeactivate As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Property IsActive As Boolean
        Get
            Return Me.IsCapacityMapActive()
        End Get
        Set(value As Boolean)
            Me.ActivateCapacityMap(value)
        End Set
    End Property

#End Region ' Overrides

#Region " Properties by dot (.) operator "

    Public Overrides Property Description() As String
        Get
            Return CStr(GetVariable(eVarNameFlags.Description))
        End Get
        Set(ByVal value As String)
            SetVariable(eVarNameFlags.Description, value)
        End Set
    End Property

#End Region ' Properties by dot (.) operator

#Region " Internals "

    Private Function ActivateCapacityMap(bEnable As Boolean) As Boolean
        Dim manager As cMapResponseInteractionManager = Me.m_core.CapacityMapInteractionManager
        Dim map As IEnviroInputMap = manager.Map(Me)
        If (map IsNot Nothing) Then
            map.isLayerActive = bEnable
        End If
        Return IsCapacityMapActive()
    End Function

    Private Function IsCapacityMapActive() As Boolean
        Dim manager As cMapResponseInteractionManager = Me.m_core.CapacityMapInteractionManager
        Dim map As IEnviroInputMap = manager.Map(Me)
        If (map IsNot Nothing) Then
            Return map.isLayerActive
        End If
        Return True
    End Function

#End Region ' Internals

End Class
