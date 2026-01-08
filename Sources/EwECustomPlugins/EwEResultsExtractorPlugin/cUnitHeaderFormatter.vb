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
Imports EwECore.Style
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cUnitHeaderFormatter

    Private m_uic As cUIContext

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
    End Sub

    Public Function Format(var As eVarNameFlags) As String

        Dim fmt As New cVarnameTypeFormatter()
        Dim units As New cUnits(Me.m_uic.Core)

        Dim md As cVariableMetaData = cVariableMetaData.Get(var)
        Dim s1 As String = fmt.ToString(var)
        Dim s2 As String = units.ToString(md)

        If (Not String.IsNullOrWhiteSpace(s2)) Then
            Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, s1, s2)
        End If
        Return s1

    End Function

End Class
