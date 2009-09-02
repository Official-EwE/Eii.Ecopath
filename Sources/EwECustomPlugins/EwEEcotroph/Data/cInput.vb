Option Explicit On
Option Strict On

Imports System.IO
Imports System.Globalization

Public Class cInput

#Region "Private fields"
    Private Const TL_OUT_INIT As Double = 2.0
    Private Const TL_OUT_FINAL As Double = 7.0
    Private Const TL_INCRM As Double = 0.1
    Private Const NUM_KINETIC_PARAMETER As Integer = 3
    Private Const NUM_EFFORT_MULTIPLIER As Integer = 11

    Private m_ni As NumberFormatInfo = Nothing

#Region "Transpose"
    Private m_SmoothFactor As Single
    Private m_Sigma() As Single
    Private m_Access() As Single
#End Region 'Transpose
#Region "CTSA"
    Private m_CTSANumTL As Integer
    Private m_WaterTemp As Single
    Private m_TETL12 As Single
    Private m_TETL2 As Single
    Private m_CTSATopD() As Single
    Private m_CTSAFormD() As Single
    Private m_Asymptote As Single
    Private m_TL50 As Single
    Private m_Slope As Single
    Private m_Catches() As Single
    Private m_KineticParameter() As Single
    Private m_SeedNameFwdCal As String
    Private m_SeedValueFwdCal As Single
    Private m_TTL As Single 'Terminal TL in CTSA backward calculation
    Private m_SeedNameBwdCal As String
    Private m_SeedValueBwdCal As Single
    'Private m_SlopeSelectivityTTL As Single
    Private m_TransposeAlgorImport As String
    Private m_SmoothFactorImport As Single
    Private m_NumFleetImport As Integer
    Private m_NumLivingImport As Integer
    Private m_NumGroupImport As Integer
    Private m_TLImport() As Single
    Private m_SigmaImport() As Single
    Private m_CatchesImport(,) As Single
#End Region 'CTSA
#Region "Diagnosis"
    Private m_DiagnosisNumTL As Integer
    Private m_DiagnosisTopD() As Single
    Private m_DiagnosisFormD() As Single
    Private m_DiagnosisBeta As Single
    Private m_EffortMultiplier() As Single
#End Region 'Diagnosis
#Region "Dynamics"
    Private m_DynamicsNumTL As Integer
    Private m_DynamicsTopD() As Single
    Private m_DynamicsFormD() As Single
    Private m_DynamicsBeta As Single
    Private m_ReferenceYear As Integer
    Private m_NumForecastYear As Integer
    Private m_IndexPPForecast() As Single
    Private m_CatchMultiplier() As Single
    Private m_PastAnalysisYear() As Integer
    Private m_NumPastAnalysisYear As Integer
    Private m_IndexPPPastAnalysis() As Single
    Private m_CatchPastAnalysis(,) As Single
#End Region 'Dynamics
#End Region 'Private fields

#Region " Constructor "

    Public Sub New()
        ' Culturization ;)
        Dim ci As CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-us")
        Me.m_ni = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)
        Me.m_ni.NumberDecimalSeparator = "."
    End Sub

#End Region ' Constructor

#Region "Public properties"
#Region "Transpose"
    Public Property SmoothFactor() As Single
        Get
            Return m_SmoothFactor
        End Get
        Set(ByVal value As Single)
            m_SmoothFactor = value
        End Set
    End Property

    Public Property Sigma(ByVal Index As Integer) As Single
        Get
            Return m_Sigma(Index)
        End Get
        Set(ByVal value As Single)
            m_Sigma(Index) = value
        End Set
    End Property

    Public Property Access(ByVal Index As Integer) As Single
        Get
            Return m_Access(Index)
        End Get
        Set(ByVal value As Single)
            m_Access(Index) = value
        End Set
    End Property
