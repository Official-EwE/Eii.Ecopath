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
Imports System.Xml
Imports EwECore
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Data container for a list of games.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cShellData

#Region " Private vars "

    ''' <summary>The core to operate onto.</summary>
    Private m_core As cCore = Nothing
    ''' <summary>All available games.</summary>
    Private m_games As New List(Of cGame)

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="core">The core to operate onto.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

#Region " Game access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all available games.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Games As cGame()
        Get
            Return Me.m_games.ToArray()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain a <see cref="cGame"/> by name.
    ''' </summary>
    ''' <param name="strGame">The name of the game. It is.</param>
    ''' <returns>The game with a given name, or Nothing if the game could not be found.</returns>
    ''' <remarks>To obtain the first game pass in an empty name</remarks>
    ''' -----------------------------------------------------------------------
    Public Function Game(strGame As String) As cGame
        If (Games.Count = 0) Then Return Nothing
        If (Not String.IsNullOrWhiteSpace(strGame)) Then
            For Each g As cGame In Me.m_games
                If (String.Compare(g.Name, strGame, True) = 0) Then Return g
            Next
        End If
        Return Games(0)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a <see cref="cGame">game</see>.
    ''' </summary>
    ''' <param name="g">The g.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Add(g As cGame)
        Me.m_games.Add(g)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a <see cref="cGame">game</see>.
    ''' </summary>
    ''' <param name="g">The g.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Remove(g As cGame)
        Me.m_games.Remove(g)
    End Sub

#End Region ' Game access

#Region " Serialization "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Serialize the games list from XML.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function FromXML(str As String) As Boolean

        Me.m_games.Clear()

        If (Not String.IsNullOrWhiteSpace(str)) Then
            Try
                Dim doc As New XmlDocument()
                doc.LoadXml(str)
                For Each xnGame As XmlNode In doc.SelectNodes("//game")
                    Dim game As New cGame(Me.m_core)
                    game.FromXML(xnGame)
                    Me.m_games.Add(game)
                Next
                Return True
            Catch ex As Exception
                ' Whoah!
                Console.WriteLine("cShellData.FromXML: " & ex.Message)
            End Try
        End If
        Return False

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Serialize the games list to XML.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Function ToXML() As String

        Dim xnRoot As XmlNode = Nothing
        Dim doc As XmlDocument = cXMLUtils.NewDoc("settings", xnRoot)
        Dim xnNode As XmlNode = Nothing
        Dim xnGames As XmlNode = Nothing

        xnGames = doc.CreateElement("games")
        xnRoot.AppendChild(xnGames)

        For Each g As cGame In Me.Games
            xnGames.AppendChild(g.ToXML(doc))
        Next
        Return doc.OuterXml

    End Function

#End Region ' Serialization

End Class
