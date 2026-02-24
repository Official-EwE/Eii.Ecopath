' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cEnvironmentalPressure
    Inherits cPressure

    ''' <summary>The grid data wrapped by the pressure, binned for fast display.</summary>
    Protected m_grid As cGrid = Nothing

    Public Sub New(name As String)
        MyBase.New(name)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create an environmental pressure.
    ''' </summary>
    ''' <param name="name">The name of the pressure to define.</param>
    ''' <param name="iNumRows">The number of rows in the pressure grid.</param>
    ''' <param name="iNumColumns">The number of columns in the pressure grid.</param>
    ''' <param name="data">Optional initial data for the pressure.</param>
    ''' <seealso cref="Grid"/>
    ''' -----------------------------------------------------------------------
    Public Sub New(name As String, iNumColumns As Integer, iNumRows As Integer, Optional data As Double(,) = Nothing)
        Me.New(name)
        Me.m_grid = New cGrid(name, iNumColumns, iNumRows, data)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a reference to the <see cref="cGrid"/> with display-formatted data wrapped by the pressure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Grid As cGrid
        Get
            Return Me.m_grid
        End Get
    End Property

End Class
