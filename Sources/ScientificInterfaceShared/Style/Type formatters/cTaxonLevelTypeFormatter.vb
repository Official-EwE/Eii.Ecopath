' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Utilities

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

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim val As eTaxonClassificationType = DirectCast(value, eTaxonClassificationType)
            Dim fmt As New cVarnameTypeFormatter()

            Select Case val
                Case eTaxonClassificationType.Phylum
                    Return fmt.ToString(eVarNameFlags.Phylum, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Order
                    Return fmt.ToString(eVarNameFlags.Order, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Class
                    Return fmt.ToString(eVarNameFlags.Class, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Family
                    Return fmt.ToString(eVarNameFlags.Family, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Genus
                    Return fmt.ToString(eVarNameFlags.Genus, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Species
                    Return fmt.ToString(eVarNameFlags.Species, eDescriptorTypes.Name)
                Case eTaxonClassificationType.Latin
                    Return fmt.ToString(eVarNameFlags.Name, eDescriptorTypes.Name)
                Case Else
                    Debug.Assert(False)
            End Select

            Return ""
        End Function

    End Class

End Namespace
