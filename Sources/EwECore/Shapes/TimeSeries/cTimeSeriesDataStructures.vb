'==============================================================================
'
' $Log: cTimeSeriesDataStructures.vb,v $
' Revision 1.2  2008/09/26 20:29:00  villyc
' ecosimmontecarlo stuff
'
' Revision 1.1  2008/09/26 07:30:34  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.15  2008/09/23 16:24:21  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.14  2008/08/11 21:13:29  joeb
' Changes for Bug Fix 459 Added Search Modes and changed loading of Forcing Data
'
' Revision 1.13  2008/02/11 03:30:25  jeroens
' Datasets are separate entities now, no longer just defined by name in Time Series
'
' Revision 1.12  2007/10/30 01:19:43  jeroens
' * Fixed endless loop in LoadApplied
'
' Revision 1.11  2007/10/29 15:59:01  jeroens
' * TS datastructures allow individual TS to be applied
'
' Revision 1.10  2007/10/18 22:10:49  joeb
' clearFishForced when loading data
'
' Revision 1.9  2007/10/12 20:35:15  jeroens
' + Added strDatasetName, nDatasetNumTimeSeries
'
' Revision 1.8  2007/10/12 16:30:40  joeb
' Added Exception handling to DoDatValCalcultions
'
' Revision 1.7  2007/10/04 16:17:41  joeb
' Changed a comment
'
' Revision 1.6  2007/09/28 18:52:42  joeb
' changed apply
'
' Revision 1.5  2007/09/24 14:20:59  joeb
' Added   EcosimData.redimFishingRatesToForcingData(NdatYear) when reference data is loaded
'
' Revision 1.4  2007/09/04 17:01:04  jeroens
' * Fixed crash on deleting last TS
'
' Revision 1.3  2007/07/12 16:29:24  jeroens
' + Moved
'
' Revision 1.1  2007/07/12 15:50:02  jeroens
' * Moved
'
' Revision 1.15  2007/06/13 20:37:54  joeb
'  Changed RedimAppliedTimeSeries() from Friend to Public for Villy
'
' Revision 1.14  2007/06/12 20:45:58  joeb
' Fixed a mistake in the last commit to cEcospaceDataStructures
'
' Revision 1.13  2007/06/12 20:44:42  joeb
' Added cEcospaceTimeSeriesDataStructures for ecospace time series data
'
' Revision 1.12  2007/06/11 17:56:46  joeb
' Changed indexing when updating applied data
'
' Revision 1.11  2007/06/11 16:16:24  jeroens
' * DatQ and DatSS ready for the lime light
'
' Revision 1.10  2007/06/11 04:25:13  jeroens
' + Tested Apply
'
' Revision 1.9  2007/06/11 03:23:50  jeroens
' * Implemented Apply
'
' Revision 1.8  2007/06/08 21:09:49  joeb
' AccumulateDataInfo and PlotDataInfo bugs
'
' Revision 1.7  2007/06/05 21:43:46  joeb
' Added DoDatValCalculations()
'
' Revision 1.6  2007/06/05 17:37:40  jeroens
' + Added ApplyAll. THIS IS A TEMPORARY HACK!
'
' Revision 1.5  2007/05/24 16:43:47  joeb
' More forcing/timeseries data moved to cTimeSeriesDataStructures
'
' Revision 1.4  2007/05/23 16:38:47  jeroens
' * Split data in input and applied structures
'
' Revision 1.3  2007/05/22 13:23:41  jeroens
' + Commented
'
'==============================================================================

Option Strict On