#End Region 'Transpose
#Region "CTSA"
    Public Property WaterTemp() As Single
        Get
            Return m_WaterTemp
        End Get
        Set(ByVal value As Single)
            m_WaterTemp = value
        End Set
    End Property

    Public Property TETL12() As Single
        Get
            Return m_TETL12
        End Get
        Set(ByVal value As Single)
            m_TETL12 = value
        End Set
    End Property

    Public Property TETL2() As Single
        Get
            Return m_TETL2
        End Get
        Set(ByVal value As Single)
            m_TETL2 = value
        End Set
    End Property

    Public Property CTSATopD() As Single()
        Get
            Return m_CTSATopD
        End Get
        Set(ByVal value As Single())
            m_CTSATopD = value
        End Set
    End Property

    Public Property CTSAFormD() As Single()
        Get
            Return m_CTSAFormD
        End Get
        Set(ByVal value As Single())
            m_CTSAFormD = value
        End Set
    End Property

    Public Property Asymptote() As Single
        Get
            Return m_Asymptote
        End Get
        Set(ByVal value As Single)
            m_Asymptote = value
        End Set
    End Property

    Public Property TL50() As Single
        Get
            Return m_TL50
        End Get
        Set(ByVal value As Single)
            m_TL50 = value
        End Set
    End Property

    Public Property Slope() As Single
        Get
            Return m_Slope
        End Get
        Set(ByVal value As Single)
            m_Slope = value
        End Set
    End Property

    'Public Property Catches(ByVal Index As Integer) As Single
    '    Get
    '        Return m_Catches(Index)
    '    End Get
    '    Set(ByVal value As Single)
    '        m_Catches(Index) = value
    '    End Set
    'End Property

    Public Property Catches() As Single()
        Get
            Return m_Catches
        End Get
        Set(ByVal value As Single())
            m_Catches = value
        End Set
    End Property

    Public Property KineticParameter(ByVal Index As Integer) As Single
        Get
            Return m_KineticParameter(Index)
        End Get
        Set(ByVal value As Single)
            m_KineticParameter(Index) = value
        End Set
    End Property

    Public Property SeedNameFwdCal() As String
        Get
            Return m_SeedNameFwdCal
        End Get
        Set(ByVal value As String)
            m_SeedNameFwdCal = value
        End Set
    End Property

    Public Property SeedValueFwdCal() As Single
        Get
            Return m_SeedValueFwdCal
        End Get
        Set(ByVal value As Single)
            m_SeedValueFwdCal = value
        End Set
    End Property

    Public Property TTL() As Single
        Get
            Return m_TTL
        End Get
        Set(ByVal value As Single)
            m_TTL = value
        End Set
    End Property

    'Public Property SlopeSelectivityTTL() As Single
    '    Get
    '        Return m_SlopeSelectivityTTL
    '    End Get
    '    Set(ByVal value As Single)
    '        m_SlopeSelectivityTTL = value
    '    End Set
    'End Property

    Public Property SeedNameBwdCal() As String
        Get
            Return m_SeedNameBwdCal
        End Get
        Set(ByVal value As String)
            m_SeedNameBwdCal = value
        End Set
    End Property

    Public Property SeedValueBwdCal() As Single
        Get
            Return m_SeedValueBwdCal
        End Get
        Set(ByVal value As Single)
            m_SeedValueBwdCal = value
        End Set
    End Property

    Public Property TransposeAlgorImport() As String
        Get
            Return m_TransposeAlgorImport
        End Get
        Set(ByVal value As String)
            m_TransposeAlgorImport = value
        End Set
    End Property

    Public Property SmoothFactorImport() As Single
        Get
            Return m_SmoothFactorImport
        End Get
        Set(ByVal value As Single)
            m_SmoothFactorImport = value
        End Set
    End Property

    Public Property NumFleetImport() As Integer
        Get
            Return m_NumFleetImport
        End Get
        Set(ByVal value As Integer)
            m_NumFleetImport = value
        End Set
    End Property

    Public Property NumLivingImport() As Integer
        Get
            Return m_NumLivingImport
        End Get
        Set(ByVal value As Integer)
            m_NumLivingImport = value
        End Set
    End Property

    Public Property NumGroupImport() As Integer
        Get
            Return m_NumGroupImport
        End Get
        Set(ByVal value As Integer)
            m_NumGroupImport = value
        End Set
    End Property

    Public Property TLImport() As Single()
        Get
            Return m_TLImport
        End Get
        Set(ByVal value As Single())
            m_TLImport = value
        End Set
    End Property

    Public Property SigmaImport() As Single()
        Get
            Return m_SigmaImport
        End Get
        Set(ByVal value As Single())
            m_SigmaImport = value
        End Set
    End Property

    Public Property CatchesImport() As Single(,)
        Get
            Return m_CatchesImport
        End Get
        Set(ByVal value As Single(,))
            m_CatchesImport = value
        End Set
    End Property
#End Region 'CTSA
#Region "Diagnosis"
    Public Property DiagnosisTopD() As Single()
        Get
            Return m_DiagnosisTopD
        End Get
        Set(ByVal value As Single())
            m_DiagnosisTopD = value
        End Set
    End Property

    Public Property DiagnosisFormD() As Single()
        Get
            Return m_DiagnosisFormD
        End Get
        Set(ByVal value As Single())
            m_DiagnosisFormD = value
        End Set
    End Property

    Public Property DiagnosisBeta() As Single
        Get
            Return m_DiagnosisBeta
        End Get
        Set(ByVal value As Single)
            m_DiagnosisBeta = value
        End Set
    End Property

    Public Property EffortMultiplier() As Single()
        Get
            Return m_EffortMultiplier
        End Get
        Set(ByVal value As Single())
            m_EffortMultiplier = value
        End Set
    End Property
#End Region 'Diagnosis
#Region "Dynamics"
    Public Property DynamicsTopD() As Single()
        Get
            Return m_DynamicsTopD
        End Get
        Set(ByVal value As Single())
            m_DynamicsTopD = value
        End Set
    End Property

    Public Property DynamicsFormD() As Single()
        Get
            Return m_DynamicsFormD
        End Get
        Set(ByVal value As Single())
            m_DynamicsFormD = value
        End Set
    End Property

    Public Property DynamicsBeta() As Single
        Get
            Return m_DynamicsBeta
        End Get
        Set(ByVal value As Single)
            m_DynamicsBeta = value
        End Set
    End Property

    Public Property ReferenceYear() As Integer
        Get
            Return m_ReferenceYear
        End Get
        Set(ByVal value As Integer)
            m_ReferenceYear = value
        End Set
    End Property

    Public Property NumForecastYear() As Integer
        Get
            Return m_NumForecastYear
        End Get
        Set(ByVal value As Integer)
            m_NumForecastYear = value
        End Set
    End Property

    Public Property IndexPPForecast() As Single()
        Get
            Return m_IndexPPForecast
        End Get
        Set(ByVal value As Single())
            m_IndexPPForecast = value
        End Set
    End Property

    Public Property CatchMultiplier() As Single()
        Get
            Return m_CatchMultiplier
        End Get
        Set(ByVal value As Single())
            m_CatchMultiplier = value
        End Set
    End Property

    Public Property PastAnalysisYear() As Integer()
        Get
            Return m_PastAnalysisYear
        End Get
        Set(ByVal value As Integer())
            m_PastAnalysisYear = value
        End Set
    End Property

    Public Property NumPastAnalysisYear() As Integer
        Get
            Return m_NumPastAnalysisYear
        End Get
        Set(ByVal value As Integer)
            m_NumPastAnalysisYear = value
        End Set
    End Property

    Public Property IndexPPPastAnalysis() As Single()
        Get
            Return m_IndexPPPastAnalysis
        End Get
        Set(ByVal value As Single())
            m_IndexPPPastAnalysis = value
        End Set
    End Property

    Public Property CatchPastAnalysis() As Single(,)
        Get
            Return m_CatchPastAnalysis
        End Get
        Set(ByVal value As Single(,))
            m_CatchPastAnalysis = value
        End Set
    End Property
