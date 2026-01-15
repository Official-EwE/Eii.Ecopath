
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


Imports EwECore
Imports EwEMSPLink



Namespace Emulator

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Set of test input for configured <see cref="cPressure">pressures</see>>
    ''' </summary>
    ''' <seealso cref="EwEMSPLink.IMELItem" />
    ''' ---------------------------------------------------------------------------
    Public Class cTestset
        Implements IMELItem

        Private m_inputs As New Dictionary(Of cPressure, String)
        Private m_game As cGame = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Createa a new <see cref="cTestset"/>.
        ''' </summary>
        ''' <param name="strName">Name of the test set.</param>
        ''' <param name="g">The <see cref="cGame">game</see> to define the testset for.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(strName As String, g As cGame)
            Me.m_game = g
            Me.Name = strName
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of the testset.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Name As String Implements IMELItem.Name

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the testdata for a given <see cref="cPressure">pressure</see>
        ''' </summary>
        ''' <value>
        ''' The testdata.
        ''' </value>
        ''' -------------------------------------------------------------------
        Public Property Testdata(pressure As cPressure) As String
            Get
                If (Not Me.m_inputs.ContainsKey(pressure)) Then
                    If (TypeOf pressure Is cFishingEffortPressure) Then Return CStr(1.0F)
                    Return ""
                End If
                Return Me.m_inputs(pressure)
            End Get
            Set(value As String)
                If (TypeOf pressure Is cFishingEffortPressure) Then
                    If (value = CStr(cCore.NULL_VALUE)) Then
                        value = CStr(1.0F)
                    End If
                End If
                Me.m_inputs(pressure) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain the pressures for which testdata is defined.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Pressures As ICollection(Of cPressure)
            Get
                Return Me.m_game.Pressures
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Formatter, displays the name of the testset.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="System.String" /> that represents the testset
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function ToString() As String
            Return Me.Name
        End Function

    End Class

End Namespace
