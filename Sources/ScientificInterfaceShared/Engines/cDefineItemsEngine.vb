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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Definitions

#End Region ' 

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Item representing a <see cref="cCoreInputOutputBase"/> entity in EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cItemInfo

        ''' <summary>The status of an item.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original
        Private m_dbid As Integer = 0
        Private m_dbidNew As Integer = 0

        Private m_dtVars As New Dictionary(Of eVarNameFlags, Object)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="item">The <see cref="cCoreInputOutputBase"/> to create the item for</param>
        ''' <param name="vars">Optional collection of <see cref="eVarNameFlags"/> to include into object management.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal item As cCoreInputOutputBase, Optional vars As eVarNameFlags() = Nothing)

            Debug.Assert(item IsNot Nothing)

            If (vars IsNot Nothing) Then
                For Each var As eVarNameFlags In vars
                    Me.m_dtVars(var) = item.GetVariable(var)
                Next
            End If

            Me.Index = item.Index
            Me.Name = item.Name
            Me.DBID = item.DBID

            Me.Status = eItemStatusTypes.Original
            Me.ConsolidateDBID()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this item unit.</param>
        ''' <param name="vars">Optional collection of <see cref="eVarNameFlags"/> to include into object management.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String, Optional vars As eVarNameFlags() = Nothing)

            If (vars IsNot Nothing) Then
                For Each var As eVarNameFlags In vars
                    Dim md As cVariableMetaData = cVariableMetaData.Get(var)
                    Me.m_dtVars(var) = md.NullValue
                Next
            End If

            Me.Name = strName
            Me.Status = eItemStatusTypes.Added

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Name() As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEcospaceMPA.DBID"/> of an associated unit, if any.
        ''' <seealso cref="ConsolidateDBID()"/>
        ''' </summary>
        ''' <remarks>
        ''' Use <see cref="ConsolidateDBID()"/> to update any differences between
        ''' the current and a desired database ID.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property DBID As Integer
            Get
                ' Return the actual DBID 
                Return Me.m_dbid
            End Get
            Set(value As Integer)
                ' Set a provisinal new DBID, pending DB transaction success
                Me.m_dbidNew = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' When all DB changes have committed succesfully, use this method to 
        ''' set the internal DBID to the provisional DBID assigned during the
        ''' DB transaction.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ConsolidateDBID()
            If (Me.m_dbid <= 0 And Me.m_dbidNew > 0) Then
                Me.m_dbid = Me.m_dbidNew
                Me.m_dbidNew = 0
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cEcospaceMPA.Index"/> of an associated unit, if any.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Index As Integer = 0

        Public Function GetVariable(var As eVarNameFlags) As Object
            ' No safety checking here. You are supposed to know what you're doing.
            Return Me.m_dtVars(var)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eItemStatusTypes">item status</see> for the unit 
        ''' object.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Status As eItemStatusTypes
            Get
                Return Me.m_status
            End Get
            Private Set(value As eItemStatusTypes)
                Me.m_status = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the item has changed.
        ''' </summary>
        ''' <returns>
        ''' True when MPA <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged(ByVal item As cCoreInputOutputBase) As Boolean
            If Me.IsNew Then Return False
            If (String.Compare(item.Name, Me.Name, False) <> 0) Then Return True
            ' Check variables
            For Each var As eVarNameFlags In Me.m_dtVars.Keys
                If (Not Object.Equals(Me.m_dtVars(var), item.GetVariable(var))) Then Return True
            Next
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the item is to be created.
        ''' </summary>
        ''' <returns>
        ''' True when MPA <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.DBID <= 0)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this item is flagged for deletion. Toggling this flag
        ''' will update the <see cref="Status">Status</see> of the item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = eItemStatusTypes.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Not Me.IsNew() Then
                    If bDelete Then
                        Me.Status = eItemStatusTypes.Removed
                    Else
                        Me.Status = eItemStatusTypes.Original
                    End If
                Else
                    If bDelete Then
                        Me.Status = eItemStatusTypes.Invalid
                    Else
                        Me.Status = eItemStatusTypes.Added
                    End If
                End If
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Engine for creating, deleting, renaming and reordering EwE core items.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cDefineItemsEngine

#Region " Private vars "

        Private m_lItems As New List(Of cItemInfo)
        Private m_lDeleted As New List(Of cItemInfo)

#End Region ' Private vars

