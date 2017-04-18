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

#End Region ' Imports

Public Class cTransectDatastructures

    Private m_transects As New List(Of cTransect)
    Private m_selection As cTransect = Nothing

    Public ReadOnly Property Transects As cTransect()
        Get
            Return Me.m_transects.ToArray()
        End Get
    End Property

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

    Public Sub OnChanged()
        Try
            RaiseEvent OnTransectsChanged(Me)
        Catch ex As Exception

        End Try
    End Sub

    Public Property Autosaving As Boolean = False

    Public Event OnTransectSelected(sender As cTransectDatastructures, transect As cTransect)
    Public Event OnTransectAdded(sender As cTransectDatastructures, transect As cTransect)
    Public Event OnTransectRemoved(sender As cTransectDatastructures, transect As cTransect)
    Public Event OnTransectsChanged(sender As cTransectDatastructures)

End Class
