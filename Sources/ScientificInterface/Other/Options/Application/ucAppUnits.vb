#Region "Imports directive"

Option Explicit On
Option Strict On

#End Region

Namespace Other

    Public Class ucAppUnits

        Private m_strCurrencyUnit As String = String.Empty
        Private m_strTimeUnit As String = String.Empty

        Private Sub rbTimeUnit_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbYear.CheckedChanged, rbTimeOther.CheckedChanged, rbDay.CheckedChanged

            ' ToDo_FG: Localize this entire method

            If rbYear.Checked Then
                m_strTimeUnit = "year"
            ElseIf rbDay.Checked Then
                m_strTimeUnit = "day"
            ElseIf rbTimeOther.Checked Then
                m_strTimeUnit = txbTimeOther.Text
            End If
        End Sub

        Private Sub rbCurrencyUnit_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbWetWeight.CheckedChanged, rbPhosporus.CheckedChanged, rbNutrientOther.CheckedChanged, rbNitrogen.CheckedChanged, rbJoules.CheckedChanged, rbEnergyOther.CheckedChanged, rbDryWeight.CheckedChanged, rbCarbon.CheckedChanged, rbCalorie.CheckedChanged

            ' ToDo_FG: Localize this entire method

            If rbWetWeight.Checked Then
                m_strCurrencyUnit = String.Format("t/km{0}", ChrW(178))
            ElseIf rbJoules.Checked Then
                m_strCurrencyUnit = String.Format("J/m{0}", ChrW(178))
            ElseIf rbCalorie.Checked Then
                m_strCurrencyUnit = String.Format("kcal/m{0}", ChrW(178))
            ElseIf rbCarbon.Checked Then
                m_strCurrencyUnit = String.Format("g/m{0}", ChrW(178))
            ElseIf rbDryWeight.Checked Then
                m_strCurrencyUnit = String.Format("g/m{0}", ChrW(178))
            ElseIf rbEnergyOther.Checked Then
                m_strCurrencyUnit = txbNutrientOther.Text
            ElseIf rbNitrogen.Checked Then
                m_strCurrencyUnit = String.Format("mg N/m{0}", ChrW(178))
            ElseIf rbPhosporus.Checked Then
                m_strCurrencyUnit = String.Format("mg P/m{0}", ChrW(178))
            ElseIf rbNutrientOther.Checked Then
                m_strCurrencyUnit = txbNutrientOther.Text
            End If

        End Sub

        Private Sub ucAppUnits_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim sg As StyleGuide = StyleGuide.GetInstance()

            m_strCurrencyUnit = sg.CurrencyUnit
            m_strTimeUnit = sg.TimeUnit

            ' ToDo_JS: Localize this entire method
            If m_strTimeUnit = "year" Then
                rbYear.Checked = True
            ElseIf m_strTimeUnit = "day" Then
                rbDay.Checked = True
            Else
                rbTimeOther.Checked = True
                txbTimeOther.Text = m_strTimeUnit
            End If

            If m_strCurrencyUnit = String.Format("t/km{0}", ChrW(178)) Then
                rbWetWeight.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("J/m{0}", ChrW(178)) Then
                rbJoules.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("kcal/m{0}", ChrW(178)) Then
                rbCalorie.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("g/m{0}", ChrW(178)) Then
                rbCarbon.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("g/m{0}", ChrW(178)) Then
                rbDryWeight.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("mg N/m{0}", ChrW(178)) Then
                rbNitrogen.Checked = True
            ElseIf m_strCurrencyUnit = String.Format("mg P/m{0}", ChrW(178)) Then
                rbPhosporus.Checked = True
            Else
                rbEnergyOther.Checked = True
                txbNutrientOther.Text = m_strCurrencyUnit
            End If

        End Sub

        Public Sub SaveUnitsOptions()

            Dim sg As StyleGuide = StyleGuide.GetInstance

            sg.CurrencyUnit = m_strCurrencyUnit
            sg.TimeUnit = m_strTimeUnit

        End Sub

        Private Sub txbNutrientOther_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txbNutrientOther.Enter
            Me.rbNutrientOther.Checked = True
        End Sub

        Private Sub txbEnergyOther_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txbEnergyOther.Enter
            Me.rbEnergyOther.Checked = True
        End Sub

        Private Sub txbTimeOther_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles txbTimeOther.Enter
            Me.rbTimeOther.Checked = True
        End Sub

    End Class

End Namespace

