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
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cDataTypeFormatter
    Implements ITypeFormatter

    Public Function GetDescribedType() As System.Type Implements ScientificInterfaceShared.Style.ITypeFormatter.GetDescribedType
        Return GetType(eDataTypes)
    End Function

    Public Function GetDescriptor(value As Object, Optional descriptor As ScientificInterfaceShared.Style.eDescriptorTypes = ScientificInterfaceShared.Style.eDescriptorTypes.Name) As String Implements ScientificInterfaceShared.Style.ITypeFormatter.GetDescriptor

        ' ToDo: Globalize this

        Select Case DirectCast(value, eDataTypes)
            Case eDataTypes.CapacityMediation : Return "Functional response"
            Case eDataTypes.Mediation : Return "Mediation"
            Case eDataTypes.PriceMediation : Return "Price mediation"
            Case eDataTypes.Forcing : Return "Forcing function"
            Case eDataTypes.EggProd : Return "Egg production"
            Case Else : Return value.ToString()
        End Select

        Return "?"

    End Function
End Class
