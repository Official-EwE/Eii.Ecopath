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
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

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
    ''' <param name="data">Optional Ecospace data structures to apply the driver
    ''' to. If not provided, the driver data must be applied through <see cref="ICoreInputOutput"/>
    ''' interfaces instead.</param>
    ''' <param name="multiplier"></param>
    ''' <remarks>When used in the EwE user interface and MEL Emulator, data will be 
    ''' applied to <see cref="cCoreInputOutputBase">core I/O classes</see> to provide 
    ''' user feedback to test data changes. The MSP game will operate directly on 
    ''' the <see cref="cEcospaceDataStructures"/> for speed reasons, where no UI 
    ''' synchronizing is needed.</remarks>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function Apply(pressure As cPressure, Optional data As cEcospaceDataStructures = Nothing, Optional multiplier As Double = 1.0!) As Boolean

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
    ''' Data support identifier; override to state which <see cref="cPressure.eDataTypes">
    ''' pressure data type</see> this driver can be connected to.
    ''' </summary>
    ''' <returns>The pressure type.</returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function DataType() As cPressure.eDataTypes

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
