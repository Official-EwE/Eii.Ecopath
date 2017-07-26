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
Imports EwECore
Imports EwECore.Ecopath
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

Public Class cDatabaseReader

    'ToDo Validation of groups with currently loaded DB

    Private m_EcopathData As cEcopathDataStructures
    Private m_Core As cCore

    Public Sub New(EwECore As cCore, EcopathData As cEcopathDataStructures)
        Me.m_Core = EwECore
        Me.m_EcopathData = EcopathData
    End Sub

    Public Function ImportDietPreferences(ModelFileName As String, ByRef DietPrefenences As cDietPreferences) As Boolean
        'Reads diets from external database
        Dim bReturn As Boolean

        Try

            Dim core As cCore = Me.getCoreFromFilename(ModelFileName)

            If Me.Validate(core) Then
                DietPrefenences = New cDietPreferences(core.EcopathDataStructures)
                bReturn = True
            Else

                bReturn = False
            End If

            'Clean up our mess
            If core IsNot Nothing Then
                core.Dispose()
                core = Nothing
            End If

            Return bReturn

        Catch ex As Exception
            cLog.Write(ex)
        End Try

        Return False

    End Function

    Private Function getCoreFromFilename(strModel As String) As cCore
        Dim core As New cCore()
        Dim ds As EwECore.DataSources.IEwEDataSource = EwECore.DataSources.cDataSourceFactory.Create(strModel)
        Dim bReturnCore As Boolean = False
        Dim bBalanced As Boolean = False

        If (ds Is Nothing) Then Return Nothing
        If (ds.Open(strModel, core, eDataSourceTypes.NotSet, True) <> eDatasourceAccessType.Opened) Then Return Nothing

        If (core.LoadModel(ds)) Then

            If core.RunEcoPath(bBalanced) Then
                If bBalanced Then
                    bReturnCore = True
                Else
                    'Message that the model needs to balancing
                    Dim fbMsg As New EwECore.cFeedbackMessage("Model in imported database failed to balance. Do you want to continue importing the diets?",
                                                                                eCoreComponentType.Plugin, eMessageType.DataImport, eMessageImportance.Critical, eMessageReplyStyle.YES_NO)
                    Me.m_Core.Messages.SendMessage(fbMsg)
                    If fbMsg.Reply = eMessageReply.YES Then
                        bReturnCore = True
                    End If
                End If
            End If

        End If

        If (ds.IsOpen) Then ds.Close()
        ds.Dispose()

        If bReturnCore Then
            Return core
        End If

        Return Nothing

    End Function


    Private Function Validate(Core As EwECore.cCore) As Boolean
        Dim bPassed As Boolean = False
        Try

            If Core IsNot Nothing Then
                'first step
                Dim epData As EwECore.cEcopathDataStructures = Core.EcopathDataStructures

                If Core.nGroups = Me.m_Core.nGroups Then
                    bPassed = True
                    'Next step
                    'same number of groups
                    For igrp As Integer = 1 To Me.m_Core.nGroups
                        If Not Me.m_Core.EcopathDataStructures.GroupName(igrp).Contains(epData.GroupName(igrp)) Then
                            'Nope failed the group name test
                            bPassed = False
                        End If 'Not Me.m_Core.EcopathDataStructures.GroupName(igrp).Contains(epData.GroupName(igrp))

                    Next igrp
                End If 'Core.nGroups = Me.m_Core.nGroups
            End If 'Core IsNot Nothing

        Catch ex As Exception
            'some kind of a message????
            Me.m_Core.Messages.SendMessage(New EwECore.cMessage("Exception while Importing Diets: " + ex.Message,
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))
            cLog.Write(ex)

            Return False
        End Try

        If Not bPassed Then
            'Message that the model failed validation
            Me.m_Core.Messages.SendMessage(New EwECore.cMessage("Imported model does not have the same structure as the currently loaded model. Diets cannot be imported.",
                                                                eMessageType.DataImport, eCoreComponentType.Plugin, eMessageImportance.Critical))
        End If

        Return bPassed

    End Function


End Class