''' <summary>
''' Class that holds and manages all time series-related data in the EwE core.
''' </summary>
Public Class cTimeSeriesDataStructures

    ''' <summary>New to EwE6: a custom group-related variable that each time series should apply to.</summary>
    ''' <remarks>
    ''' This flag is added with future extensions to the time series system in mind. With this flag,
    ''' users can design and import time series for any group-related variable, which is nice, but 
    ''' will be fully responsible for writing the code that applies the time series. Hey, we can only
    ''' cater so much! At least there is a generic structure now that allows users to store their
    ''' own time series in the underlying datasource, transparently managed by the EwE core.
    ''' </remarks>
    Public strCustomVariableName() As String

    Public nGroups As Integer = 0

    ' ------------------------------------------------
    ' Dataset structures
    ' ------------------------------------------------

    Public ActiveDatasetIndex As Integer = cCore.NULL_VALUE

    ''' <summary>Number of available datasets.</summary>
    Public nDatasets As Integer = 0
    ''' <summary>Dataset database IDs</summary>
    Public iDatasetDBID() As Integer
    ''' <summary>Names of available datasets.</summary>
    Public strDatasetNames() As String
    ''' <summary>Authors of available datasets.</summary>
    Public strDatasetAuthor() As String
    Public strDatasetContact() As String
    ''' <summary>Descriptions of available datasets.</summary>
    Public strDatasetDescription() As String
    ''' <summary>Number of time series contained in each dataset.</summary>
    Public nDatasetNumTimeSeries() As Integer
    Public nDatasetFirstYear() As Integer
    Public nDatasetNumYears() As Integer

    ' ------------------------------------------------
    ' Interface structures
    ' ------------------------------------------------

    ''' <summary>Number of time series in the model.</summary>
    Public nNumTimeSeries As Integer
    ''' <summary>Maximum number of years across all time series.</summary>
    Public nMaxYears As Integer
    ''' <summary>Database ID for each time series.</summary>
    Public iTimeSeriesDBID() As Integer
    ''' <summary>Name of each time series.</summary>
    Public strName() As String
    '''' <summary>Number of years of each time series.</summary>
    'Public iNumYears() As Integer
    ''' <summary>Array of flags indicating which a time series must be applied.</summary>
    Public bEnable() As Boolean
    ''' <summary>Type of each time series.</summary>
    Public iType() As Integer
    ''' <summary>Index of the core object that each time series links to. The type
    ''' of the core object is implied by <see cref="iType">iType</see>.</summary>
    Public iPool() As Integer
    ''' <summary>Weight type for each time series.</summary>
    Public sWeight() As Single
    '''' <summary>First year of each time series.</summary>
    'Public iFirstYear() As Integer
    ''' <summary>Annual values for each time series, indexed as (iYear, iSeries).</summary>
    Public sValues(,) As Single
    Public sDatSS() As Single
    Public sDatQ() As Single


    ' ------------------------------------------------
    ' Applied structures
    ' ------------------------------------------------

    ''' <summary>Number of applied time series.</summary>
    Public NdatType As Integer
    ''' <summary>Max number of years across all applied time series.</summary>
    Public NdatYear As Integer

    'ToDo_jb change DatType to eTimeSeriesType
    ''' <summary><see cref="eTimeSeriesType">Type</see> of each applied time series.</summary>
    Public DatType() As Integer
    ''' <summary>Index of the core object that each applied time series links to. The type
    ''' of the core object is implied by <see cref="DatType">DatType</see>.</summary>
    Public DatPool() As Integer
    ''' <summary>Weight type for each applied time series.</summary>
    Public WtType() As Single
    ''' <summary>Annual values for each applied time series, indexed as (iYear, iSeries).</summary>
    Public DatVal(,) As Single
    ''' <summary>Start year for each applied time series.</summary>
    Public DatYear() As Integer
    Public DatSS() As Single
    Public DatQ() As Single

    Public PoolForceBB(,) As Single
    Public PoolForceZ(,) As Single
    Public PoolForceCatch(,) As Single

    ''' <summary>
    ''' Index to the current year/datatype
    ''' </summary>
    ''' <remarks>This is increment for each data type each time the stats are collected. Once a year.</remarks>
    Public Iobs As Integer
    Public Wt() As Single

    Public Yhat() As Single
    Public Erpred() As Single

    Friend Sub RedimTimeSeriesDatasets()

        ReDim Me.iDatasetDBID(nDatasets)
        ReDim Me.strDatasetNames(nDatasets)
        ReDim Me.strDatasetDescription(nDatasets)
        ReDim Me.strDatasetAuthor(nDatasets)
        ReDim Me.strDatasetContact(nDatasets)
        ReDim Me.nDatasetFirstYear(nDatasets)
        ReDim Me.nDatasetNumYears(nDatasets)
        ReDim Me.nDatasetNumTimeSeries(nDatasets)

    End Sub

    Friend Sub RedimTimeSeries()

        Debug.Assert(nNumTimeSeries >= 0, Me.ToString & ".RedimTimeSeries() nNumTimeSeries cannot be negative")
        Debug.Assert(nMaxYears >= 0, Me.ToString & ".RedimTimeSeries() NdatYear cannot be negative")

        ' Redim interface time series arrays
        ReDim iTimeSeriesDBID(nNumTimeSeries)
        ReDim strName(nNumTimeSeries)
        ReDim bEnable(nNumTimeSeries)
        ReDim iPool(nNumTimeSeries)
        ReDim sWeight(nNumTimeSeries)
        ReDim iType(nNumTimeSeries)
        ReDim sValues(nMaxYears + 1, nNumTimeSeries)
        ReDim strCustomVariableName(nNumTimeSeries)
        ReDim sDatSS(nNumTimeSeries)
        ReDim sDatQ(nNumTimeSeries)

        ReDim DatSS(nNumTimeSeries)
        ReDim DatQ(nNumTimeSeries)

    End Sub

    Public Sub RedimAppliedTimeSeries()

        Debug.Assert(NdatType >= 0, Me.ToString & ".RedimAppliedTimeSeries() NdatType cannot be negative")
        Debug.Assert(NdatYear >= 0, Me.ToString & ".RedimAppliedTimeSeries() NdatYear cannot be negative")

        ' Redim applied time series arrays
        ReDim DatPool(NdatType)
        ReDim DatType(NdatType)
        ReDim WtType(NdatType)
        ReDim DatVal(NdatYear + 1, NdatType)
        ReDim DatYear(NdatYear)
        ReDim DatSS(NdatType)
        ReDim DatQ(NdatType)

    End Sub

    ''' <summary>
    ''' Redim time series forcing data PoolForceBB(nGroups, nYears),PoolForceZ(nGroups, nYears) and PoolForceCatch(nGroups, nYears)
    ''' </summary>
    ''' <param name="nYears"></param>
    ''' <remarks></remarks>
    Public Sub redimFocingData(ByVal nYears As Integer)

        ReDim PoolForceBB(nGroups, nYears)
        ReDim PoolForceZ(nGroups, nYears)
        ReDim PoolForceCatch(nGroups, nYears)

    End Sub

    ''' <summary>
    ''' Apply all flagged time series to the Ecosim model.
    ''' </summary>
    Friend Sub loadEnabled(Optional ByVal iTSIndex As Integer = -1)

        Dim iTS As Integer = -1
        Dim iTSApply As Integer = -1
        Dim bFound As Boolean = False

        ' Single TS index given?
        If (iTSIndex > 0) Then
            ' Try to reload applied data for a single TS
            iTSApply = 0
            iTS = 0

            ' Determine Applied index 
            While iTS < Math.Min(iTSIndex, nNumTimeSeries)
                ' Try next
                iTS += 1
                ' Is an applied TS?
                If Me.bEnable(iTS) Then
                    ' #Yes: count it
                    iTSApply += 1
                    ' Check if found
                    bFound = (iTSIndex = iTS)
                End If
            End While

            If bFound Then
                ' Sanity check
                If (iTSApply <= NdatType) Then
                    Me.LoadAppliedTS(iTS, iTSApply)
                    Return
                End If
            End If
        End If

        ' Default: reload all applied
        Me.LoadAppliedData()

    End Sub

    Protected Sub LoadAppliedData()
        Dim iTS As Integer = 0
        Dim iYear As Integer = 0
        Dim iTSApply As Integer = 0

        NdatType = 0
        NdatYear = nMaxYears

        ' Determine no. of time series to apply
        For iTS = 1 To nNumTimeSeries
            If Me.bEnable(iTS) Then NdatType += 1
        Next iTS

        RedimAppliedTimeSeries()

        If nNumTimeSeries > 0 Then

            DatYear(1) = Me.nDatasetFirstYear(Me.ActiveDatasetIndex)
            For iYear = 2 To NdatYear
                DatYear(iYear) = DatYear(iYear - 1) + 1
            Next

            For iTS = 1 To nNumTimeSeries
                If Me.bEnable(iTS) Then
                    iTSApply += 1
                    Me.LoadAppliedTS(iTS, iTSApply)
                End If
            Next iTS

        End If

    End Sub

    Private Sub LoadAppliedTS(ByVal iTS As Integer, ByVal iTSApply As Integer)
        Debug.Assert(Me.bEnable(iTS))

        DatPool(iTSApply) = iPool(iTS)
        DatType(iTSApply) = iType(iTS)
        WtType(iTSApply) = sWeight(iTS)
        For iYear As Integer = 0 To NdatYear
            DatVal(iYear, iTSApply) = sValues(iYear, iTS)
        Next iYear
    End Sub

    Friend Sub Update()

        Dim iTS As Integer = 0
        Dim iTSapplied As Integer = 0

        For iTS = 1 To nNumTimeSeries
            If Me.bEnable(iTS) Then
                iTSapplied += 1 'DatSS and DatQ are indexed from one
                sDatSS(iTS) = DatSS(iTSapplied)
                sDatQ(iTS) = DatQ(iTSapplied)

            Else
                sDatSS(iTS) = 0.0!
                sDatQ(iTS) = 0.0!
            End If
        Next iTS

    End Sub


    Public Sub LoadForcingData(ByVal EcosimData As cEcosimDatastructures, ByVal nYears As Integer)
        'Time series forcing data is stored the same way as other time series data
        'but is applied to fixed arrays i.e. PoolForceBB().....
        Try
            Me.loadEnabled()
            Me.redimFocingData(nYears)
            Me.DoDatValCalculations(EcosimData)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Load data from datval() into forcing arrays used by the models. Calculate the 
    ''' </summary>
    ''' <param name="EcosimData"></param>
    ''' <remarks>This needs to be called after the time series data is loaded to update other data arrays.</remarks>
    Public Sub DoDatValCalculations(ByRef EcosimData As cEcosimDatastructures)

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'CAUTION
        'jb Ecosim.SetFFromGear() need to be call after this 
        'this works now because SetFFromGear() gets called when ecosim is initialized after the scenario is loaded
        'if this is moved to the interface SetFFromGear() will no longer be called
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Dim i As Integer
        Dim j As Integer
        Dim K As Integer
        Dim Tim As Integer
        Dim ig As Integer
        'Dim ip As Integer
        Dim HoldIobs As Integer
        HoldIobs = Iobs
        Iobs = 0

        'clear out the FishForced falg
        EcosimData.clearFishForced()

        Try

            For i = 1 To Me.NdatYear
                For j = 1 To NdatType
                    Select Case DatType(j)

                        Case 0, 1

                            If DatVal(i, j) > 0 Then Iobs = Iobs + 1
                            PoolForceBB(DatPool(j), i) = 0

                        Case -1 'pool biomass forcing

                            PoolForceBB(DatPool(j), i) = DatVal(i, j)

                        Case 2 'time forcing data
                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            'jb Time forcing data (Shapes) are handled through the Shape manager and not loaded with the Time series in EwE6
                            'this is the code from EwE5
                            '        If DatPool(j) > ForcingShapes + 3 Then
                            '            ForcingShapes = DatPool(j) - 3
                            'ReDim Preserve ForcingTitle(ForcingShapes) As String
                            'ReDim Preserve SeasonTitle(3) As String
                            'ReDim Preserve zscale(ForcePoints, ForcingShapes + 3) As Single
                            '            ReDim Preserve tval(ForcingShapes + 3)
                            '        End If
                            'If DatPool(j) > 3 And DatPool(j) <= ForcingShapes + 3 Then 'a valid long term shape
                            '    ForcingTitle(DatPool(j) - 3) = DatName(j)
                            '    For K = 1 To 12
                            '        Tim = 12 * (DatYear(i) - DatYear(1)) + K    ': If Tim > 1200 Then Tim = 1200
                            '        zscale(Tim, DatPool(j)) = DatVal(i, j)
                            '    Next
                            'End If
                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        Case 3  'effort data by gear type

                            If DatPool(j) > 0 And DatPool(j) <= EcosimData.nGear Then
                                For K = 1 To 12
                                    Tim = 12 * (DatYear(i) - DatYear(1)) + K    ': If Tim > 1200 Then Tim = 1200
                                    ig = DatPool(j)
                                    EcosimData.FishRateGear(ig, Tim) = DatVal(i, j)
                                Next
                            End If

                        Case 4 'F by pool

                            If DatPool(j) > 0 And DatPool(j) <= nGroups Then
                                EcosimData.FisForced(DatPool(j)) = True
                                For K = 1 To 12
                                    Tim = 12 * (DatYear(i) - DatYear(1)) + K        ': If Tim > 1200 Then Tim = 1200
                                    EcosimData.FishRateNo(DatPool(j), Tim) = DatVal(i, j)
                                    If EcosimData.FishRateMax(DatPool(j)) < EcosimData.FishRateNo(DatPool(j), Tim) Then
                                        EcosimData.FishRateMax(DatPool(j)) = CSng(EcosimData.FishRateNo(DatPool(j), Tim) * 1.01)
                                    End If
                                Next
                                'Also check the fishratemax(pool):
                            End If

                        Case 5, -5 'Z by pool

                            If Math.Abs(DatVal(i, j)) > 0 Then Iobs = Iobs + 1 'now also with forced Z
                            If DatType(j) = -5 Then
                                PoolForceZ(DatPool(j), i) = DatVal(i, j)
                            Else
                                PoolForceZ(DatPool(j), i) = 0
                            End If

                        Case -6, 6 'Catches, -6 is forced
                            If Math.Abs(DatVal(i, j)) > 0 Then Iobs = Iobs + 1 '....Added by SM for Catch Fitting.
                            If DatType(j) = -6 Then
                                PoolForceCatch(DatPool(j), i) = DatVal(i, j)
                            Else
                                PoolForceCatch(DatPool(j), i) = 0
                            End If

                            'Martell playing here!
                        Case 7 'Mean Body Weight data for split pool groups
                            'jb EwE6 does not have split pools! I'm not sure if this also applies to multi stanza groups??
                            If DatVal(i, j) > 0 Then Iobs = Iobs + 1

                        Case Else
                    End Select
                    '      End If 'If IsDatShown(j) = True Then
                Next
            Next
            j = 0
            For i = 1 To NdatType
                If WtType(i) > 0 Then j = j + 1
            Next

            'jb was????? 
            ' If ReadingCsvFile Or j = 0 Then
            If j = 0 Then
                For i = 1 To NdatType
                    If WtType(i) = 0 And (DatType(i) = 0 Or DatType(i) = 1 Or DatType(i) = 5 _
                    Or Math.Abs(DatType(i)) = 6 Or DatType(i) = 7) Then WtType(i) = 1
                Next
            End If

            If Iobs = 0 Then Iobs = HoldIobs

            ReDim Wt(Iobs)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'CAUTION
            'jb Ecosim.SetFFromGear() need to be call after this 
            'this works now because SetFFromGear() gets called when ecosim is initialized after the scenario is loaded
            'if this is moved to the interface SetFFromGear() will no longer be called
            'EwE5 reset fishing rates by group to values predicted from effort except for forced groups
            ' SetFFromGear()
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".DoDatValCalculations(). ", ex)
        End Try

    End Sub


