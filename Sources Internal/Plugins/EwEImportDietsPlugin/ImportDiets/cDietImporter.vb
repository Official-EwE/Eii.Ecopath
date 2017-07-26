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

        Try

            If Me.CheckEcopathState() Then
                If DBReader.ImportDietPreferences(ExternalModelFileName, DietPrefs) Then

                    If DietCalculator.DietsFromPreferences(DietPrefs) Then
                        'Yep it worked...
                        'DietCalculator.DietsFromPreferences() posted a message if the diets where loaded
                    End If

                End If
            End If ' If Me.CheckEcopathState() Then


        Catch ex As Exception
            cLog.Write(ex)
            'Message that the model needs to balancing
            Me.m_Core.Messages.SendMessage(New EwECore.cMessage("Exception while importing diets: " + ex.Message,
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))
        End Try

    End Sub

    Private Function CheckEcopathState() As Boolean

        'Ok If Ecopath hasn't run this can not be run
        'In the current implementation this was handled by the UI
        If Me.m_Core.StateMonitor.HasEcopathRan Then
            Return True
        End If

        'shouldn't happen
        Me.m_Core.Messages.SendMessage(New EwECore.cMessage("You must run Ecopath to balance the current model before Importing Diets",
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))

        Return False

    End Function



End Class
