' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'






''' ---------------------------------------------------------------------------
''' <summary>
''' Pressure data derived from MSP game play actions to impact the Ecospace model.
''' </summary>
''' <remarks>
''' In a <see cref="cGame">MSP game</see>, player actions translate to pressures.
''' This pressure data is received in cPressure classes, and are passed on to mapped 
''' <see cref="cDriver">Ecospace drivers</see> to impact the Ecospace model.
''' </remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cPressure
    Implements IMELItem

#Region " Constructors "

    Public Sub New(name As String)
        Me.Name = name
    End Sub

#End Region ' Constructors

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the pressure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

#End Region ' Public bits

End Class
