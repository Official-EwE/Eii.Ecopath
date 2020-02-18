' ===============================================================================
' This file is part of the Safenet toolkit.
'
' To use Safenet tools please contact Marta Coll or Jeroen Steenbeek at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Option Strict On
Imports System.Drawing

<HideModuleName()>
Public Module modDefinitions

    Private m_tracker As Tracker = Nothing

    Private ReadOnly Property Tracker As Tracker
        Get
            If (m_tracker Is Nothing) Then
                m_tracker = New Tracker()
            End If
            Return m_tracker
        End Get
    End Property

    Public Sub StartTracking(job As String)
        Tracker.Start(job)
    End Sub

    Public Sub StartTracking(job As String, ParamArray arg() As Object)
        Tracker.Start(String.Format(job, arg))
    End Sub

    Public Sub Track(task As String, Optional bStep As Boolean = True, Optional bTimed As Boolean = True)
        Tracker.Log(task, bStep, bTimed)
    End Sub

    Public Sub Track(task As String, ParamArray arg() As Object)
        Track(String.Format(task, arg))
    End Sub

    Public Function StopTracking() As String
        Return Tracker.Stop()
    End Function

    Public Const CODE_UNKNOWN As String = "<unknown>"
    Public Const FILE_EXCLUDED_STUDIES As String = "excluded_studies.txt"
    Public Const FILE_DIETCALC_LOG As String = "dietcalc_log_{0}-{1}-{2}.csv"

    Public COLOR_SELECTED As Color = Color.DarkGreen
    Public COLOR_ERROR As Color = Color.Red
    Public COLOR_WARNING As Color = Color.DarkOrange
    Public COLOR_EXCLUDED As Color = Color.LightGray

    ''' <summary>
    ''' Fields that can have a pedigree assignment
    ''' </summary>
    Public Enum PedigreeFields As Integer
        NotSet = 0
        Region = 1
        Year = 2
        Data = 3
        DietMethod = 4
    End Enum

    ''' <summary>
    ''' Diet data types
    ''' </summary>
    Public Enum DietData As Integer
        NotSet = 0
        AdultsJuveniles = 1
        Adults = 2
        Juveniles = 3
        Larvae = 4
    End Enum

    ''' <summary>
    ''' Diet method gathering types
    ''' </summary>
    Public Enum DietMethods As Integer
        NotSet = 0
        StableIsotopes = 1
        VisualObservations = 2
        StomachContent = 3
        Other = 4
    End Enum

    ''' <summary>
    ''' Regions
    ''' </summary>
    Public Enum Regions As Integer
        NotSet = 0
        MediterraneanSea = 1
        WesternMediterraneanSea = 2
        EasternMediterraneanSea = 3
        CentralMediterraneanSea = 4
        CatalanSea = 5
        BalearicSea = 6
        GulfOfLions = 7
        LigurianSea = 8
        TyrrhenianSea = 9
        AdriaticSea = 10
        IonianSea = 11
        AegeanSea = 12
        OutsideMediterraneanSea = 13
        NorthwesternMediterranean = 14
    End Enum

    ''' <summary>
    ''' Group role types in the food web
    ''' </summary>
    Public Enum GroupRole As Integer
        NotUsed = 0
        Detritus = 1
        Producer = 2
        Consumer = 3
    End Enum

    ''' <summary>
    ''' Diet calculator error codes 
    ''' </summary>
    <Flags>
    Public Enum ErrorCode As Integer
        OK = 0
        NoPred = 1
        NoPrey = 2
        NoFuncGroup = 4
        NoDiet = 8
        NoBiomass = 16
        Malformed = 64
    End Enum

    ''' <summary>
    ''' User alert types
    ''' </summary>
    Public Enum eAlert As Integer
        OK = 0
        Warning
        [Error]
    End Enum

    ''' <summary>
    ''' Diet averaging methods
    ''' </summary>
    Public Enum AverageMethod As Integer
        ''' <summary>
        ''' Average diets on all prey across all studies, even if diet 
        ''' combinations have not been identified in all studies.
        ''' </summary>
        AverageAll
        ''' <summary>
        ''' Average diets on prey only in the studies that identified the 
        ''' predator-prey link. This method emphasizes diet records that have 
        ''' been identified in not all studies.
        ''' </summary>
        AverageOccurring
    End Enum

    ''' <summary>
    ''' Functional response type
    ''' </summary>
    Public Enum eFunctionType As Integer
        NotUsed = 0
        Depth
        Salinity
        Temperature
        IceConcentration
    End Enum

    ''' <summary>
    ''' Functional response extraction type
    ''' </summary>
    Public Enum eExtractionType As Integer
        All = 0
        BestFive
        ''' <summary>Top 95 percent in the biomass</summary>
        Top95Percent
    End Enum

    ''' <summary>
    ''' Functional response repositories
    ''' </summary>
    Public Enum eFunctionRepoType As Integer
        NotSet = 0
        AquaMaps
        AquaMapsCorrected
        GabrielR
    End Enum

    Public Enum eInsertionType As Integer
        ''' <summary>Clear and recreate all shapes</summary>
        Redefine = 0
        ''' <summary>Update existing shapes, define new ones where needed, and delete shapes that do not occur in the new repository</summary>
        Maintain
    End Enum

End Module
