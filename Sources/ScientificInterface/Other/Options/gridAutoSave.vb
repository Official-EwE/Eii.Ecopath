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
#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Core
Imports SourceGrid2

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Grid that blends autosaving plug-ins with the EwE autosave UI. Not ready for
    ''' deployment yet - in progress.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridAutoSave
        Inherits EwEGrid

        Private Const c_iIndentSize As Integer = 20

        Private m_autosaveplugins([Enum].GetValues(GetType(eAutosaveTypes)).Length - 1) As List(Of IAutoSavePlugin)
        Private m_bInit As Boolean = False
        Private m_strBasePath As String = ""

        Private Enum eColumnTypes As Integer
            Check
            Name
            Path
        End Enum

        Public Sub New()
            MyBase.New()
            For Each t As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
                Me.m_autosaveplugins(t) = New List(Of IAutoSavePlugin)
            Next
            Throw New NotImplementedException("gridAutoSave not ready to be used yet")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Selection.SelectionMode = GridSelectionMode.Row
            Me.Selection.EnableMultiSelection = False
            Me.FixedColumnWidths = False

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            ' Fleet index cell
            Me(0, eColumnTypes.Check) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell("Name")
            Me(0, eColumnTypes.Path) = New EwEColumnHeaderCell("Location")

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return

            If Me.m_bInit = False Then
                Me.BuildPluginList()
                Me.m_bInit = True
            End If

            Me.AddParentRow("Auto-save all", 0)
            Me.AddAutosaveTypeRow(eAutosaveTypes.Ecopath, "Ecopath", 1)
            Me.AddParentRow("Ecosim", 1)
            Me.AddAutosaveTypeRow(eAutosaveTypes.Ecosim, "Ecosim run results", 2)
            Me.AddAutosaveTypeRow(eAutosaveTypes.MonteCarlo, "Monte Carlo", 2)
            Me.AddAutosaveTypeRow(eAutosaveTypes.MSE, "MSE", 2)
            Me.AddParentRow("Ecospace", 1)
            Me.AddAutosaveTypeRow(eAutosaveTypes.EcospaceASC, "Ecospace ASC", 2)
            Me.AddAutosaveTypeRow(eAutosaveTypes.EcospaceCSV, "Ecospace CSV", 2)
            Me.AddAutosaveTypeRow(eAutosaveTypes.Ecotracer, "Ecortacer", 1)
            Me.AddPluginRows(eAutosaveTypes.NotSet, 1)

            Me.AutoSizeColumn(eColumnTypes.Name, 100)

        End Sub

        Public Property BasePath As String
            Get
                Return Me.m_strBasePath
            End Get
            Set(value As String)
                Me.m_strBasePath = value
                Me.UpdatePathColumn()
            End Set
        End Property

        Private Sub UpdatePathColumn()
            For iRow As Integer = 1 To Me.RowsCount - 1
                Dim obj As Object = Me.Rows(iRow).Tag
                Dim strPath As String = ""
                If (obj IsNot Nothing) Then
                    If TypeOf obj Is IAutoSavePlugin Then
                        Dim aspi As IAutoSavePlugin = DirectCast(obj, IAutoSavePlugin)
                        strPath = IO.Path.Combine(Core.DefaultOutputPath(aspi.AutoSaveType, Me.m_strBasePath, True), aspi.AutoSaveSubPath)
                    Else
                        strPath = Core.DefaultOutputPath(DirectCast(obj, eAutosaveTypes), Me.m_strBasePath, True)
                    End If
                End If
                Me(iRow, eColumnTypes.Path).Value = strPath
            Next
            Me.AutoSizeColumn(eColumnTypes.Path, 100)
        End Sub

        Private Function AddParentRow(strLabel As String, iIndent As Integer) As Integer

            Return Me.AddAutosaveRow(strLabel, iIndent, Nothing)

        End Function

        Private Function AddAutosaveTypeRow(t As eAutosaveTypes, strLabel As String, iIndent As Integer) As Integer

            Dim iRow As Integer = Me.AddAutosaveRow(strLabel, iIndent, t)
            Me.AddPluginRows(t, iIndent + 1)
            Return iRow

        End Function

        Private Sub AddPluginRows(t As eAutosaveTypes, iIndent As Integer)
            For Each aspi As IAutoSavePlugin In Me.m_autosaveplugins(t)
                Me.AddAutosaveRow(aspi.AutoSaveName, iIndent, aspi)
            Next
        End Sub

        Private Function AddAutosaveRow(strLabel As String, iIndent As Integer, tag As Object) As Integer
            Dim iRow As Integer = Me.AddRow()
            Dim viz As New cEwEGridRowHeaderVisualizer()
            viz.Indentation = iIndent * c_iIndentSize
            Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(strLabel)
            Me(iRow, eColumnTypes.Name).VisualModel = viz
            Me(iRow, eColumnTypes.Check) = New EwECheckboxCell(False)
            Me(iRow, eColumnTypes.Path) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.Path).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Me.Rows(iRow).Tag = tag
            Return iRow
        End Function

        Private Sub BuildPluginList()

            Debug.Assert(Me.Core IsNot Nothing)

            Dim pm As cPluginManager = Me.Core.PluginManager
            For Each pi As IPlugin In pm.GetPlugins(GetType(IAutoSavePlugin))
                Dim aspi As IAutoSavePlugin = DirectCast(pi, IAutoSavePlugin)
                Me.m_autosaveplugins(aspi.AutoSaveType).Add(aspi)
            Next pi

        End Sub

        Private Sub ClearPluginList()
            For i As Integer = 0 To Me.m_autosaveplugins.Length - 1
                Me.m_autosaveplugins(i).Clear()
            Next
        End Sub

    End Class

End Namespace