#Region " Constructor "

        ''' <summary>
        ''' Initialized the engine to core data.
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="cc"></param>
        Public Sub New(core As cCore, cc As eCoreCounterTypes, newitemamask As String)

            Me.Core = core
            Me.CoreCounter = cc
            Me.NewItemMask = newitemamask

            For i As Integer = 1 To Me.Core.GetCoreCounter(Me.CoreCounter)
                Dim coreItem As cCoreInputOutputBase = Me.GetCoreItem(i)
                Debug.Assert(coreItem IsNot Nothing)
                Me.m_lItems.Add(New cItemInfo(coreItem, Me.Variables))
            Next

        End Sub

#End Region ' Constructor

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get all available items.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Items() As IEnumerable(Of cItemInfo)
            Return Me.m_lItems
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new item
        ''' </summary>
        ''' <param name="iIndex">The index for the new item. If -1 is specified,
        ''' the new item will be appended to the list.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function AddItem(iIndex As Integer) As cItemInfo

            Dim item As cItemInfo = Nothing
            Dim names As New List(Of String)

            If (iIndex = -1) Then iIndex = Me.m_lItems.Count

            ' Collect all current item names
            For Each item In Me.m_lItems
                names.Add(item.Name)
            Next

            item = New cItemInfo(cStringUtils.Localize(Me.NewItemMask, cStringUtils.GetNextNumber(names, Me.NewItemMask)))
            If (iIndex >= names.Count) Then
                Me.m_lItems.Add(item)
            Else
                Me.m_lItems.Insert(Math.Max(iIndex, 0), item)
            End If
            Return item

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Toggle deletion status.
        ''' </summary>
        ''' <param name="item"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function ToggleDeleteItem(item As cItemInfo) As Boolean

            ' Toggle deletion flag
            item.FlaggedForDeletion = Not item.FlaggedForDeletion

            ' Check to see what is to happen to the item now
            Select Case item.Status
                Case eItemStatusTypes.Original
                    Me.m_lDeleted.Remove(item)
                Case eItemStatusTypes.Added
                    Me.m_lDeleted.Remove(item)
                Case eItemStatusTypes.Removed
                    Me.m_lDeleted.Add(item)
                Case eItemStatusTypes.Invalid
                    Me.m_lItems.Remove(item)
            End Select
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Move an item in the order of it all. Everywhere.
        ''' </summary>
        ''' <param name="item"></param>
        ''' <param name="iNewIndex"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function MoveItem(item As cItemInfo, iNewIndex As Integer) As Boolean
            Me.m_lItems.Remove(item)
            Me.m_lItems.Insert(iNewIndex, item)
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply changes to the core.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bRestructured As Boolean = False
            Dim bChanged As Boolean = False
            Dim item As cItemInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim i As Integer = 0
            Dim bSuccess As Boolean = True

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess changes
            For i = 0 To Me.m_lItems.Count - 1
                item = Me.m_lItems(i)

                If item.IsNew() Then
                    bRestructured = True
                Else
                    bRestructured = bRestructured Or (item.Index <> (i + 1))
                    bChanged = bChanged Or item.IsChanged(Me.GetCoreItem(item.Index))
                End If
            Next i

            ' Ask for confirmation
            If (Me.m_lDeleted.Count > 0) Then

                strPrompt = cStringUtils.Localize(My.Resources.GENERIC_CONFIRMDELETE_PROMPT, Me.m_lDeleted.Count)
                Dim fmsg As New cFeedbackMessage(strPrompt, eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                Me.Core.Messages.SendMessage(fmsg)

                Select Case fmsg.Reply
                    Case eMessageReply.CANCEL, eMessageReply.NO
                        ' Abort Apply process
                        Return False
                    Case Else
                        ' NOP
                        bRestructured = True
                End Select

            End If

            ' Handle added and removed items
            If (bRestructured) Then

                If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False
                cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.GENERIC_STATUS_APPLYCHANGES)

                ' Add new core objects and fix item order
                For i = 0 To Me.m_lItems.Count - 1
                    item = Me.m_lItems(i)
                    If (item.IsNew) Then
                        bSuccess = bSuccess And Me.CreateCoreItem(item, i + 1, item.DBID)
                    Else
                        bSuccess = bSuccess And Me.MoveCoreItem(item, i + 1)
                    End If
                Next

                ' Remove items
                For i = 0 To Me.m_lDeleted.Count - 1
                    item = Me.m_lDeleted(i)

                    ' Sanity check
                    Debug.Assert(Not item.IsNew())

                    bSuccess = bSuccess And Me.DeleteCoreItem(item)
                Next i

                ' The core will reload now
                bSuccess = Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace, bSuccess)
                cApplicationStatusNotifier.EndProgress(Me.Core)

            End If

            If (Not bSuccess) Then Return False

            ' Remove completed deletions
            Me.m_lDeleted.Clear()
            ' Consolidate new IDs
            For Each item In Me.m_lItems
                item.ConsolidateDBID()
            Next

            ' Update core objects
            If (bChanged) Then

                ' Build quick local lookup for locating items by DBID
                Dim dtCoreItems As New Dictionary(Of Integer, cCoreInputOutputBase)
                Dim coreItem As cCoreInputOutputBase = Nothing

                For i = 1 To Me.Core.GetCoreCounter(Me.CoreCounter)
                    coreItem = Me.GetCoreItem(i)
                    dtCoreItems(coreItem.DBID) = coreItem
                Next

                ' For each local admin unit
                For i = 0 To Me.m_lItems.Count - 1
                    ' Get local admin unit
                    item = Me.m_lItems(i)
                    If (Not item.FlaggedForDeletion) Then
                        ' Get core item
                        coreItem = dtCoreItems(item.DBID)
                        ' Has user changed the item?
                        If item.IsChanged(coreItem) Then
                            ' #Yes: update it
                            Me.UpdateCoreItem(item, coreItem)
                        End If
                    End If
                Next
            End If

            Return bSuccess

        End Function

