#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form baseclass for implementing an Ecosim 'Apply Forcing' or 'Apply 
    ''' Mediation' interface.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
     Public Class frmApplyShapeBase
        Inherits frmEwE

        Private m_ApplyShapeGrid As ApplyShapeEwEGrid = Nothing

        Public Sub New()
            Me.m_ApplyShapeGrid = New ApplyShapeEwEGrid(Me.ApplyShapeMode, Me.ApplyTargetMode)
        End Sub

#Region " Baseclass overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            ' Hook up to core messages
            ' * Shapes manager to refresh lists of avialable FFs
            ' * Ecopath to refresh lists of available groups
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager, eCoreComponentType.EcoPath, eCoreComponentType.PPIManager}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            MyBase.OnFormClosed(e)
            ' Release core messages
            Me.CoreComponents = Nothing
        End Sub

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ApplyShapeGrid.UIContext = value
            End Set
        End Property

#End Region ' Baseclass overrides

#Region " Base functionality "

        Protected Sub ClearAllPairs()
            Me.Grid.ClearAllPairs()
        End Sub

        Protected Sub SetAllPairs()
            Me.Grid.SetAllPairs()
        End Sub

#End Region ' Base functionality

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
