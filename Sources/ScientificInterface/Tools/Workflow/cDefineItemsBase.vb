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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwEUtils.Utilities
Imports EwECore
Imports EwEUtils.Core

Public Class cDefineItemsBase
    Implements IDefineItems

    Private m_lItems As New List(Of IItemInfo)
    Private m_lItemsRemoved As New List(Of IItemInfo)
    Private m_core As cCore = Nothing

    Public Sub New(ByVal core As cCore)
        Me.m_core = core
    End Sub

    Public Function Items() As IItemInfo() Implements IDefineItems.Items
        Return Me.m_lItems.ToArray
    End Function

    Public Function CreateItem(ByVal t As Type, ByVal strNameMask As String) As IItemInfo _
        Implements IDefineItems.CreateItem

        ' Gather all names
        Dim lstrNames As New List(Of String)
        Dim info As IItemInfo = Nothing
        Dim obj As Object = Nothing

        For Each info In Me.m_lItems : lstrNames.Add(info.Name) : Next
        For Each info In Me.m_lItemsRemoved : lstrNames.Add(info.Name) : Next

        Try
            obj = Activator.CreateInstance(t, New Object() {String.Format(strNameMask, cStringUtils.GetNextNumber(lstrNames.ToArray(), strNameMask))})
            Debug.Assert(TypeOf obj Is IItemInfo)
        Catch ex As Exception
            Debug.Assert(False)
        End Try

        Me.m_lItems.Add(DirectCast(obj, IItemInfo))
        Return info

    End Function

    Public Function ToggleDeleteItem(ByVal item As IItemInfo) As Boolean _
        Implements IDefineItems.ToggleDeleteItem

        item.IsRemoved = Not item.IsRemoved

        ' Check to see what is to happen to the item now
        Select Case item.Status

            Case eItemStatusTypes.Original, eItemStatusTypes.Added
                ' Clear removed state of item. Yes, it should not occur on the 'added' list, but hey. This always works
                Me.m_lItemsRemoved.Remove(item)

            Case eItemStatusTypes.Removed, eItemStatusTypes.Invalid
                ' Remove item from org list if is New, there is no need to preserve it.
                If item.IsNew() Then Me.m_lItems.Remove(item)
                ' Add to removed list if item is an original
                If Not item.IsNew() Then Me.m_lItemsRemoved.Add(item)

        End Select

    End Function

    Public Overridable Function ApplyAddRemove() As Boolean _
        Implements IDefineItems.ApplyAddRemove

    End Function

    Public Overridable Function ApplyUpdate() As Boolean _
        Implements IDefineItems.ApplyUpdate

    End Function

    Public Function CanApply() As Boolean Implements IDefineItems.CanApply

    End Function

#Region " Internals "

    Protected Function MustAddRemove() As Boolean

    End Function

    Protected Function MustUpdate() As Boolean

    End Function

    Protected overridable Function ValidateUniqueNames() As Boolean

        Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_DUPLICATE_NAMES, eCoreComponentType.External, _
                                         eMessageType.DataValidation, eMessageImportance.Question, _
                                         cFeedbackMessage.eReplyStyle.YES_NO, eDataTypes.NotSet, cFeedbackMessage.eReply.NO)
        Dim bHasDuplicates As Boolean = False
        Dim bHasBlank As Boolean = False
        Dim lstrHandled As New List(Of String)

        For Each info As IItemInfo In Me.m_lItems
            If String.IsNullOrWhiteSpace(info.Name) Then
                bHasBlank = True
            ElseIf Not Me.IsNameUnique(info) Then
                If Not lstrHandled.Contains(info.Name) Then
                    fmsg.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation, _
                                                         String.Format(My.Resources.PROMPT_DUPLICATE_NAME, info.Name), _
                                                         eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, cCore.NULL_VALUE))
                    lstrHandled.Add(info.Name)
                End If
                bHasDuplicates = True
            End If
        Next

        If bHasBlank Then
            Me.m_core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_EMPTY_NAMES, eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
            Return False
        End If

        If bHasDuplicates Then
            Me.m_core.Messages.SendMessage(fmsg)
            Return fmsg.Reply = cFeedbackMessage.eReply.YES
        End If

        Return True

    End Function

    Private Function IsNameUnique(ByVal info As IItemInfo) As Boolean

        ' Check if name is unique
        For Each infoTmp As IItemInfo In Me.m_lItems
            ' Only compare new items
            If (infoTmp.Status <> eItemStatusTypes.Removed And info.Status <> eItemStatusTypes.Removed) Then
                ' Does name already exist?
                If (Not Object.ReferenceEquals(infoTmp, info)) And (String.Compare(info.Name, infoTmp.Name, True) = 0) Then
                    ' Report failure
                    Return False
                End If
            End If
        Next
        Return True

    End Function

#End Region

End Class
