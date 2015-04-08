Imports EwECore

Public Class cResultsCSVWriter
    Public m_StartYear As Integer
    Public m_nTimeRecords As Integer
    Public m_Path As String
    Public m_IdentifierColumns() As String
    Public m_core As cCore

    Public m_Path

    Public Sub Initialise(MSE As cMSE)
        m_core = MSE.Core
    End Sub

    Public Sub Write2File()
        'Save the Realised F trajectories

        For iGrp = 1 To m_core.nGroups
            For iRow = 1 To Realised_F_Tab.Rows.Count
                TempRow = Realised_F_Tab.Rows(iRow - 1)

                'Output the realised F's to file
                swRealised_F(iGrp - 1).Write("{0},{1},{2},{3}", _
                                           cStringUtils.ToCSVField(m_core.EcoPathGroupInputs(iGrp).Name), _
                                           cStringUtils.FormatNumber(iModel), _
                                           cStringUtils.ToCSVField(TempRow.Field(Of String)("StrategyName")), _
                                           cStringUtils.ToCSVField("TotalF"))
                TempArrayResultsTarget = TempRow.Field(Of Double(,))("TotalF")
                For iTime = 1 To NYearsProject * EcosimData.NumStepsPerYear
                    swRealised_F(iGrp - 1).Write("," & cStringUtils.FormatNumber(TempArrayResultsTarget(iGrp - 1, iTime - 1)))
                Next
                swRealised_F(iGrp - 1).WriteLine()

            Next
        Next

    End Sub


End Class
