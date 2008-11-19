'==============================================================================
'
' $Log: dlgSensitivityOfSStoV.vb,v $
' Revision 1.1  2008/11/19 14:40:54  jeroens
' Moved and renamed
'
' Revision 1.2  2008/10/07 18:34:39  villyc
' updating a vulmult pred-prey swap
'
' Revision 1.1  2008/09/26 07:31:53  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.13  2008/07/02 22:07:52  jeroens
' Update ran after search
'
' Revision 1.12  2008/03/20 19:01:46  joeb
' SetDefaultVulnerabilities() was not setting Detritus
'
' Revision 1.11  2008/03/20 18:35:13  joeb
' Fixed bug Detritus not being included in fit when set from interface
' Removed dead code
'
' Revision 1.10  2008/01/26 00:18:09  joeb
' Move binning of sensitivity to manager/model
'
' Revision 1.9  2008/01/24 17:26:22  jeroens
' Restyled
'
' Revision 1.8  2007/11/30 20:50:34  joeb
' Change the way the binning of the colour works,
' This may need to change again once it's clear what the intent of the display is
'
' Revision 1.7  2007/11/30 16:46:38  joeb
' Fixed indexing bug in progressbar
'
' Revision 1.6  2007/11/11 16:52:47  jeroens
' * Updated to new block selector logic
'
' Revision 1.5  2007/11/08 18:19:13  jeroens
' + Implemented transfer methods
'
' Revision 1.4  2007/11/08 08:02:57  jeroens
' * Getting there
'
' Revision 1.3  2007/11/08 00:08:53  jeroens
' * Fixed sync object screw-up
' * Dialog controlled and kept up to date by form
'
' Revision 1.2  2007/11/06 20:32:35  jeroens
' ~ To appease the masses
'
' Revision 1.1  2007/11/06 17:59:19  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Drawing
Imports EwECore
Imports EwECore.FitToTimeSeries
Imports ScientificInterface.Other

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class dlgSensitivityOfSStoV

#Region " Private variables "

    Private m_core As cCore = Nothing
    Private m_SSPreyPred(,) As Single ' Sen by pred/prey
    Private m_iNumBlocks As Integer
    Private m_F2TSManager As cF2TSManager = Nothing
    Private m_SSbase As Single = 0.0
    Private m_runType As eRunType = eRunType.Idle
    Private m_runResultType As eRunType = eRunType.Idle

#End Region ' Private variables

#Region " Constructors "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub New(ByVal core As cCore, ByVal manager As cF2TSManager)
        Me.InitializeComponent()

        ' Sanity checks
        Debug.Assert(core IsNot Nothing)
        Debug.Assert(manager IsNot Nothing)

        Me.m_core = core
        Me.m_F2TSManager = manager

        ReDim Me.m_SSPreyPred(core.nGroups, core.nGroups)

        Me.m_ucVulBlocks.Init(core)
    End Sub

#End Region ' Constructors

#Region " Events "

#Region " Form "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub dlgSensitivityOfSStoV_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.UpdateControls()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clean-up
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub dlgSensitivityOfSStoV_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Me.m_F2TSManager = Nothing
    End Sub

#End Region ' Form

#Region " Controls "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnSearchCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbSearchPred.CheckedChanged, m_rbSearchPredPrey.CheckedChanged
        '
        Me.UpdateControls()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub OnTransferCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbTransferPredCol.CheckedChanged, m_rbTransferPredPreyCell.CheckedChanged, m_rbSearchPred.CheckedChanged
        '
        Me.UpdateControls()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub m_btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnSearch.Click
        Me.StartRun()
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOk.Click

        If (Me.StopRun() = False) Then Return
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub Cancel_button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click

        If (Me.StopRun() = False) Then Return
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub m_btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnUpdate.Click

        Me.UpdateDisplay()

    End Sub

#End Region ' Controls

