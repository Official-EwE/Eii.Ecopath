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

Imports System.IO
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for reflecting an autosave option in the 
    ''' <see cref="ucOptionsFileManagement"/> interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Class ucAutorunOption
        Implements IUIElement

        Private Const cINDENT_SIZE As Integer = 18
        Private m_autoruntype As eCoreComponentType = eCoreComponentType.NotSet
        Private m_iIndent As Integer = 0
        Private m_pi As IAutoRunPlugin = Nothing

#Region " Construction / destruction "

        Friend Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an autosave parent item. This item is not associated with
        ''' an <see cref="eAutosaveTypes"/> value.
        ''' </summary>
        ''' <param name="uic">UI Context to connect to the item.</param>
        ''' <param name="strLabel">Label to use for the item.</param>
        ''' <param name="iIndent">Checkbox indentation to use.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext,
                       ByVal strLabel As String,
                       ByVal iIndent As Integer)

            MyBase.New()
            Me.InitializeComponent()

            Me.UIContext = uic
            Me.m_autoruntype = eCoreComponentType.NotSet
            Me.m_iIndent = iIndent
            Me.m_cbOption.Text = strLabel

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an autosave item associated with an <see cref="eAutosaveTypes"/> value.
        ''' </summary>
        ''' <param name="uic">UI Context to connect to the item.</param>
        ''' <param name="autoruntype">The <see cref="eAutosaveTypes"/> value to
        ''' associate the item with.</param>
        ''' <param name="iIndent">Checkbox indentation to use.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext,
                       ByVal autoruntype As eCoreComponentType,
                       ByVal iIndent As Integer)
            Me.New()

            Me.UIContext = uic
            Me.m_autoruntype = autoruntype
            Me.m_iIndent = iIndent

            Dim fmt As New cCoreComponentTypeFormatter()
            Me.m_cbOption.Text = fmt.GetDescriptor(Me.m_autoruntype, eDescriptorTypes.Name)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an autosave item associated with a plug-in.
        ''' </summary>
        ''' <param name="uic">UI Context to connect to the item.</param>
        ''' <param name="pi"><see cref="IAutoSavePlugin"/> to associate the 
        ''' item with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext,
                       ByVal pi As IAutoRunPlugin,
                       ByVal type As eCoreComponentType,
                       ByVal iIndent As Integer)
            Me.New()

            Me.UIContext = uic
            Me.m_pi = pi
            Me.m_autoruntype = type
            Me.m_cbOption.Text = pi.DisplayName
            Me.m_iIndent = iIndent

        End Sub

        Private Property UIContext As cUIContext = Nothing Implements IUIElement.UIContext

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    Me.UIContext = Nothing
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Public interfaces "

        Public Function Checkbox() As CheckBox
            Return Me.m_cbOption
        End Function

        Public Sub Apply()

            Try
                ' Represents a stock writer or a plug-in?
                If (Me.m_pi IsNot Nothing) Then
                    Me.m_pi.AutoRun(Me.m_autoruntype) = (Me.m_cbOption.Checked = True)
                End If
            Catch ex As Exception
                ' Whoah!
                cLog.Write(ex, "ucAutoRunOption.Apply(" & Me.m_autoruntype & ")")
            End Try

        End Sub

#End Region ' Public interfaces

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Apply indentation
            If cSystemUtils.IsRightToLeft Then
                Me.m_cbOption.Location = New Point(Me.m_cbOption.Location.X - Me.m_iIndent * cINDENT_SIZE, Me.m_cbOption.Location.Y)
            Else
                Me.m_cbOption.Location = New Point(Me.m_cbOption.Location.X + Me.m_iIndent * cINDENT_SIZE, Me.m_cbOption.Location.Y)
            End If
            Me.m_cbOption.Width -= Me.m_iIndent * 20

            ' Set initial state
            If (Me.m_pi IsNot Nothing) Then
                Me.m_cbOption.Checked = Me.m_pi.AutoRun(Me.m_autoruntype)
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace
