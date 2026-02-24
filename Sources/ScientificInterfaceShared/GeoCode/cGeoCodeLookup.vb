' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace GeoCode

    ''' <summary>
    ''' Structure holding information for a single geo location,
    ''' obtained from a <see cref="IGeoCodeLookup"/>
    ''' </summary>
    Public Class cGeoCodeLocation

        Friend Sub New(strDescription As String,
                       sEast As Single, sNorth As Single,
                       sWest As Single, sSouth As Single)
            Me.Description = strDescription
            Me.North = sNorth
            Me.West = sWest
            Me.South = sSouth
            Me.East = sEast
        End Sub

        Public ReadOnly Property Description() As String

        Public ReadOnly Property West() As Single

        Public ReadOnly Property East() As Single

        Public ReadOnly Property South() As Single

        Public ReadOnly Property North() As Single

        Public Overrides Function ToString() As String
            Return Me.Description
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
        Function FindPlaces(strTerm As String) As cGeoCodeLocation()

    End Interface

End Namespace