End Class


''' <summary>
''' Time series reference data for Ecospace
''' </summary>
''' <remarks></remarks>
Public Class cEcospaceTimeSeriesDataStructures
    Inherits cTimeSeriesDataStructures

    ' ------------------------------------------------
    ' Interface structures
    ' ------------------------------------------------
    Public iSPRegion() As Integer

    ' ------------------------------------------------
    ' Applied structures used by the models
    ' ------------------------------------------------
    Public SPRegion() As Integer


    Friend Overloads Sub RedimTimeSeries()
        MyBase.RedimTimeSeries()

        ReDim iSPRegion(nNumTimeSeries)

    End Sub


    Friend Overloads Sub RedimAppliedTimeSeries()
        MyBase.RedimAppliedTimeSeries()

        ReDim iSPRegion(NdatType)

    End Sub

    ''' <summary>
    ''' EwE5 DoSpaceDatValCalculations
    ''' </summary>
    ''' <remarks></remarks>
    Friend Overloads Sub DoDatValCalculations(ByRef EcospaceData As cEcospaceDataStructures)

    End Sub

    ''' <summary>
    ''' Apply all flagged time series to the Ecosim model.
    ''' </summary>
    Friend Overloads Sub Apply(ByRef EcospaceData As cEcospaceDataStructures)

        'load the the user selected data into the data used by the model
        MyBase.LoadAppliedData()

        'load the data from datval() into the ecosim data 
        DoDatValCalculations(EcospaceData)

    End Sub

End Class
