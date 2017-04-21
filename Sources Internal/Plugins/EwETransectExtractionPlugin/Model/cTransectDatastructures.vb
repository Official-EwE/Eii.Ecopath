Option Strict On
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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Imports EwECore
Imports EwECore.Core
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Datastructures for holding <see cref="cTransect">transects</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransectDatastructures
    Implements IEcospaceLayerManager

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_transects As New List(Of cTransect)
    Private m_selection As cTransect = Nothing

#End Region ' Private vars

#Region " Construction "

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

#End Region ' Construction

#Region " Events "

    ''' <summary>
    ''' Event to notify that the selected transect has changed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="transect">The newly selected transect.</param>
    Public Event OnTransectSelected(sender As cTransectDatastructures, transect As cTransect)

    ''' <summary>
    ''' Event to notify that a transect was added.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="transect">The selected that was added.</param>
    Public Event OnTransectAdded(sender As cTransectDatastructures, transect As cTransect)

    ''' <summary>
    ''' Event to notify that a transect was removed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="transect">The selected that was removed.</param>
    Public Event OnTransectRemoved(sender As cTransectDatastructures, transect As cTransect)

    ''' <summary>
    ''' Event to notify that a transect has been modified.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="transect">The selected that was modified.</param>
    Public Event OnTransectChanged(sender As cTransectDatastructures, transect As cTransect)

#End Region ' Events

#Region " Public access "

    Public Sub Add(t As cTransect)
        Me.m_transects.Add(t)
        Try
            RaiseEvent OnTransectAdded(Me, t)
        Catch ex As Exception

        End Try
        If (Me.m_selection Is Nothing) Then Me.Selection = t
    End Sub

    Public Sub Delete(t As cTransect)
        Me.m_transects.Remove(t)
        Try
            RaiseEvent OnTransectRemoved(Me, t)
        Catch ex As Exception

        End Try
        If (Object.ReferenceEquals(Me.m_selection, t)) Then Me.Selection = Nothing
    End Sub

    Public Property Selection As cTransect
        Get
            Return Me.m_selection
        End Get
        Set(value As cTransect)
            If (Not Object.ReferenceEquals(Me.m_selection, value)) Then
                Me.m_selection = value
                Try
                    RaiseEvent OnTransectSelected(Me, Me.m_selection)
                Catch ex As Exception

                End Try
            End If
        End Set
    End Property

    Public Sub OnChanged(t As cTransect)
        Try
            RaiseEvent OnTransectChanged(Me, t)
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property Transects As cTransect()
        Get
            Return Me.m_transects.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether transect data is automatically saved.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Autosaving As Boolean = False

#End Region ' Public access

#Region " IEcospaceLayerManager implementation "

    Public Function Layers(Optional varName As eVarNameFlags = eVarNameFlags.NotSet) As cEcospaceLayer() Implements IEcospaceLayerManager.Layers
        Return Nothing
    End Function

    Public Function Layer(varName As eVarNameFlags, Optional iIndex As Integer = -9999) As cEcospaceLayer Implements IEcospaceLayerManager.Layer
        Return Nothing
    End Function

    Public Function LayerData(varName As eVarNameFlags, iIndex As Integer) As Object Implements IEcospaceLayerManager.LayerData
        If (Me.Selection Is Nothing) Then Return Nothing
        Return Me.Selection.Cells(Me.m_core.EcospaceBasemap)
    End Function

#End Region ' IEcospaceLayerManager implementation

End Class
