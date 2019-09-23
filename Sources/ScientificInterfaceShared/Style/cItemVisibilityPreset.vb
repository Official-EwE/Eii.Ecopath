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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Style

    Public Class cItemVisibilityPreset

        ' -- group visibility --
        ''' <summary>List of indexes of groups to hide.</summary>
        Private m_lHiddenGroups As New List(Of Integer)
        ''' <summary>List of indexes of fleets to hide.</summary>
        Private m_lHiddenFleets As New List(Of Integer)

        Public Sub New(bIsDefault As Boolean)
            Me.IsDefault = bIsDefault
        End Sub

        Public Property IsDefault As Boolean

        Public Property GroupVisible(ByVal iEcopathGroupID As Integer) As Boolean
            Get
                ' Return whether group is not hidden
                Return (Me.m_lHiddenGroups.IndexOf(iEcopathGroupID) = -1)
            End Get
            Set(ByVal bVisible As Boolean)

                Me.IsChanged = False

                If bVisible Then
                    ' Remove group from hidden list, if applicable
                    If (Me.m_lHiddenGroups.IndexOf(iEcopathGroupID) <> -1) Then
                        Me.m_lHiddenGroups.Remove(iEcopathGroupID)
                        Me.IsChanged = True
                    End If
                Else
                    ' Add group to hidden list, if applicable
                    If (Me.m_lHiddenGroups.IndexOf(iEcopathGroupID) = -1) Then
                        Me.m_lHiddenGroups.Add(iEcopathGroupID)
                        Me.IsChanged = True
                    End If
                End If

            End Set
        End Property

        Public Property FleetVisible(ByVal iEcopathFleetID As Integer) As Boolean
            Get
                ' Return whether fleet is not hidden
                Return (Me.m_lHiddenFleets.IndexOf(iEcopathFleetID) = -1)
            End Get
            Set(ByVal bVisible As Boolean)

                Me.IsChanged = False

                If bVisible Then
                    ' Remove fleet from hidden list, if applicable
                    If (Me.m_lHiddenFleets.IndexOf(iEcopathFleetID) <> -1) Then
                        Me.m_lHiddenFleets.Remove(iEcopathFleetID)
                        Me.IsChanged = True
                    End If
                Else
                    ' Add fleet to hidden list, if applicable
                    If (Me.m_lHiddenFleets.IndexOf(iEcopathFleetID) = -1) Then
                        Me.m_lHiddenFleets.Add(iEcopathFleetID)
                        Me.IsChanged = True
                    End If
                End If

            End Set
        End Property

        Public Sub Reset()
            Me.m_lHiddenFleets.Clear()
            Me.m_lHiddenGroups.Clear()
        End Sub

        Public Property IsChanged As Boolean = False

        Public Function HasHiddenItems() As Boolean
            Return ((Me.m_lHiddenFleets.Count + Me.m_lHiddenGroups.Count) > 0)
        End Function

        Friend Function HiddenGroups() As List(Of Integer)
            Return Me.m_lHiddenGroups
        End Function

        Friend Function HiddenFleets() As List(Of Integer)
            Return Me.m_lHiddenFleets
        End Function

    End Class

End Namespace
