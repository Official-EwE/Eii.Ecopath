'==============================================================================
'
' $Log: frmApplyShapeBase.vb,v $
' Revision 1.3  2009/02/05 17:48:36  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.2  2009/01/16 18:30:39  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/12/15 19:54:04  jeroens
' *** empty log message ***
'
' Revision 1.2  2008/12/15 15:58:48  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/06/06 16:01:38  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.2  2008/06/02 00:01:30  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2008/05/23 15:54:36  jeroens
' Moved
'
' Revision 1.1  2008/01/22 02:41:40  jeroens
' Properly fixed grid apply mode
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class frmApplyShapeBase
        Inherits frmEwE

        Private m_ApplyShapeGrid As ApplyShapeEwEGrid = Nothing

        Public Sub New()
            Me.m_ApplyShapeGrid = New ApplyShapeEwEGrid(Me.ApplyShapeMode, Me.ApplyTargetMode)
        End Sub

#Region " Event handlers "

        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Hook up to core messages
            ' * Shapes manager to refresh lists of avialable FFs
            ' * Ecopath to refresh lists of available groups
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager, eCoreComponentType.EcoPath, eCoreComponentType.PPIManager}
        End Sub

        Private Sub DoDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            ' Release core messages
            Me.CoreComponents = Nothing
        End Sub

        Protected Sub ClearAllPairs()
            Me.Grid.ClearAllPairs()
        End Sub

        Protected Sub SetAllPairs()
            Me.Grid.SetAllPairs()
        End Sub

#End Region ' Event handlers

#Region " Mandatory overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)

            Dim bMustRedimension As Boolean = False
            Dim bMustUpdate As Boolean = False

            If (msg.Source = eCoreComponentType.ShapesManager) Then
                If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                    ' Redimension when number of shapes has changed
                    bMustRedimension = True
                End If
            End If

            ' Refresh when Ecopath number of groups has changed
            If (msg.Source = eCoreComponentType.EcoPath) Then
                If ((msg.Type = eMessageType.DataAddedOrRemoved) And (msg.DataType = eDataTypes.EcoPathGroupInput)) Then
                    bMustRedimension = True
                ElseIf (msg.Type = eMessageType.DietComp) Then
                    bMustUpdate = True
                End If
            End If

            If (msg.Source = eCoreComponentType.PPIManager) Then
                ' Update content to show new assignments
                bMustUpdate = True
            End If

            If bMustRedimension Then
                Me.Grid.RefreshContent()
            Else
                If bMustUpdate Then
                    Me.Grid.UpdateContent()
                End If
            End If
        End Sub

        Protected Overridable Function ApplyTargetMode() As eApplyTargetTypes
            Return eApplyTargetTypes.NotSet
        End Function

        Protected Overridable Function ApplyShapeMode() As eApplyShapeTypes
            Return eApplyShapeTypes.NotSet
        End Function

        Protected Function Grid() As ApplyShapeEwEGrid
            Return Me.m_ApplyShapeGrid
        End Function

#End Region ' Mandatory overrides

    End Class

End Namespace
