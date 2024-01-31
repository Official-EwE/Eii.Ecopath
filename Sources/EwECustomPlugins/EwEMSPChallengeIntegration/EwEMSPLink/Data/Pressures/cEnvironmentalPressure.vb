' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

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
