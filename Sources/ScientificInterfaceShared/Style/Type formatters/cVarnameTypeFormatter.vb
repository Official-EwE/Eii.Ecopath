#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwECore

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of core variables.
    ''' </summary>
    ''' <remarks>
    ''' <para>This class tries to obtain a string from the ScientificShared resources
    ''' to describe a <see cref="eVarNameFlags">core variable</see>. The string is
    ''' expected to be formatted as follows:</para>
    ''' <para>VARIABLE_[varname] = "[symbol]|[abbr]|[name]|[description]"</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cVarnameTypeFormatter
        Implements ITypeFormatter(Of eVarNameFlags)

        Public Function GetDescriptor(ByVal vn As eVarNameFlags, _
                                      Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                      Implements ITypeFormatter(Of eVarNameFlags).GetDescriptor

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

            Dim strVar As String = cin.GetVarName(vn)
            Dim strDescr As String = cResourceUtils.LoadString("VARIABLE_" & strVar.ToUpper, Me.GetType.Assembly)
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
                    strBit = strVar
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

End Namespace ' Style
