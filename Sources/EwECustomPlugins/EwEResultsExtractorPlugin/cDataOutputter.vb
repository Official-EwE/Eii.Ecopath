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


Imports System.IO
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports Microsoft.Office.Interop
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cDataOutputer

    Private mFunctionalGroupData As List(Of cDataSheet)
    Private mFisheriesData As List(Of cDataSheet)
    Private mIndicators As List(Of cDataSheet)
    Private mDiagnostics As List(Of cDataSheet)
    Private mOutputType As eOutputTypes
    Private mStrPath As String
    Private mNDataItems As Integer
    Private mMsg As cMessage = Nothing
    Private mExcelInteropEnabled As Boolean = False
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cDataOutputer)()

    Public Enum eOutputTypes As Integer
        CSV
        Excel
    End Enum

    Public Sub New()
        Me.mFunctionalGroupData = New List(Of cDataSheet)
        Me.mFisheriesData = New List(Of cDataSheet)
        Me.mIndicators = New List(Of cDataSheet)
        Me.mDiagnostics = New List(Of cDataSheet)
        Me.mExcelInteropEnabled = Me.IsExcelAccessible()
    End Sub

    'List containing all objects for each option selected that is a functional group
    Public Sub AddFunctionalGroup(ByRef Group As cDataSheet)
        Me.mFunctionalGroupData.Add(Group)
        Me.mNDataItems += 1
    End Sub

    'List containing all objects for each option selected that is a Fishery group
    Public Sub AddFisheries(ByRef Fisheries As cDataSheet)
        Me.mFisheriesData.Add(Fisheries)
        Me.mNDataItems += 1
    End Sub

    'List containing all objects for each option selected that is a indicator group
    Public Sub AddIndicators(ByRef Indicator As cDataSheet)
        Me.mIndicators.Add(Indicator)
        Me.mNDataItems += 1
    End Sub

    Public Sub AddDiagnostics(ByRef Diagnostics As cDataSheet)
        Me.mDiagnostics.Add(Diagnostics)
        Me.mNDataItems += 1
    End Sub

    ''' <summary>
    ''' Returns whether Excel interop is enabled on a computer
    ''' </summary>
    Public ReadOnly Property ExcelInteropEnabled As Boolean
        Get
            Return Me.mExcelInteropEnabled
        End Get
    End Property

    'Property that sets what file type to output
    Public Property POutputType() As eOutputTypes
        Get
            Return Me.mOutputType
        End Get
        Set(value As eOutputTypes)
            If value = eOutputTypes.Excel And Not Me.mExcelInteropEnabled Then value = eOutputTypes.CSV
            Me.mOutputType = value
        End Set
    End Property

    ''' <summary>
    ''' Sets and returns the directory path that the user wants to save data files to
    ''' </summary>
    Public Property PPath() As String
        Get
            Return Me.mStrPath
        End Get
        Set(value As String)
            Me.mStrPath = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the number of data items that are held by the dataoutputer
    ''' </summary>
    Public ReadOnly Property GetNumDataItems() As Integer
        Get
            Return Me.mNDataItems
        End Get
    End Property

    ''' <summary>
    ''' This is the subroutine that the client calls to output the data
    ''' </summary>
    ''' <returns>
    ''' An informational message with a summary of output results
    ''' </returns>
    Public Function OutputData() As cMessage

        Me.PrepareExportMessage()

        Try
            Select Case Me.mOutputType
                Case eOutputTypes.CSV
                    Me.CreateCSVFiles()
                Case eOutputTypes.Excel
                    Me.CreateExcelFiles()
                Case Else
                    Debug.Assert(False, "Unsupported output format " & Me.mOutputType.ToString & " specified")
            End Select
        Catch ex As Exception
            Me.LogException(ex.Message)
        End Try

        Return Me.CompleteExportMessage()

    End Function

    Private Sub CreateCSVFiles()

        Dim fileName As String
        Dim fDateTime As DateTime = DateTime.Now
        Dim CurrentTime As String = "(D" & fDateTime.Day & "-" & fDateTime.Month & "-" & fDateTime.Year & ")(T" & _
        fDateTime.Hour.ToString & "-" & fDateTime.Minute.ToString & "-" _
        & fDateTime.Second.ToString & ")"
        Dim ArrayData(,) As Object
        Dim DataItem As String

        'Create the functional group files
        If Me.mFunctionalGroupData.Count > 0 Then
            For Each i In Me.mFunctionalGroupData
                fileName = cFileUtils.ToValidFileName(cStringUtils.Localize(My.Resources.GENERIC_CSV_FILE, i.Name, CurrentTime), False)
                Dim sw As StreamWriter = New StreamWriter(Path.Combine(Me.mStrPath, fileName), False)
                ArrayData = CType(i.Data, Object(,))
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = cStringUtils.ToCSVField(ArrayData(x, y))
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
                Me.LogExport(CStr(i.Name), fileName)
            Next
        End If

        'Create the fishery group files
        If Me.mFisheriesData.Count > 0 Then
            For Each i In Me.mFisheriesData
                fileName = cFileUtils.ToValidFileName(cStringUtils.Localize(My.Resources.GENERIC_CSV_FILE, i.Name, CurrentTime), False)
                Dim sw As StreamWriter = New StreamWriter(Path.Combine(Me.mStrPath, fileName), False)
                ArrayData = CType(i.Data, Object(,))
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = cStringUtils.ToCSVField(ArrayData(x, y))
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
                Me.LogExport(CStr(i.Name), fileName)
            Next
        End If

        'Create the indicator files
        If Me.mIndicators.Count > 0 Then
            For Each i In Me.mIndicators
                fileName = cFileUtils.ToValidFileName(cStringUtils.Localize(My.Resources.GENERIC_CSV_FILE, i.Name, CurrentTime), False)
                Dim sw As StreamWriter = New StreamWriter(Path.Combine(Me.mStrPath, fileName), False)
                ArrayData = CType(i.Data, Object(,))
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = cStringUtils.ToCSVField(ArrayData(x, y))
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
                Me.LogExport(CStr(i.Name), fileName)
            Next
        End If

        'Create the indicator files
        If Me.mDiagnostics.Count > 0 Then
            For Each i In Me.mDiagnostics
                fileName = cFileUtils.ToValidFileName(cStringUtils.Localize(My.Resources.GENERIC_CSV_FILE, i.Name, CurrentTime), False)
                Dim sw As StreamWriter = New StreamWriter(Path.Combine(Me.mStrPath, fileName), False)
                ArrayData = CType(i.Data, Object(,))
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = cStringUtils.ToCSVField(ArrayData(x, y))
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
                Me.LogExport(CStr(i.Name), fileName)
            Next
        End If

    End Sub

    Private Sub CreateExcelFiles()

        Dim ex As New Excel.Application
        Dim FileExists As Boolean = False
        Dim DirectInfo As New DirectoryInfo(Me.mStrPath)
        Dim files As FileInfo() = DirectInfo.GetFiles
        Dim FunctionalWB As Excel.Workbook
        Dim FisheriesWB As Excel.Workbook
        Dim IndicatorsWB As Excel.Workbook
        Dim DiagnosticsWB As Excel.Workbook
        Dim sheet As Excel.Worksheet
        Dim fDateTime As DateTime = DateTime.Now
        Dim fileName As String
        Dim ArrayData(,) As Object

        Dim CurrentTime As String = "(D" & fDateTime.Day & "-" & fDateTime.Month & "-" & fDateTime.Year & ")(T" & _
                fDateTime.Hour.ToString & "-" & fDateTime.Minute.ToString & "-" _
                & fDateTime.Second.ToString & ")"


        If Me.mFunctionalGroupData.Count > 0 Then
            fileName = My.Resources.FUNC_GROUP & CurrentTime
            FunctionalWB = ex.Workbooks.Add()
            For Each i In Me.mFunctionalGroupData
                sheet = CType(FunctionalWB.Worksheets.Add(), Excel.Worksheet)
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Object(,))
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
                Me.LogExport(CStr(i.Name), fileName)
            Next
            FunctionalWB.SaveAs(Path.Combine(Me.mStrPath, fileName))
        End If

        If Me.mFisheriesData.Count > 0 Then
            fileName = My.Resources.FISHERIES & CurrentTime
            FisheriesWB = ex.Workbooks.Add()
            For Each i In Me.mFisheriesData
                sheet = CType(FisheriesWB.Worksheets.Add(), Excel.Worksheet)
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Object(,))
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
                Me.LogExport(CStr(i.Name), fileName)
            Next
            FisheriesWB.SaveAs(Path.Combine(Me.mStrPath, fileName))
        End If

        If Me.mIndicators.Count > 0 Then
            fileName = My.Resources.INDICATORS & CurrentTime
            IndicatorsWB = ex.Workbooks.Add()
            For Each i In Me.mIndicators
                sheet = CType(IndicatorsWB.Worksheets.Add(), Excel.Worksheet)
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Object(,))
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
                Me.LogExport(CStr(i.Name), fileName)
            Next
            IndicatorsWB.SaveAs(Path.Combine(Me.mStrPath, fileName))
        End If

        If Me.mDiagnostics.Count > 0 Then
            fileName = My.Resources.DIAGNOSTICS & CurrentTime
            DiagnosticsWB = ex.Workbooks.Add()
            For Each i In Me.mDiagnostics
                sheet = CType(DiagnosticsWB.Worksheets.Add(), Excel.Worksheet)
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Object(,))
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
                Me.LogExport(CStr(i.Name), fileName)
            Next
            DiagnosticsWB.SaveAs(Path.Combine(Me.mStrPath, fileName))
        End If

        FunctionalWB = Nothing
        FisheriesWB = Nothing
        IndicatorsWB = Nothing
        DiagnosticsWB = Nothing
        ex.Quit()

    End Sub


#Region " Message "

    Private Sub PrepareExportMessage()
        Me.mMsg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_SAVE, Me.PPath), _
                                        eMessageType.DataExport, eCoreComponentType.Ecosim, eMessageImportance.Information)
        Me.mMsg.Hyperlink = Me.PPath
    End Sub

    Private Sub LogException(strError As String)
        Me.mMsg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_SAVE_EXCEPTION, Me.PPath, strError), _
                               eMessageType.DataExport, eCoreComponentType.Ecosim, eMessageImportance.Warning)
    End Sub

    ''' <summary>
    ''' Record the export of a given CSV file.
    ''' </summary>
    ''' <param name="strName">The data was exported for.</param>
    ''' <param name="strFile">The file the data was exported to.</param>
    ''' <param name="status">Export succes status.</param>
    Private Sub LogExport(strName As String, _
                          strFile As String, _
                          Optional status As eStatusFlags = eStatusFlags.OK)
        If (Me.mMsg Is Nothing) Then Return
        Dim vs As cVariableStatus = Nothing
        Select Case Me.mOutputType
            Case eOutputTypes.CSV
                vs = New cVariableStatus(status, _
                                         cStringUtils.Localize(My.Resources.GENERIC_SAVE_CSV, strName, strFile), _
                                         eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.Ecosim, 0)
            Case eOutputTypes.Excel
                vs = New cVariableStatus(status, _
                                         cStringUtils.Localize(My.Resources.GENERIC_SAVE_EXCEL, strName, strFile), _
                                         eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.Ecosim, 0)
        End Select
        Me.mMsg.AddVariable(vs)
    End Sub

    Private Function CompleteExportMessage() As cMessage
        Dim msg As cMessage = Me.mMsg
        Me.mMsg = Nothing
        Return msg
    End Function

    Private Function IsExcelAccessible() As Boolean
        Try
            Dim ex As New Excel.Application()
            Dim wb As Excel.Workbook = ex.Workbooks.Add()
            Return (wb IsNot Nothing)
        Catch ex As Exception
            m_logger.LogError(ex, "EwEResultExtractorPlugin::IsExcelAccessible")
        End Try
        Return False
    End Function

#End Region ' Message

End Class