Imports EwEUtils.Core
Imports EwEUtils.Utilities

Namespace Style

    Public Class cUnitFormatter

        Private m_core As cCore = Nothing

        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a formatted unit string for a given unit type.
        ''' </summary>
        ''' <param name="unitType"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetUnitString(ByVal unitType As eUnitType) As String

            Dim strUnitString As String = ""
            Dim model As cEwEModel = Me.m_core.EwEModel

            If model Is Nothing Then Return ""

            Select Case unitType
                Case eUnitType.Currency
                    Dim fmt As New cCurrencyUnitFormatter(model.UnitCurrencyCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitCurrency)

                Case eUnitType.Time
                    Dim fmt As New cTimeUnitFormatter(model.UnitTimeCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitTime)

                Case eUnitType.Monetary
                    strUnitString = model.UnitMonetary

                Case eUnitType.Nominal
                    strUnitString = "#"

                Case eUnitType.Area
                    Dim fmt As New cAreaUnitFormatter(model.UnitAreaCustomText)
                    strUnitString = fmt.GetDescriptor(model.UnitArea)

                Case eUnitType.Biomass
                    strUnitString = "unit biomass" ' Fixed

                Case eUnitType.None
                    ' NOP

                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Format one or more units to a string using en-US annotation.
        ''' </summary>
        ''' <param name="aUnitTypes">An array of units to display.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function FormatUnitString(ByVal aUnitTypes As eUnitType()) As String

            Dim str As String = ""

            For i As Integer = 0 To aUnitTypes.Length - 1
                If i > 0 Then str = str & "/"
                str = str & Me.GetUnitString(aUnitTypes(i))
            Next
            Return str

        End Function

    End Class

End Namespace
