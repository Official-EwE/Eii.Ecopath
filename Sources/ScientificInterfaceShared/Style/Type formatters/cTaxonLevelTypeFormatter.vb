#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eOrganismTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cTaxonLevelTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eTaxonLevelType)
        End Function

        Public Function GetDescriptor(ByVal value As Object, Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.GetDescriptor

            Dim val As eTaxonLevelType = DirectCast(value, eTaxonLevelType)

            Select Case val
                Case eTaxonLevelType.Any
                    Return My.Resources.GENERIC_VALUE_ALL
                Case eTaxonLevelType.Common
                    Return My.Resources.HEADER_COMMON_NAME
                    'Case eTaxonLevelType.Kingdom
                Case eTaxonLevelType.Phylum
                    Return My.Resources.HEADER_PHYLUM
                Case eTaxonLevelType.Order
                    Return My.Resources.HEADER_ORDER
                Case eTaxonLevelType.Class
                    Return My.Resources.HEADER_CLASS
                Case eTaxonLevelType.Family
                    Return My.Resources.HEADER_FAMILY
                Case eTaxonLevelType.Genus
                    Return My.Resources.HEADER_GENUS
                Case eTaxonLevelType.Species
                    Return My.Resources.HEADER_SPECIES
            End Select

            Return "?"
        End Function

    End Class

End Namespace
