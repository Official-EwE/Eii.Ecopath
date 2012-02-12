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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Imports EwEUtils.Commands

#End Region ' Imports

Namespace Commands

    Public Class cDisplayGroupsCommand
        Inherits cCommand

        Private m_bShowGroups As Boolean = True
        Private m_bShowTotals As Boolean = False

        Public Shared cCOMMAND_NAME As String = "~displaygroups"

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, cDisplayGroupsCommand.cCOMMAND_NAME)
        End Sub

        Public Overloads Sub Invoke(Optional ByVal bShowGroups As Boolean = True, Optional ByVal bShowTotals As Boolean = False)
            Me.m_bShowGroups = bShowGroups
            Me.m_bShowTotals = bShowTotals
            MyBase.Invoke()
        End Sub

        Public Property ShowGroups() As Boolean
            Get
                Return Me.m_bShowGroups
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowGroups = value
            End Set
        End Property

        Public Property ShowTotals() As Boolean
            Get
                Return Me.m_bShowTotals
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowTotals = value
            End Set
        End Property

    End Class

End Namespace
