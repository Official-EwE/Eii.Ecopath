' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports EwECore.Plugins
Imports EwECore.Plugins.UI
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' <summary>
''' Grid to control which <see cref="IAutoRunPlugin">Auto-executing plug-ins</see>
''' are allowed to auto-run.
''' </summary>

Public Class gridAutoRun
    Inherits cEwEGrid

    Private Class cPluginNameSort
        Implements IComparer(Of IPlugin)

        Public Function Compare(x As IPlugin, y As IPlugin) As Integer Implements IComparer(Of IPlugin).Compare
            Return String.Compare(x.DisplayName, y.DisplayName)
        End Function
    End Class

    Private m_IsComponentLoaded As New Dictionary(Of eCoreComponentType, Boolean)

    Private Enum eColumnTypes As Integer
        Index
        Plugin
        Ecopath
        Ecosim
        MonteCarlo
        Ecospace
    End Enum

    Public Sub New()
        MyBase.New()
    End Sub

    Public Event OnValueChanged()

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim sm As cCoreStateMonitor = Me.Core.StateMonitor

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
        Me(0, eColumnTypes.Plugin) = New cEwEColumnHeaderCell(SharedResources.HEADER_PLUGIN)
        Me(0, eColumnTypes.Ecopath) = New cEwEColumnHeaderCell(SharedResources.HEADER_ECOPATH) With {
            .Tag = eCoreComponentType.Ecopath
        }
        Me(0, eColumnTypes.Ecosim) = New cEwEColumnHeaderCell(SharedResources.HEADER_ECOSIM) With {
            .Tag = eCoreComponentType.Ecosim
        }
        Me(0, eColumnTypes.MonteCarlo) = New cEwEColumnHeaderCell(SharedResources.HEADER_MONTECARLO) With {
            .Tag = eCoreComponentType.EcoSimMonteCarlo
        }
        Me(0, eColumnTypes.Ecospace) = New cEwEColumnHeaderCell(SharedResources.HEADER_ECOSPACE) With {
            .Tag = eCoreComponentType.Ecospace
        }

        Me.m_IsComponentLoaded(eCoreComponentType.Ecopath) = sm.HasEcopathLoaded
        Me.m_IsComponentLoaded(eCoreComponentType.Ecosim) = sm.HasEcosimLoaded
        Me.m_IsComponentLoaded(eCoreComponentType.EcoSimMonteCarlo) = sm.HasEcosimLoaded
        Me.m_IsComponentLoaded(eCoreComponentType.Ecospace) = sm.HasEcospaceLoaded

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return

        Dim pm As cPluginManager = Me.Core.PluginManager
        If (pm Is Nothing) Then Return

        Dim styleNA As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
        Dim cols As eColumnTypes() = CType([Enum].GetValues(GetType(eColumnTypes)), eColumnTypes())

        Dim lPlugins As New List(Of IPlugin)
        lPlugins.AddRange(pm.GetPlugins(GetType(IAutoRunPlugin)))
        lPlugins.Sort(New cPluginNameSort())

        For i As Integer = 0 To lPlugins.Count - 1
            Dim pi As IAutoRunPlugin = DirectCast(lPlugins(i), IAutoRunPlugin)
            Dim features As eCoreComponentType() = pi.AutoRunTypes

            Dim iRow As Integer = Me.AddRow()
            For j As Integer = 0 To cols.Length - 1
                Select Case cols(j)
                    Case eColumnTypes.Index
                        Me(iRow, j) = New cEwERowHeaderCell(CStr(i + 1))
                    Case eColumnTypes.Plugin
                        Me(iRow, j) = New cEwERowHeaderCell(pi.DisplayName) With {
                            .Tag = pi
                        }
                    Case Else
                        Dim comp As eCoreComponentType = DirectCast(Me(0, j).Tag, eCoreComponentType)
                        If features.Contains(comp) And Me.m_IsComponentLoaded(comp) Then
                            Me(iRow, j) = New cEwECheckboxCell(pi.AutoRun(comp) = True)
                            Me(iRow, j).Behaviors.Add(Me.EwEEditHandler)
                        Else
                            Me(iRow, j) = New cEwECell("", styleNA)
                        End If
                End Select
            Next
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        If (Me.UIContext Is Nothing) Then Return
        Me.FixedColumnWidths = True

    End Sub

    Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean
        Try
            RaiseEvent OnValueChanged()
        Catch ex As Exception

        End Try
        Return MyBase.OnCellValueChanged(p, cell)
    End Function

    Public Sub CheckAll()
        Me.CheckAll(True)
    End Sub

    Public Sub ClearAll()
        Me.CheckAll(False)
    End Sub

    Public Function Apply() As Boolean

        Dim cols As eColumnTypes() = CType([Enum].GetValues(GetType(eColumnTypes)), eColumnTypes())

        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim pi As IAutoRunPlugin = DirectCast(Me(iRow, eColumnTypes.Plugin).Tag, IAutoRunPlugin)
            Dim features As eCoreComponentType() = pi.AutoRunTypes

            For j As Integer = 0 To cols.Length - 1
                Select Case cols(j)
                    Case eColumnTypes.Index, eColumnTypes.Plugin
                        ' NOP
                    Case Else
                        Dim comp As eCoreComponentType = DirectCast(Me(0, j).Tag, eCoreComponentType)
                        If features.Contains(comp) And Me.m_IsComponentLoaded(comp) Then
                            Debug.Assert(TypeOf Me(iRow, j) Is cEwECheckboxCell)
                            pi.AutoRun(comp) = DirectCast(Me(iRow, j), cEwECheckboxCell).Checked
                        End If
                End Select
            Next
        Next

    End Function

    Private Sub CheckAll(bCheck As Boolean)
        Dim cols As eColumnTypes() = CType([Enum].GetValues(GetType(eColumnTypes)), eColumnTypes())
        For iRow As Integer = 1 To Me.RowsCount - 1
            For j As Integer = 0 To cols.Length - 1
                Select Case cols(j)
                    Case eColumnTypes.Index, eColumnTypes.Plugin
                        ' NOP
                    Case Else
                        If (TypeOf Me(iRow, j) Is cEwECheckboxCell) Then
                            Me(iRow, j).Value = bCheck
                        End If
                End Select
            Next
        Next
    End Sub

End Class
