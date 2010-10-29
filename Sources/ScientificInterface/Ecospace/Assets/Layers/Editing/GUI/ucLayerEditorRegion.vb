#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Layer editor interface for editing a regions layer.
    ''' </summary>
    ''' =======================================================================
    Public Class ucLayerEditorRegion

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            Try
                If bDisposing Then
                    If (Me.UIContext Is Nothing) Then Return
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(bDisposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.UIContext.Core
            Dim reg As cEcospaceRegion = Nothing

            Me.m_cmbRegion.Items.Clear()
            ' Add 'None' item to allow users to clear regions
            Me.m_cmbRegion.Items.Add(New cCoreInputOutputControlItem(SharedResources.HEADER_NONE))
            ' Add region items
            For iReg As Integer = 1 To core.nRegions
                reg = core.EcospaceRegions(iReg)
                Me.m_cmbRegion.Items.Add(New cCoreInputOutputControlItem(reg))
            Next

            If (core.nRegions = 0) Then
                Me.m_cmbRegion.Enabled = False
            Else
                Me.m_cmbRegion.Enabled = True
                Me.m_cmbRegion.SelectedIndex = 0
            End If

        End Sub

        Private Sub m_cmbRegion_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_cmbRegion.SelectedIndexChanged

            Dim item As cCoreInputOutputControlItem = DirectCast(Me.m_cmbRegion.SelectedItem, cCoreInputOutputControlItem)
            Dim src As cCoreInputOutputBase = Nothing

            If (item IsNot Nothing) Then
                src = item.Source
                If src Is Nothing Then
                    Me.Editor.CellValue = 0
                Else
                    Me.Editor.CellValue = src.Index
                End If
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace
