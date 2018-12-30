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
Imports EwEPlugin
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Auto-run UI item engine. This engine creates a hierarchy of 
    ''' <see cref="ucAutorunOption"/> controls that reflect the various
    ''' components in EwE that support auto-run functionality.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Class cAutoRunItemEngine
        Implements IDisposable

#Region " Private vars "

        Private m_pl As Panel = Nothing
        Private m_cbh As cCheckboxHierarchy = Nothing
        Private m_lControls As List(Of ucAutorunOption) = Nothing

#End Region ' Private vars

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class to sort plug-ins by name.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cPluginSorter
            Implements IComparer(Of IAutoRunPlugin)

            Public Function Compare(x As IAutoRunPlugin, y As IAutoRunPlugin) As Integer _
                Implements IComparer(Of IAutoRunPlugin).Compare
                Return String.Compare(x.Name, y.Name)
            End Function

        End Class

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext"/> to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext)
            Me.UIContext = uic
            Me.m_lControls = New List(Of ucAutorunOption)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disposal.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Detach()
            Me.m_cbh.Dispose()
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach the engine to a <see cref="Panel"/>. This will create the
        ''' auto-run control hierarchy. Do not forget to call <see cref="Detach"/> 
        ''' to clean up.
        ''' </summary>
        ''' <param name="pl">The <see cref="Panel"/> to create the control
        ''' hierarchy into.</param>
        ''' -------------------------------------------------------------------
        Public Sub Attach(pl As Panel)

            ' Store panel ref
            Me.m_pl = pl

            Dim core As cCore = Me.UIContext.Core
            Dim pm As cPluginManager = core.PluginManager
            Dim lPlugins([Enum].GetValues(GetType(eCoreComponentType)).Length - 1) As List(Of IAutoRunPlugin)

            ' Build lists of auto-saving plug-ins, per type
            For i As Integer = 0 To lPlugins.Length - 1
                lPlugins(i) = New List(Of IAutoRunPlugin)
            Next

            ' Make inventory of autorun plug-ins
            If (pm IsNot Nothing) Then
                For Each pi As IPlugin In pm.GetPlugins(GetType(IAutoRunPlugin))
                    Dim aspi As IAutoRunPlugin = DirectCast(pi, IAutoRunPlugin)
                    For Each cc As eCoreComponentType In aspi.AutoRunTypes
                        lPlugins(cc).Add(aspi)
                    Next cc
                Next pi
            End If

            ' Build control tree
            Me.m_pl.SuspendLayout()
            Try
                Me.BuildControlTree(eCoreComponentType.NotSet, Nothing, 0, lPlugins)
            Catch ex As Exception
                ' Whoah!
            End Try
            Me.m_pl.ResumeLayout()

            ' Start!
            Me.m_cbh.ManageCheckedStates = True

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Detach the engine from the UI.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Detach()

            Me.m_pl.SuspendLayout()
            For Each uc As ucAutorunOption In Me.m_lControls
                Me.m_pl.Controls.Remove(uc)
            Next
            Me.m_pl.ResumeLayout()
            Me.m_pl = Nothing

            Me.m_lControls.Clear()
            Me.m_cbh.Dispose()
            Me.m_cbh = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply control changes to the underlying components.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Apply()
            For Each uc As ucAutorunOption In Me.m_lControls
                uc.Apply()
            Next
        End Sub

#End Region ' Public access

#Region " Internals "

        Private ReadOnly Property UIContext As cUIContext = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recursive core to build the hierarchy control structure.
        ''' </summary>
        ''' <param name="t"><see cref="eCoreComponentType"/> to build a node for.</param>
        ''' <param name="cbParent">Parent checkbox, if any.</param>
        ''' <param name="iIndent">Control indentation.</param>
        ''' <param name="lPlugins">2-dimensional list of autorunning plug-ins.</param>
        ''' -------------------------------------------------------------------
        Private Sub BuildControlTree(ByVal t As eCoreComponentType,
                                     ByVal cbParent As CheckBox,
                                     ByVal iIndent As Integer,
                                     ByVal lPlugins() As List(Of IAutoRunPlugin))

            Dim core As cCore = Me.UIContext.Core
            Dim ctrl As ucAutorunOption = Nothing

            Select Case t
                Case eCoreComponentType.NotSet
                    ctrl = New ucAutorunOption(Me.UIContext, SharedResources.AUTORUN_ALL, 0)
                    Me.Add(ctrl, Nothing)
                    Dim cbRoot As CheckBox = ctrl.Checkbox

                    ctrl = New ucAutorunOption(Me.UIContext, SharedResources.HEADER_ECOPATH, 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eCoreComponentType.EcoPath, ctrl.Checkbox, 2, lPlugins)

                    ctrl = New ucAutorunOption(Me.UIContext, SharedResources.HEADER_ECOSIM, 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eCoreComponentType.EcoSim, ctrl.Checkbox, 2, lPlugins)

                    ctrl = New ucAutorunOption(Me.UIContext, SharedResources.HEADER_MONTECARLO, 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eCoreComponentType.EcoSimMonteCarlo, ctrl.Checkbox, 2, lPlugins)

                    ctrl = New ucAutorunOption(Me.UIContext, SharedResources.HEADER_ECOSPACE, 1)
                    Me.Add(ctrl, cbRoot)
                    Me.BuildControlTree(eCoreComponentType.EcoSpace, ctrl.Checkbox, 2, lPlugins)

                Case Else
                    Me.Add(lPlugins(t), cbParent, t, iIndent)

            End Select

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a <see cref="ucAutosaveOption"/> control to the managed panel.
        ''' </summary>
        ''' <param name="uc">The control to add.</param>
        ''' <param name="parent">The parent checkbox for this control, if any.</param>
        ''' -------------------------------------------------------------------
        Private Sub Add(ByVal uc As ucAutorunOption, ByVal parent As CheckBox)

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add controls for a list of plug-ins.
        ''' </summary>
        ''' <param name="l"></param>
        ''' <param name="parent"></param>
        ''' <param name="iIndent"></param>
        ''' -------------------------------------------------------------------
        Private Sub Add(ByVal l As List(Of IAutoRunPlugin),
                        ByVal parent As CheckBox,
                        ByVal cc As eCoreComponentType,
                        ByVal iIndent As Integer)

            Dim api As IAutoRunPlugin() = l.ToArray
            Array.Sort(api, New cPluginSorter())
            For Each pi As IAutoRunPlugin In api
                Me.Add(New ucAutorunOption(Me.UIContext, pi, cc, iIndent), parent)
            Next

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Other
