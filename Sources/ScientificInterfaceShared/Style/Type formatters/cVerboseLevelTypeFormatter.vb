' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Class for providing a textual description of a <see cref="eVerboseLevel"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cVerboseLevelTypeFormatter
        Implements ITypeFormatter

        Public Function GetDescribedType() As System.Type _
            Implements ITypeFormatter.GetDescribedType
            Return GetType(eVerboseLevel)
        End Function

        Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
            Implements ITypeFormatter.ToString

            Dim strValue As String = value.ToString
            Return cResourceUtils.LoadString("VERBOSE_" & strValue.ToUpper, My.Resources.ResourceManager)

        End Function

    End Class

End Namespace
