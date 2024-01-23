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
Imports EwECore
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eTimeSeriesType">time series types</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTimeSeriesTypeFormatter
        Implements ITypeFormatter

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim ts As eTimeSeriesType = DirectCast(value, eTimeSeriesType)
            Dim strDescr As String = cResourceUtils.LoadString("TS_TYPE_" & ts.ToString().ToUpper, My.Resources.ResourceManager)

            If String.IsNullOrWhiteSpace(strDescr) Then
                strDescr = ts.ToString
            End If

            Select Case descriptor
                Case eDescriptorTypes.Description
                    Dim strApplication As String = If(cTimeSeries.IsDriver(ts), My.Resources.VALUE_GENERIC_FORCING, My.Resources.VALUE_GENERIC_REFERENCE)
                    Dim strScale As String = If(cTimeSeries.IsAbsolute(ts), My.Resources.VALUE_GENERIC_ABSOLUTE, My.Resources.VALUE_GENERIC_RELATIVE)
                    Return cStringUtils.Localize(My.Resources.GENERIC_LABEL_POINT, strDescr, strApplication, strScale)
            End Select
            Return strDescr

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eTimeSeriesType)
        End Function

    End Class

End Namespace
