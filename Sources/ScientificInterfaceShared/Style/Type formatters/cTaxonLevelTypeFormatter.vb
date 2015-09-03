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
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eTaxonClassificationType"/>s.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTaxonClassificationTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eTaxonClassificationType)
        End Function

        Public Function GetDescriptor(ByVal value As Object, Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.GetDescriptor

            Dim val As eTaxonClassificationType = DirectCast(value, eTaxonClassificationType)

            Select Case val
                Case eTaxonClassificationType.Common
                    Return My.Resources.HEADER_COMMON_NAME
                Case eTaxonClassificationType.Phylum
                    Return My.Resources.HEADER_PHYLUM
                Case eTaxonClassificationType.Order
                    Return My.Resources.HEADER_ORDER
                Case eTaxonClassificationType.Class
                    Return My.Resources.HEADER_CLASS
                Case eTaxonClassificationType.Family
                    Return My.Resources.HEADER_FAMILY
                Case eTaxonClassificationType.Genus
                    Return My.Resources.HEADER_GENUS
                Case eTaxonClassificationType.Species
                    Return My.Resources.HEADER_SPECIES
                    'Case eTaxonClassificationType.Kingdom
                Case Else
                    Debug.Assert(False)
            End Select

            Return ""
        End Function

    End Class

End Namespace
