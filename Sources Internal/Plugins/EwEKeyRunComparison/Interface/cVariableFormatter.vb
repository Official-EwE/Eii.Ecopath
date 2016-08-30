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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cVariableFormatter
    Implements ITypeFormatter

    Private m_cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
    Private m_fmtDefault As New cVarnameTypeFormatter()

    Public Function GetDescribedType() As System.Type _
        Implements ITypeFormatter.GetDescribedType
        Return GetType(String)
    End Function

    Public Function GetDescriptor(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
        Implements ITypeFormatter.GetDescriptor

        Dim strVariable As String = CStr(value)
        Dim vn As eVarNameFlags = (Me.m_cin.GetVarName(strVariable))

        ' Is an EwE6 var name?
        If (vn <> eVarNameFlags.NotSet) Then
            ' #Yes: return EwE6 localized version
            Return m_fmtDefault.GetDescriptor(vn)
        End If

        Dim strKey As String = "VARIABLE_" & strVariable.ToUpper()
        Dim strDescr As String = cResourceUtils.LoadString(strKey, Me.GetType.Assembly)

        If String.IsNullOrWhiteSpace(strDescr) Then
            Debug.Assert(False, "Key " & strKey & " not found in resources")
            Return strVariable
        End If

        Return strDescr

    End Function

End Class
