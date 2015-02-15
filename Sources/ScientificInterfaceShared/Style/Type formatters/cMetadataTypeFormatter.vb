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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwECore
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="cVariableMetadata"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMetadataTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            ' ToDo: globalize this

            If (value Is Nothing) Then Return ""

            Debug.Assert(value.GetType.IsAssignableFrom(Me.GetDescribedType()))

            Dim md As cVariableMetaData = DirectCast(value, cVariableMetaData)
            Dim strDescr As String = ""

            Select Case md.VarType
                Case ValueWrapper.eValueTypes.Bool, ValueWrapper.eValueTypes.BoolArray
                    strDescr = "true or false"

                Case ValueWrapper.eValueTypes.Int, ValueWrapper.eValueTypes.IntArray
                    Dim bLowerLimit As Boolean = md.Min > Integer.MinValue
                    Dim bUpperLimit As Boolean = md.Max < Integer.MaxValue
                    Dim bDefault As Boolean = CInt(md.NullValue) <> cCore.NULL_VALUE
                    strDescr = String.Format("integer, {0}{1},{2}{3}", _
                                             cSystemUtils.IIF((TypeOf md.MinOperator Is cGreaterThan) Or Not bLowerLimit, "<", "["), _
                                             cSystemUtils.IIF(bLowerLimit, md.Min, "inf"), _
                                             cSystemUtils.IIF(bUpperLimit, md.Max, "inf"), _
                                             cSystemUtils.IIF((TypeOf md.MaxOperator Is cLessThan) Or Not bUpperLimit, ">", "]"))

                Case ValueWrapper.eValueTypes.Sng, ValueWrapper.eValueTypes.SingleArray
                    Dim bLowerLimit As Boolean = md.Min > Single.MinValue
                    Dim bUpperLimit As Boolean = md.Max < Single.MaxValue
                    Dim bDefault As Boolean = CSng(md.NullValue) <> cCore.NULL_VALUE
                    strDescr = String.Format("floating point, {0}{1},{2}{3}", _
                                             cSystemUtils.IIF((TypeOf md.MinOperator Is cGreaterThan) Or Not bLowerLimit, "<", "["), _
                                             cSystemUtils.IIF(bLowerLimit, md.Min, "inf"), _
                                             cSystemUtils.IIF(bUpperLimit, md.Max, "inf"), _
                                             cSystemUtils.IIF((TypeOf md.MaxOperator Is cLessThan) Or Not bUpperLimit, ">", "]"))

                Case ValueWrapper.eValueTypes.Str
                    Dim iMax As Integer = Math.Min(CInt(2 ^ 16) - 1, md.Length)
                    strDescr = String.Format("Text of max. {0} characters", iMax)
            End Select
            Return strDescr

        End Function

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(cVariableMetaData)
        End Function

    End Class

End Namespace ' Style
