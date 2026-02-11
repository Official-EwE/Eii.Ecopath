' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Text

Namespace NetUtilities

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' String writer that accepts setting of diffferent <see cref="encoding">encoding formats</see>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cFlexibleEncodingStringWriter
        Inherits StringWriter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="System.Text.encoding"/> format to use for this string writer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CustomEncoding As Encoding = Encoding.UTF8

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="StringWriter.Encoding"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return Me.CustomEncoding
            End Get
        End Property
    End Class

End Namespace
