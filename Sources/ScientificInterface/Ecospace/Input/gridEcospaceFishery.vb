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

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports ScientificInterfaceShared.Style.cStyleGuide

#End Region

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid to configure Ecospace habitat fishing limitations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridEcospaceFishery
        : Inherits EwEGrid

        Private m_iStartHabCol As Integer = -1
        Private m_iStartMPACol As Integer = -1
        Private m_bInUpdate As Boolean = False
        Private WithEvents m_bpEffort As cBooleanProperty = Nothing

        Public Sub New()

            MyBase.New()
            Me.FixedColumnWidths = False

        End Sub

#Region " Overrides "

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
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

            'Call base class InitStyle method. 
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Me.Core.nFleets + 1, 3 + Me.Core.nHabitats + Me.Core.nMPAs + 2)
            Me.m_iStartHabCol = 3
            Me.m_iStartMPACol = 3 + Me.Core.nHabitats

            'Set header cells #(0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.ECOSPACE_HEADER_AREAFISHING)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_FISH_EVERYWHERE)

            'Dynamic row header - Fleet name
            For i As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcospaceFleets(i)
                Me(i, 0) = New EwERowHeaderCell(CStr(i))
                '# Fleet name header 
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            'Dynamic column header - Habitats
            For j As Integer = 0 To Me.Core.nHabitats - 1
                source = Me.Core.EcospaceHabitats(j)
                Me(0, Me.m_iStartHabCol + j) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name, Nothing, My.Resources.HEADER_HABITAT_X)
            Next

            'Dynamic column header - MPAs
            For j As Integer = 1 To Me.Core.nMPAs
                source = Me.Core.EcospaceMPAs(j)
                Me(0, Me.m_iStartMPACol + j - 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name, Nothing, My.Resources.HEADER_MPA_X)
            Next

            'Column header cell - Effective power
            Me(0, Me.ColumnsCount - 2) = New EwEColumnHeaderCell(SharedResources.HEADER_EFFPOWER)
            'Column header cell - Tot.Eff.Multip.
            Me(0, Me.ColumnsCount - 1) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTEFFMULTI)


        End Sub

        Protected Overrides Sub FillData()

            Dim bEnable As Boolean = (CBool(Me.m_bpEffort.GetValue()) = True)
            Dim cell As EwECellBase = Nothing

            For i As Integer = 1 To Me.Core.nFleets

                Dim source As cEcospaceFleetInput = Me.Core.EcospaceFleets(i)

                If (bEnable) Then
                    Me(i, 2) = New Cells.Real.CheckBox(False)
                    Me(i, 2).Behaviors.Add(Me.EwEEditHandler)

                    For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                        Me(i, Me.m_iStartHabCol + iHabitat) = New Cells.Real.CheckBox(source.HabitatFishery(iHabitat))
                        Me(i, Me.m_iStartHabCol + iHabitat).Behaviors.Add(Me.EwEEditHandler)
                    Next

                    For iMPA As Integer = 1 To Me.Core.nMPAs
                        Me(i, Me.m_iStartMPACol + iMPA - 1) = New Cells.Real.CheckBox(CBool(source.MPAFishery(iMPA)))
                        Me(i, Me.m_iStartMPACol + iMPA - 1).Behaviors.Add(Me.EwEEditHandler)
                    Next

                    Me(i, Me.ColumnsCount - 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EffectivePower)
                    Me(i, Me.ColumnsCount - 1) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SEmult)

                    Me.UpdateRow(i)
                Else
                    Me(i, 2) = New EwECell("", GetType(String), eStyleFlags.NotEditable Or eStyleFlags.Null)

                    For iHabitat As Integer = 0 To Me.Core.nHabitats - 1
                        Me(i, Me.m_iStartHabCol + iHabitat) = New EwECell("", GetType(String), eStyleFlags.NotEditable Or eStyleFlags.Null)
                    Next

                    For iMPA As Integer = 1 To Me.Core.nMPAs
                        Me(i, Me.m_iStartMPACol + iMPA - 1) = New EwECell("", GetType(String), eStyleFlags.NotEditable Or eStyleFlags.Null)
                    Next

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EffectivePower)
                    cell.Style = cell.Style Or eStyleFlags.NotEditable
                    Me(i, Me.ColumnsCount - 2) = cell

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.SEmult)
                    cell.Style = cell.Style Or eStyleFlags.NotEditable
                    Me(i, Me.ColumnsCount - 1) = cell

                End If

            Next

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSpace}
            End Get
        End Property

#End Region ' Overrides

#Region " Events "

        Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

            If (Me.m_bInUpdate) Then Return True

            Dim fleet As cEcospaceFleetInput = Me.Core.EcospaceFleets(p.Row)
            Dim bChecked As Boolean = CBool(cell.GetValue(p))

            Select Case p.Column

                Case 2
                    If (bChecked) Then
                        ' "Fish everywhere" toggle clicked
                        For i As Integer = 0 To Me.Core.nHabitats - 1
                            fleet.HabitatFishery(i) = (i = 0)
                        Next
                        For i As Integer = 1 To Me.Core.nMPAs
                            fleet.MPAFishery(i) = bChecked
                        Next
                    End If

                Case Me.m_iStartHabCol To Me.m_iStartHabCol + (Me.Core.nHabitats - 1)
                    ' Habitat clicked
                    fleet.HabitatFishery(p.Column - Me.m_iStartHabCol) = bChecked

                Case Else
                    ' MPA clicked
                    fleet.MPAFishery(p.Column - Me.m_iStartMPACol + 1) = bChecked

            End Select

            UpdateRow(p.Row)

        End Function

#End Region ' Events

#Region " Internals "

        ''' <summary>
        ''' Update check boxes in a row
        ''' </summary>
        ''' <param name="i">Row number to update.</param>
        Private Sub UpdateRow(ByVal i As Integer)

            Dim source As cEcospaceFleetInput = Me.Core.EcospaceFleets(i)
            Dim bChecked As Boolean = False
            Dim bAllHabs As Boolean = CBool(source.HabitatFishery(0))
            Dim nHabs As Integer = 0
            Dim bAllChecked As Boolean = True

            Me.m_bInUpdate = True

            For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                If CBool(source.HabitatFishery(iHabitat)) Then nHabs += 1
            Next
            bAllHabs = bAllHabs Or (nHabs = Me.Core.nHabitats)
            bAllChecked = bAllHabs

            Me(i, Me.m_iStartHabCol).Value = bAllHabs

            For iHabitat As Integer = 1 To Me.Core.nHabitats - 1
                bChecked = CBool(source.HabitatFishery(iHabitat))
                Me(i, Me.m_iStartHabCol + iHabitat).Value = bChecked And Not bAllHabs
            Next

            For iMPA As Integer = 1 To Me.Core.nMPAs
                bChecked = CBool(source.MPAFishery(iMPA))
                Me(i, Me.m_iStartMPACol + iMPA - 1).Value = bChecked
                bAllChecked = bAllChecked And bChecked
            Next

            ' Set 'all' column state
            Me(i, 2).SetValue(New Position(i, 2), bAllChecked)

            Me.m_bInUpdate = False

        End Sub

#End Region ' Internals

#Region " Event handlers "

        Private Sub m_bpEffort_PropertyChanged(prop As cProperty, changeFlags As cProperty.eChangeFlags) _
            Handles m_bpEffort.PropertyChanged

            Try
                BeginInvoke(New MethodInvoker(AddressOf RefreshContent))
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Event handlers
    End Class

End Namespace

