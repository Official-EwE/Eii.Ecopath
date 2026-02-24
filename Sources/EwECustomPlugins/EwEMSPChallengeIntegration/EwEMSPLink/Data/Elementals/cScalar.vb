' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Data for a single value in MSP.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cScalar
    Implements IMELItem

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new <see cref="cScalar"/>.
    ''' </summary>
    ''' <param name="name">The name for the scalar.</param>
    ''' <param name="value">The value to assign to the scalar.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(name As String, value As Double)
        Me.Name = name
        Me.Value = value
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the scalar.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of the scalar.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Value As Double

End Class
