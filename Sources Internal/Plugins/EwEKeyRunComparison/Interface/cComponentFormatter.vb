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
Imports System.Text
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class cComponentFormatter
    Implements ITypeFormatter

    Public Function GetDescribedType() As System.Type _
        Implements ITypeFormatter.GetDescribedType
        Return GetType(String)
    End Function

    Public Function GetDescriptor(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
        Implements ITypeFormatter.GetDescriptor

        Dim strVar As String = CStr(value)
        Dim strResourceKey As String = "COMPONENT_" & strVar.ToUpper()
        Dim strDescr As String = cResourceUtils.LoadString(strResourceKey, Me.GetType.Assembly)

        If (String.IsNullOrWhiteSpace(strDescr)) Then
            Debug.Assert(False, strResourceKey & " not found in resources")
            Return strVar
        End If

        Return strDescr

    End Function

End Class
