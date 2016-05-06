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
' Copyright 1991- UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'

Namespace GeoCode

    ''' <summary>
    ''' Structure holding information for a single geo location,
    ''' obtained from a <see cref="IGeoCodeLookup"/>
    ''' </summary>
    Public Class cGeoCodeLocation

        Private m_strDescription As String
        Private m_sWest As Single
        Private m_sSouth As Single
        Private m_sEast As Single
        Private m_sNorth As Single

        Friend Sub New(ByVal strDescription As String, _
                       ByVal sEast As Single, ByVal sNorth As Single, _
                       ByVal sWest As Single, ByVal sSouth As Single)
            Me.m_strDescription = strDescription
            Me.m_sNorth = sNorth
            Me.m_sWest = sWest
            Me.m_sSouth = sSouth
            Me.m_sEast = sEast
        End Sub

        Public ReadOnly Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
        End Property

        Public ReadOnly Property West() As Single
            Get
                Return Me.m_sWest
            End Get
        End Property

        Public ReadOnly Property East() As Single
            Get
                Return Me.m_sEast
            End Get
        End Property

        Public ReadOnly Property South() As Single
            Get
                Return Me.m_sSouth
            End Get
        End Property

        Public ReadOnly Property North() As Single
            Get
                Return Me.m_sNorth
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.m_strDescription
        End Function

    End Class

    ''' <summary>
    ''' Interface for implementing a geo locator service.
    ''' </summary>
    ''' <remarks>
    ''' Usage example:
    ''' <code>
    ''' Dim lookup As New cGoogleMapsLookup()
    ''' Dim locations as cGeoCodeLocation() = lookup.FindLocations("waddenzee") 
    ''' Dim location as cGeoCodeLocation = Nothing
    ''' 
    ''' If (locations IsNot Nothing) Then
    '''    For each location in locations
    '''       Console.WriteLine("Area for '{0}': {1}x{2} to {3}x{4}", _
    '''                         location.Term, _
    '''                         location.East, location.North, _
    '''                         location.West, location.South)
    '''    Next location
    ''' End If
    ''' </code>
    ''' </remarks>
    Public Interface IGeoCodeLookup

        Property Term() As String
        Function FindPlaces(ByVal strTerm As String) As cGeoCodeLocation()

    End Interface

End Namespace