#End Region 'Dynamics
#End Region 'Public properties

#Region "Public methods"
    Public Sub WriteFile(ByVal FileName As String, ByVal EcotrophManager As cEcotrophManager)
        Dim FileDir As String
        Dim ModelName As String
        Dim ModifiedFileName As String
        Dim Writer As StreamWriter

        FileDir = Path.GetTempPath
        ModelName = EcotrophManager.CoreData.EwEModel.Name.Replace("/", "s")
        ModifiedFileName = FileName & ModelName & ".txt"
        Writer = File.CreateText(FileDir & ModifiedFileName)
        Select Case FileName
            Case "SmoothFactor"
                Writer.WriteLine(m_SmoothFactor)
            Case "Sigma"
                For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
                    Writer.WriteLine(m_Sigma(Idx))
                Next
            Case "Access"
                For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
                    Writer.WriteLine(m_Access(Idx))
                Next
            Case "CTSAParameter"
                Writer.WriteLine(m_WaterTemp)
                Writer.WriteLine(m_TETL12)
                Writer.WriteLine(m_TETL2)
                For Idx As Integer = 1 To m_CTSANumTL
                    Writer.WriteLine(m_CTSATopD(Idx))
                Next
                For Idx As Integer = 1 To m_CTSANumTL
                    Writer.WriteLine(m_CTSAFormD(Idx))
                Next
                Writer.WriteLine(m_Asymptote)
                Writer.WriteLine(m_TL50)
                Writer.WriteLine(m_Slope)
                For Idx As Integer = 1 To m_CTSANumTL
                    Writer.WriteLine(m_Catches(Idx))
                Next
            Case "KineticParameter"
                For Idx As Integer = 1 To NUM_KINETIC_PARAMETER
                    Writer.WriteLine(m_KineticParameter(Idx))
                Next
            Case "CTSACatches"
                'Plugin does not modify CTSACatches file
            Case "CTSAFwdCalParameter"
                Writer.WriteLine(m_SeedNameFwdCal)
                Writer.WriteLine(m_SeedValueFwdCal)
            Case "CTSABwdCalParameter"
                Writer.WriteLine(m_TTL)
                'Writer.WriteLine(m_SlopeSelectivityTTL)
                Writer.WriteLine(m_SeedNameBwdCal)
                Writer.WriteLine(m_SeedValueBwdCal)
            Case "DiagnosisParameter"
                For Idx As Integer = 1 To m_DiagnosisNumTL
                    Writer.WriteLine(m_DiagnosisTopD(Idx))
                Next
                For Idx As Integer = 1 To m_DiagnosisNumTL
                    Writer.WriteLine(m_DiagnosisFormD(Idx))
                Next
                Writer.WriteLine(m_DiagnosisBeta)
            Case "EffortMultiplier"
                For Idx As Integer = 1 To NUM_EFFORT_MULTIPLIER
                    Writer.WriteLine(m_EffortMultiplier(Idx))
                Next
            Case "DynamicsParameter"
                For Idx As Integer = 1 To m_DynamicsNumTL
                    Writer.WriteLine(m_DynamicsTopD(Idx))
                Next
                For Idx As Integer = 1 To m_DynamicsNumTL
                    Writer.WriteLine(m_DynamicsFormD(Idx))
                Next
                Writer.WriteLine(m_DynamicsBeta)
            Case "ForecastYear"
                Writer.WriteLine(m_ReferenceYear)
                Writer.WriteLine(m_NumForecastYear)
            Case "IndexPPForecast"
                For Idx As Integer = 1 To m_NumForecastYear
                    Writer.WriteLine(m_IndexPPForecast(Idx))
                Next
            Case "CatchMultiplier"
                For Idx As Integer = 1 To m_NumForecastYear
                    Writer.WriteLine(m_CatchMultiplier(Idx))
                Next
            Case "IndexPPPastAnalysis"
                For Idx As Integer = 1 To m_NumPastAnalysisYear
                    Writer.WriteLine(m_IndexPPPastAnalysis(Idx))
                Next
            Case "CatchPastAnalysis"
                'Plugin does not modify CatchPastAnalysis file
        End Select
        Writer.Close()
    End Sub

    Public Function ReadFile(ByVal FileName As String, ByVal EcotrophManager As cEcotrophManager, _
    Optional ByVal CatchFilePath As String = "") As Boolean
        Dim FileDir As String
        Dim ModelName As String
        Dim ModifiedFileName As String
        Dim FilePath As String
        Dim Reader As StreamReader
        Dim LineItems() As String

        If CatchFilePath = "" Then
            FileDir = Path.GetTempPath
            ModelName = EcotrophManager.CoreData.EwEModel.Name.Replace("/", "s")
            ModifiedFileName = FileName & ModelName & ".txt"
            FilePath = FileDir & ModifiedFileName
        Else
            FilePath = CatchFilePath
        End If

        Try
            If File.Exists(FilePath) Then
                Reader = File.OpenText(FilePath)
                Select Case FileName
                    Case "SmoothFactor"
                        m_SmoothFactor = Single.Parse(Reader.ReadLine, Me.m_ni)
                    Case "Sigma"
                        ReDim m_Sigma(EcotrophManager.EcopathData.NumGroups)

                        For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
                            m_Sigma(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "Access"
                        ReDim m_Access(EcotrophManager.EcopathData.NumGroups)

                        For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
                            m_Access(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "CTSAParameter"
                        m_CTSANumTL = 1
                        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            m_CTSANumTL = m_CTSANumTL + 1
                        Next
                        ReDim m_CTSATopD(m_CTSANumTL)
                        ReDim m_CTSAFormD(m_CTSANumTL)
                        ReDim m_Catches(m_CTSANumTL)

                        m_WaterTemp = Single.Parse(Reader.ReadLine, Me.m_ni)
                        m_TETL12 = Single.Parse(Reader.ReadLine, Me.m_ni)
                        m_TETL2 = Single.Parse(Reader.ReadLine, Me.m_ni)
                        For Idx As Integer = 1 To m_CTSANumTL
                            m_CTSATopD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        For Idx As Integer = 1 To m_CTSANumTL
                            m_CTSAFormD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        m_Asymptote = Single.Parse(Reader.ReadLine, Me.m_ni)
                        m_TL50 = Single.Parse(Reader.ReadLine, Me.m_ni)
                        m_Slope = Single.Parse(Reader.ReadLine, Me.m_ni)
                        For Idx As Integer = 1 To m_CTSANumTL
                            m_Catches(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "KineticParameter"
                        ReDim m_KineticParameter(NUM_KINETIC_PARAMETER)

                        For Idx As Integer = 1 To NUM_KINETIC_PARAMETER
                            m_KineticParameter(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "CTSACatches"
                        LineItems = ItemsPerLine(Reader, 1)
                        m_TransposeAlgorImport = LineItems(1)

                        Select Case m_TransposeAlgorImport
                            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                                LineItems = ItemsPerLine(Reader, 2)
                                m_SmoothFactorImport = Single.Parse(LineItems(1), Me.m_ni)
                                LineItems = ItemsPerLine(Reader, 3)
                                m_NumFleetImport = LineItems.GetUpperBound(0) - 2
                                m_NumGroupImport = NumberLinePerFile(Reader) - 2
                                m_NumLivingImport = m_NumGroupImport - 1

                                ReDim m_TLImport(m_NumGroupImport)
                                ReDim m_CatchesImport(m_NumFleetImport, m_NumLivingImport)
                                For LineNum As Integer = 3 To m_NumGroupImport + 2
                                    LineItems = ItemsPerLine(Reader, LineNum)
                                    m_TLImport(LineNum - 2) = Single.Parse(LineItems(2), Me.m_ni)
                                    If LineNum < m_NumGroupImport + 2 Then
                                        For FleetNum As Integer = 1 To m_NumFleetImport
                                            m_CatchesImport(FleetNum, LineNum - 2) = Single.Parse(LineItems(FleetNum + 2), Me.m_ni)
                                        Next
                                    End If
                                Next
                            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                                LineItems = ItemsPerLine(Reader, 2)
                                m_NumFleetImport = LineItems.GetUpperBound(0) - 3
                                m_NumGroupImport = NumberLinePerFile(Reader) - 1
                                m_NumLivingImport = m_NumGroupImport - 1

                                ReDim m_TLImport(m_NumGroupImport)
                                ReDim m_CatchesImport(m_NumFleetImport, m_NumLivingImport)
                                ReDim m_SigmaImport(m_NumLivingImport)
                                For LineNum As Integer = 2 To m_NumGroupImport + 1
                                    LineItems = ItemsPerLine(Reader, LineNum)
                                    m_TLImport(LineNum - 1) = Single.Parse(LineItems(2), Me.m_ni)
                                    If LineNum < m_NumGroupImport + 1 Then
                                        For FleetNum As Integer = 1 To m_NumFleetImport
                                            m_CatchesImport(FleetNum, LineNum - 1) = Single.Parse(LineItems(FleetNum + 2), Me.m_ni)
                                        Next
                                        m_SigmaImport(LineNum - 1) = Single.Parse(LineItems(LineItems.GetUpperBound(0)), Me.m_ni)
                                    End If
                                Next
                            Case Else
                                'Do not read file further
                        End Select
                    Case "CTSAFwdCalParameter"
                        m_SeedNameFwdCal = Reader.ReadLine
                        m_SeedValueFwdCal = Single.Parse(Reader.ReadLine, Me.m_ni)
                    Case "CTSABwdCalParameter"
                        m_TTL = Single.Parse(Reader.ReadLine, Me.m_ni)
                        'm_SlopeSelectivityTTL = Single.Parse(Reader.ReadLine, Me.m_ni)
                        m_SeedNameBwdCal = Reader.ReadLine
                        m_SeedValueBwdCal = Single.Parse(Reader.ReadLine, Me.m_ni)
                    Case "DiagnosisParameter"
                        m_DiagnosisNumTL = 1
                        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            m_DiagnosisNumTL = m_DiagnosisNumTL + 1
                        Next
                        ReDim m_DiagnosisTopD(m_DiagnosisNumTL)
                        ReDim m_DiagnosisFormD(m_DiagnosisNumTL)

                        For Idx As Integer = 1 To m_DiagnosisNumTL
                            m_DiagnosisTopD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        For Idx As Integer = 1 To m_DiagnosisNumTL
                            m_DiagnosisFormD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        m_DiagnosisBeta = Single.Parse(Reader.ReadLine, Me.m_ni)
                    Case "EffortMultiplier"
                        ReDim m_EffortMultiplier(NUM_EFFORT_MULTIPLIER)

                        For Idx As Integer = 1 To NUM_EFFORT_MULTIPLIER
                            m_EffortMultiplier(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "DynamicsParameter"
                        m_DynamicsNumTL = 1
                        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            m_DynamicsNumTL = m_DynamicsNumTL + 1
                        Next
                        ReDim m_DynamicsTopD(m_DynamicsNumTL)
                        ReDim m_DynamicsFormD(m_DynamicsNumTL)

                        For Idx As Integer = 1 To m_DynamicsNumTL
                            m_DynamicsTopD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        For Idx As Integer = 1 To m_DynamicsNumTL
                            m_DynamicsFormD(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                        m_DynamicsBeta = Single.Parse(Reader.ReadLine, Me.m_ni)
                    Case "ForecastYear"
                        m_ReferenceYear = Integer.Parse(Reader.ReadLine, Me.m_ni)
                        m_NumForecastYear = Integer.Parse(Reader.ReadLine, Me.m_ni)
                    Case "CatchPastAnalysis"
                        LineItems = ItemsPerLine(Reader, 1)
                        m_TransposeAlgorImport = LineItems(1)

                        Select Case m_TransposeAlgorImport
                            Case My.Resources.TREE_NODE_AUTO_SMOOTH
                                LineItems = ItemsPerLine(Reader, 2)
                                m_SmoothFactorImport = Single.Parse(LineItems(1), Me.m_ni)
                                LineItems = ItemsPerLine(Reader, 3)
                                m_NumPastAnalysisYear = LineItems.GetUpperBound(0)
                                ReDim m_PastAnalysisYear(m_NumPastAnalysisYear)
                                For YearNum As Integer = 1 To m_NumPastAnalysisYear
                                    m_PastAnalysisYear(YearNum) = Integer.Parse(LineItems(YearNum), Me.m_ni)
                                Next
                                m_NumFleetImport = 1
                                m_NumGroupImport = NumberLinePerFile(Reader) - 3
                                m_NumLivingImport = m_NumGroupImport - 1

                                ReDim m_TLImport(m_NumGroupImport)
                                ReDim m_CatchesImport(m_NumFleetImport, m_NumLivingImport)
                                ReDim m_CatchPastAnalysis(EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1, m_NumPastAnalysisYear)
                                For YearNum As Integer = 1 To m_NumPastAnalysisYear
                                    'Read catches(TL) one year at a time
                                    For LineNum As Integer = 4 To m_NumGroupImport + 3
                                        LineItems = ItemsPerLine(Reader, LineNum)
                                        m_TLImport(LineNum - 3) = Single.Parse(LineItems(2), Me.m_ni)
                                        If LineNum < m_NumGroupImport + 3 Then
                                            m_CatchesImport(m_NumFleetImport, LineNum - 3) = Single.Parse(LineItems(YearNum + 2), Me.m_ni)
                                        End If
                                    Next
                                    'Transpose catches -> EcotrophManager.AEFCatches
                                    ConnectToComputation.cTranspose.RunTransposeAEFCatches()
                                    'For Row As Integer = 1 To EcotrophManager.AEFCatches.GetUpperBound(0)
                                    '    Console.WriteLine(EcotrophManager.AEFCatches(Row).ToString("F4"))
                                    'Next
                                    'Interpolate transposed catches -> EcotrophManager.DynamicsIntrpCatches
                                    ConnectToComputation.cDynamics.RunDynamicsCatches(My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH)
                                    'For Row As Integer = 1 To EcotrophManager.DynamicsIntrpCatches.GetUpperBound(0)
                                    '    Console.WriteLine(EcotrophManager.DynamicsIntrpCatches(Row).ToString("F4"))
                                    'Next
                                    'Set m_CatchPastAnalysis
                                    For Row As Integer = 1 To EcotrophManager.DynamicsIntrpCatches.GetUpperBound(0)
                                        m_CatchPastAnalysis(Row, YearNum) = EcotrophManager.DynamicsIntrpCatches(Row)
                                    Next
                                Next
                            Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                                LineItems = ItemsPerLine(Reader, 2)
                                m_NumPastAnalysisYear = LineItems.GetUpperBound(0)
                                ReDim m_PastAnalysisYear(m_NumPastAnalysisYear)
                                For YearNum As Integer = 1 To m_NumPastAnalysisYear
                                    m_PastAnalysisYear(YearNum) = Integer.Parse(LineItems(YearNum), Me.m_ni)
                                Next
                                m_NumFleetImport = 1
                                m_NumGroupImport = NumberLinePerFile(Reader) - 2
                                m_NumLivingImport = m_NumGroupImport - 1

                                ReDim m_TLImport(m_NumGroupImport)
                                ReDim m_CatchesImport(m_NumFleetImport, m_NumLivingImport)
                                ReDim m_SigmaImport(m_NumLivingImport)
                                ReDim m_CatchPastAnalysis(EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1, m_NumPastAnalysisYear)
                                For YearNum As Integer = 1 To m_NumPastAnalysisYear
                                    'Read catches(TL) one year at a time
                                    For LineNum As Integer = 3 To m_NumGroupImport + 2
                                        LineItems = ItemsPerLine(Reader, LineNum)
                                        m_TLImport(LineNum - 2) = Single.Parse(LineItems(2), Me.m_ni)
                                        If LineNum < m_NumGroupImport + 2 Then
                                            m_CatchesImport(m_NumFleetImport, LineNum - 2) = Single.Parse(LineItems(YearNum + 2), Me.m_ni)
                                            m_SigmaImport(LineNum - 2) = Single.Parse(LineItems(LineItems.GetUpperBound(0)), Me.m_ni)
                                        End If
                                    Next
                                    'Transpose catches -> EcotrophManager.UserDefValCatches
                                    ConnectToComputation.cTranspose.RunTransposeUserDefValCatches()
                                    'For Row As Integer = 1 To EcotrophManager.UserDefValCatches.GetUpperBound(0)
                                    '    Console.WriteLine(EcotrophManager.UserDefValCatches(Row).ToString("F4"))
                                    'Next
                                    'Interpolate transposed catches -> EcotrophManager.DynamicsIntrpCatches
                                    ConnectToComputation.cDynamics.RunDynamicsCatches(My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA)
                                    'For Row As Integer = 1 To EcotrophManager.DynamicsIntrpCatches.GetUpperBound(0)
                                    '    Console.WriteLine(EcotrophManager.DynamicsIntrpCatches(Row).ToString("F4"))
                                    'Next
                                    'Set m_CatchPastAnalysis
                                    For Row As Integer = 1 To EcotrophManager.DynamicsIntrpCatches.GetUpperBound(0)
                                        m_CatchPastAnalysis(Row, YearNum) = EcotrophManager.DynamicsIntrpCatches(Row)
                                    Next
                                Next
                            Case Else
                                'Do not read file further
                                Throw New Exception
                        End Select
                        'For Row As Integer = 1 To (EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1) + 1
                        '    Dim Line As String = Reader.ReadLine
                        '    Dim Col As Integer = 1
                        '    Dim EndOfLine As Boolean = False
                        '    Do
                        '        Select Case Row
                        '            Case 1
                        '                ReDim Preserve m_PastAnalysisYear(Col)
                        '                m_PastAnalysisYear(Col) = CInt(CatchTLIntrpYear(Line, Col, EndOfLine))
                        '            Case 2
                        '                ReDim Preserve m_CatchPastAnalysis(EcotrophManager.DynamicsIntrpTL.GetUpperBound(0) - 1, Col)
                        '                m_CatchPastAnalysis(Row - 1, Col) = CatchTLIntrpYear(Line, Col, EndOfLine)
                        '            Case Else
                        '                m_CatchPastAnalysis(Row - 1, Col) = CatchTLIntrpYear(Line, Col, EndOfLine)
                        '        End Select
                        '        Col = Col + 1
                        '    Loop Until EndOfLine
                        'Next
                        'm_NumPastAnalysisYear = m_PastAnalysisYear.GetUpperBound(0)
                    Case "IndexPPForecast"
                        ReDim m_IndexPPForecast(m_NumForecastYear)

                        For Idx As Integer = 1 To m_NumForecastYear
                            m_IndexPPForecast(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "CatchMultiplier"
                        ReDim m_CatchMultiplier(m_NumForecastYear)

                        For Idx As Integer = 1 To m_NumForecastYear
                            m_CatchMultiplier(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                    Case "IndexPPPastAnalysis"
                        ReDim m_IndexPPPastAnalysis(m_NumPastAnalysisYear)

                        For Idx As Integer = 1 To m_NumPastAnalysisYear
                            m_IndexPPPastAnalysis(Idx) = Single.Parse(Reader.ReadLine, Me.m_ni)
                        Next
                End Select
                Reader.Close()
                Return True 'file found and file data is correct
            Else
                Select Case FileName
                    Case "SmoothFactor"
                        SetSmoothFactorDefault()
                    Case "Sigma"
                        'Set Sigma to omnivory index
                        SetSigmaDefault(EcotrophManager)
                    Case "Access"
                        SetAccessDefault(EcotrophManager)
                    Case "CTSAParameter"
                        SetCTSAParameterDefault()
                    Case "KineticParameter"
                        SetKineticParameterDefault()
                    Case "CTSACatches"
                        'Do not set default value, just return false below
                    Case "CTSAFwdCalParameter"
                        SetCTSAFwdCalParameterDefault()
                    Case "CTSABwdCalParameter"
                        SetCTSABwdCalParameterDefault()
                    Case "DiagnosisParameter"
                        SetDiagnosisParameterDefault()
                    Case "EffortMultiplier"
                        SetEffortMultiplierDefault()
                    Case "DynamicsParameter"
                        SetDynamicsParameterDefault()
                    Case "ForecastYear"
                        SetForecastYearDefault()
                    Case "CatchPastAnalysis"
                        'Do not set default value, just return false below
                    Case "IndexPPForecast"
                        SetIndexPPForecastDefault()
                    Case "CatchMultiplier"
                        SetCatchMultiplierDefault()
                    Case "IndexPPPastAnalysis"
                        SetIndexPPPastAnalysisDefault()
                End Select
                Return False 'file not found
            End If
        Catch ex As Exception
            Return False 'possibly file found but file data is incorrect
        End Try
    End Function
#End Region 'Public methods

#Region "Helper methods"
#Region "Transpose"
    Private Sub SetSmoothFactorDefault()
        m_SmoothFactor = 0.07
    End Sub

    Private Sub SetSigmaDefault(ByVal EcotrophManager As cEcotrophManager)
        ReDim m_Sigma(EcotrophManager.EcopathData.NumGroups)

        For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
            m_Sigma(Idx) = EcotrophManager.EcopathData.BQB(Idx)
        Next
    End Sub

    Private Sub SetAccessDefault(ByVal EcotrophManager As cEcotrophManager)
        ReDim m_Access(EcotrophManager.EcopathData.NumGroups)

        For Idx As Integer = 1 To EcotrophManager.EcopathData.NumGroups
            m_Access(Idx) = 1.0
        Next
    End Sub
#End Region 'Transpose
#Region "CTSA"
    Private Sub SetCTSAParameterDefault()
        m_CTSANumTL = 1
        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
            m_CTSANumTL = m_CTSANumTL + 1
        Next
        ReDim m_CTSATopD(m_CTSANumTL)
        ReDim m_CTSAFormD(m_CTSANumTL)
        ReDim m_Catches(m_CTSANumTL)

        m_WaterTemp = cUtility.DEFAULT_CTSA_WATER_TEMP
        m_TETL12 = cUtility.DEFAULT_CTSA_TE_TL12
        m_TETL2 = cUtility.DEFAULT_CTSA_TE_TL2
        For Idx As Integer = 1 To m_CTSANumTL
            m_CTSATopD(Idx) = cUtility.DEFAULT_CTSA_TOPD
        Next
        For Idx As Integer = 1 To m_CTSANumTL
            m_CTSAFormD(Idx) = cUtility.DEFAULT_CTSA_FORMD
        Next
        m_Asymptote = cUtility.DEFAULT_CTSA_ASYMPTOTE
        m_TL50 = cUtility.DEFAULT_CTSA_TL50
        m_Slope = cUtility.DEFAULT_CTSA_SLOPE
        For Idx As Integer = 1 To m_CTSANumTL
            m_Catches(Idx) = cUtility.DEFAULT_CTSA_CATCHES
        Next
    End Sub

    Private Sub SetKineticParameterDefault()
        ReDim m_KineticParameter(NUM_KINETIC_PARAMETER)

        m_KineticParameter(1) = 20.188
        m_KineticParameter(2) = -3.259
        m_KineticParameter(3) = 0.0414
    End Sub

    Private Sub SetCTSAFwdCalParameterDefault()
        m_SeedNameFwdCal = My.Resources.DROP_DWN_LST_ITM_BIOM_TL2
        m_SeedValueFwdCal = 5.0
    End Sub

    Private Sub SetCTSABwdCalParameterDefault()
        m_TTL = 5.8
        'm_SlopeSelectivityTTL = 1.0
        m_SeedNameBwdCal = My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
        m_SeedValueBwdCal = 0.15
    End Sub
#End Region 'CTSA
#Region "Diagnosis"
    Private Sub SetDiagnosisParameterDefault()
        m_DiagnosisNumTL = 1
        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
            m_DiagnosisNumTL = m_DiagnosisNumTL + 1
        Next
        ReDim m_DiagnosisTopD(m_DiagnosisNumTL)
        ReDim m_DiagnosisFormD(m_DiagnosisNumTL)

        For Idx As Integer = 1 To m_DiagnosisNumTL
            m_DiagnosisTopD(Idx) = cUtility.DEFAULT_DIAGNOSIS_TOPD
        Next
        For Idx As Integer = 1 To m_DiagnosisNumTL
            m_DiagnosisFormD(Idx) = cUtility.DEFAULT_DIAGNOSIS_FORMD
        Next
        m_DiagnosisBeta = cUtility.DEFAULT_DIAGNOSIS_BETA
    End Sub

    Private Sub SetEffortMultiplierDefault()
        ReDim m_EffortMultiplier(NUM_EFFORT_MULTIPLIER)

        m_EffortMultiplier(1) = 0.0
        m_EffortMultiplier(2) = 1.0
        m_EffortMultiplier(3) = 4.0
        m_EffortMultiplier(4) = 6.0
        m_EffortMultiplier(5) = 8.0
        m_EffortMultiplier(6) = 10.0
        m_EffortMultiplier(7) = 12.0
        m_EffortMultiplier(8) = 14.0
        m_EffortMultiplier(9) = 16.0
        m_EffortMultiplier(10) = 18.0
        m_EffortMultiplier(11) = 20.0
    End Sub
#End Region 'Diagnosis
#Region "Dynamics"
    Private Sub SetDynamicsParameterDefault()
        m_DynamicsNumTL = 1
        For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
            m_DynamicsNumTL = m_DynamicsNumTL + 1
        Next
        ReDim m_DynamicsTopD(m_DynamicsNumTL)
        ReDim m_DynamicsFormD(m_DynamicsNumTL)

        For Idx As Integer = 1 To m_DynamicsNumTL
            m_DynamicsTopD(Idx) = cUtility.DEFAULT_DYNAMICS_TOPD
        Next
        For Idx As Integer = 1 To m_DynamicsNumTL
            m_DynamicsFormD(Idx) = cUtility.DEFAULT_DYNAMICS_FORMD
        Next
        m_DynamicsBeta = cUtility.DEFAULT_DYNAMICS_BETA
    End Sub

    Private Sub SetForecastYearDefault()
        m_ReferenceYear = 2008
        m_NumForecastYear = 15
    End Sub

    Private Sub SetIndexPPForecastDefault()
        ReDim m_IndexPPForecast(m_NumForecastYear)

        For Idx As Integer = 1 To m_NumForecastYear
            m_IndexPPForecast(Idx) = 1.0
        Next
    End Sub

    Private Sub SetCatchMultiplierDefault()
        ReDim m_CatchMultiplier(m_NumForecastYear)

        For Idx As Integer = 1 To m_NumForecastYear
            m_CatchMultiplier(Idx) = 1.5
        Next
    End Sub

    Private Sub SetIndexPPPastAnalysisDefault()
        ReDim m_IndexPPPastAnalysis(m_NumPastAnalysisYear)

        For Idx As Integer = 1 To m_NumPastAnalysisYear
            m_IndexPPPastAnalysis(Idx) = 1.2
        Next
    End Sub

    Private Function CatchTLIntrpYear(ByVal Line As String, ByVal ColNum As Integer, ByRef EndOfLine As Boolean) As Single
        Dim Chars() As Char = {","c, " "c, Chr(9)}
        Static PosInitSep As Integer
        Dim PosNextSep As Integer
        Dim CatchEstimate As Single

        Select Case ColNum
            Case 1
                'first item in the Line
                PosNextSep = Line.IndexOfAny(Chars)
                PosInitSep = PosNextSep
                CatchEstimate = Single.Parse(Line.Substring(0, PosNextSep), Me.m_ni)
                EndOfLine = False
                Return CatchEstimate
                'Case m_CatchPastAnalysis.GetUpperBound(1)
                '    CatchEstimate = Single.Parse(Line.Substring(PosInitSep + 1), Me.m_ni)
                '    Return CatchEstimate
            Case Else
                PosNextSep = Line.IndexOfAny(Chars, PosInitSep + 1)
                If PosNextSep <> -1 Then
                    CatchEstimate = Single.Parse(Line.Substring(PosInitSep + 1, PosNextSep - PosInitSep - 1), Me.m_ni)
                    PosInitSep = PosNextSep
                    EndOfLine = False
                    Return CatchEstimate
                Else
                    'last item in the Line reached
                    CatchEstimate = Single.Parse(Line.Substring(PosInitSep + 1), Me.m_ni)
                    EndOfLine = True
                    Return CatchEstimate
                End If
        End Select
    End Function

    Private Function NumberLinePerFile(ByVal Reader As StreamReader) As Integer
        Dim NumLine As Integer

        NumLine = 0
        Reader.BaseStream.Seek(0, SeekOrigin.Begin)
        Do Until Reader.EndOfStream
            Reader.ReadLine()
            NumLine = NumLine + 1
        Loop
        Return NumLine
    End Function

    Private Function ItemsPerLine(ByVal Reader As StreamReader, ByVal LineNum As Integer) As String()
        Dim Line As String
        Dim Items() As String
        Dim ItemNum As Integer
        Dim Chars() As Char = {","c, Chr(9)} '{","c, " "c, Chr(9)}
        Dim PosSep As Integer
        Dim PosNextSep As Integer

        Reader.BaseStream.Seek(0, SeekOrigin.Begin)
        For Row As Integer = 1 To LineNum - 1
            Reader.ReadLine()
        Next

        Line = Reader.ReadLine()
        ItemNum = 1
        Do
            Select Case ItemNum
                Case 1
                    'first item in the line
                    ReDim Items(ItemNum)
                    PosNextSep = Line.IndexOfAny(Chars)
                    If PosNextSep <> -1 Then
                        Items(ItemNum) = Line.Substring(0, PosNextSep)
                        ItemNum = ItemNum + 1
                        PosSep = PosNextSep
                    Else
                        'last item in the line reached
                        Items(ItemNum) = Line.Substring(0)
                        Exit Do
                    End If
                Case Else
                    ReDim Preserve Items(ItemNum)
                    PosNextSep = Line.IndexOfAny(Chars, PosSep + 1)
                    If PosNextSep <> -1 Then
                        Items(ItemNum) = Line.Substring(PosSep + 1, PosNextSep - PosSep - 1)
                        ItemNum = ItemNum + 1
                        PosSep = PosNextSep
                    Else
                        'last item in the line reached
                        Items(ItemNum) = Line.Substring(PosSep + 1)
                        ItemNum = ItemNum + 1
                        Exit Do
                    End If
            End Select
        Loop
        Do Until Reader.EndOfStream
            Reader.ReadLine()
        Loop
        Return Items
    End Function
#End Region 'Dynamics
#End Region 'Helper methods

End Class
