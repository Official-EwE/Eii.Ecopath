#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim

#End Region ' Imports

Public Class frmMSEAssessFleets

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As cUIContext)
            MyBase.UIContext = value

            If (value IsNot Nothing) Then
                Me.GridFishingCV1.UIContext = value
                Me.m_blocks.UIContext = value
                Me.m_blocks.ParmBlockCodes.NumBlocks = Me.UIContext.Core.nFleets
                Me.m_blocks.ParmBlockCodes.SelectedBlock = 1
            End If

        End Set
    End Property

End Class