#Region " F2TS manager interface "

    Public Sub OnRunStarted(ByVal runType As eRunType, ByVal nSteps As Integer)
        ' Sanity check
        Debug.Assert(runType = Me.m_runType)

        Console.WriteLine("Dlg: run started " & runType)

        Me.m_pbSearch.Maximum = nSteps
        Me.m_pbSearch.Visible = True
        Me.m_btnSearch.Enabled = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnRunStep(ByVal runType As eRunType, ByVal iPred As Integer, ByVal iPrey As Integer, ByVal sSen As Single)

        ' Sanity check
        Debug.Assert(runType = Me.m_runType)

        Select Case runType

            Case eRunType.SensitivitySS2VByPredPrey
                ' Keep the ss for this prey pred for later use
                Me.m_SSPreyPred(iPrey, iPred) = sSen

            Case eRunType.SensitivitySS2VByPredator
                ' Keep the ss for this pred for later use
                For iPrey = 1 To Me.m_core.nGroups
                    Me.m_SSPreyPred(iPred, iPrey) = sSen
                Next

        End Select

        Me.m_pbSearch.Value += 1
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The model run has completed
    ''' </summary>
    ''' <param name="runType"></param>
    ''' -----------------------------------------------------------------------
    Public Sub OnRunStopped(ByVal runType As eRunType)

        ' Sanity check
        Debug.Assert(runType = Me.m_runType)
        Me.m_pbSearch.Visible = False
        Me.m_btnSearch.Enabled = True

        'write out the SS matrix that was collected in m_F2TSManager_OnRunStep
        For i As Integer = 1 To m_core.nGroups
            For j As Integer = 1 To m_core.nGroups
                System.Console.Write(Me.m_SSPreyPred(i, j).ToString & ", ")
            Next
            System.Console.WriteLine()
        Next

        Me.m_runResultType = runType
        Me.UpdateControls()
        Me.UpdateDisplay()

    End Sub

#End Region ' F2TS manager

#End Region ' Events

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumBlocks() As Integer
        Get
            Return Me.m_iNumBlocks
        End Get
        Set(ByVal value As Integer)
            Me.m_iNumBlocks = value

            ' Create some purdy working colours
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Me.m_ucVulBlocks.RefreshContent()
            Me.m_ucVulBlocks.BlockColors = sg.GetColorRamp(Me.m_iNumBlocks)
            Me.m_ucVulBlocks.BlockColors(0) = Color.Black

            Me.m_nudNumBlocks.Maximum = Me.m_iNumBlocks
            Me.m_nudNumBlocks.Value = Me.m_iNumBlocks
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property VulnerabilityBlocks() As Integer(,)
        Get
            Return Me.m_ucVulBlocks.Vulblocks
        End Get
    End Property

#End Region ' Public access

#Region " Internal implementation "

#Region "Sorting of sensitivities "

    'This code is no longer used
    'sorting is handled by the manager in cF2TSManager.setNBlocksFromSensitivity(nBlocks)
#If 0 Then


    Private Sub UpdateByPredPrey()
        Me.NumBlocks = CInt(Me.m_nudNumBlocks.Value)
        SortMostSensitiveLinks()
    End Sub

    Private Sub UpdateByPredatorColumn()
        Me.NumBlocks = CInt(Me.m_nudNumBlocks.Value)
        SortMostSensitiveLinksByPredators(Me.m_iNumBlocks, True)
    End Sub

    Private Sub UpdateByPreyRow()
        Me.NumBlocks = CInt(Me.m_nudNumBlocks.Value)
        SortMostSensitiveLinksByPredators(Me.m_iNumBlocks, False)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iNumMostSensitiveConsumers"></param>
    ''' <param name="blnPredator"></param>
    ''' -----------------------------------------------------------------------
    Private Sub SortMostSensitiveLinksByPredators(ByVal iNumMostSensitiveConsumers As Integer, ByVal blnPredator As Boolean)
        Dim MaxS(1) As Single
        Dim StoreSens(m_core.nGroups, m_core.nGroups) As Single
        Dim MaxSens(iNumMostSensitiveConsumers, 3) As Single
        Dim GrpSens(m_core.nGroups) As Single
        Dim Smax As Single

        'jb m_SSPreyPred(i,j) contains SS(i,j)-SSbase

        ' Determine SMax
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                'Smax = Math.Max(Smax, Math.Abs(m_SSPreyPred(iPred, iPrey) - m_SSbase))
                Smax = Math.Max(Smax, Math.Abs(m_SSPreyPred(iPred, iPrey)))
            Next iPrey
        Next iPred

        'Scale and temp store sensibilities
        'sum up sensibilities by consumers
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                StoreSens(iPred, iPrey) = Math.Abs(m_SSPreyPred(iPred, iPrey)) / Smax
                ' StoreSens(iPred, iPrey) = Math.Abs(m_SSPreyPred(iPred, iPrey) - m_SSbase) / Smax
                If blnPredator Then
                    GrpSens(iPred) += StoreSens(iPred, iPrey)
                Else
                    GrpSens(iPrey) += StoreSens(iPred, iPrey)
                End If
            Next iPrey
        Next iPred

        'Now find the 'intNumMostSensitiveConsumers' most important predators
        For k As Integer = iNumMostSensitiveConsumers To 1 Step -1
            MaxS(0) = -1
            'Find the max sensitivity
            For i As Integer = 1 To m_core.nLivingGroups 'NumLiving
                If GrpSens(i) > MaxS(0) Then
                    MaxS(0) = GrpSens(i)
                    MaxS(1) = i
                End If
            Next

            MaxSens(k, 3) = MaxS(1) '=grp 'i'
            GrpSens(CInt(MaxS(1))) = 0
        Next

        'jb I changed this
        'bin the sensitiviy to change in V into the number of blocks (variables to search) selected by the user
        'I think this is the intent of the EwE5 code in frmSearch.Command6_Click()
        Dim ibin As Integer
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups

                If Me.m_F2TSManager.isPredPrey(iPred, iPrey) Then
                    ibin = CInt(m_SSPreyPred(iPred, iPrey) / Smax * (NumBlocks - 1)) + 1
                    Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = ibin
                End If
            Next iPrey
        Next iPred


        ''We've got the info now, so transfer it to the sens form
        'For k As Integer = 1 To iNumMostSensitiveConsumers
        '    For iPred As Integer = 1 To Me.m_core.nGroups
        '        For iPrey As Integer = 1 To Me.m_core.nGroups
        '            ' Set to 0
        '            Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = 0
        '            ' Determine what to do next
        '            If blnPredator Then 'sort most sensitive links by predator column
        '                If iPred = MaxSens(k, 3) Then
        '                    'Only these columns of blocks will be changed to color code other than 0
        '                    Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = k

        '                End If
        '            Else 'sort most sensitive links by prey row
        '                If iPrey = MaxSens(k, 3) Then
        '                    'Only these rows of blocks will be changed to color code other than 0
        '                    Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = k
        '                End If
        '            End If
        '        Next iPrey
        '    Next iPred
        'Next k

        Me.m_ucVulBlocks.Invalidate()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub SortMostSensitiveLinks()
        Dim MaxS(2) As Single
        Dim StoreSens(Me.m_core.nGroups, Me.m_core.nGroups) As Single
        Dim MaxSens(Me.m_iNumBlocks, 4) As Single
        Dim Smax As Single

        ' Determine SMax
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                Smax = Math.Max(Smax, Math.Abs(m_SSPreyPred(iPred, iPrey)))
                '  Smax = Math.Max(Smax, Math.Abs(m_SSPreyPred(iPred, iPrey) - m_SSbase))
            Next iPrey
        Next iPred

        'Scale and temp store sensibilities
        'sum up sensibilities by consumers
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                StoreSens(iPred, iPrey) = Math.Abs(m_SSPreyPred(iPred, iPrey)) / Smax
                '  StoreSens(iPred, iPrey) = Math.Abs(m_SSPreyPred(iPred, iPrey) - m_SSbase) / Smax
            Next iPrey
        Next iPred

        For k As Integer = Me.m_iNumBlocks To 1 Step -1
            MaxS(0) = -1
            'Find the max sensitivity
            For iPred As Integer = 1 To Me.m_core.nGroups
                For iPrey As Integer = 1 To Me.m_core.nGroups
                    If StoreSens(iPred, iPrey) > MaxS(0) Then
                        MaxS(0) = StoreSens(iPred, iPred)
                        MaxS(1) = iPred
                        MaxS(2) = iPrey
                    End If
                Next
            Next
            'Now the link with max sensititivy (maxs(0)) is stored in maxs(1)
            MaxSens(k, 0) = MaxS(0)
            MaxSens(k, 1) = MaxS(1)
            MaxSens(k, 2) = MaxS(2)
            StoreSens(CInt(MaxS(1)), CInt(MaxS(2))) = 0
        Next

        'We've got the info now, so transfer it to the sens form

        'Initialize Vulnerability Blocks' color code to 0 (black).  Some blocks will be changed later to other color code
        'MakeSearchVisibleTrueFalse True, True, False
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = 0
            Next iPrey
        Next iPred

        For k As Integer = 1 To Me.m_iNumBlocks
            'Only these blocks will be changed to color code other than 0
            Me.m_ucVulBlocks.Vulblocks(CInt(MaxSens(k, 1)), CInt(MaxSens(k, 2))) = k
        Next

        Me.m_ucVulBlocks.Invalidate()
    End Sub

#End If

#End Region

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        ' Sanity check
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_F2TSManager Is Nothing) Then Return

        ' Enable transfer buttons based on available search type results

        ' Enable OK and Apply when having valid run results
        Me.m_btnOk.Enabled = (Me.m_runResultType <> eRunType.Idle)
        Me.m_btnUpdate.Enabled = (Me.m_runResultType <> eRunType.Idle)

    End Sub

    Private Function HasNonDefaultVulnerabilty() As Boolean
        Dim groupPath As cEcoPathGroupInput = Nothing
        Dim groupSim As cEcoSimGroupInput = Nothing

        For iPred As Integer = 1 To Me.m_core.nLivingGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                groupPath = Me.m_core.EcoPathGroupInputs(iPred)
                groupSim = Me.m_core.EcoSimGroupInputs(iPred)
                If groupPath.DietComp(iPrey) > 0 Then
                    If Math.Abs(groupSim.VulMult(iPrey) - 2) > 0.01 Then Return True
                End If
            Next iPrey
        Next iPred
        Return False
    End Function

    Private Function SetDefaultVulnerabilities() As Boolean
        Dim groupPath As cEcoPathGroupInput = Nothing
        Dim groupSim As cEcoSimGroupInput = Nothing

        For iPrey As Integer = 1 To Me.m_core.nGroups
            groupSim = Me.m_core.EcoSimGroupInputs(iPrey)
            For iPred As Integer = 1 To Me.m_core.nGroups
                '  groupPath = Me.m_core.EcoPathGroupInputs(iPred)
                'groupSim = Me.m_core.EcoSimGroupInputs(iPred)
                '    If groupPath.DietComp(iPrey) > 0 Then
                groupSim.VulMult(iPred) = 2.0
                '   End If
            Next iPred
        Next iPrey

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns>True if a new run was started succesfully.</returns>
    ''' -----------------------------------------------------------------------
    Private Function StartRun() As Boolean
        If (Me.m_F2TSManager.IsRunning()) Then Return False

        ' Reset controls
        Me.m_runResultType = eRunType.Idle
        Me.m_pbSearch.Value = 0

        If (Me.m_rbSearchPredPrey.Checked) Then
            If (Me.m_F2TSManager.RunSensitivitySS2VByPredPrey() = False) Then
                Return False
            End If
            Me.m_runType = eRunType.SensitivitySS2VByPredPrey
        Else

            If Me.HasNonDefaultVulnerabilty() Then
                If MsgBox("Reset all vulnerabilities to default (2)?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    Me.SetDefaultVulnerabilities()
                End If
            End If

            If (Me.m_F2TSManager.RunSensitivitySS2VByPredator() = False) Then
                Return False
            End If
            Me.m_runType = eRunType.SensitivitySS2VByPredator
        End If

        Me.UpdateControls()

        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stop an active F2TS manager run.
    ''' </summary>
    ''' <returns>
    ''' True if the manager is no longer running (or was not running at all).
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Private Function StopRun() As Boolean
        Return Me.m_F2TSManager.StopRun()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' -------------------------------------------r----------------------------
    Private Function UpdateDisplay() As Boolean

        Me.NumBlocks = CInt(Me.m_nudNumBlocks.Value)
        'have the manager sort the blocks acording to the last run sensitivity type
        Me.m_F2TSManager.setNBlocksFromSensitivity(Me.NumBlocks)

        Dim vblocks(,) As Integer = m_F2TSManager.VulnerabilityBlocks
        For iPred As Integer = 1 To Me.m_core.nGroups
            For iPrey As Integer = 1 To Me.m_core.nGroups
                If Me.m_F2TSManager.isPredPrey(iPred, iPrey) Then
                    Me.m_ucVulBlocks.Vulblocks(iPred, iPrey) = vblocks(iPred, iPrey)
                End If
            Next iPrey
        Next iPred

        Me.m_ucVulBlocks.Invalidate()

        'If Me.m_rbTransferPredPreyCell.Checked = True Then
        '    Me.UpdateByPredPrey()
        'End If

        'If Me.m_rbTransferPredCol.Checked = True Then
        '    Me.UpdateByPredatorColumn()
        'End If

        'If Me.m_rbTransferPredRow.Checked = True Then
        '    Me.UpdateByPreyRow()
        'End If

        ' Validate inputs
        Me.UpdateControls()
    End Function

#End Region ' Internal implementation

End Class