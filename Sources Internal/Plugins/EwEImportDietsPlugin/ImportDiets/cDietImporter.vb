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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
'Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
'Imports ScientificInterfaceShared.Controls

#End Region



Public Class cDietImporter
    Private m_EcopathData As cEcopathDataStructures
    Private m_Core As cCore

    Public Sub New(EwECore As cCore, EcopathData As cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData

    End Sub



    Public Sub Run(ExternalModelFileName As String)
        Dim DietPrefs As cDietPreferences
        Dim DBReader As New cDatabaseReader(Me.m_Core, Me.m_EcopathData)
        Dim DietCalculator As New cDietCalculator(Me.m_Core, Me.m_EcopathData)

        If Me.CheckEcopathState() Then
            If DBReader.ImportDietPreferences(ExternalModelFileName, DietPrefs) Then

                If DietCalculator.DietsFromPreferences(DietPrefs) Then
                    'Yep it worked...
                End If

            End If ' If DBReader.ImportDietPreferences(ExternalModelFileName, DietPrefs) Then
        End If ' If Me.CheckEcopathState() Then

    End Sub

    Private Function CheckEcopathState() As Boolean

        If Me.m_Core.StateMonitor.HasEcopathRan Then
            Return True
        End If

        'Ok Ecopath hasn't run
        'Ask the user to run it
        '
        'xxxxxxxxxxxxxxxxxxx
        'Me.m_Core.Messages(...)
        'xxxxxxxxxxxxxxxxxxxxx
        Return False



    End Function



End Class
