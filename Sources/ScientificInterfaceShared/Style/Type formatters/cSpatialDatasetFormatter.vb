' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="ISpatialDataSet"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cSpatialDatasetFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(ISpatialDataSet)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim strResult As String = ""

            Try
                If (value IsNot Nothing) Then

                    Dim obj As ISpatialDataSet = DirectCast(value, ISpatialDataSet)
                    strResult = obj.CustomName

                    Select Case descriptor

                        Case eDescriptorTypes.Name
                            ' Has configuration?
                            If (obj.IsConfigured) Then
                                ' #Yes: filter by lower time limit
                                Select Case obj.TimeStart

                                    Case DateTime.MinValue
                                        ' Has upper range set?
                                        If (obj.TimeEnd < DateTime.MaxValue) Then
                                            ' #Yes: Return <inf,upper] range
                                            strResult = String.Format(My.Resources.LABEL_VALUE_UPTO, obj.CustomName, obj.TimeEnd.ToShortDateString)
                                        End If

                                    Case DateTime.MaxValue
                                        ' Something is screwed
                                        Debug.Assert(False)

                                    Case obj.TimeEnd
                                        ' Start = end, and neither is inf: show a single date
                                        strResult = String.Format(My.Resources.LABEL_VALUE_AT, obj.CustomName, obj.TimeStart.ToShortDateString)

                                    Case Else
                                        ' Has upper range set?
                                        If (obj.TimeEnd < DateTime.MaxValue) Then
                                            ' #Yes: show [lower, upper] range
                                            strResult = String.Format(My.Resources.LABEL_VALUE_RANGE, obj.CustomName, obj.TimeStart.ToShortDateString, obj.TimeEnd.ToShortDateString)
                                        Else
                                            ' #No: show [lower, inf> range
                                            strResult = String.Format(My.Resources.LABEL_VALUE_FROM, obj.CustomName, obj.TimeStart.ToShortDateString)
                                        End If

                                End Select
                            End If

                        Case eDescriptorTypes.Description
                            strResult = obj.CustomDescription
                    End Select
                Else
                    strResult = My.Resources.GENERIC_VALUE_NONE
                End If

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return strResult

        End Function

    End Class

End Namespace
