'==============================================================================
'
' $Log: frmNetworkMain.vb,v $
' Revision 1.1  2008/09/26 07:30:57  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2007/06/20 18:13:57  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Imports EwECore

Public Class frmNetworkMain

    ''' <summary>
    ''' Network Manager provides an interface to all the Network analysis methods. 
    ''' Its life span and state are handled by the Plugin because it handles all comumication with the core
    ''' </summary>
    ''' <remarks></remarks>
    Private WithEvents NetworkManager As cNetworkManager


    Public Sub New(ByRef theNetworkManager As cNetworkManager)
        Me.InitializeComponent()

        NetworkManager = theNetworkManager

    End Sub


    Private Sub btTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btTest.Click

        Me.prgProgress.Maximum = 5000

        'Me.lbTestMessage.Text = "Finding Cycles and Pathways"
        'Me.lbTestMessage.Refresh()
        'Me.prgProgress.Value = 0

        'NetworkManager.Test()

        Me.prgProgress.Value = 0

        NetworkManager.RunRequiredPrimaryProd()
        Me.prgProgress.Value = 0

  

        ''write the pathways to the console window
        'For Each pathway As String In NetworkManager.PathWays
        '    System.Console.WriteLine(pathway)
        'Next


    End Sub

    Private Sub NetworkManager_CalculateRequiredPPProgress(ByVal nPaths As Integer) Handles NetworkManager.CalculateRequiredPPProgress

        If Me.prgProgress.Value < Me.prgProgress.Maximum Then
            Me.prgProgress.Value += 1
        Else
            Me.prgProgress.Value = 1
        End If

        Me.lbTestMessage.Text = "CalculateRequiredPP Number of paths = " & nPaths
        Me.lbTestMessage.Refresh()

    End Sub

    Private Sub NetworkManager_CycleFound(ByVal iCycle As Integer) Handles NetworkManager.CycleFound
        Me.prgProgress.Value = 1
        Me.lbTestMessage.Text = "Cycles = " & iCycle
        Me.lbTestMessage.Refresh()
    End Sub

 

    Private Sub NetworkManager_RunMainNetworkProgress(ByVal iProgress As Integer) Handles NetworkManager.RunMainNetworkProgress

        If Me.prgProgress.Value < Me.prgProgress.Maximum Then
            Me.prgProgress.Value += 1
        Else
            Me.prgProgress.Value = 1
        End If

        Me.lbTestMessage.Text = "RunMainNetwork = " & iProgress
        Me.lbTestMessage.Refresh()


    End Sub


    Private Sub frmNetworkMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub NetworkManager_FindPathwaysProgress(ByVal iPath As Integer) Handles NetworkManager.FindPathwaysProgress
        Me.lbTestMessage.Text = "FindPathways = " & iPath
        Me.lbTestMessage.Refresh()
    End Sub
End Class