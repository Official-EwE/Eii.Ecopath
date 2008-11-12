
Public Enum eMPAOptimizationModels
    EcoSeed
    RandomSearch
End Enum


Public Class cMPAOptDataStructures

    Const MIN_RUN_LENGTH As Integer = 3
 
    Public CurRow As Integer
    Public CurCol As Integer
    Public bestrow As Integer
    Public bestcol As Integer
    Public StopRun As Boolean
    Public BoundaryWeight As Single
    Public MPASeed(,) As Integer
    Public SeedBlockSize2 As Integer

    'value of objective function  relative to the base value
    Public objFuncEconomicValue As Single
    Public objFuncMandatedValue As Single
    Public objFuncSocialValue As Single
    Public objFuncEcologicalValue As Single
    Public objFuncAreaBorder As Single

    Public objBiomassDiversity As Single

    Public objFuncTotal As Single

    Public SearchType As eMPAOptimizationModels

    Public stepSize As Integer
    Public MaxArea As Integer
    Public MinArea As Integer
    Public nIterations As Integer

    Public iMPAtoUse As Integer
    Public bUseCellWeight As Boolean

    Public EcoSpaceStartYear As Integer = 3
    Public EcoSpaceEndYear As Integer

    Private m_cells As List(Of cMPACell)

    Public Sub New()

        SearchType = eMPAOptimizationModels.RandomSearch

        nIterations = 100
        stepSize = 10
        MaxArea = 20
        MinArea = 20
        iMPAtoUse = 1

        m_cells = New List(Of cMPACell)

    End Sub

    ''' <summary>
    ''' Clear out the current Ecoseed values
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        CurRow = 0
        CurCol = 0
        bestrow = 0
        bestcol = 0

        objFuncEconomicValue = 0
        objFuncMandatedValue = 0
        objFuncSocialValue = 0
        objFuncEcologicalValue = 0
        objFuncAreaBorder = 0
        objBiomassDiversity = 0
        objFuncTotal = 0

    End Sub

    Public Sub setObjectiveValues(ByVal SearchData As cSearchDatastructures)

    End Sub




    Public Sub AddCell(ByVal Row As Integer, ByVal col As Integer, ByVal iMPA As Integer)
        m_cells.Add(New cMPACell(Row, col, iMPA))
    End Sub

    Public Sub ClearCells()
        m_cells.Clear()
    End Sub

    Public Function Cells() As List(Of cMPACell)
        Return m_cells
    End Function

    Public ReadOnly Property MinRunLength() As Integer
        Get
            Return MIN_RUN_LENGTH
        End Get
    End Property

End Class



''' <summary>
''' MPA cell selected during a trial
''' </summary>
''' <remarks></remarks>
Public Class cMPACell
    Public Row As Integer
    Public Col As Integer
    Public iMPA As Integer

    Public Sub New(ByVal theRow As Integer, ByVal theCol As Integer, ByVal theMPAIndex As Integer)
        Row = theRow
        Col = theCol
        iMPA = theMPAIndex
    End Sub

End Class