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
