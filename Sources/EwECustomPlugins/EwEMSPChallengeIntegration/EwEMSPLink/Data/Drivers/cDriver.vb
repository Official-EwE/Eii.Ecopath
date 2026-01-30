' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



''' ---------------------------------------------------------------------------
''' <summary>
''' Base driver class for inserting MSP pressure data into Ecospace input variables.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class cDriver
    Implements IMELItem

#Region " Variables "

    ''' <summary>The <see cref="cCore"/> to connect to.</summary>
    Protected m_core As cCore = Nothing
    ''' <summary>The <see cref="cGame"/> to connect to.</summary>
    Protected m_game As cGame = Nothing

#End Region ' Variables

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new <see cref="cDriver">driver</see>.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore"/> to connect to.</param>
    ''' <param name="game">The <see cref="cGame"/> to connect to.</param>
    ''' <param name="strName">Name of the new driver.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, game As cGame, strName As String)
        Me.m_core = core
        Me.m_game = game
        Me.Name = strName
    End Sub

#End Region ' Construction

#Region " Obligatory overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply a MSP driver value to Ecospace
    ''' </summary>
    ''' <param name="pressure">The MEL-derived pressure value to apply to the driver.</param>
    ''' <param name="bDirect">Flag, indicating whether a value needs to be 
    ''' injected directly into the EwE data structures (true) or into the EwE 
    ''' input/output objects (false).</param>
    ''' <param name="multiplier"></param>
    ''' <remarks>When used in the EwE user interface and MEL Emulator, data will be 
    ''' applied to <see cref="cCoreInputOutputBase">core I/O classes</see> to provide 
    ''' user feedback to test data changes. The MSP game will operate directly on 
    ''' the <see cref="cEcospaceDataStructures"/> for speed reasons, where no UI 
    ''' synchronizing is needed.</remarks>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function Apply(pressure As cPressure, bDirect As Boolean, Optional multiplier As Double = 1.0!) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Driver identifier; override to provide the unique EwE <see cref="cCoreInputOutputBase.getID()">object identifier</see> 
    ''' for the driver. This ID is needed for persistent pressure - driver mapping
    ''' storage.
    ''' </summary>
    ''' <returns>The unique ID.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function ValueID() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data support identifier; override to state which pressure type this 
    ''' driver can be connected to.
    ''' </summary>
    ''' <returns>The pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function PressureType() As Type

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the driver.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' String represting this driver.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="System.String" /> that represents this driver.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

#End Region ' Obligatory overrides

End Class
