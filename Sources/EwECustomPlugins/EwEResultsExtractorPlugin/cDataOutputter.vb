
Imports Microsoft.Office.Interop
Imports System.IO

Public Class cDataOutputer

    Private mFunctionalGroupData As List(Of cDataSheet)
    Private mFisheriesData As List(Of cDataSheet)
    Private mIndicators As List(Of cDataSheet)
    Private mDiagnostics As List(Of cDataSheet)
    Private mOutputType As String
    Private mStrPath As String
    Private mNDataItems As Integer

    Public Sub New()
        mFunctionalGroupData = New List(Of cDataSheet)
        mFisheriesData = New List(Of cDataSheet)
        mIndicators = New List(Of cDataSheet)
        mDiagnostics = New List(Of cDataSheet)

    End Sub

    'List containing all objects for each option selected that is a functional group
    Public Sub AddFunctionalGroup(ByRef Group As cDataSheet)
        mFunctionalGroupData.Add(Group)
        mNDataItems += 1
    End Sub

    'List containing all objects for each option selected that is a Fishery group
    Public Sub AddFisheries(ByRef Fisheries As cDataSheet)
        mFisheriesData.Add(Fisheries)
        mNDataItems += 1
    End Sub

    'List containing all objects for each option selected that is a indicator group
    Public Sub AddIndicators(ByRef Indicator As cDataSheet)
        mIndicators.Add(Indicator)
        mNDataItems += 1
    End Sub

    Public Sub AddDiagnostics(ByRef Diagnostics As cDataSheet)
        mDiagnostics.Add(Diagnostics)
        mNDataItems += 1
    End Sub

    'Property that sets what file type to output
    Public Property POutputType() As String
        Get
            Return mOutputType
        End Get
        Set(ByVal value As String)
            If value = "csv" Then
                mOutputType = value
            ElseIf value = "excel" Then
                mOutputType = value
            Else
                MsgBox("Attempt by a client object to send and invalid file type to dataoutputter", MsgBoxStyle.Critical)
            End If
        End Set
    End Property

    'Sets and returns the directory path that the user wants to save data files to
    Public Property PPath() As String
        Get
            Return mStrPath
        End Get
        Set(ByVal value As String)
            mStrPath = value
        End Set
    End Property

    'Returns the number of data items that are held by the dataoutputer
    Public ReadOnly Property GetNumDataItems() As Integer
        Get
            Return mNDataItems
        End Get
    End Property

    'This is the subroutine that the client calls to output the data
    Public Sub OutputData()

        If mOutputType = "csv" Then
            CreateCSVFiles()
        ElseIf mOutputType = "excel" Then
            CreateExcelFiles()
        End If

    End Sub

    Private Sub CreateCSVFiles()

        Dim fileName As String
        Dim fDateTime As DateTime = DateTime.Now
        Dim CurrentTime As String = "(D" & fDateTime.Day & "-" & fDateTime.Month & "-" & fDateTime.Year & ")(T" & _
        fDateTime.Hour.ToString & "-" & fDateTime.Minute.ToString & "-" _
        & fDateTime.Second.ToString & ")"
        Dim ArrayData(,) As Object
        Dim DataItem As String

        'Create the functional group files
        If mFunctionalGroupData.Count > 0 Then
            For Each i In mFunctionalGroupData
                fileName = i.Name & CurrentTime & ".csv"
                Dim sw As StreamWriter = New StreamWriter(mStrPath & "\" & fileName, False)
                ArrayData = CType(i.Data, Array)
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = "" & ArrayData(x, y) & ""
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
            Next
        End If

        'Create the fishery group files
        If mFisheriesData.Count > 0 Then
            For Each i In mFisheriesData
                fileName = i.Name & CurrentTime & ".csv"
                Dim sw As StreamWriter = New StreamWriter(mStrPath & "\" & fileName, False)
                ArrayData = CType(i.Data, Array)
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = "" & ArrayData(x, y) & ""
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
            Next
        End If

        'Create the indicator files
        If mIndicators.Count > 0 Then
            For Each i In mIndicators
                fileName = i.Name & CurrentTime & ".csv"
                Dim sw As StreamWriter = New StreamWriter(mStrPath & "\" & fileName, False)
                ArrayData = CType(i.Data, Array)
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = """" & ArrayData(x, y) & """"
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
            Next
        End If

        'Create the indicator files
        If mDiagnostics.Count > 0 Then
            For Each i In mDiagnostics
                fileName = i.Name & CurrentTime & ".csv"
                Dim sw As StreamWriter = New StreamWriter(mStrPath & "\" & fileName, False)
                ArrayData = CType(i.Data, Array)
                For y = 0 To ArrayData.GetLength(1) - 1
                    For x = 0 To ArrayData.GetLength(0) - 1
                        DataItem = """" & ArrayData(x, y) & """"
                        sw.Write(DataItem)
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next
                sw.Close()
            Next
        End If

    End Sub

    Private Sub CreateExcelFiles()


        Dim ex As New Excel.Application
        Dim FileExists As Boolean = False
        Dim DirectInfo As New DirectoryInfo(mStrPath)
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


        If mFunctionalGroupData.Count > 0 Then
            fileName = "FunctGroup" & CurrentTime
            FunctionalWB = ex.Workbooks.Add()
            For Each i In mFunctionalGroupData
                sheet = FunctionalWB.Worksheets.Add()
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Array)
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
            Next
            FunctionalWB.SaveAs(mStrPath & "\" & fileName)

        End If

        If mFisheriesData.Count > 0 Then
            fileName = "Fisheries" & CurrentTime
            FisheriesWB = ex.Workbooks.Add()
            For Each i In mFisheriesData
                sheet = FisheriesWB.Worksheets.Add()
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Array)
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
            Next
            FisheriesWB.SaveAs(mStrPath & "\" & fileName)
        End If

        If mIndicators.Count > 0 Then
            fileName = "Indicators" & CurrentTime
            IndicatorsWB = ex.Workbooks.Add()
            For Each i In mIndicators
                sheet = IndicatorsWB.Worksheets.Add()
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Array)
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
            Next
            IndicatorsWB.SaveAs(mStrPath & "\" & fileName)
        End If

        If mDiagnostics.Count > 0 Then
            fileName = "Diagnostics" & CurrentTime
            DiagnosticsWB = ex.Workbooks.Add()
            For Each i In mDiagnostics
                sheet = DiagnosticsWB.Worksheets.Add()
                sheet.Name = i.Name
                ArrayData = CType(i.Data, Array)
                For x = 0 To ArrayData.GetLength(0) - 1
                    For y = 0 To ArrayData.GetLength(1) - 1
                        sheet.Cells(y + 1, x + 1) = ArrayData(x, y)
                    Next
                Next
            Next
            DiagnosticsWB.SaveAs(mStrPath & "\" & fileName)
        End If

        FunctionalWB = Nothing
        FisheriesWB = Nothing
        IndicatorsWB = Nothing
        DiagnosticsWB = Nothing
        ex.Quit()

    End Sub


End Class