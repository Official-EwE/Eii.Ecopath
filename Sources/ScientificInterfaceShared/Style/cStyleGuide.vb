#Region " Imports "

Option Strict On

Imports System.Globalization
Imports System.Threading
Imports System.Drawing
Imports System.Text
Imports VB = Microsoft.VisualBasic
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Drawing
Imports EwEUtils.Utilities
Imports SAUPUtil.Misc.Colours

#End Region ' Imports

Namespace Style

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The style guide provides the one and only interface to standardized user 
    ''' interface color feedback.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Class cStyleGuide

#Region " Private bits "

        ''' <summary>Admin: Monetary unit name lookup table.</summary>
        Private m_dtMonetaryUnitNames As New Dictionary(Of eUnitMonetaryType, String)

        ''' <summary>States the number of decimal digits to be displayed</summary>
        Private m_iNumDigits As Integer = 3
        ''' <summary>States whether numbers are formatted in groups.</summary>
        Private m_bGroupDigits As Boolean = False

        ' -- units --
        ''' <summary>Default currency unit.</summary>
        Private m_unitCurrency As eUnitCurrencyType = eUnitCurrencyType.Nitrogen
        ''' <summary>Currency unit custom text.</summary>
        Private m_strUnitCurrencyCustom As String = ""
        ''' <summary>Default currency unit.</summary>
        Private m_unitTime As eUnitTimeType = eUnitTimeType.Year
        ''' <summary>Time unit custom text.</summary>
        Private m_strUnitTimeCustom As String = ""
        ''' <summary>Default monetary unit.</summary>
        Private m_unitMonetary As eUnitMonetaryType = eUnitMonetaryType.EUR
        ''' <summary>Monetary unit custom text.</summary>
        Private m_strUnitMonetaryCustom As String = ""

        ' -- internal management --
        ''' <summary>States whether the StyleGuide contains unsaved changes</summary>
        Private m_bChanged As Boolean = False
        ''' <summary>Application colour scheme.</summary>
        Private m_dtApplicationColors As New Dictionary(Of cStyleGuide.eApplicationColorType, Color)
        ''' <summary>Color ramp for obtaining EwE5 group colors</summary>
        Private m_colorrampGroups As New SAUPColorRamp()
        ''' <summary>Color ramp for obtaining fleet colors</summary>
        Private m_colorrampFleets As New ARGBColorRamp(New Color() {Color.Green, Color.LightGreen, Color.LightBlue, Color.Blue, Color.DarkBlue}, New Double() {0.0#, 0.4#, 0.3#, 0.2#, 0.1#})
        ''' <summary>Color ramp for obtaining pedigree colors</summary>
        Private m_colorrampPedigree As New ARGBColorRamp(New Color() {Color.FromArgb(255, 210, 210, 255), Color.FromArgb(255, 80, 80, 200), Color.FromArgb(255, 0, 0, 130)}, New Double() {0.0#, 0.6#, 0.4#})
        ''' <summary>Start offset for colour ramp.</summary>
        Private Const c_sRampOffsetStart As Single = 0.15!
        ''' <summary>End offset for colour ramp.</summary>
        Private Const c_sRampOffsetEnd As Single = 1.0!

        ' -- graphs --
        ''' <summary></summary>
        Private m_dtFontFamilyName As New Dictionary(Of eApplicationFontType, String)
        ''' <summary></summary>
        Private m_dtFontSize As New Dictionary(Of eApplicationFontType, Single)
        ''' <summary></summary>
        Private m_dtFontStye As New Dictionary(Of eApplicationFontType, FontStyle)
        ''' <summary>Usage of legends.</summary>
        ''' <remarks>UseDefault = selective, True or False</remarks>
        Private m_tsShowLegends As TriState = TriState.UseDefault
        ''' <summary>Show transparent backgrounds where applicable</summary>
        Private m_bTransparentBackgrounds As Boolean = False

        ' -- group visibility --
        ''' <summary>List of indexes of groups to hide.</summary>
        Private m_lHiddenGroups As New List(Of Integer)
        ''' <summary>List of indexes of fleets to hide.</summary>
        Private m_lHiddenFleets As New List(Of Integer)
        Private m_bHideTotalCatch As Boolean = False
        Private m_bHideTotalValue As Boolean = False

        ' -- thumbnails --
        ''' <summary>Size (width and height) of thumbnails in EwE6.</summary>
        Private m_iThumbnailSize As Integer = 48

        ' -- event locks --
        ''' <summary>Event lock count.</summary>
        Private m_nEventLock As Integer = 0
        ''' <summary>States whether there are events withheld and pending while an event lock is active.</summary>
        Private m_pendingChangeEventTypes As eChangeType = eChangeType.None

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks>Singleton enforced: constructor is only accessible locally</remarks>
        ''' -----------------------------------------------------------------------
        Public Sub New()

            ' Control how colour ramp delivers its colours
            Me.m_colorrampGroups.ColorOffsetStart = c_sRampOffsetStart
            Me.m_colorrampGroups.ColorOffsetEnd = c_sRampOffsetEnd

            ' Load up
            Me.ResetApplicationColors()
            Me.LoadMonetaryUnitNames()

        End Sub

#End Region ' Private bits

#Region " Public Methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Resets application colors to default values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ResetApplicationColors()
            'Default colors
            Me.m_dtApplicationColors.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Resets application fonts to default values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ResetApplicationFonts()
            Me.m_dtFontFamilyName.Clear()
            Me.m_dtFontSize.Clear()
            Me.m_dtFontStye.Clear()
        End Sub

#End Region ' Public interfaces

#Region " Public access "

#Region " Enums and events "

        ''' <summary>
        ''' Public enumerator stating the visual feedback required for rendering a value
        ''' </summary>
        Public Enum eStyleFlags As Integer

            '-----------------------------------------------------------------
            ' Directly mapped Core flags
            '-----------------------------------------------------------------

            ''' <summary>All well, value is OK and does not require any kind of formatting.</summary>
            OK = CInt(eStatusFlags.OK)

            ''' <summary>Flag indicating that Data Validation Failed for a value.</summary>
            FailedValidation = CInt(eStatusFlags.FailedValidation)

            ''' <summary>Flag indicating that a value was Computed, not entered.</summary>
            ValueComputed = CInt(eStatusFlags.ValueComputed)

            ''' <summary>Flag indicating that a value was Computed to an Invalid Result.</summary>
            InvalidModelResult = CInt(eStatusFlags.InvalidModelResult)

            ''' <summary>Flag indicating that a value is Not Editable, e.g. should not
            ''' be modified by user input.</summary>
            ''' <remarks>This flag is also known as ReadOnly or BlockedForInput (EwE5)</remarks>
            NotEditable = CInt(eStatusFlags.NotEditable)

            ''' <summary>Flag indicating that an Unknown Error has been encountered regarding this value.</summary>
            ErrorEncountered = CInt(eStatusFlags.ErrorEncountered)

            ''' <summary>Flag indicating that a value is a Missing Parameter for one of the EwE models.</summary>
            ''' <remarks>
            ''' This flag is different from <see cref="eStatusFlags.Null">StyleFlags.Null</see>; Null values
            ''' are model-technically not initialized while Missing Parameter values do not contain a valid
            ''' value for the model that they are used in.
            ''' </remarks>
            MissingParameter = CInt(eStatusFlags.MissingParameter)

            ''' <summary>
            ''' Flag indicating that the core deemed a value as important for whatever reason. The
            ''' core is not able to communicate such reasons, and highlighting is therefore an
            ''' ad-hoc process on a per-case basis.
            ''' </summary>
            Checked = CInt(eStatusFlags.CoreHighlight)

            ''' <summary>Flag indicating that a value is Null; its value has not been set or has been
            ''' set to the <see cref="cCore.NULL_VALUE">Core NULL value</see>.</summary>
            Null = CInt(eStatusFlags.Null)


            ''' <summary>Bit-pattern mask to separate core statuses from GUI statuses.</summary>
            CoreStatusFlagsMask = 4095

            '-----------------------------------------------------------------
            ' GUI-specific flags
            '-----------------------------------------------------------------

            '''' <summary>EcoSim GUI flag.</summary>
            '''' <remarks>JS 31may07: Is this style used at all?</remarks>
            'FishingPressure = 4096 ' 2^12

            '''' <summary>EcoSim GUI flag.</summary>
            '''' <remarks>JS 31may07: Is this style used at all?</remarks>
            'Profit = 8192 ' 2^13

            '''' <summary>EcoSim GUI flag.</summary>
            '''' <remarks>JS 31may07: Is this style used at all?</remarks>
            'TotalCatch = 16384 ' 2^14

            '''' <summary>EcoSim GUI flag</summary>
            '''' <remarks>JS 31may07: Is this style used at all?</remarks>
            'TrophicLink = 32768 ' 2^15

            ''' <summary>EcoPath GUI flag; indicates whether a value has associated remarks.</summary>
            Remarks = 65536 ' 2^16

            ''' <summary>Flag indicating that a value provides a Summary of other values in the same screen.</summary>
            Sum = 131072 ' 2^17

            ''' <summary>Flag indicating that a value is Highlighted.</summary>
            Highlight = 262144 ' 2^18

            ''' <summary>Flag indicating that a value is a Name.</summary>
            Names = 524288 ' 2^19

            ''' <summary>Flag indicating that a value is an italic taxon code.</summary>
            TaxonItalics = 1048576 ' 2^20

            ''' <summary>Flag indicating that a value is a regular taxon code.</summary>
            TaxonReg = OK

        End Enum

        Public Enum eLegendPosition As Integer

            ''' <summary>Do not show legends.</summary>
            Hidden = cCore.NULL_VALUE
            ''' <summary>Show legend to the left of graphs.</summary>
            Left = ZedGraph.LegendPos.Left
            ''' <summary>Show legend to the right of graphs.</summary>
            Right = ZedGraph.LegendPos.Right
            ''' <summary>Show legend above graphs.</summary>
            Above = ZedGraph.LegendPos.TopCenter
            ''' <summary>Show legend below graphs.</summary>
            Below = ZedGraph.LegendPos.BottomCenter

        End Enum

        ''' <summary>
        ''' Types of changes that can occur in the StyleGuide.
        ''' </summary>
        Public Enum eChangeType As Integer
            None = 0
            Colours = &H1
            NumberFormatting = &H2
            Units = &H4
            Fonts = &H8
            GroupVisibility = &H10
            FleetVisibility = &H20
            Thumbnails = &H40
            Legends = &H80
            All = &HFFFFFFFF
        End Enum

        ''' <summary>Good old-fashioned (but slightly blunt) way</summary>
        Public Event StyleGuideChanged(ByVal changeType As eChangeType)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Notify listeners of StyleGuide changes
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub FireChangeEvent(ByVal changeType As eChangeType)
            ' Are events locked?
            If (Me.m_nEventLock > 0) Then
                ' #Yes: remember that an event is pending
                Me.m_pendingChangeEventTypes = Me.m_pendingChangeEventTypes Or changeType
                ' Abort, leave the event for later
                Return
            End If

            ' Broadcast change event to listeners
            RaiseEvent StyleGuideChanged(changeType)
            ' No more change events pending
            Me.m_pendingChangeEventTypes = eChangeType.None
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Lock down the broadcasting of StyleGuide events.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub SuspendEvents()
            Me.m_nEventLock += 1
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Unlocks broadcasting of StyleGuide change events.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ResumeEvents()
            Me.m_nEventLock -= 1
            ' Did this clear the event lock?
            If (Me.m_nEventLock <= 0) And (m_pendingChangeEventTypes <> eChangeType.None) Then
                ' Fire remaining event(s)
                FireChangeEvent(Me.m_pendingChangeEventTypes)
                ' Clear cache
                Me.m_pendingChangeEventTypes = eChangeType.None
            End If
        End Sub

#End Region ' Enums and events

#Region " System settings "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the current UI culture is right-to-left ordered.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function IsRightToLeft() As Boolean
            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Return ci.TextInfo.IsRightToLeft
        End Function

#End Region ' System settings

#Region " Number formatting "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the number of decimal digits to display.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property NumDigits() As Integer
            Get

                Return Me.m_iNumDigits

            End Get
            Set(ByVal nNumDigits As Integer)

                ' Is this a change?
                If (nNumDigits = Me.m_iNumDigits) Then
                    ' #No: abort
                    Return
                End If
                ' Update number of digits to maintain
                Me.m_iNumDigits = nNumDigits
                ' Notify listeners
                Me.FireChangeEvent(eChangeType.NumberFormatting)

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether formatted numbers are grouped via the thousands
        ''' separator.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property GroupDigits() As Boolean
            Get

                Return Me.m_bGroupDigits

            End Get
            Set(ByVal bGroupDigits As Boolean)

                ' Is this a change?
                If (bGroupDigits = Me.m_bGroupDigits) Then
                    ' #No: abort
                    Return
                End If
                ' Update 
                Me.m_bGroupDigits = bGroupDigits
                ' Notify listeners
                Me.FireChangeEvent(eChangeType.NumberFormatting)

            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <para>Format an Integer number to a string. The number will be rendered
        ''' with 0 relevant decimal digits.</para>
        ''' </summary>
        ''' <param name="iValue">The value to format.</param>
        ''' <param name="style">Optional <see cref="eStyleFlags">style flags</see> to
        ''' that may need specific formatting. Computed values for instance will
        ''' be represented with exactly the requested number of decimal digits, instead
        ''' of the </param>
        ''' <returns>A formatted value that always displays the least significant precision digit.</returns>
        ''' -----------------------------------------------------------------------
        Public Function FormatNumber(ByVal iValue As Integer, _
                                     Optional ByVal style As eStyleFlags = eStyleFlags.OK) As String
            Return Me.FormatNumber(CDbl(iValue), style, 0)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <para>Format a Single number to a string. The number will be rendered
        ''' with a requested number of relevant decimal digits.</para>
        ''' </summary>
        ''' <param name="sValue">The value to format.</param>
        ''' <param name="iNumDigits">
        ''' <para>The minimum precision that should be used to format the value.</para>
        ''' <para>If left at its default value, this number is obtained from the 
        ''' <see cref="NumDigits">precision setting</see> in the StyleGuide.</para>
        ''' </param>
        ''' <param name="style">Optional <see cref="eStyleFlags">style flags</see> to
        ''' that may need specific formatting. Computed values for instance will
        ''' be represented with exactly the requested number of decimal digits, instead
        ''' of the </param>
        ''' <returns>A formatted value that always displays the least significant precision digit.</returns>
        ''' -----------------------------------------------------------------------
        Public Function FormatNumber(ByVal sValue As Single, _
                                     Optional ByVal style As eStyleFlags = eStyleFlags.OK, _
                                     Optional ByVal iNumDigits As Integer = -1, _
                                     Optional ByVal tsGroupDigits As TriState = TriState.UseDefault) As String
            Return Me.FormatNumber(CDbl(sValue), style, iNumDigits, tsGroupDigits)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <para>Format a Double number to a string. The number will be rendered
        ''' with a requested number of relevant decimal digits.</para>
        ''' </summary>
        ''' <param name="dValue">The value to format.</param>
        ''' <param name="iNumDigits">
        ''' <para>The minimum precision that should be used to format the value.</para>
        ''' <para>If left at its default value, this number is obtained from the 
        ''' <see cref="NumDigits">precision setting</see> in the StyleGuide.</para>
        ''' </param>
        ''' <param name="style">Optional <see cref="eStyleFlags">style flags</see> to
        ''' that may need specific formatting. Computed values for instance will
        ''' be represented with exactly the requested number of decimal digits.</param>
        ''' <returns>A formatted value that always displays the least significant precision digit.</returns>
        ''' -----------------------------------------------------------------------
        Public Function FormatNumber(ByVal dValue As Double, _
                                     Optional ByVal style As eStyleFlags = eStyleFlags.OK, _
                                     Optional ByVal iNumDigits As Integer = -1, _
                                     Optional ByVal tsGroupDigits As TriState = TriState.UseDefault) As String

            ' Use styleguide numdigits setting if value not provided
            If iNumDigits < 0 Then iNumDigits = Me.m_iNumDigits

            Dim dTest As Double = CDbl(Math.Abs(dValue))
            Dim iMinPrecision As Integer = 0
            Dim iMaxPrecision As Integer = Math.Min(iNumDigits * 2, 10)

            If (style And eStyleFlags.Null) > 0 Then
                Return ""
            End If

            If (tsGroupDigits = TriState.UseDefault) Then
                If Me.m_bGroupDigits Then
                    tsGroupDigits = TriState.True
                Else
                    tsGroupDigits = TriState.False
                End If
            End If

            ' Calculated values must be formatted with a hard number of digits
            If (style And (eStyleFlags.ValueComputed Or eStyleFlags.Sum)) > 0 Then
                Return VB.FormatNumber(dValue, iNumDigits, _
                                       TriState.UseDefault, _
                                       TriState.UseDefault, _
                                       tsGroupDigits)
            End If

            ' Need to try to figure out num of decimal digits?
            If (dTest <> 0.0) Then
                ' #Yes: find min number of relevant decimal digits
                While Math.Floor(dTest) = 0
                    dTest *= 10.0#
                    iMinPrecision += 1
                End While
                ' First relevant decimal digit found: show iNumDigits decimals including this first value
                iMinPrecision += (iNumDigits - 1)

                ' Has decimals?
                If (Math.Abs(dValue) > 1) Then
                    ' #Yes: Find max number of decimal digits
                    dTest = 1.0#
                    For iTest As Integer = iNumDigits To 0 Step -1
                        dTest *= 10.0#
                        iMaxPrecision = iTest
                        If (dValue <= dTest) Then Exit For
                    Next
                End If
            End If

            ' Format the value with selected number of decimal digits
            Return VB.FormatNumber(dValue, _
                                   Math.Min(Math.Max(iNumDigits, iMinPrecision), iMaxPrecision), _
                                   TriState.UseDefault, _
                                   TriState.UseDefault, _
                                   tsGroupDigits)

        End Function

#End Region ' Number formatting

#Region " Units "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Types of dynamic units supported by the StyleGuide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eUnitType As Byte
            ''' <summary>Not a dynamic unit.</summary>
            None = 0
            ''' <summary>Currency unit.</summary>
            Currency
            ''' <summary>Time unit.</summary>
            Time
            ''' <summary>Monetary unit.</summary>
            Monetary
            ''' <summary>Nominal.</summary>
            Nominal
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Call this when application unit settings have changed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub UnitsChanged()
            Me.FireChangeEvent(eChangeType.Units)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a formatted unit string for a given unit type.
        ''' </summary>
        ''' <param name="unitType"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetUnitString(ByVal unitType As cStyleGuide.eUnitType) As String
            Dim strUnitString As String = ""
            Select Case unitType
                Case cStyleGuide.eUnitType.Currency
                    strUnitString = Me.CurrencyUnitText(Me.CurrencyUnit)
                Case cStyleGuide.eUnitType.Time
                    strUnitString = Me.TimeUnitText(Me.TimeUnit)
                Case cStyleGuide.eUnitType.Monetary
                    strUnitString = Me.MonetaryUnitText(Me.MonetaryUnit)
                Case cStyleGuide.eUnitType.Nominal
                    strUnitString = Me.NominalUnitText()
                Case Else
                    Debug.Assert(False)
            End Select
            Return strUnitString
        End Function

#Region " Currency units "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get/set the currency unit text.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CurrencyUnit() As eUnitCurrencyType
            Get
                Return Me.m_unitCurrency
            End Get
            Set(ByVal value As eUnitCurrencyType)
                If (Me.m_unitCurrency <> value) Then
                    Me.m_unitCurrency = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property CurrencyUnitText(ByVal unit As eUnitCurrencyType) As String
            Get
                Select Case unit
                    Case eUnitCurrencyType.Calorie
                        Return My.Resources.UNIT_CURRENCY_CALORIE
                    Case eUnitCurrencyType.Carbon
                        Return My.Resources.UNIT_CURRENCY_CARBON
                    Case eUnitCurrencyType.DryWeight
                        Return My.Resources.UNIT_CURRENCY_DRYWEIGHT
                    Case eUnitCurrencyType.Joules
                        Return My.Resources.UNIT_CURRENCY_JOULES
                    Case eUnitCurrencyType.Nitrogen
                        Return My.Resources.UNIT_CURRENCY_NITROGEN
                    Case eUnitCurrencyType.Phosporous
                        Return My.Resources.UNIT_CURRENCY_PHOSPOROUS
                    Case eUnitCurrencyType.WetWeight
                        Return My.Resources.UNIT_CURRENCY_WETWEIGHT
                End Select
                Return Me.CustomCurrencyUnitText()
            End Get
        End Property

        Public Property CustomCurrencyUnitText() As String
            Get
                Return Me.m_strUnitCurrencyCustom
            End Get
            Set(ByVal value As String)
                If (String.Compare(Me.m_strUnitCurrencyCustom, value) <> 0) Then
                    Me.m_strUnitCurrencyCustom = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

#End Region ' Currency units

#Region " Time units "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get/set the time unit text.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property TimeUnit() As eUnitTimeType
            Get
                Return Me.m_unitTime
            End Get
            Set(ByVal value As eUnitTimeType)
                If (Me.m_unitTime <> value) Then
                    Me.m_unitTime = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property TimeUnitText(ByVal unit As eUnitTimeType) As String
            Get
                Select Case unit
                    Case eUnitTimeType.Day
                        Return My.Resources.UNIT_TIME_DAY
                    Case eUnitTimeType.Year
                        Return My.Resources.UNIT_TIME_YEAR
                End Select
                Return Me.CustomTimeUnitText()
            End Get
        End Property

        Public Property CustomTimeUnitText() As String
            Get
                Return Me.m_strUnitTimeCustom
            End Get
            Set(ByVal value As String)
                If (String.Compare(Me.m_strUnitTimeCustom, value) <> 0) Then
                    Me.m_strUnitTimeCustom = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

#End Region ' Time units

#Region " Monetary units "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get/set the monetary unit to show in the application.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property MonetaryUnit() As eUnitMonetaryType
            Get
                Return Me.m_unitMonetary
            End Get
            Set(ByVal value As eUnitMonetaryType)
                If (Me.m_unitMonetary <> value) Then
                    Me.m_unitMonetary = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get the monetary unit text to show in the application.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property MonetaryUnitDescription(ByVal unit As eUnitMonetaryType) As String
            Get
                If Me.m_dtMonetaryUnitNames.ContainsKey(unit) Then
                    Return Me.m_dtMonetaryUnitNames(unit)
                Else
                    Return Me.CustomMonetaryUnitText()
                End If
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get the monetary unit text to show in the application.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property MonetaryUnitText(ByVal unit As eUnitMonetaryType) As String
            Get
                If Me.m_dtMonetaryUnitNames.ContainsKey(unit) Then
                    Return unit.ToString()
                Else
                    Return Me.CustomMonetaryUnitText()
                End If
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get/set the text for the monetary unit text to show 
        ''' in the application.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CustomMonetaryUnitText() As String
            Get
                Return Me.m_strUnitMonetaryCustom
            End Get
            Set(ByVal value As String)
                If (String.Compare(Me.m_strUnitMonetaryCustom, value) <> 0) Then
                    Me.m_strUnitMonetaryCustom = value
                    Me.UnitsChanged()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, get the nominal unit text to show in the application.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NominalUnitText() As String
            Get
                Return "#"
            End Get
        End Property

#End Region ' Monetary units

#End Region ' Units

#Region " Maps and charts "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set how graphs should show legends.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowLegends() As TriState
            Get
                Return Me.m_tsShowLegends
            End Get
            Set(ByVal value As TriState)
                Me.m_tsShowLegends = value
                Me.LegendsChanged()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Broadcast a <see cref="eChangeType.Legends">legends changed event</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub LegendsChanged()
            Me.FireChangeEvent(eChangeType.Legends)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set how graphs should display backgrounds.
        ''' </summary>
        ''' <remarks>
        ''' Whenever this setting changes a <see cref="eChangeType.Colours">Colours</see>
        ''' change is broadcasted.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property UseTransparentBackgrounds() As Boolean
            Get
                Return Me.m_bTransparentBackgrounds
            End Get
            Set(ByVal value As Boolean)
                If value <> Me.m_bTransparentBackgrounds Then
                    Me.m_bTransparentBackgrounds = value
                    Me.ColorsChanged()
                End If
            End Set
        End Property

#End Region ' Maps and charts

#Region " Color access "

#Region " Group "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color to represent a group.
        ''' </summary>
        ''' <remarks>
        ''' Setting the Alpha component of the ARGB colour value to 0 will
        ''' trigger the style guide to issue default colours for groups.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property GroupColor(ByVal core As cCore, ByVal iGroup As Integer) As Color
            Get
                Dim clr As Color = Color.Transparent
                If (0 < iGroup) And (iGroup <= core.nGroups) Then
                    Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                    clr = cColorUtils.IntToColor(grp.PoolColor)
                End If
                If clr.A = 0 Then
                    clr = Me.GroupColorDefault(core, iGroup)
                End If
                Return clr
            End Get
            Set(ByVal value As Color)
                If (0 < iGroup) And (iGroup <= core.nGroups) Then
                    Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                    ' Optimization
                    If grp.PoolColor = cColorUtils.ColorToInt(value) Then Return
                    ' Apply
                    grp.PoolColor = cColorUtils.ColorToInt(value)
                    ' Notify the world
                    Me.ColorsChanged()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a group.
        ''' </summary>
        ''' <param name="core">Core to operate onto.</param>
        ''' <param name="iGroup">The group index to obtain the default colour for.</param>
        ''' <returns>
        ''' Default group colours are picked from the Ecopath 5 group colour scheme.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function GroupColorDefault(ByVal core As cCore, _
                                          ByVal iGroup As Integer) As Color
            Return Me.GroupColorDefault(iGroup, core.nGroups)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a group.
        ''' </summary>
        ''' <param name="iGroup">The group index to obtain the default colour for.</param>
        ''' <param name="nGroups">The number of groups to scale the colour to.</param>
        ''' <returns>
        ''' Default group colours are picked from the Ecopath 5 group colour scheme.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function GroupColorDefault(ByVal iGroup As Integer, _
                                          ByVal nGroups As Integer) As Color
            Return Me.m_colorrampGroups.GetColor(iGroup, nGroups)
        End Function

#End Region ' Group

#Region " Fleet "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color to represent a fleet.
        ''' </summary>
        ''' <remarks>
        ''' Setting the Alpha component of the ARGB colour value to 0 will
        ''' trigger the style guide to issue default colours for fleets.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property FleetColor(ByVal core As cCore, ByVal iFleet As Integer) As Color
            Get
                Dim clr As Color = Color.Transparent
                If (0 <= iFleet) And (iFleet <= core.nFleets) Then
                    Dim flt As cFleetInput = core.FleetInputs(iFleet)
                    clr = cColorUtils.IntToColor(flt.PoolColor)
                End If
                If clr.A = 0 Then
                    clr = Me.FleetColorDefault(core, iFleet)
                End If
                Return clr
            End Get
            Set(ByVal value As Color)
                If (0 <= iFleet) And (iFleet <= core.nFleets) Then
                    Dim flt As cFleetInput = core.FleetInputs(iFleet)
                    ' Optimization
                    If flt.PoolColor = cColorUtils.ColorToInt(value) Then Return
                    ' Apply
                    flt.PoolColor = cColorUtils.ColorToInt(value)
                    ' Notify the world
                    Me.ColorsChanged()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a fleet.
        ''' </summary>
        ''' <param name="iFleet">The fleet index to obtain the default colour for.</param>
        ''' <param name="nFleets">Number of fleets to scale colour by, or -1 to
        ''' use the max number of fleets as dictated by the <paramref name="core">core</paramref>.</param>
        ''' <returns>
        ''' Default fleet colours are picked from a colour ramp that runs from
        ''' green to blue.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function FleetColorDefault(ByVal iFleet As Integer, _
                                          ByVal nFleets As Integer) As Color
            Return Me.m_colorrampFleets.GetColor(iFleet, nFleets)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a fleet.
        ''' </summary>
        ''' <param name="core">Core to operate onto.</param>
        ''' <param name="iFleet">The fleet index to obtain the default colour for.</param>
        ''' <returns>
        ''' Default fleet colours are picked from a colour ramp that runs from
        ''' green to blue.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function FleetColorDefault(ByVal core As cCore, _
                                          ByVal iFleet As Integer) As Color
            Return FleetColorDefault(iFleet, core.nFleets)
        End Function

#End Region ' Fleet 

#Region " Pedigree "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color to represent a pedigree level.
        ''' </summary>
        ''' <remarks>
        ''' Setting the Alpha component of the ARGB colour value to 0 will
        ''' trigger the style guide to issue default colours for pedigree levels.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property PedigreeColor(ByVal core As cCore, ByVal vn As eVarNameFlags, ByVal iLevel As Integer) As Color
            Get
                Dim clr As Color = Color.Transparent
                Dim man As cPedigreeManager = core.GetPedigreeManager(vn)
                If (man Is Nothing) Then Return clr
                If (0 <= iLevel) And (iLevel < man.NumLevels) Then
                    Dim lvl As cPedigreeLevel = man.Level(iLevel)
                    clr = cColorUtils.IntToColor(lvl.PoolColor)
                End If
                If clr.A = 0 Then
                    clr = Me.PedigreeColorDefault(iLevel, man.NumLevels - 1)
                End If
                Return clr
            End Get
            Set(ByVal value As Color)
                Dim man As cPedigreeManager = core.GetPedigreeManager(vn)
                If (man IsNot Nothing) Then
                    If (0 < iLevel) And (iLevel < man.NumLevels) Then
                        Dim lvl As cPedigreeLevel = man.Level(iLevel)
                        ' Optimization
                        If lvl.PoolColor = cColorUtils.ColorToInt(value) Then Return
                        ' Apply
                        lvl.PoolColor = cColorUtils.ColorToInt(value)
                        ' Notify the world
                        Me.ColorsChanged()
                    End If
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a pedigree level.
        ''' </summary>
        ''' <param name="iLevel">The level index to obtain the default colour for.</param>
        ''' <param name="nLevels">Number of levels to scale colour by.</param>
        ''' <returns>
        ''' Default pedgree colours are picked from a colour ramp that runs from
        ''' red, via yellow, to green.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function PedigreeColorDefault(ByVal iLevel As Integer, _
                                             ByVal nLevels As Integer) As Color
            Return Me.m_colorrampPedigree.GetColor(iLevel, nLevels)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a default colour for a pedigree level.
        ''' </summary>
        ''' <param name="core">Core to operate onto.</param>
        ''' <param name="iLevel">The level index to obtain the default colour for.</param>
        ''' <param name="vn">The variable of the level to query.</param>
        ''' <returns>
        ''' Default group colours are picked from a colour ramp that runs from
        ''' green to blue.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function PedigreeColorDefault(ByVal core As cCore, _
                                             ByVal iLevel As Integer, _
                                             ByVal vn As eVarNameFlags) As Color
            Debug.Assert(core.IsPedigreeVariableSupported(vn))
            Return PedigreeColorDefault(iLevel, core.GetPedigreeManager(vn).NumLevels)
        End Function

#End Region ' Pedigree

#Region " Application "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type defining the types of EwE6 user interface elements
        ''' for which custom colour coding is available.
        ''' </summary>
        ''' <remarks>
        ''' Not all styles will support both foreground and background colours.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Enum eApplicationColorType As Integer
            NotSet = 0
            DEFAULT_TEXT
            DEFAULT_BACKGROUND
            READONLY_BACKGROUND
            REMARKS_BACKGROUND
            SUM_BACKGROUND
            NAMES_TEXT
            NAMES_BACKGROUND
            CHECKED_BACKGROUND
            FAILEDVALIDATION_TEXT
            MISSINGPARAMETER_BACKGROUND
            COMPUTED_TEXT
            INVALIDMODELRESULT_TEXT
            GENERICERROR_TEXT
            PROFIT_TEXT
            FISHINGPRESSURE_TEXT
            TOTALCATCH_TEXT
            TROPHICLINK_TEXT
            HIGHLIGHT
            IMAGE_BACKGROUND
            PLOT_BACKGROUND
            MAP_BACKGROUND
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type defining the types of EwE6 user interface elements
        ''' for which custom font options are available.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eApplicationFontType As Integer
            NotSet = 0
            ''' <summary>The font to use for graphs and charts major titles.</summary>
            Title
            ''' <summary>The font to use for graphs and charts legend text.</summary>
            Legend
            ''' <summary>The font to use for graphs and charts minor titles, 
            ''' such as subtitles, axis labels, legend titles, etc.</summary>
            SubTitle
            ''' <summary>The font to use for graph and chart axis labels.</summary>
            Scale
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get colours for a given combination of <see cref="eStyleFlags">styles</see>.
        ''' </summary>
        ''' <param name="eStatus">The bitwise pattern of <see cref="eStyleFlags">style</see>
        ''' to retrieve a foreground and background colour for.</param>
        ''' <param name="colorText">A foreground color that will be returned for the
        ''' given style pattern.</param>
        ''' <param name="colorBackground">A background color that will be returned for the
        ''' given style pattern.</param>
        ''' <remarks>
        ''' The algorithm that picks the colour to return analyzes that provided
        ''' style flags by order of priority. This priority is arbitrary, where
        ''' style flags indicating severe statuses will precede over lesser,
        ''' mere informational style flags.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub GetStyleColors(ByVal eStatus As cStyleGuide.eStyleFlags, _
                                  ByRef colorText As Color, _
                                  ByRef colorBackground As Color)

            ' Default priorities, used when the provided priorities did not yield
            ' a status to display, or when no priority sequence has been provided.
            Dim ePriorities() As cStyleGuide.eStyleFlags = { _
                    cStyleGuide.eStyleFlags.Null, _
                    cStyleGuide.eStyleFlags.InvalidModelResult, _
                    cStyleGuide.eStyleFlags.FailedValidation, _
                    cStyleGuide.eStyleFlags.ErrorEncountered, _
                    cStyleGuide.eStyleFlags.ValueComputed, _
                    cStyleGuide.eStyleFlags.Remarks, _
                    cStyleGuide.eStyleFlags.Sum, _
                    cStyleGuide.eStyleFlags.Names, _
                    cStyleGuide.eStyleFlags.Checked, _
                    cStyleGuide.eStyleFlags.NotEditable, _
                    cStyleGuide.eStyleFlags.OK}

            ' JS 02Aug08: disabled, not used at all
            'StyleGuide.eStyleFlags.FishingPressure, _
            'StyleGuide.eStyleFlags.Profit, _
            'StyleGuide.eStyleFlags.TotalCatch, _
            'StyleGuide.eStyleFlags.TrophicLink, _

            ' Set defaults
            Dim eColorText As cStyleGuide.eApplicationColorType = 0
            Dim eColorBack As cStyleGuide.eApplicationColorType = 0

            ' Variable statuses may have a text style, a background style or both.
            ' 
            ' This code can probably do with some serious optimizing.

            ' Now iterate in REVERSE ORDER (e.g. least important styles first)
            ' through the constructed priorities array. Every time a style match
            ' is encountered, available style parts are 'upgraded'
            For i As Integer = ePriorities.Length - 1 To 0 Step -1

                Select Case (eStatus And ePriorities(i))

                    Case cStyleGuide.eStyleFlags.Null
                        ' No specific colour feedback

                    Case cStyleGuide.eStyleFlags.InvalidModelResult
                        eColorText = eApplicationColorType.INVALIDMODELRESULT_TEXT

                    Case cStyleGuide.eStyleFlags.FailedValidation
                        eColorText = eApplicationColorType.FAILEDVALIDATION_TEXT

                    Case cStyleGuide.eStyleFlags.ErrorEncountered
                        eColorText = eApplicationColorType.GENERICERROR_TEXT

                    Case cStyleGuide.eStyleFlags.ValueComputed
                        eColorText = eApplicationColorType.COMPUTED_TEXT

                    Case cStyleGuide.eStyleFlags.Remarks
                        eColorBack = eApplicationColorType.REMARKS_BACKGROUND

                    Case cStyleGuide.eStyleFlags.Sum
                        eColorBack = eApplicationColorType.SUM_BACKGROUND

                        'Case StyleGuide.eStyleFlags.FishingPressure
                        '    eColorText = eApplicationColorType.FISHINGPRESSURE_TEXT

                        'Case StyleGuide.eStyleFlags.Profit
                        '    eColorText = eApplicationColorType.PROFIT_TEXT

                        'Case StyleGuide.eStyleFlags.TotalCatch
                        '    eColorText = eApplicationColorType.TOTALCATCH_TEXT

                        'Case StyleGuide.eStyleFlags.TrophicLink
                        '    eColorText = eApplicationColorType.TROPHICLINK_TEXT

                    Case eStyleFlags.Checked
                        eColorBack = eApplicationColorType.CHECKED_BACKGROUND

                    Case cStyleGuide.eStyleFlags.Names
                        eColorText = eApplicationColorType.NAMES_TEXT
                        eColorBack = eApplicationColorType.NAMES_BACKGROUND

                    Case cStyleGuide.eStyleFlags.NotEditable
                        eColorBack = eApplicationColorType.READONLY_BACKGROUND

                    Case cStyleGuide.eStyleFlags.OK
                        eColorText = eApplicationColorType.DEFAULT_TEXT
                        eColorBack = eApplicationColorType.DEFAULT_BACKGROUND

                End Select
            Next i

            ' Finally fetch the real colours
            If eColorText > 0 Then colorText = Color.FromArgb(Me.ApplicationColor(eColorText).ToArgb)
            If eColorBack > 0 Then colorBackground = Color.FromArgb(Me.ApplicationColor(eColorBack).ToArgb)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color for a particular type of <see cref="eApplicationColorType">application feedback.</see>.
        ''' </summary>
        ''' <param name="colorType">The <see cref="eApplicationColorType">application feedback type</see>
        ''' to affect.</param>
        ''' -------------------------------------------------------------------
        Public Property ApplicationColor(ByVal colorType As cStyleGuide.eApplicationColorType) As Color
            Get
                ' Sanity check
                If (Me.m_dtApplicationColors.ContainsKey(colorType)) Then
                    Return Me.m_dtApplicationColors(colorType)
                End If
                Return Me.DefaultColor(colorType)
            End Get
            Set(ByVal value As Color)
                If (Me.m_dtApplicationColors.ContainsKey(colorType)) Then
                    ' Optimization
                    If Me.m_dtApplicationColors(colorType) = value Then Return
                    Me.m_dtApplicationColors.Remove(colorType)
                End If
                ' Apply
                Me.m_dtApplicationColors(colorType) = value
                ' Notify the world
                Me.ColorsChanged()
            End Set
        End Property

#End Region ' Application

#Region " Generics "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a list of colours, picked from the Ecopath 5 group colour
        ''' scheme.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetEwE5ColorRamp(ByVal iNumLevels As Integer) As List(Of Color)
            Dim lColors As New List(Of Color)
            For i As Integer = 0 To iNumLevels
                Dim clr As Color = Me.m_colorrampGroups.GetColor(i, iNumLevels)
                lColors.Add(clr)
            Next
            Return lColors
        End Function

        Public Sub ColorsChanged()
            Me.FireChangeEvent(eChangeType.Colours)
        End Sub

        Private m_iAngle As Integer = cCore.NULL_VALUE
        Private Const sFactor As Single = 180.0! / Math.PI

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a random color.
        ''' </summary>
        ''' <returns>A random color.</returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NextRandomColor() As Color
            Get
                If (Me.m_iAngle = cCore.NULL_VALUE) Or (Me.m_iAngle > 200000) Then
                    Me.m_iAngle = New Random().Next(0, 31452)
                End If
                Me.m_iAngle += 33
                Return Color.FromArgb(CInt(Math.Sin((Me.m_iAngle) * 1.412 / sFactor) * 115 + 115), _
                                      CInt(Math.Sin((Me.m_iAngle) * 3.81 / sFactor) * 105 + 150), _
                                      CInt(Math.Sin((Me.m_iAngle) * 2.1231 / sFactor) * 115 + 140))
            End Get
        End Property

#End Region ' Generics

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate a series of alternating <see cref="HSV">HSV colors</see> 
        ''' over a range.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <param name="iLen"></param>
        ''' <param name="iHueScale"></param>
        ''' <param name="iSaturationRange"></param>
        ''' <param name="iValueRange"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function CalculateAlternatingColors(ByVal i As Integer, _
                                                          ByVal iLen As Integer, _
                                                          Optional ByVal iHueScale As Integer = 9, _
                                                          Optional ByVal iSaturationRange As Integer = 240, _
                                                          Optional ByVal iValueRange As Integer = 200) As HSV

            Dim nCount As Integer = CInt(Math.Ceiling(Math.Sqrt(iLen / iHueScale)))
            Dim iHueTick As Integer = 255 \ iHueScale
            Dim iSaturationTick As Integer = 0
            Dim iValueTick As Integer = 0

            If nCount > 1 Then
                iSaturationTick = iSaturationRange \ nCount
                iValueTick = iValueRange \ nCount
            End If

            Dim i1 As Integer = (i - 1) Mod iHueScale
            Dim i2 As Integer = ((i - 1) \ iHueScale) Mod nCount
            Dim i3 As Integer = ((i - 1) \ (iHueScale * nCount)) Mod nCount
            Return New HSV(i1 * iHueTick, 255 - i2 * iSaturationTick, 255 - i3 * iValueTick)

        End Function

        Public Shared Function CalculateAlternatingStanzaGroupColor(ByVal hsvGroup As HSV, ByVal iLifeStage As Integer, ByVal iNumLifeStages As Integer) As HSV

            Dim sRange As Integer = 255
            Dim vRange As Integer = 100

            Dim sTick As Integer = sRange \ iNumLifeStages
            Dim vTick As Integer = vRange \ iNumLifeStages

            Return New HSV(hsvGroup.Hue, hsvGroup.Saturation - iLifeStage * sTick, hsvGroup.Value - (iNumLifeStages - iLifeStage - 1) * vTick)

        End Function

#End Region ' Color access

#Region " Fonts "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the font for a given application type. The font size is specified
        ''' in <see cref="GraphicsUnit.Point">points</see>.
        ''' </summary>
        ''' <param name="ft"></param>
        ''' <remarks>You must manually dispose the font after usage.</remarks>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Font(ByVal ft As eApplicationFontType) As Font
            Get
                Return New Font(Me.FontFamilyName(ft), Me.FontSize(ft), Me.FontStyle(ft), GraphicsUnit.Point)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="FontFamily.Name">font family name</see> for 
        ''' a given application type.
        ''' </summary>
        ''' <param name="ft"></param>
        ''' -------------------------------------------------------------------
        Public Property FontFamilyName(ByVal ft As eApplicationFontType) As String
            Get
                If Me.m_dtFontFamilyName.ContainsKey(ft) Then
                    Dim strName As String = Me.m_dtFontFamilyName(ft)
                    If Not String.IsNullOrEmpty(strName) Then
                        Return strName
                    End If
                End If
                Return Me.DefaultFontFamilyName(ft)
            End Get
            Set(ByVal value As String)
                Me.m_dtFontFamilyName(ft) = value
                Me.FontsChanged()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="FontStyle">font style</see> for a given 
        ''' application type.
        ''' </summary>
        ''' <param name="ft"></param>
        ''' -------------------------------------------------------------------
        Public Property FontStyle(ByVal ft As eApplicationFontType) As FontStyle
            Get
                If Me.m_dtFontStye.ContainsKey(ft) Then
                    Return Me.m_dtFontStye(ft)
                End If
                Return Me.DefaultFontStyle(ft)
            End Get
            Set(ByVal value As FontStyle)
                Me.m_dtFontStye(ft) = value
                Me.FontsChanged()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the font size for a given application type. The font size 
        ''' is specified in <see cref="GraphicsUnit.Point">points</see>.
        ''' </summary>
        ''' <param name="ft"></param>
        ''' -------------------------------------------------------------------
        Public Property FontSize(ByVal ft As eApplicationFontType) As Single
            Get
                If Me.m_dtFontSize.ContainsKey(ft) Then
                    Dim sSize As Single = Me.m_dtFontSize(ft)
                    If sSize >= 6 Then
                        Return sSize
                    End If
                End If
                Return Me.DefaultFontSize(ft)
            End Get
            Set(ByVal value As Single)
                Me.m_dtFontSize(ft) = value
                Me.FontsChanged()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Broadcast a <see cref="eChangeType.Fonts">font changed event</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub FontsChanged()
            Me.FireChangeEvent(eChangeType.Fonts)
        End Sub

#End Region ' Fonts

#Region " Thumbnails "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the size of thumbnails.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ThumbnailSize() As Integer
            Get
                Return Me.m_iThumbnailSize
            End Get
            Set(ByVal value As Integer)
                Me.m_iThumbnailSize = value
                Me.ThumbnailsChanged()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Broadcast a <see cref="eChangeType.Thumbnails">thumbnails changed event</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ThumbnailsChanged()
            Me.FireChangeEvent(eChangeType.Thumbnails)
        End Sub

#End Region ' Thumbnails

#Region " Item visibility "

        Public Property GroupVisible(ByVal iGroup As Integer) As Boolean
            Get
                ' Return whether group is not hidden
                Return (Me.m_lHiddenGroups.IndexOf(iGroup) = -1)
            End Get
            Set(ByVal bVisible As Boolean)

                Dim bChanged As Boolean = False

                If bVisible Then
                    ' Remove group from hidden list, if applicable
                    If (Me.m_lHiddenGroups.IndexOf(iGroup) <> -1) Then
                        Me.m_lHiddenGroups.Remove(iGroup)
                        bChanged = True
                    End If
                Else
                    ' Add group to hidden list, if applicable
                    If (Me.m_lHiddenGroups.IndexOf(iGroup) = -1) Then
                        Me.m_lHiddenGroups.Add(iGroup)
                        bChanged = True
                    End If
                End If

                If bChanged Then Me.FireChangeEvent(eChangeType.GroupVisibility)
            End Set
        End Property

        Public Property FleetVisible(ByVal iFleet As Integer) As Boolean
            Get
                ' Return whether fleet is not hidden
                Return (Me.m_lHiddenFleets.IndexOf(iFleet) = -1)
            End Get
            Set(ByVal bVisible As Boolean)

                Dim bChanged As Boolean = False

                If bVisible Then
                    ' Remove fleet from hidden list, if applicable
                    If (Me.m_lHiddenFleets.IndexOf(iFleet) <> -1) Then
                        Me.m_lHiddenFleets.Remove(iFleet)
                        bChanged = True
                    End If
                Else
                    ' Add fleet to hidden list, if applicable
                    If (Me.m_lHiddenFleets.IndexOf(iFleet) = -1) Then
                        Me.m_lHiddenFleets.Add(iFleet)
                        bChanged = True
                    End If
                End If

                If bChanged Then Me.FireChangeEvent(eChangeType.FleetVisibility)
            End Set
        End Property

        Public Property TotalCatchVisible() As Boolean
            Get
                Return (Me.m_bHideTotalCatch = False)
            End Get
            Set(ByVal bShow As Boolean)
                Me.m_bHideTotalCatch = (bShow = False)
            End Set
        End Property

        Public Property TotalValueVisible() As Boolean
            Get
                Return (Me.m_bHideTotalValue = False)
            End Get
            Set(ByVal bShow As Boolean)
                Me.m_bHideTotalValue = (bShow = False)
            End Set
        End Property

        Public Sub ResetVisibleFlags(Optional ByVal bFireChangeEvent As Boolean = True)
            Me.m_lHiddenGroups.Clear()
            Me.m_lHiddenFleets.Clear()
            Me.m_bHideTotalCatch = False
            Me.m_bHideTotalValue = False

            If bFireChangeEvent Then Me.FireChangeEvent(eChangeType.GroupVisibility Or eChangeType.FleetVisibility)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Broadcast a <see cref="eChangeType.GroupVisibility">group</see> and
        ''' <see cref="eChangeType.FleetVisibility">fleet</see> visibility 
        ''' changed event.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ItemVisibilityChanged()
            Me.FireChangeEvent(eChangeType.GroupVisibility Or eChangeType.FleetVisibility)
        End Sub

#End Region ' Item visibility

#End Region ' Public access

#Region " Internal implementation "

        Private Function DefaultColor(ByVal colorType As eApplicationColorType) As Color
            Select Case colorType
                Case eApplicationColorType.DEFAULT_TEXT : Return Color.Black
                Case eApplicationColorType.DEFAULT_BACKGROUND : Return Color.White
                Case eApplicationColorType.NAMES_TEXT : Return Color.Black
                Case eApplicationColorType.NAMES_BACKGROUND : Return Color.FromArgb(255, 232, 232, 232)
                Case eApplicationColorType.HIGHLIGHT : Return Color.Orange
                Case eApplicationColorType.INVALIDMODELRESULT_TEXT : Return Color.DarkViolet
                Case eApplicationColorType.FAILEDVALIDATION_TEXT : Return Color.DarkGoldenrod
                Case eApplicationColorType.GENERICERROR_TEXT : Return Color.Firebrick
                Case eApplicationColorType.COMPUTED_TEXT : Return Color.FromArgb(255, 0, 0, 244)
                Case eApplicationColorType.FISHINGPRESSURE_TEXT : Return Color.Red
                Case eApplicationColorType.PROFIT_TEXT : Return Color.Blue
                Case eApplicationColorType.TOTALCATCH_TEXT : Return Color.LightCoral
                Case eApplicationColorType.TROPHICLINK_TEXT : Return Color.LavenderBlush
                Case eApplicationColorType.CHECKED_BACKGROUND : Return Color.Coral
                Case eApplicationColorType.REMARKS_BACKGROUND : Return Color.White
                Case eApplicationColorType.SUM_BACKGROUND : Return Color.FromArgb(255, 255, 254, 225)
                Case eApplicationColorType.READONLY_BACKGROUND : Return Color.FromArgb(255, 231, 235, 250)
                Case eApplicationColorType.MISSINGPARAMETER_BACKGROUND : Return Color.MediumPurple
                Case eApplicationColorType.IMAGE_BACKGROUND : Return Color.White
                Case eApplicationColorType.PLOT_BACKGROUND : Return Color.White
                Case eApplicationColorType.MAP_BACKGROUND : Return SystemColors.ControlDark
                Case eApplicationColorType.NotSet
                    Return Color.Transparent
            End Select
            ' This should not happen, a default should always be available
            Debug.Assert(False)
            Return Color.Black
        End Function

        Private Sub LoadMonetaryUnitNames()

            Dim dtNames As New Dictionary(Of String, String)
            Dim astrBits As String() = My.Resources.GENERIC_CURRENCIES.Split("|"c)

            ' Sanity check
            For i As Integer = 0 To astrBits.Length - 1 Step 2
                If astrBits(i).Length <> 3 Then
                    Debug.Assert(False, String.Format("Error near currency {0}, expected three-letter currency abbreviation", astrBits(i)))
                    Return
                End If
                dtNames(astrBits(i)) = astrBits(i + 1)
            Next

            For Each unit As eUnitMonetaryType In [Enum].GetValues(GetType(eUnitMonetaryType))
                If unit <> eUnitMonetaryType.NotSet Then
                    Me.m_dtMonetaryUnitNames(unit) = dtNames(unit.ToString())
                End If
            Next

            dtNames.Clear()

        End Sub

        Private Function DefaultFontFamilyName(ByVal ft As eApplicationFontType) As String
            Return "Microsoft Sans Serif"
        End Function

        Private Function DefaultFontStyle(ByVal ft As eApplicationFontType) As FontStyle
            Return Drawing.FontStyle.Regular
        End Function

        Private Function DefaultFontSize(ByVal ft As eApplicationFontType) As Single
            Select Case ft
                Case eApplicationFontType.Title
                    Return 12
                Case eApplicationFontType.Legend, eApplicationFontType.SubTitle
                    Return 10
                Case eApplicationFontType.Scale
                    Return 8.25
                Case Else
                    Debug.Assert(False)
            End Select
            Return -1
        End Function

#End Region ' Internal implementation

    End Class

End Namespace
