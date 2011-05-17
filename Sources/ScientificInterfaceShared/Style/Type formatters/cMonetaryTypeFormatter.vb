#Region " Imports "

Option Strict On
Imports System.Globalization

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="RegionInfo"/> 
    ''' currency information.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMonetaryTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(RegionInfo)
        End Function

        Public Function GetDescriptor(ByVal value As Object, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter.GetDescriptor

            Dim ri As RegionInfo = DirectCast(value, RegionInfo)
            Dim strDescr As String = ""

            Select Case descriptor
                Case eDescriptorTypes.Symbol
                    strDescr = ri.CurrencySymbol

                Case eDescriptorTypes.Abbreviation
                    strDescr = ri.ISOCurrencySymbol

                Case eDescriptorTypes.Name
                    strDescr = ri.CurrencyEnglishName()

                Case eDescriptorTypes.Description
                    strDescr = ri.CurrencyNativeName

            End Select
            Return strDescr

        End Function

    End Class

End Namespace
