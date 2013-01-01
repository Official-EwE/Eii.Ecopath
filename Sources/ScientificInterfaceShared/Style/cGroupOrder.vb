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

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class that orders EwE groups by multi-stanza configurations.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cGroupOrder

#Region " Private vars "

        Private m_lGroups As New List(Of cCoreGroupBase)

#End Region ' Private vars

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core that contains the data to order.</param>
        ''' <remarks>Note that this code does NOT attempt to sort stanza life stages
        ''' by age. This is left to the user to organize.</remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal core As cCore)

            Dim grp As cCoreGroupBase = Nothing
            Dim grpTest As cCoreGroupBase = Nothing
            Dim bIncluded(core.nGroups) As Boolean

            ' For all groups:
            For i As Integer = 1 To core.nGroups
                ' Get group
                grp = core.EcoPathGroupInputs(i)
                ' Group not included in final list?
                If Not bIncluded(i) Then
                    ' #Yes: add group
                    Me.m_lGroups.Add(grp)
                    ' Is multi-stanza?
                    If (grp.isMultiStanza) Then
                        ' #Yes: Add all related stanza for this group:
                        For j As Integer = i + 1 To core.nGroups
                            ' Get remaining group
                            grpTest = core.EcoPathGroupInputs(j)
                            ' Is of same stanza?
                            If (grpTest.iStanza = grp.iStanza) Then
                                ' #Yes: add below current group
                                Me.m_lGroups.Add(grpTest)
                                ' Remember that this group has been included already
                                bIncluded(j) = True
                            End If
                        Next j
                        grpTest = Nothing
                    End If
                End If
            Next i

            grp = Nothing

        End Sub

#End Region ' Construction

#Region " Public properties "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the ordered list of <see cref="cCoreGroupBase">groups</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Groups As cCoreGroupBase()
            Get
                Return Me.m_lGroups.ToArray
            End Get
        End Property

#End Region ' Public properties

    End Class

End Namespace
