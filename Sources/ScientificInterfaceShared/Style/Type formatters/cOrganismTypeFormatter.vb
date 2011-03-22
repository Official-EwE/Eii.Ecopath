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
    Public Class cOrganismTypeFormatter
        Implements ITypeFormatter(Of eOrganismTypes)

        Public Function GetDescriptor(ByVal value As eOrganismTypes, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eOrganismTypes).GetDescriptor

            If value = eOrganismTypes.NotSet Then Return ""

            Dim strValue As String = value.ToString
            Dim strDescr As String = cResourceUtils.LoadString("ORGANISM_" & strValue.ToUpper, Me.GetType.Assembly)
            Dim astrBits As String() = Nothing
            Dim iNumBits As Integer = 0
            Dim strBit As String = ""

            If (strDescr IsNot Nothing) Then
                astrBits = strDescr.Split("|"c)
                iNumBits = astrBits.Length
            End If

            For i As Integer = 0 To Math.Min(descriptor, iNumBits)

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
