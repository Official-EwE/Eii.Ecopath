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

        Dim core As cCore = Me.getCoreFromFilename(ModelFileName)

        If Me.ValidateEcopathData(core.EcopathDataStructures) Then
            DietPrefenences = New cDietPreferences(core.EcopathDataStructures)
            Return True
        End If

        'Clean up our mess
        If core IsNot Nothing Then
            core.Dispose()
            core = Nothing
        End If

        Return False

    End Function

    Private Function getCoreFromFilename(strModel As String) As cCore

        Dim core As New cCore()
        Dim ds As EwECore.DataSources.IEwEDataSource = EwECore.DataSources.cDataSourceFactory.Create(strModel)
        Dim bSuccess As Boolean = False

        If (ds Is Nothing) Then Return Nothing
        If (ds.Open(strModel, core, eDataSourceTypes.NotSet, True) <> eDatasourceAccessType.Opened) Then Return Nothing

        If (core.LoadModel(ds)) Then

            Dim bBalanced As Boolean
            If core.RunEcoPath(bBalanced) Then
                If bBalanced Then
                    Return core
                End If
            End If

            ' JS 25Apr16: User is responsible for importing from a compatible model

            '' Test compatibility
            'If (core.SampleManager.ModelHash <> Me.ModelHash) Then
            '    Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.SAMPLES_IMPORT_ERROR_INCOMPATIBLE, strModel),
            '                                                eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
            '    Return False
            'End If

            '' Test if there are models
            'If (core.SampleManager.nSamples = 0) Then
            '    Me.m_Core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.SAMPLES_IMPORT_ERROR_NOSAMPLES, strModel),
            '                                                eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
            '    Return False
            'End If



        End If

        If (ds.IsOpen) Then ds.Close()
        ds.Dispose()
        'core.Dispose()

        Return Nothing

    End Function


    Private Function ValidateEcopathData(EcopathData As cEcopathDataStructures) As Boolean
        'Giver ehhh...
        Return True
    End Function


End Class
