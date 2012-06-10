#Region " Imports "

Option Strict On
Imports System.Web
Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Generic
Imports System.Net
Imports System.IO
Imports System.Globalization

#End Region ' Imports

Namespace GeoCode

    ''' <summary>
    ''' Google Maps based <see cref="cGeoCodeLookup">geo location lookup</see>.
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
    Public Class cGoogleMapsLookup
        Implements IGeoCodeLookup

        ''' <summary>Google geocode api v2 - deprecated</summary>
        Private Const SERVICE_URL_DPER As String = "http://maps.google.com/maps/geo?q={0}&output=xml&key=ABQIAAAAuXdMTY5VIU1FvkgOOP1dNBTsILMTMKRV-aJhd94IQkaJhVJ0YBS2qNSZGm8TaefqbXBT6lUXeMZ6tA"
        ''' <summary>Google geocode api v3 - per nov 2010</summary>
        Private Const SERVICE_URL As String = "http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&key=ABQIAAAAuXdMTY5VIU1FvkgOOP1dNBTsILMTMKRV-aJhd94IQkaJhVJ0YBS2qNSZGm8TaefqbXBT6lUXeMZ6tA"

        Private m_strSearchTerm As String = ""

        Public Sub New()
        End Sub

        Private Function GetPageAsString(ByVal uriAddress As Uri) As String

            Dim httRequest As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Dim result As String = ""
            Try
                ' Create the web request  
                httRequest = DirectCast(WebRequest.Create(uriAddress), HttpWebRequest)
                ' Get response  
                response = DirectCast(httRequest.GetResponse(), HttpWebResponse)
                ' Get the response stream into a reader  
                reader = New StreamReader(response.GetResponseStream())
                ' Read the whole contents and return as a string  
                result = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try

            Return result

        End Function

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

            Dim uriGoogleGeolocApi As New Uri(String.Format(cGoogleMapsLookup.SERVICE_URL, HttpUtility.UrlEncode(strTerm)))
            Dim lLocations As New List(Of cGeoCodeLocation)
            Dim loc As cGeoCodeLocation = Nothing
            Dim xd As New XmlDocument()
            Dim xnAddress As XmlNode = Nothing
            Dim xnGeometry As XmlNode = Nothing
            Dim xnBounds As XmlNode = Nothing
            Dim xnSW As XmlNode = Nothing
            Dim xnNE As XmlNode = Nothing
            Dim strName As String = ""
            Dim sNorth As Single = 0.0
            Dim sWest As Single = 0.0
            Dim sEast As Single = 0.0
            Dim sSouth As Single = 0.0
            Dim bOK As Boolean = False
            Dim ci As New CultureInfo("en-US")

            ' Only allow search terms of 4 characters or longer
            Me.Term = strTerm
            If strTerm.Length < 4 Then Return lLocations.ToArray

            xd.LoadXml(Me.GetPageAsString(uriGoogleGeolocApi))

            For Each xnResult As XmlNode In xd.GetElementsByTagName("result")

                loc = Nothing

                xnAddress = xnResult.Item("formatted_address")
                If (xnAddress IsNot Nothing) Then
                    strName = xnAddress.InnerText
                    bOK = Not String.IsNullOrEmpty(strName)
                End If

                xnGeometry = xnResult.Item("geometry")
                If xnGeometry IsNot Nothing Then
                    xnBounds = xnGeometry.Item("bounds")
                    If xnBounds IsNot Nothing Then
                        xnSW = xnBounds.Item("southwest")
                        xnNE = xnBounds.Item("northeast")
                        If (xnSW IsNot Nothing) And (xnNE IsNot Nothing) Then
                            bOK = bOK And Single.TryParse(xnNE.Item("lng").InnerText, Globalization.NumberStyles.Any, ci, sEast) And _
                                  Single.TryParse(xnNE.Item("lat").InnerText, Globalization.NumberStyles.Any, ci, sNorth) And _
                                  Single.TryParse(xnSW.Item("lng").InnerText, Globalization.NumberStyles.Any, ci, sWest) And _
                                  Single.TryParse(xnSW.Item("lat").InnerText, Globalization.NumberStyles.Any, ci, sSouth)
                        End If
                    End If
                End If

                If bOK Then
                    loc = New cGeoCodeLocation(strName, sEast, sNorth, sWest, sSouth)
                    lLocations.Add(loc)
                End If
            Next

            Return lLocations.ToArray

        End Function

    End Class

End Namespace

