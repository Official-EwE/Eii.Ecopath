' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Globalization
Imports EwEUtils.Utilities



Namespace GeoCode

    Public Class cMarineRegionsLookup
        Implements IGeoCodeLookup

        Private Shared s_wsdl As New cMarineRegionsWSDL()
        Private m_ci As CultureInfo = CultureInfo.GetCultureInfo("en-US")

        Public Property Term As String Implements IGeoCodeLookup.Term

        Public Function FindPlaces(strTerm As String) As cGeoCodeLocation() _
            Implements IGeoCodeLookup.FindPlaces

            Dim lLocations As New List(Of cGeoCodeLocation)

            Try
                For Each r As gazetteerRecord In s_wsdl.getGazetteerRecordsByName(strTerm, False, True)
                    Dim x0, x1, y0, y1 As Single
                    If (Single.TryParse(r.minLongitude, NumberStyles.Float, Me.m_ci, x0) And Single.TryParse(r.maxLongitude, NumberStyles.Float, Me.m_ci, x1)) And
                       (Single.TryParse(r.minLatitude, NumberStyles.Float, Me.m_ci, y0) And Single.TryParse(r.maxLatitude, NumberStyles.Float, Me.m_ci, y1)) Then

                        Dim strName As String = ""
                        If String.IsNullOrWhiteSpace(r.placeType) Then
                            strName = r.preferredGazetteerName
                        Else
                            strName = cStringUtils.Localize(My.Resources.GENERIC_LABEL_DETAILED, r.preferredGazetteerName, r.placeType)
                        End If
                        Dim loc As New cGeoCodeLocation(strName, x0, y1, x1, y0)
                        lLocations.Add(loc)
                    End If

                Next
            Catch ex As Exception

            End Try

            Return lLocations.ToArray()

        End Function

    End Class

End Namespace
