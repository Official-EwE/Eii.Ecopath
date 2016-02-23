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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit Layer user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEditLayer

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_qehGrid As cQuickEditHandler = Nothing

        ''' <summary>Original layer this dialog was invoked for.</summary>
        Private m_layerOriginal As cDisplayRasterLayer = Nothing
        Private m_layerDepth As cDisplayRasterLayer = Nothing
        Private m_edittype As eLayerEditTypes

        ''' <summary>Work layer (a copy of the original) for this dialog to work on.</summary>
        Private m_layerWork As cDisplayRasterLayer = Nothing
        ''' <summary>Editor to transmogrify the representation of the layer.</summary>
        Private m_ucEditVisualStyle As ucEditVisualStyle = Nothing

        Private m_fpName As cEwEFormatProvider = Nothing
        Private m_fpWeight As cEwEFormatProvider = Nothing
        Private m_fpDescription As cEwEFormatProvider = Nothing

        ' -- Hackerdihack

        Private m_bIsVectorData As Boolean = False
        Private m_iVectorData As Integer = 0

#End Region ' Private variables

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uic"></param>
        ''' <param name="layer"></param>
        ''' <param name="edittype"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByRef layer As cDisplayRasterLayer, _
                       ByVal edittype As eLayerEditTypes)

            Debug.Assert(layer IsNot Nothing)

            Me.InitializeComponent()

            ' Set the references
            Me.m_uic = uic
            Me.m_grid.UIContext = Me.m_uic
            Me.m_zoommap.UIContext = Me.m_uic

            Me.m_layerOriginal = layer

            ' Resolve depth layer
            If Not (TypeOf layer.Data Is cEcospaceLayerDepth) Then
                Dim fact As New cLayerFactoryInternal()
                Me.m_layerDepth = fact.GetLayers(uic, eVarNameFlags.LayerDepth)(0)
            End If
            Me.m_edittype = edittype

            Me.m_layerWork = New cDisplayRasterLayer(uic, layer) ' Work on a clone
            Me.m_layerWork.AllowValidation = False
            Me.m_layerWork.IsSelected = True ' Select layer, otherwise its content may not be rendered

            ' First set default index, then make vector stuff 'live' if need be ;)
            Me.m_tscmbVectorData.SelectedIndex = 0
            Me.m_bIsVectorData = (TypeOf Me.m_layerWork.Data Is cEcospaceLayerVector)

        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_grid.DataName = Me.m_layerOriginal.Name

            Me.m_qehGrid = New cQuickEditHandler()
            Me.m_qehGrid.ShowImportExport = False
            Me.m_qehGrid.Attach(Me.m_grid, Me.m_uic, Me.m_tsGrid)
            Me.m_qehGrid.IsOutputGrid = Me.m_layerWork.Editor.IsReadOnly

            ' Show your stuff
            Me.m_zoommap.Map.AddLayer(Me.m_layerWork)

            ' Do not add depth layer if already showing depth layer
            If ((Not Object.ReferenceEquals(Me.m_layerOriginal, Me.m_layerDepth)) And _
                (Not Object.ReferenceEquals(Me.m_layerDepth, Nothing))) Then
                Me.m_zoommap.Map.AddLayer(Me.m_layerDepth)
            End If

            Me.m_tcLayerView.SelectedIndex = CInt(Me.m_edittype)

            ' Set up format providers
            Me.m_fpName = New cEwEFormatProvider(Me.m_uic, Me.m_tbNameValue, GetType(String))
            Me.m_fpWeight = New cEwEFormatProvider(Me.m_uic, Me.m_nudWeight, GetType(Single))
            Me.m_fpDescription = New cEwEFormatProvider(Me.m_uic, Me.m_tbNameValue, GetType(String))

            Me.LoadLayer()
            Me.UpdateControls()
            Me.DrawPreview()

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                AddHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                RemoveHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

            Me.m_qehGrid.Detach()
            Me.m_qehGrid = Nothing
            Me.m_grid.UIContext = Nothing

            Me.m_fpName.Release()
            Me.m_fpWeight.Release()
            Me.m_fpDescription.Release()

            Me.m_layerDepth = Nothing
            Me.m_layerOriginal = Nothing
            Me.m_layerWork.Dispose()
            Me.m_layerWork = Nothing

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Overrides

#Region " Local events "

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            If Not Me.ApplyChanges() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnApply(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Apply_Button.Click

            Me.ApplyChanges()

        End Sub

        Private Sub OnVisualStyleChanged(ByVal sender As ucEditVisualStyle)

            ' Update work layer Visual Style
            Me.m_ucEditVisualStyle.Apply(Me.m_layerWork.Renderer.VisualStyle)
            Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.VisualStyle)

        End Sub

#Region " Import "

        ' Oooh, this is nasty! Three different import methods, handled by three different classes!
        ' ToDo: revamp this into a set of base classes that import and export one file format from or to an IRaster
        '       This code can be used by the spatial assets plug-in to provide access to obscure data formats, wrapped as datasets
        '       This code can be used by the core, using Joe's xD wrappers to provide access to IRaster data, to export data too

        Private Sub OnImportCSV(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiImportCSV.Click
            Try
                Me.m_qehGrid.ImportGridFromCSV()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnImportXYZ(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiImportXYZ.Click
            Try
                Dim cmd As cImportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
                cmd.Invoke(New cEcospaceLayer() {Me.m_layerWork.Data}, cImportLayerCommand.eImportFormatTypes.XYZ)
                Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.Map)
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnImportAscii(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiAsc.Click

            Try
                Dim ofd As New OpenFileDialog()
                ofd.Title = SharedResources.CAPTION_SELECT_FILE
                ofd.Filter = SharedResources.FILEFILTER_ASCFILE

                If (ofd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                    If Me.ReadASCFile(ofd.FileName) Then
                        Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.Map)
                        Me.m_grid.RefreshContent()
                    End If
                End If
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Import

#Region " Export "

        Private Sub OnExportCSV(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiExportCSV.Click
            Try
                Me.m_qehGrid.ExportGridToCSV()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnExportAsc(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiExportAsc.Click

            Try
                Dim sfd As New SaveFileDialog()

                sfd.CheckPathExists = True
                sfd.Title = SharedResources.CAPTION_SELECT_FILE
                sfd.Filter = SharedResources.FILEFILTER_ASCFILE

                If (sfd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                    Me.WriteASCFile(sfd.FileName)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnExportLayer(sender As System.Object, e As System.EventArgs) Handles m_tsmiExportXYZ.Click
            Try
                Dim cmd As cExportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cExportLayerCommand.cCOMMAND_NAME), cExportLayerCommand)
                cmd.Invoke(New cDisplayRasterLayer() {Me.m_layerWork})
                Me.UpdateControls()
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Export

        Private Sub OnNameChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbNameValue.TextChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnSelectData(sender As System.Object, e As System.EventArgs) _
            Handles m_tscmbVectorData.SelectedIndexChanged

            If (Me.m_bIsVectorData) Then
                Me.m_grid.VectorFieldIndex = Me.m_tscmbVectorData.SelectedIndex
                Me.m_grid.RefreshContent()
            End If

        End Sub

#End Region ' Local events

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Diagnostic method, states if a layer has a unique core variable 
        ''' link. Layers with unique sources support extra's that can be stored
        ''' in the database such as remarks and visual styles.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function HasUniqueSource() As Boolean
            If (Me.m_layerOriginal.Source Is Nothing) Then Return False
            If (TypeOf Me.m_layerOriginal.Source Is cEcospaceBasemap) Then Return False
            Return True
        End Function

        Private Sub LoadLayer()

            Dim vs As cVisualStyle = Me.m_layerWork.Renderer.VisualStyle
            Dim src As cCoreInputOutputBase = Me.m_layerWork.Source

            Me.m_lblWeight.Visible = False
            Me.m_nudWeight.Visible = False
            Me.m_lblDescription.Visible = False
            Me.m_tbDescription.Visible = False

            If (Me.HasUniqueSource()) Then
                Me.m_fpName.Enabled = True
                Me.m_tbRemarks.Text = src.Remark
                Me.m_tbRemarks.Enabled = True

                If TypeOf src Is cEcospaceLayerImportance Then
                    Me.m_lblWeight.Visible = True
                    Me.m_nudWeight.Visible = True
                    Me.m_lblDescription.Visible = True
                    Me.m_tbDescription.Visible = True

                    Me.m_fpWeight.Value = src.GetVariable(eVarNameFlags.ImportanceWeight)
                    Me.m_fpDescription.Value = src.GetVariable(eVarNameFlags.Description)
                End If
            Else
                Me.m_fpName.Enabled = False
                Me.m_tbRemarks.Text = My.Resources.STATUS_REMARKS_NOT_SUPPORTED
                Me.m_tbRemarks.Enabled = False
            End If

            ' Do not use display text; user may want to edit this
            Me.m_fpName.Value = m_layerWork.Name

            Me.m_ucEditVisualStyle = ucEditVisualStyle.GetEditor(Me.m_uic, vs, Me.m_layerWork.Renderer.VisualStyleFlags)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                Me.m_plAppearance.Height = Me.m_ucEditVisualStyle.Height
                Me.m_ucEditVisualStyle.Dock = DockStyle.Fill
                Me.m_plAppearance.Controls.Add(Me.m_ucEditVisualStyle)
            End If

            Me.m_grid.Layer = Me.m_layerWork
            Me.m_grid.VectorFieldIndex = Me.m_iVectorData
            Me.m_grid.RefreshContent()

            Me.m_tlpDetails.PerformLayout()
            Me.m_tlpBits.PerformLayout()

        End Sub

        Private Sub DrawPreview()
            Me.m_zoommap.Map.Refresh()
        End Sub

        Private Sub UpdateControls()

            Dim bEditable As Boolean = True

            If (Me.m_layerOriginal.Editor IsNot Nothing) Then
                bEditable = (Me.m_layerOriginal.Editor.IsReadOnly = False)
            End If

            Me.m_tsddImport.Enabled = bEditable
            Me.Text = cStringUtils.Localize(My.Resources.ECOSPACE_CAPTION_EDITLAYER, Me.m_tbNameValue.Text)

            Me.m_tscmbVectorData.Visible = Me.m_bIsVectorData

        End Sub

        Private Function ApplyChanges() As Boolean

            Dim cf As cDisplayLayer.eChangeFlags = 0
            Dim src As cCoreInputOutputBase = Me.m_layerOriginal.Source

            If (HasUniqueSource()) Then

                Dim p As cProperty = Me.m_layerOriginal.GetProperty()
                If (p IsNot Nothing) Then
                    p.SetRemark(Me.m_tbRemarks.Text)
                    p.SetValue(CStr(Me.m_fpName.Value))
                End If

                If TypeOf Me.m_layerOriginal.Source Is cEcospaceLayerImportance Then
                    src.SetVariable(eVarNameFlags.ImportanceWeight, Me.m_fpWeight.Value)
                    src.SetVariable(eVarNameFlags.Description, Me.m_fpDescription.Value)
                End If

            End If

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                ' Apply changes
                Me.m_ucEditVisualStyle.Apply(Me.m_layerOriginal.Renderer.VisualStyle)
                cf = cf Or cDisplayLayer.eChangeFlags.VisualStyle
            End If

            Me.m_grid.Apply(Me.m_layerOriginal)
            cf = cf Or cDisplayLayer.eChangeFlags.Map

            ' Fire layer changed notification
            Me.m_layerOriginal.Update(cf)

            Return True

        End Function

#End Region ' Internal implementation

#Region " This should really live somewhere else... "

        ' ToDo_JS: merge with core ASCII map logic, and build provisions to use spatial temporal framework
        Protected Function ReadASCFile(ByVal strFilename As String) As Boolean

            Dim bLoaded As Boolean
            Dim strm As New StreamReader(strFilename)
            Dim iNullCells As Integer
            Dim msg As cMessage = Nothing

            bLoaded = Me.ReadASCIIHeader(strm) And Me.ReadASCIIBody(strm, iNullCells)

            strm.Close()

            If Not bLoaded Then
                msg = New cMessage(String.Format(SharedResources.GENERIC_FILELOAD_FAILURE, Me.m_grid.DataName, strFilename), _
                     eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
            Else
                msg = New cMessage(String.Format(SharedResources.GENERIC_FILELOAD_SUCCES, Me.m_grid.DataName, strFilename), _
                     eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strFilename)

                If (iNullCells > 0) Then
                    Dim vs As New cVariableStatus(eStatusFlags.MissingParameter, _
                                                  String.Format(SharedResources.PROMPT_MAPLOAD_MISSING, iNullCells), _
                                                  eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0)
                    msg.AddVariable(vs)
                End If
            End If

            Me.m_uic.Core.Messages.SendMessage(msg)
            Return bLoaded

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Too hack to be true
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Function ReadASCIIHeader(ByVal reader As StreamReader) As Boolean
            Dim bsuccess As Boolean = True
            Dim strLine As String = ""
            Dim bFoundData As Boolean = False
            Try

                'jb 19-Aug-2015 changed to be more robust
                'at least it reports if the file fails to read
                While Not reader.EndOfStream
                    'jb Trim the line just in case. We have had files that contain a leading space in the headers. Really...
                    strLine = reader.ReadLine.Trim
                    If Not String.IsNullOrWhiteSpace(strLine) Then
                        If cStringUtils.BeginsWith(strLine, "NODATA_value", True) Then
                            bFoundData = True
                            Exit While
                        End If
                    End If
                End While

                'While (String.IsNullOrWhiteSpace(strLine) Or (Not cStringUtils.BeginsWith(strLine, "NODATA_value", True))) And _
                '(Not reader.EndOfStream)
                '    strLine = reader.ReadLine
                'End While

            Catch ex As Exception
                bsuccess = False
            End Try

            Return bsuccess And bFoundData

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Too hack to be true
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Function ReadASCIIBody(ByVal reader As StreamReader, ByRef nNullCells As Integer) As Boolean

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim exclusion As cEcospaceLayerExclusion = bm.LayerExclusion
            Dim value As Single = 0
            Dim strValue As String = ""
            Dim bSuccess As Boolean = True
            Dim isDepthLayer As Boolean = (Me.m_layerWork.VarName = eVarNameFlags.LayerDepth)

            Try

                For ir As Integer = 1 To bm.InRow
                    'ASC files written by GDAL contain a space at the start of the line so strip it off
                    'this should not affect other ASC file reading
                    Dim strLine As String = reader.ReadLine.Trim
                    Dim astrBits() As String = strLine.Split(" "c)
                    For ic As Integer = 1 To Math.Min(bm.InCol, astrBits.Length)
                        If depth.IsWaterCell(ir, ic) Or isDepthLayer Then
                            bSuccess = bSuccess And Single.TryParse(astrBits(ic - 1), value)
                        Else
                            value = cCore.NULL_VALUE
                        End If

                        'jb 19-Aug-2015 when loading a new basemap 
                        'I think it's better to load all the data 'as is'
                        'this way you don't get fragments from any previously loaded data
                        Me.m_layerWork.Value(ir, ic) = value

                        'Count the number of null values in water cells
                        If depth.IsWaterCell(ir, ic) And (Not exclusion.IsExcludedCell(ir, ic)) And (value = CSng(cCore.NULL_VALUE)) And Not isDepthLayer Then
                            nNullCells += 1
                        End If

                        'Could also test to see if it actually set the value in the map
                        'Passed all the validation rules
                        'if  (Me.m_layerWork.Value(ir, ic) = value).....

                    Next
                Next
            Catch ex As Exception
                bSuccess = False
            End Try
            Return bSuccess

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write an entire ASCII file for a group, time step and variable.
        ''' </summary>
        ''' <param name="strFileName"></param>
        ''' -----------------------------------------------------------------------
        Protected Sub WriteASCFile(ByVal strFileName As String)

            Dim msg As cMessage = Nothing

            Try
                Using wr As New StreamWriter(strFileName)
                    Me.WriteASCIIHeader(wr)
                    Me.WriteASCIIBody(wr)
                    wr.Close()
                End Using

                Using wr As New StreamWriter(Path.ChangeExtension(strFileName, ".prj"))
                    wr.WriteLine(Me.m_uic.Core.EcospaceBasemap.ProjectionString)
                    wr.Close()
                End Using

                msg = New cMessage(String.Format(My.Resources.GENERIC_FILESAVE_SUCCES, Me.m_grid.DataName, strFileName), _
                    eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strFileName)

            Catch ex As Exception
                msg = New cMessage(String.Format(My.Resources.GENERIC_FILESAVE_FAILURE, Me.m_grid.DataName, strFileName, ex.Message), _
                  eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
            End Try

            ' Log!
            Me.m_uic.Core.Messages.SendMessage(msg)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write ESRI ASCII header block.
        ''' </summary>
        ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
        ''' -----------------------------------------------------------------------
        Protected Sub WriteASCIIHeader(ByVal writer As StreamWriter)

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            writer.WriteLine("ncols         " & bm.InCol)
            writer.WriteLine("nrows         " & bm.InRow)
            writer.WriteLine("xllcorner     " & bm.PosTopLeft.X)
            writer.WriteLine("yllcorner     " & bm.PosBottomRight.Y)
            writer.WriteLine("cellsize      " & bm.CellSize)
            writer.WriteLine("NODATA_value  " & cCore.NULL_VALUE)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write ESRI ASCII body block.
        ''' </summary>
        ''' <param name="writer">The <see cref="StreamWriter"/> to write to.</param>
        ''' -----------------------------------------------------------------------
        Protected Sub WriteASCIIBody(ByVal writer As StreamWriter)

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim value As Double = 0
            Dim strValue As String = ""

            For ir As Integer = 1 To bm.InRow
                For ic As Integer = 1 To bm.InCol
                    If ic > 1 Then writer.Write(" ")
                    If depth.IsWaterCell(ir, ic) Or Me.m_layerWork.VarName = eVarNameFlags.LayerDepth Then
                        value = CSng(Me.m_layerWork.Value(ir, ic))
                    Else
                        value = cCore.NULL_VALUE
                    End If

                    ' Fix #1321 - always make sure the first cell value is written as floating point
                    strValue = cStringUtils.FormatNumber(value)
                    If (ir = 1 And ic = 1) Then
                        If (strValue.IndexOf("."c) = -1) Then
                            strValue = strValue + ".0"
                        End If
                    End If

                    writer.Write(strValue)
                Next
                writer.WriteLine("")
            Next

        End Sub

#End Region ' This should really live somewhere else...

    End Class

End Namespace