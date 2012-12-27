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

#End Region ' Imports

Namespace Other

    Friend Class cPluginSorter
        Implements IComparer(Of IAutoSavePlugin)

        Public Function Compare(x As EwEPlugin.IAutoSavePlugin, _
                                y As EwEPlugin.IAutoSavePlugin) As Integer _
                            Implements IComparer(Of EwEPlugin.IAutoSavePlugin).Compare
            Return String.Compare(x.Name, y.Name)
        End Function

    End Class

    Friend Class cAutoSaveItemEngine
        Implements IDisposable

        Private m_uic As cUIContext = Nothing
        Private m_pl As Panel = Nothing
        Private m_cbh As cCheckboxHierarchy = Nothing
        Private m_lControls As List(Of ucAutosaveOption) = Nothing

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.m_lControls = New List(Of ucAutosaveOption)
        End Sub

        Public Sub Attach(pl As Panel)

            Me.m_pl = pl

            Dim core As cCore = Me.m_uic.Core
            Dim pm As cPluginManager = core.PluginManager
            Dim lPlugins([Enum].GetValues(GetType(eAutosaveTypes)).Length - 1) As List(Of IAutoSavePlugin)

            For Each t As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
                lPlugins(t) = New List(Of IAutoSavePlugin)
            Next

            ' Make inventory of autosave plug-ins
            If (pm IsNot Nothing) Then
                For Each pi As IPlugin In pm.GetPlugins(GetType(IAutoSavePlugin))
                    Dim aspi As IAutoSavePlugin = DirectCast(pi, IAutoSavePlugin)
                    lPlugins(aspi.AutoSaveType).Add(aspi)
                Next pi
            End If
            Me.BuildControlTree(eAutosaveTypes.NotSet, Nothing, 0, lPlugins)
            Me.m_cbh.ManageCheckedStates = True

        End Sub

        Public Sub Apply()
            For Each uc As ucAutosaveOption In Me.m_lControls
                uc.Apply()
            Next
        End Sub

        Public Sub Detach()
            Me.m_pl.SuspendLayout()
            For Each uc As ucAutosaveOption In Me.m_lControls
                Me.m_pl.Controls.Remove(uc)
            Next
            Me.m_lControls.Clear()
            Me.m_pl.ResumeLayout()
            Me.m_pl = Nothing
            Me.m_cbh.Dispose()
            Me.m_cbh = Nothing

        End Sub

        Public Sub SetOutputMask(ByVal strMask As String)
            For Each uc As ucAutosaveOption In Me.m_lControls
                uc.SetOutputMask(strMask)
            Next
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Detach()
            Me.m_cbh.Dispose()
            GC.SuppressFinalize(Me)
        End Sub

        Private Sub BuildControlTree(ByVal t As eAutosaveTypes, _
                                     ByVal parent As CheckBox, _
                                     ByVal iIndent As Integer, _
                                     ByVal lPlugins() As List(Of IAutoSavePlugin))

            Dim cbParent As CheckBox = Nothing
            Dim ctrl As ucAutosaveOption = Nothing

            ' ToDo: globalize this method

            Select Case t
                Case eAutosaveTypes.NotSet
                    ctrl = New ucAutosaveOption(Me.m_uic, "Auto-save all", 0)
                    Me.Add(ctrl, Nothing)
                    Dim cbRoot As CheckBox = ctrl.Checkbox

                    ctrl = New ucAutosaveOption(Me.m_uic, "Ecopath", 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eAutosaveTypes.Ecopath, ctrl.Checkbox, 2, lPlugins)

                    ctrl = New ucAutosaveOption(Me.m_uic, "Ecosim", 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eAutosaveTypes.Ecosim, ctrl.Checkbox, 2, lPlugins)

                    ctrl = New ucAutosaveOption(Me.m_uic, "Ecospace", 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eAutosaveTypes.Ecospace, ctrl.Checkbox, 2, lPlugins)

                    Me.BuildControlTree(eAutosaveTypes.Ecotracer, ctrl.Checkbox, 1, lPlugins)

                Case eAutosaveTypes.Ecopath
                    Me.Add(lPlugins(t), parent, iIndent)

                Case eAutosaveTypes.Ecosim
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)
                    Me.BuildControlTree(eAutosaveTypes.MonteCarlo, ctrl.Checkbox, iIndent, lPlugins)
                    Me.BuildControlTree(eAutosaveTypes.MSE, ctrl.Checkbox, iIndent, lPlugins)
                    Me.BuildControlTree(eAutosaveTypes.MSY, ctrl.Checkbox, iIndent, lPlugins)
                    Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                Case eAutosaveTypes.Ecospace
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)
                    Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                Case eAutosaveTypes.Ecotracer
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)

                Case eAutosaveTypes.MonteCarlo
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)
                    Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                Case eAutosaveTypes.MSY
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)
                    Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

                Case eAutosaveTypes.MSE
                    ctrl = New ucAutosaveOption(Me.m_uic, t, iIndent)
                    Me.Add(ctrl, parent)
                    Me.Add(lPlugins(t), ctrl.Checkbox, iIndent)

            End Select
        End Sub

        Private Sub Add(ByVal uc As ucAutosaveOption, ByVal parent As CheckBox)
            Me.m_pl.Controls.Add(uc)
            uc.Location = New Point(0, (Me.m_pl.Controls.Count - 1) * uc.Height)
            uc.Width = Me.m_pl.Width
            uc.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top

            If (parent IsNot Nothing) Then
                Me.m_cbh.Add(uc.Checkbox, parent)
            Else
                Me.m_cbh = New cCheckboxHierarchy(uc.Checkbox)
            End If

            Me.m_lControls.Add(uc)
        End Sub

        Private Sub Add(ByVal l As List(Of IAutoSavePlugin), _
                        ByVal parent As CheckBox, _
                        ByVal iIndent As Integer)

            Dim api As IAutoSavePlugin() = l.ToArray
            Array.Sort(api, New cPluginSorter())
            For Each pi As IAutoSavePlugin In api
                Me.Add(New ucAutosaveOption(Me.m_uic, pi), parent)
            Next

        End Sub

    End Class

End Namespace ' Other
