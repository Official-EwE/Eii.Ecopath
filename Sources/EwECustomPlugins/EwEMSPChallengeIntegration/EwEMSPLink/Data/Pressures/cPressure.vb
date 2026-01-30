' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
