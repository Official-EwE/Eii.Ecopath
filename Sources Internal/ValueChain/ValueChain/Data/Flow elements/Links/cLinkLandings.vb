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
Imports System.ComponentModel
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports ValueChain.Utilities
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Species-dependent link.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Landings"), _
    Serializable()> _
Public Class cLinkLandings
    : Inherits cLink

#Region " Helper classes "


#End Region ' Helper classes

#Region " Private bits "

    Private m_species As String = ""

#End Region ' Private bits

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Ecopath integration "

    Public Overrides Property Name() As String

#End Region ' Ecopath integration

#Region " Overrides "

    <Browsable(False)>
    Public Overridable Property Species() As String
        Get
            Return Me.m_species
        End Get
        Friend Set(value As String)
            Me.m_species = value
        End Set
    End Property

    <Browsable(False)>
    Public Overrides Property ValuePerTon() As Single
        Get
            Return 0
        End Get
        Set(value As Single)
            ' nop
        End Set
    End Property

    Public Overrides ReadOnly Property IsDefault As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Function IsVisible() As Boolean
        'If (TypeOf Me.Source Is cProducerUnit) Then
        '    Dim fleet As cEcopathFleetInput = DirectCast(Me.Source, cProducerUnit).Fleet
        '    Dim group As cEcoPathGroupInput = Me.Group
        '    If (fleet IsNot Nothing) And (group IsNot Nothing) Then
        '        Return (fleet.Landings(group.Index) > 0)
        '    End If
        'End If
        Return True
    End Function

    Public Overrides ReadOnly Property IsConfigured() As Boolean
        Get
            Return MyBase.IsConfigured And Not String.IsNullOrWhiteSpace(DirectCast(Me.Source, cProducerUnit).Fleet)
        End Get
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
        If (obj Is Nothing) Then Return False
        If (Not TypeOf obj Is cLinkLandings) Then Return False
        Dim ll As cLinkLandings = DirectCast(obj, cLinkLandings)
        Return MyBase.Equals(obj) And (String.Compare(ll.m_species, Me.Species) = 0)
    End Function

#End Region ' Overrides

End Class