#Region " Core manipulations and internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The new item mask to apply. The mask must have a {0} placeholder to 
        ''' receive new item autonumbers.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property NewItemMask As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property Core As cCore = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property CoreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function GetCoreItem(iIndex As Integer) As cCoreInputOutputBase

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Optionally add extra variables that you may wish to include in the engine. 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function Variables() As eVarNameFlags()
            Return Nothing
        End Function


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function CreateCoreItem(item As cItemInfo, iIndex As Integer, ByRef iDBID As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function MoveCoreItem(item As cItemInfo, iIndex As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function DeleteCoreItem(item As cItemInfo) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function UpdateCoreItem(item As cItemInfo, coreItem As cCoreInputOutputBase) As Boolean
            coreItem.Name = item.Name
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function ValidateContent() As Boolean
            Return Me.ValidateNames
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function ValidateNames() As Boolean

            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_DUPLICATE_NAMES, eCoreComponentType.External, eMessageType.DataValidation, eMessageImportance.Question, eMessageReplyStyle.YES_NO, eDataTypes.NotSet, eMessageReply.NO)
            Dim bHasDuplicates As Boolean = False
            Dim bHasBlank As Boolean = False
            Dim handled As New List(Of String)

            For Each item As cItemInfo In Me.m_lItems
                If String.IsNullOrEmpty(item.Name) Then
                    bHasBlank = True
                ElseIf Not Me.IsNameUnique(item.Name, item) Then
                    If Not handled.Contains(item.Name) Then
                        fmsg.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation,
                                                             cStringUtils.Localize(My.Resources.PROMPT_DUPLICATE_NAME, item.Name),
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, cCore.NULL_VALUE))
                        handled.Add(item.Name)
                    End If
                    bHasDuplicates = True
                End If
            Next

            If bHasBlank Then
                Me.Core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_EMPTY_NAMES, eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
                Return False
            End If

            If bHasDuplicates Then
                Me.Core.Messages.SendMessage(fmsg)
                Return fmsg.Reply = eMessageReply.YES
            End If

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function IsNameUnique(ByVal strName As String, ByVal item As cItemInfo) As Boolean

            ' Check if name is unique
            For i As Integer = 0 To Me.m_lItems.Count - 1

                Dim tmp As cItemInfo = Me.m_lItems(i)
                Dim bCompareItem As Boolean = (tmp.Status <> eItemStatusTypes.Removed)

                If (item IsNot Nothing) Then
                    bCompareItem = (item.Status <> eItemStatusTypes.Removed)
                End If

                ' Only compare new items
                If (bCompareItem) Then
                    ' Does name already exist?
                    If (Not ReferenceEquals(tmp, item)) And (String.Compare(strName, tmp.Name, True) = 0) Then
                        ' Report failure
                        Return False
                    End If
                End If
            Next
            Return True

        End Function

#End Region ' Core manipulations internals

    End Class

End Namespace ' Controls
