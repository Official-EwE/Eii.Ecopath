' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwEUtils.Utilities



Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eUnitTimeType"/> objects.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTimeUnitFormatter
        Implements ITypeFormatter

        Private m_strCustom As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new type <see cref="eUnitCurrencyType"/>formatter.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(strCustom As String)
            Me.m_strCustom = strCustom
        End Sub

        Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
            Return GetType(eUnitAreaType)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim unit As eUnitTimeType = DirectCast(value, eUnitTimeType)

            Select Case unit
                Case eUnitTimeType.Year
                    Return My.Resources.CoreDefaults.UNIT_TIME_YEAR
                Case eUnitTimeType.Day
                    Return My.Resources.CoreDefaults.UNIT_TIME_DAY
                Case eUnitTimeType.Custom
                    Return Me.m_strCustom
            End Select

            Return String.Empty
        End Function

    End Class

End Namespace