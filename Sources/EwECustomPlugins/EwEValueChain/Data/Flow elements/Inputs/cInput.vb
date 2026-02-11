' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

''' ===========================================================================
''' <summary>
''' A value that entered a cUnit during processing.
''' </summary>
''' ===========================================================================
Public Class cInput

    Private m_sTons As Single = 0.0!
    Private m_sValue As Single = 1.0!
    Private m_src As cUnit = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="sTons">Weight of the product, in tons</param>
    ''' <param name="sValue">Total value of the product.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(src As cUnit, sTons As Single, sValue As Single)
        Me.m_src = src
        Me.m_sTons = sTons
        Me.m_sValue = sValue
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the weight of input in tons of this input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Tons() As Single
        Get
            Return Me.m_sTons
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the total value of this input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Value() As Single
        Get
            Return Me.m_sValue
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The <see cref="cUnit">source</see> of this unit.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Source As cUnit
        Get
            Return Me.m_src
        End Get
    End Property

End Class
