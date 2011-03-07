#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of <see cref="eEcologyTypes"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEcologyTypeFormatter
        Implements ITypeFormatter(Of eEcologyTypes)

        Public Function GetDescriptor(ByVal value As eEcologyTypes, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eEcologyTypes).GetDescriptor


            Dim strValue As String = value.ToString
            Dim strDescr As String = cResourceUtils.LoadString("ECOLOGY_" & strValue.ToUpper, GetType(cEcologyTypeFormatter).Assembly)
            Dim astrBits As String() = Nothing
            Dim iNumBits As Integer = 0
            Dim strBit As String = ""

            If (strDescr IsNot Nothing) Then
                astrBits = strDescr.Split("|"c)
                iNumBits = astrBits.Length
            End If

            For i As Integer = 0 To descriptor

                ' Is first part?
                If (i = 0) Then
                    ' #Yes: remember default
                    strBit = strValue
                End If

                If i < iNumBits Then
                    ' Has a part?
                    If Not String.IsNullOrEmpty(astrBits(i)) Then
                        ' #Yes: update bit
                        strBit = astrBits(i).Trim
                    End If
                End If

            Next
            Return strBit
        End Function

    End Class

End Namespace
