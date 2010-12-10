#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecosim Vulnerabilities interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmVulnerabilities

#Region " Private vars "

        Private m_cmdEstimateVs As cCommand = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            MyBase.New(New VulnerabilitiesEwEGrid())
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Overloads "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Me.m_cmdEstimateVs = Me.CommandHandler.GetCommand("EstimateVs")
            If (Me.m_cmdEstimateVs IsNot Nothing) Then
                Me.m_cmdEstimateVs.AddControl(Me.m_tsbEstimateVs)
            End If

#If Not Debug Then
            ' Remove estimate V's from release version while under development
            Me.m_tsbEstimateVs.Visible = False
#End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            If (Me.m_cmdEstimateVs IsNot Nothing) Then
                Me.m_cmdEstimateVs.RemoveControl(Me.m_tsbEstimateVs)
            End If
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Overloads

    End Class

End Namespace

