' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath

    Public MustInherit Class cEcopathMergeSplitGroups

#Region " Private vars "

        ''' <summary>The core that holds the source model.</summary>
        Protected m_core As cCore = Nothing

#End Region ' Private vars

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the current Ecopath model is ready to merge or split groups.
        ''' </summary>
        ''' <param name="bSendMessage"></param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Function CanMergeSplitGroups(Optional bSendMessage As Boolean = False) As Boolean

            Dim sm As cCoreStateMonitor = Me.m_core.StateMonitor

            If Not sm.HasEcopathLoaded() Then
                If bSendMessage Then Me.SendMessage(My.Resources.CoreMessages.ECOPATH_MERGESPLIT_ERROR_NOMODEL, False)
                Return False
            End If

            If Me.m_core.nEcosimScenarios > 0 Then
                If bSendMessage Then Me.SendMessage(My.Resources.CoreMessages.ECOPATH_MERGESPLIT_ERROR_HASECOSIM, False)
                Return False
            End If

            If Not Me.m_core.SaveChanges() Then Return False

            Return True

        End Function

#End Region ' Public access

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Send a message.
        ''' </summary>
        ''' <param name="strMessage"></param>
        ''' <param name="bSuccess"></param>
        ''' -----------------------------------------------------------------------
        Protected Sub SendMessage(strMessage As String, bSuccess As Boolean)
            Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.Ecopath,
                                If(bSuccess, eMessageImportance.Information, eMessageImportance.Critical))
            Me.m_core.Messages.SendMessage(msg)
        End Sub

#End Region ' Internals

    End Class

End Namespace

