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
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

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

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            ' ToDo: localize this

            Try
                If (value IsNot Nothing) Then
                    Dim obj As ISpatialDataSet = DirectCast(value, ISpatialDataSet)
                    Select Case descriptor
                        Case eDescriptorTypes.Name
                            If obj.TimeStart <> DateTime.MaxValue Then
                                If obj.TimeEnd <> DateTime.MinValue Then
                                    Return String.Format("{0} ({1} - {2})", obj.DisplayName, obj.TimeStart.ToShortDateString, obj.TimeEnd.ToShortDateString)
                                End If
                                Return String.Format("{0} ({1}-)", obj.DisplayName, obj.TimeStart.ToShortDateString)
                            End If
                        Case eDescriptorTypes.Description
                            Return obj.Description
                    End Select
                    Return obj.DisplayName
                End If

                Return My.Resources.GENERIC_VALUE_NONE

            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Return ""

        End Function

    End Class

End Namespace
