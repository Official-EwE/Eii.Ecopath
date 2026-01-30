' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to apply environmental response functions to capacity maps.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class gridApplyForagingResponses
        Inherits gridApplyShapeBase

#Region " Private vars "

        Private m_lProps As New List(Of cProperty)
        Private m_mhEcospace As cMessageHandler = Nothing

#End Region ' Private vars

#Region " Overrides "

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)

                If (Me.UIContext IsNot Nothing) Then
                    ' Deconfigure
                    Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                    Me.m_mhEcospace.Dispose()
                End If
                MyBase.UIContext = value
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_mhEcospace = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecospace, eMessageType.DataModified, Me.UIContext.SyncObject)
                    Me.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
#If DEBUG Then
                    Me.m_mhEcospace.Name = "gridApplyForagingResponses"
#End If
                End If
            End Set
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Sub OnCoreMessage(ByRef msg As cMessage)
            If (Not Me.IsDisposed) Then
                Try
                    If (msg.DataType = eDataTypes.EcospaceLayerDriver) Then
                        Me.UpdateUnits()
                    End If
                Catch ex As Exception

                End Try
                MyBase.OnCoreMessage(msg)
            End If
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing
            Dim mapManager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager
            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            Dim map As IEnviroInputData = Nothing
            Dim strUnit As String = ""

            ' Define grid dimensions
            Me.Redim(Me.Core.nGroups + 1, mapManager.nEnviroData + 2)

            For iMap As Integer = 1 To mapManager.nEnviroData

                map = mapManager.EnviroData(iMap)
                If (iMap = 1) Then
                    strUnit = bm.LayerDepth.Units
                Else
                    strUnit = ""
                End If

                Me(0, 1 + iMap) = New cPropertyColumnHeaderCell(Me.PropertyManager, DirectCast(map, cEnviroInputMap).Layer, eVarNameFlags.Name, strUnit:=strUnit)
                Me(0, 1 + iMap).Behaviors.Add(Me.m_bmRowCol)

            Next iMap

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            For iGroup As Integer = 1 To Me.Core.nGroups
                group = Me.Core.EcopathGroupInputs(iGroup)
                ' # Group index row header cells
                Me(iGroup, 0) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 0).Behaviors.Add(Me.m_bmRowCol)

                ' # Group name row header cells
                Me(iGroup, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
                Me(iGroup, 1).Behaviors.Add(Me.m_bmRowCol)
            Next

            UpdateUnits()
        End Sub

        Protected Overrides Sub FillData()

            Try
                Dim Manager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager
                Dim ShapeManager As cEnviroResponseShapeManager = Me.Core.EnviroResponseShapeManager
                Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
                Dim ff As cForcingFunction = Nothing
                Dim prop As cProperty = Nothing
                Dim strLabel As String

                For igrp As Integer = 1 To Me.Core.nGroups
                    Dim grp As cEcospaceGroupInput = Me.Core.EcospaceGroupInputs(igrp)
                    For imap As Integer = 1 To Manager.nEnviroData
                        Dim map As IEnviroInputData = Manager.EnviroData(imap)

                        prop = Nothing
                        If (imap = 1) Then
                            prop = Me.PropertyManager.GetProperty(bm.LayerDepth, eVarNameFlags.EcospaceCapacityEnabled)
                        Else
                            prop = Me.PropertyManager.GetProperty(bm.LayerDriver(imap - 1), eVarNameFlags.EcospaceCapacityEnabled)
                        End If
                        If (prop IsNot Nothing) Then
                            Me.m_lProps.Add(prop)
                            AddHandler prop.PropertyChanged, AddressOf Me.OnPropertyChanged
                        End If

                        strLabel = ""
                        Dim ishp As Integer = map.ResponseIndexForGroup(igrp)
                        If ishp > 0 Then
                            ff = ShapeManager.Item(ishp - 1)
                            strLabel = String.Format(SharedResources.GENERIC_LABEL_INDEXED, ff.Index, ff.Name)
                        End If

                        Me(igrp, imap + 1) = New cEwECell(strLabel, GetType(String))
                        Me(igrp, imap + 1).DataModel = Me.m_editor
                        Me(igrp, imap + 1).Behaviors.Add(Me.m_bmCell)
                    Next

                    prop = Me.PropertyManager.GetProperty(grp, eVarNameFlags.EcospaceCapCalType)
                    Me.m_lProps.Add(prop)
                    AddHandler prop.PropertyChanged, AddressOf Me.OnPropertyChanged
                Next
                Me.UpdateCellStates()
            Catch ex As Exception

            End Try

        End Sub

        Protected Sub UpdateUnits()

            Dim mapManager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager
            Dim map As IEnviroInputData = Nothing
            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            Dim prop As cPropertyHeaderCell = Nothing

            For iMap As Integer = 2 To mapManager.nEnviroData
                map = mapManager.EnviroData(iMap)
                prop = DirectCast(Me(0, 1 + iMap), cPropertyHeaderCell)
                prop.SetUnits(bm.LayerDriver(iMap - 1).Units)
                prop.Invalidate()
            Next

        End Sub

        Protected Overrides Sub ClearData()
            For Each prop As cProperty In Me.m_lProps
                RemoveHandler prop.PropertyChanged, AddressOf Me.OnPropertyChanged
            Next
            MyBase.ClearData()
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = False
        End Sub

        Public Overrides Sub ClearAllPairs()
            ' NOP
        End Sub

        Public Overrides Sub SetAllPairs()
            ' NOP
        End Sub

#End Region ' Overrides

#Region " Internals "

        Protected Overrides Sub CellClick(sender As Object, e As PositionEventArgs)

            Try

                Dim iGrp As Integer = e.Position.Row
                Dim iDriver As Integer = e.Position.Column - 1
                Dim cell As cEwECell = DirectCast(Me(e.Position.Row, e.Position.Column), cEwECell)

                If ((cell.Style And cStyleGuide.eStyleFlags.NotEditable) = cStyleGuide.eStyleFlags.NotEditable) Then Return

                Me.ShowSelectionDialog(eEnvironmentalResponseSelectionType.DriverGroup, iGrp, iDriver)

            Catch ex As Exception
                ' Whoah
            End Try

        End Sub

        Protected Overrides Sub OnRowColClicked(sender As Object, e As SourceGrid2.PositionEventArgs)
            Try

                Dim igrp As Integer = e.Position.Row
                Dim iDriver As Integer = e.Position.Column - 1

                ' Can no longer invoke UI for one group, multiple drivers. This makes no sense
                If (iDriver < 0) Then Return

                'just assume it is the column that the user has selected!!!
                Dim selectionType As eEnvironmentalResponseSelectionType = eEnvironmentalResponseSelectionType.Driver

                Me.ShowSelectionDialog(selectionType, igrp, iDriver)

            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnPropertyChanged(prop As cProperty, cf As cProperty.eChangeFlags)
            Me.UpdateCellStates()
        End Sub

        Private Sub ShowSelectionDialog(SelectionType As eEnvironmentalResponseSelectionType, iGrp As Integer, iDriver As Integer)
            If (iDriver = 0) Then Return
            Try
                Dim MapManager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager
                Dim ShapeManager As cBaseShapeManager = Me.Core.EnviroResponseShapeManager

                Dim dlg As New dlgSelectEnvironmentalResponse(Me.UIContext, ShapeManager, MapManager, iDriver, iGrp, SelectionType)
                dlg.ShowDialog()
                If dlg.DialogResult = DialogResult.OK Then
                    'the dialogue will update the CapacitMapInteractionManager with the selected Shapes
                    'update the interface from the CapacitMapInteractionManager data
                    Me.FillData()
                End If

            Catch ex As Exception

            End Try
        End Sub

        Private Sub UpdateCellStates()

            Dim Manager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager
            Dim ShapeManager As cEnviroResponseShapeManager = Me.Core.EnviroResponseShapeManager
            Dim mapManager As IEnvironmentalResponseManager = Me.Core.CapacityMapInteractionManager

            For igrp As Integer = 1 To Me.Core.nGroups
                Dim grp As cEcospaceGroupInput = Me.Core.EcospaceGroupInputs(igrp)
                Dim iGroup As Integer = grp.Index

                For imap As Integer = 1 To Manager.nEnviroData
                    Dim map As IEnviroInputData = Manager.EnviroData(imap)
                    Dim cell As cEwECell = CType(Me(iGroup, 1 + imap), cEwECell)

                    Dim style As cStyleGuide.eStyleFlags
                    If ((grp.CapacityCalculationType And eEcospaceCapacityCalType.EnvResponses) = eEcospaceCapacityCalType.EnvResponses) And (map.IsCapacityEnabled) Then

                        style = cStyleGuide.eStyleFlags.OK
                    Else
                        style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
                    End If
                    cell.Style = style
                    ' Reflect
                    Me.InvalidateCell(cell)
                Next
            Next

        End Sub

#End Region ' Internals

    End Class

End Namespace
