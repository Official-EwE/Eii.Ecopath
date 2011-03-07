#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Definitions
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cShapeGUIHandler">cShapeGUIHandler implementation</see> for 
    ''' handling <see cref="cTimeSeries">Time Series shapes</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cTimeSeriesShapeGUIHandler
        Inherits cShapeGUIHandler

        ''' <summary>Flag to prevent update / response loops.</summary>
        Private m_bInUpdate As Boolean = False
        ''' <summary>The Time Series to distribute.</summary>
        Private m_lShapes As New List(Of cShapeData)

#Region " Baseclass overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this handler.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext">UI contextual</see> information.</param>
        ''' <param name="stb"><see cref="ucShapeToolbox">Shape toolbox control </see> to handle, if any.</param>
        ''' <param name="stbtb"><see cref="ucShapeToolboxToolbar">Shape toolbox toolbar control </see> to handle, if any.</param>
        ''' <param name="sp"><see cref="ucSketchPad">Shape sketch pad control </see> to handle, if any.</param>
        ''' <param name="sptb"><see cref="ucSketchPadToolbar">Shape sketch pad toolbar control </see> to handle, if any.</param>
        ''' -------------------------------------------------------------------
        Public Overloads Sub Attach(ByVal uic As cUIContext, _
                                    ByVal stb As ucShapeToolbox, _
                                    ByVal stbtb As ucShapeToolboxToolbar, _
                                    ByVal sp As ucSketchPad, _
                                    ByVal sptb As ucSketchPadToolbar)

            MyBase.Attach(uic, stb, stbtb, sp, sptb)

            If Me.SketchPad IsNot Nothing Then
                ' Cannot draw onto time series shapes
                Me.SketchPad.Enabled = False
            End If

            If Me.ShapeToolBox IsNot Nothing Then
                ' Add check boxes to the toolbox
                Me.ShapeToolBox.AllowCheckboxes = True
            End If

            Me.UpdateShapeList(New cShapeData() {sp.Shape}, eAutoSelectMode.SelectFirstShape)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to ask whether a given command is supported by this handler.
        ''' Overridden to weed out non-Time Series commands.
        ''' </summary>
        ''' <param name="cmd">The command to test.</param>
        ''' <returns>True if command is supported.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportCommand(ByVal cmd As eShapeCommandTypes) As Boolean

            Select Case cmd
                Case eShapeCommandTypes.Add
                    Return True
                Case eShapeCommandTypes.Weight
                    Return True
                Case eShapeCommandTypes.Duplicate
                    Return False
                Case eShapeCommandTypes.Import
                    Return True
                Case eShapeCommandTypes.Export
                    Return True
                Case eShapeCommandTypes.Load
                    Return True
                Case eShapeCommandTypes.Modify
                    Return True
                Case eShapeCommandTypes.Remove
                    Return True
                Case eShapeCommandTypes.Seasonal
                    Return False
                Case eShapeCommandTypes.SetWeight
                    Return True
                Case eShapeCommandTypes.SaveAsImage
                    Return True
                Case eShapeCommandTypes.ResetAll
                    Return False
                Case Else
                    ' Debug.Assert(False, String.Format("Command {0} not supported", cmd))
            End Select
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to query the enables state of a given command by this handler.
        ''' Overridden to enable commands Time Series-style, kachingg!!
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <returns>True if enabled.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function EnableCommand(ByVal cmd As cShapeGUIHandler.eShapeCommandTypes) As Boolean

            Dim bHasSelection As Boolean = (Me.SelectedShapes IsNot Nothing)
            Dim bHasSingleSelection As Boolean = (Me.SelectedShape IsNot Nothing)

            Select Case cmd

                Case cShapeGUIHandler.eShapeCommandTypes.Import, _
                     eShapeCommandTypes.Load
                    Return True

                Case cShapeGUIHandler.eShapeCommandTypes.Add, _
                     eShapeCommandTypes.Weight, _
                     eShapeCommandTypes.Export
                    Return Me.Core.HasTimeSeries

                Case cShapeGUIHandler.eShapeCommandTypes.Duplicate, _
                     cShapeGUIHandler.eShapeCommandTypes.Remove
                    Return bHasSelection

                Case eShapeCommandTypes.Modify, _
                     eShapeCommandTypes.SetWeight, _
                     eShapeCommandTypes.SaveAsImage
                    Return bHasSingleSelection

            End Select

            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public interface to execute a given command by this handler. 
        ''' Overridden to implement Time Series commands.
        ''' </summary>
        ''' <param name="cmd">The <see cref="eShapeCommandTypes">command</see> to test.</param>
        ''' <param name="ashapes">The <see cref="EwECore.cShapeData">shapes</see> to apply the command to.</param>
        ''' <param name="data">Optional data to accompany the command.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub ExecuteCommand(ByVal cmd As eShapeCommandTypes, _
             Optional ByVal ashapes As cShapeData() = Nothing, Optional ByVal data As Object = Nothing)

            If (ashapes Is Nothing) Then ashapes = Me.SelectedShapes

            Select Case cmd
                Case eShapeCommandTypes.Add
                    Me.AddTimeSeries()

                Case eShapeCommandTypes.Duplicate
                    Me.DuplicateTimeSeries(ashapes)

                Case eShapeCommandTypes.Import
                    Me.ImportTimeSeries()

                Case eShapeCommandTypes.Export
                    Me.ExportTimeSeries()

                Case eShapeCommandTypes.Load
                    Me.LoadDatasets()

                Case eShapeCommandTypes.Remove
                    Me.RemoveTimeSeries(ashapes)

                Case eShapeCommandTypes.Modify
                    Me.ModifyTimeSeries(ashapes(0))

                Case eShapeCommandTypes.SetWeight
                    Me.SetWeight(ashapes(0), CSng(data))

                Case eShapeCommandTypes.SaveAsImage
                    Me.SaveAsImage(ashapes(0), Me.SketchPad)

                Case eShapeCommandTypes.Weight
                    Me.WeightTimeSeries()

            End Select
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden this to make controls respond to any kind of change in 
        ''' time series data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Refresh()
            If Me.m_bInUpdate Then Return
            Me.UpdateShapeList(Me.SelectedShapes, eAutoSelectMode.SelectCurrentShape)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Respond to local shape change.
        ''' </summary>
        ''' <param name="shape">The newly selected shape.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeChanged(ByVal shape As EwECore.cShapeData)
            If (Me.ShapeToolBox Is Nothing) Then Return
            If Me.m_bInUpdate Then Return

            Me.m_bInUpdate = True
            Me.ShapeToolBox.UpdateThumbnail(shape)
            Me.m_bInUpdate = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to kick the programmer; Time Series cannot be drawn by hand.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeFinalized(ByVal shape As EwECore.cShapeData, ByVal sketchpad As ucSketchPad)
            Debug.Assert(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cascade a newly selected shape to the managed controls.
        ''' </summary>
        ''' <param name="ashapes">The newly selected shapes.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub OnShapeSelected(ByVal ashapes As EwECore.cShapeData())
            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True
            If Me.SketchPad IsNot Nothing Then
                Me.SelectedShapes = ashapes
            End If
            Me.m_bInUpdate = False
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the colour for Time Series shapes.
        ''' </summary>
        ''' <returns>The color for Time Series shapes.</returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Color() As System.Drawing.Color
            Return Color.DarkGreen
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default sketch mode for Time Series shapes.
        ''' </summary>
        ''' <returns>The default sketch mode for Time Series shapes.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function SketchDrawMode() As eSketchDrawModeTypes
            Return eSketchDrawModeTypes.Dots
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the lower limit for the sketch pad Y-axis when displaying 
        ''' Time Series data.
        ''' </summary>
        ''' <returns>The lower limit for the sketch pad Y-axis when displaying 
        ''' Time Series data.</returns>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function MinYScale() As Single
            Return 0.0!
        End Function

#End Region ' Baseclass overrides

#Region " Internal implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Add">Add</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub AddTimeSeries()
            Dim frm As frmShapeValue = New frmShapeValue(Me.UIContext)
            If (frm.ShowDialog() = DialogResult.OK) Then
                ' Ecosim will reload, which means a reload of datasets and time series
                ' As a result, this control will be told to update
                Me.Core.LoadTimeSeries(Me.Core.ActiveTimeSeriesDatasetIndex)
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Weight">Weight</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub WeightTimeSeries()
            Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand("WeightTimeSeries")

            If cmd IsNot Nothing Then
                cmd.Invoke()
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Load">Load</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub LoadDatasets()
            Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand("LoadTimeSeries")

            If cmd IsNot Nothing Then
                cmd.Invoke()
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Duplicate">Duplicate</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub DuplicateTimeSeries(ByVal ashapes As cShapeData())

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid TS")

            Dim strNewTSName As String = ""
            Dim lstrTSNames As New List(Of String)
            Dim iNextTSNumber As Integer = 0
            Dim ts As cTimeSeries = Nothing
            Dim asValues() As Single
            Dim intDBID As Integer = -1
            Dim bSucces As Boolean = True

            ' Collect all current shape names
            For Each s As cShapeData In Me.m_lShapes
                lstrTSNames.Add(s.Name)
            Next

            ' Concoct a new name based on the numbered strings that are found
            iNextTSNumber = cStringUtils.GetNextNumber(lstrTSNames.ToArray(), My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES)
            strNewTSName = String.Format(My.Resources.ECOSIM_DEFAULT_NEWTIMESERIES, iNextTSNumber)

            ' Generate TS data
            For Each shape As cShapeData In ashapes
                ts = Me.Core.EcosimTimeSeries(shape.Index)
                ReDim asValues(ts.ShapeData.Length - 2)
                For i As Integer = 1 To ts.ShapeData.Length - 1
                    asValues(i - 1) = ts.DatVal(i)
                Next

                bSucces = bSucces And (Me.Core.AddTimeSeries(strNewTSName, _
                        ts.DataType, ts.TimeSeriesType, _
                        ts.WtType, asValues, intDBID))
            Next

            If bSucces Then
                ' Update shape to select
                Me.UpdateShapeList(Nothing, eAutoSelectMode.SelectLastShape)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Import">Import</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ImportTimeSeries()
            ' Launch via command!
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand("ImportTimeSeries")
            If cmd IsNot Nothing Then cmd.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Export">ExportTimeSeries</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub ExportTimeSeries()
            ' Launch via command!
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand("ExportTimeSeries")
            If cmd IsNot Nothing Then cmd.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implementation of the <see cref="eShapeCommandTypes.Remove">Remove</see> commmand.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub RemoveTimeSeries(ByVal ashapes As cShapeData())

            Dim fms As cFeedbackMessage = Nothing
            Dim strMessage As String = ""
            Dim bSucces As Boolean = True

            ' Sanity check
            Debug.Assert(ashapes IsNot Nothing, "Need valid TS")

            ' Prompt for confirmation
            If ashapes.Length = 1 Then
                strMessage = String.Format(My.Resources.PROMPT_TIMESERIES_DELETE, ashapes(0).Name)
            Else
                strMessage = String.Format(My.Resources.PROMPT_TIMESERIES_DELETE_MULTIPLE, ashapes.Length)
            End If

            fms = New cFeedbackMessage(strMessage, _
                                       eCoreComponentType.ShapesManager, _
                                       eMessageType.Any, _
                                       eMessageImportance.Warning, _
                                       cFeedbackMessage.eReplyStyle.YES_NO, _
                                       eDataTypes.TimeSeriesDataset, _
                                       cFeedbackMessage.eReply.OK)
            Me.Core.Messages.SendMessage(fms, True)
            If (fms.Reply = cFeedbackMessage.eReply.NO) Then Return

            ' Delete
            Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure)
            Try
                For Each shape As cShapeData In ashapes
                    Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")
                    bSucces = bSucces And Me.Core.RemoveTimeSeries(shape.DBID)
                Next
            Catch ex As Exception
                ' Whoah!
            End Try
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.TimeSeries, bSucces)

            ' Refresh
            Me.UpdateShapeList()

        End Sub

        Private Sub ModifyTimeSeries(ByVal shape As cShapeData)

            ' Sanity check
            Debug.Assert(shape IsNot Nothing, "Need valid TS")
            Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")

            Dim dlg As New frmShapeValue(Me.UIContext, shape)
            Try
                dlg.ShowDialog()
            Catch ex As Exception
                ' Whoa!
            End Try

        End Sub

        Private Sub SetWeight(ByVal shape As cShapeData, ByVal sWeight As Single)

            ' Sanity check
            Debug.Assert(shape IsNot Nothing, "Need valid TS")
            Debug.Assert(TypeOf shape Is cTimeSeries, "Need valid TS")

            DirectCast(shape, cTimeSeries).WtType = sWeight
            shape.Update()

        End Sub

#End Region ' Internal implementation 

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper enum; states how to reload data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Enum eAutoSelectMode As Byte
            None = 0
            SelectFirstShape
            SelectLastShape
            SelectCurrentShape
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; updates the list of time series to manage.
        ''' </summary>
        ''' <param name="ashapeSelect">Shapes to select.</param>
        ''' <param name="selectMode">If shape cannot be selected, or no shape 
        ''' has been provided, this mode indicates how the new selection should 
        ''' be made.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateShapeList(Optional ByVal ashapeSelect As cShapeData() = Nothing, _
                Optional ByVal selectMode As eAutoSelectMode = eAutoSelectMode.SelectCurrentShape)

            Dim ts As cTimeSeries = Nothing
            Dim shapeSelectCurr As cShapeData() = Me.SelectedShapes

            Me.m_lShapes.Clear()

            For i As Integer = 1 To Me.Core.nTimeSeries
                Me.m_lShapes.Add(Me.Core.EcosimTimeSeries(i))
            Next

            ' Select a shape
            If Object.ReferenceEquals(ashapeSelect, Nothing) Then
                If Me.m_lShapes.Count > 0 Then
                    Select Case selectMode
                        Case eAutoSelectMode.None
                            ' Haha
                        Case eAutoSelectMode.SelectCurrentShape
                            ashapeSelect = shapeSelectCurr
                        Case eAutoSelectMode.SelectFirstShape
                            ashapeSelect = New cShapeData() {Me.m_lShapes(0)}
                        Case eAutoSelectMode.SelectLastShape
                            ashapeSelect = New cShapeData() {Me.m_lShapes(Me.m_lShapes.Count - 1)}
                    End Select
                End If
            End If

            If (Me.ShapeToolBox IsNot Nothing) Then
                Me.ShapeToolBox.SetShapes(Me.m_lShapes, ashapeSelect)
                ashapeSelect = Me.ShapeToolBox.Selection
            End If

            Me.SelectedShapes = ashapeSelect

        End Sub

#End Region ' Helper methods

    End Class

End Namespace
