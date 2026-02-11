' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to configure Ecospace habitat fishing limitations.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class gridEcospaceHabitatFishery
        Inherits cEwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            All
        End Enum

        Private m_bInUpdate As Boolean = False

        ' Predict Effort flag is monitored to block the form content if PredictEffort is OFF
        Private WithEvents m_bpEffort As cBooleanProperty = Nothing

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_bpEffort = Nothing
                End If

                If (value IsNot Nothing) Then
                    Dim ecospaceModelParams As cEcospaceModelParameters = value.Core.EcospaceModelParameters()
                    Dim propMan As cPropertyManager = value.PropertyManager
                    Me.m_bpEffort = DirectCast(propMan.GetProperty(ecospaceModelParams, eVarNameFlags.PredictEffort), cBooleanProperty)
                End If

                MyBase.UIContext = value

            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(1, 3 + Me.Core.nHabitats)
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
            Me(0, eColumnTypes.All) = New cEwEColumnHeaderCell(My.Resources.HEADER_FISH_EVERYWHERE)

            For j As Integer = 1 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                Me(0, eColumnTypes.All + j) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next
            Me.FixedColumnWidths = True

        End Sub

        Protected Overrides Sub FillData()

            Dim bEnable As Boolean = (CBool(Me.m_bpEffort.GetValue()) = True)
            Dim cell As cEwECellBase = Nothing

            For i As Integer = 1 To Me.Core.nFleets

                Dim fleet As cEcospaceFleetInput = Me.Core.EcospaceFleetInputs(i)
                Dim iRow As Integer = Me.AddRow()

                Me(iRow, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, fleet, eVarNameFlags.Index)
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, fleet, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.All) = New Cells.Real.CheckBox(False)
                Me(iRow, eColumnTypes.All).Behaviors.Add(Me.EwEEditHandler)

                If (bEnable) Then

                    For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                        Me(iRow, eColumnTypes.All + iHabitat) = New Cells.Real.CheckBox(fleet.HabitatFishery(iHabitat))
                        Me(iRow, eColumnTypes.All + iHabitat).Behaviors.Add(Me.EwEEditHandler)
                    Next

                Else
                    For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                        Me(iRow, eColumnTypes.All + iHabitat) = New cEwECell("", GetType(String), eStyleFlags.NotEditable Or eStyleFlags.Null)
                    Next

                End If

            Next

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecospace}
            End Get
        End Property

#End Region ' Overrides

#Region " Events "

        Protected Overrides Function OnCellValueChanged(p As Position, cell As Cells.ICellVirtual) As Boolean

            If (Me.m_bInUpdate) Then Return True

            Dim fleet As cEcospaceFleetInput = Me.Core.EcospaceFleetInputs(p.Row)

            Select Case p.Column

                Case eColumnTypes.All
                    For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                        fleet.HabitatFishery(iHabitat) = CBool(cell.GetValue(p))
                    Next

                Case Else
                    Dim iHabitat As Integer = p.Column - eColumnTypes.All
                    fleet.HabitatFishery(iHabitat) = CBool(cell.GetValue(p))

            End Select

            Me.UpdateRow(p.Row)

        End Function

        Private Sub m_bpEffort_PropertyChanged(prop As cProperty, changeFlags As cProperty.eChangeFlags) _
            Handles m_bpEffort.PropertyChanged

            Try
                Me.BeginInvoke(New MethodInvoker(AddressOf Me.RefreshContent))
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Event 

#Region " Internals "

        Private Sub UpdateRow(iRow As Integer)

            If (Me.m_bInUpdate) Then Return
            Me.m_bInUpdate = True

            Dim fleet As cEcospaceFleetInput = Me.Core.EcospaceFleetInputs(iRow)
            Dim bFishEverywhere As Boolean = True

            For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                Dim bIsAllowed As Boolean = (fleet.HabitatFishery(iHabitat) = True)
                Me(iRow, eColumnTypes.All + iHabitat).Value = bIsAllowed
                bFishEverywhere = bFishEverywhere And bIsAllowed
            Next

            Me(iRow, eColumnTypes.All).Value = bFishEverywhere

            Me.m_bInUpdate = False

        End Sub

#End Region ' Internals

    End Class

End Namespace

