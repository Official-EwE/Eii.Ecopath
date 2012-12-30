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

Option Strict On

Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class that orders EwE groups by multi-stanza configurations
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cGroupOrder

#Region " Private vars "

    Private m_bIncludeStanza As Boolean = False
    Private m_lItems As New List(Of cCoreInputOutputBase)

#End Region ' Private vars

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance of this class.
    ''' </summary>
    ''' <param name="core">The core to grab groups and stanza configurations from.</param>
    ''' <param name="bIncludeStanza">Flag, stating whether the resulting list should
    ''' include <see cref="cStanzaGroup"/> instances.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal core As cCore, _
                   ByVal bIncludeStanza As Boolean)

        Me.m_bIncludeStanza = bIncludeStanza

        Dim grp As cCoreGroupBase = Nothing
        Dim stz As cStanzaGroup = Nothing
        Dim lStanza(core.nStanzas) As List(Of cCoreGroupBase)
        Dim lGroups As New List(Of cCoreGroupBase)

        ' Pass one: build filtered lists
        For i As Integer = 1 To core.nGroups
            grp = core.EcoPathGroupInputs(i)
            If grp.isMultiStanza Then
                If lStanza(grp.iStanza) Is Nothing Then
                    lStanza(grp.iStanza) = New List(Of cCoreGroupBase)
                    ' Add first group in stanza
                    lGroups.Add(grp)
                End If
                lStanza(grp.iStanza).Add(grp)
            Else
                lGroups.Add(grp)
            End If
        Next

        ' Pass two: sequence it all together
        For i As Integer = 0 To lGroups.Count - 1
            grp = lGroups(i)
            If grp.isMultiStanza Then
                If bIncludeStanza Then
                    Me.m_lItems.Add(core.StanzaGroups(grp.iStanza))
                End If
                Me.m_lItems.AddRange(lStanza(grp.iStanza))
            Else
                Me.m_lItems.Add(grp)
            End If
        Next

    End Sub

#End Region ' Construction

#Region " Public properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the ordered list of <see cref="cCoreGroupBase"/> and 
    ''' <see cref="cStanzaGroup"/> items.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Items As cCoreInputOutputBase()
        Get
            Return Me.m_lItems.ToArray
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether <see cref="cStanzaGroup"/>s are included in the 
    ''' <see cref="Items"/> list.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property StanzaIncluded As Boolean
        Get
            Return Me.m_bIncludeStanza
        End Get
    End Property

#End Region ' Public properties

End Class
