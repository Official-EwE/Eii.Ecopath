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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' A value that entered a cUnit during processing.
''' </summary>
''' ===========================================================================
Public Class cInput

    Private m_sTons As Single = 0.0!
    Private m_sValue As Single = 1.0!
    Private m_sCustomValuePerTon As Single = 1.0!

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="sTons">Weight of the product, in tons</param>
    ''' <param name="sValue">Total value of the product.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal sTons As Single, ByVal sValue As Single, _
                   Optional ByVal sCustomValuePerTon As Single = 1.0!)
        Me.m_sTons = sTons
        Me.m_sValue = sValue
        Me.m_sCustomValuePerTon = sCustomValuePerTon
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
    ''' Get the total value of this input. This value should correspond to
    ''' <see cref="Tons">Tons</see> x <see cref="CustomValuePerTon">ValuePerTon</see>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Value() As Single
        Get
            Return Me.m_sValue
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get custom value per ton for this input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property CustomValuePerTon() As Single
        Get
            Return Me.m_sCustomValuePerTon
        End Get
    End Property

End Class
