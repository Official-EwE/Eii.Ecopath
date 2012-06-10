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
Imports System.Web
Imports ScientificInterfaceShared.BingMapsGeoLocatorService

#End Region ' Imports

Namespace GeoCode

    ''' <summary>
    ''' Google Maps based <see cref="IGeoCodeLookup">geo location lookup</see>.
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
    Public Class cBingMapsLookup
        Implements IGeoCodeLookup
        Private m_strSearchTerm As String = ""

        <Obsolete("Do not use this class yet; web service geocoding not hooked up yet")> _
        Public Sub New()
            Throw New NotImplementedException("Do not use this class yet; web service geocoding not hooked up yet")
        End Sub

        Public Property Term As String Implements IGeoCodeLookup.Term
            Get
                Return Me.m_strSearchTerm
            End Get
            Set(value As String)
                Me.m_strSearchTerm = value
            End Set
        End Property

        Public Function FindPlaces(ByVal strTerm As String) As cGeoCodeLocation() _
            Implements IGeoCodeLookup.FindPlaces

            Me.Term = strTerm

            Dim key As String = "AhCiJySJPp8FDmpBjH1SRricNicRj302BRDp14TJBEWfI-3FG8irnYC2IjYMDpKY"
            Dim lLocations As New List(Of cGeoCodeLocation)
            Dim searchRequest As New SearchRequest()

            ' Set the credentials using a valid Bing Maps key
            searchRequest.Credentials = New Credentials()
            searchRequest.Credentials.ApplicationId = key

            ' Create the search query
            Dim ssQuery As New StructuredSearchQuery()
            ssQuery.Keyword = "water"""
            ssQuery.Location = strTerm
            searchRequest.StructuredQuery = ssQuery

            ' Define options on the search
            searchRequest.SearchOptions = New SearchOptions()
            searchRequest.SearchOptions.Filters = New FilterExpression()
            'With searchRequest.SearchOptions.Filters
            '    .
            '    .PropertyId = 3
            '    .CompareOperator = CompareOperator.GreaterThanOrEquals
            '    FilterValue = 8.16
            'End With

            ' Make the search request 
            Dim searchService As New SearchServiceClient()
            Dim searchResponse As SearchResponse = searchService.Search(searchRequest)

            ' Parse and format results
            If (searchResponse.ResultSets(0).Results.Length > 0) Then
                For i As Integer = 0 To searchResponse.ResultSets(0).Results.Length - 1
                    'resultList.Append(String.Format("{0}. {1}\n", i+1, 
                    '    searchResponse.ResultSets[0].Results[i].Name));                    
                Next
            End If

            Return lLocations.ToArray

        End Function

    End Class

End Namespace

