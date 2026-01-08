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

Option Explicit On
Option Strict On

Imports EwECore
Imports EwECore.Common

#End Region

Namespace Ecospace

    Public Class dlgDefineEffortZones
        Implements IUIElement

        Private m_uic As cUIContext = Nothing

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            Me.m_nudNoZones.Value = parms.nEffortZones
            Me.CenterToScreen()

        End Sub

#Region " Events "

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOK.Click

            Me.ChangeNumEffortZones()
            Me.Close()

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCancel.Click
            Try
                Me.Close()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub ChangeNumEffortZones()

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim regions As cEcospaceLayerEffortZone = bm.LayerEffortZone
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            Dim nZones As Integer = CInt(Me.m_nudNoZones.Value)
            Dim iMaxReg As Integer = 0

            For ir As Integer = 1 To bm.InRow
                For ic As Integer = 1 To bm.InCol
                    If bm.IsModelledCell(ir, ic) Then
                        iMaxReg = Math.Max(iMaxReg, CInt(regions.Cell(ir, ic)))
                    End If
                Next
            Next

            If (nZones < iMaxReg) Then
                ' ToDo: globalize this
                Dim fmsg As New cFeedbackMessage("There are cells that will no longer be assigned to effort distribution zones if you continue.",
                                                 eCoreComponentType.Ecospace, eMessageType.Any, eMessageImportance.Question,
                                                 eMessageReplyStyle.OK_CANCEL, eDataTypes.NotSet, eMessageReply.CANCEL)
                fmsg.Suppressable = True
                Me.m_uic.Core.Messages.SendMessage(fmsg)
                If (fmsg.Reply <> eMessageReply.OK) Then Return
            End If

            parms.nEffortZones = nZones

        End Sub

#End Region ' Internals

    End Class

End Namespace
