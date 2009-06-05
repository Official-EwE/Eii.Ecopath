'==============================================================================
'
' $Log: StyleGuide.vb,v $
' Revision 1.13  2009/06/05 16:01:34  jeroens
' Limited max precision
'
' Revision 1.12  2009/06/04 23:52:58  jeroens
' Added null style support for formatting numbers
'
' Revision 1.11  2009/05/28 12:37:48  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.10  2009/04/28 14:21:59  jeroens
' Added Fleet visibility
'
' Revision 1.9  2009/04/07 19:58:11  jeroens
' Changed default fonts
'
' Revision 1.8  2009/03/12 01:31:00  jeroens
' ResetVisibleFlags may not distribute event
'
' Revision 1.7  2009/02/24 06:04:09  jeroens
' Allow 0 decimal digits
'
' Revision 1.6  2009/02/20 17:57:30  jeroens
' Added nominal unit text
'
' Revision 1.5  2009/02/12 15:32:41  jeroens
' Fixed fonts
'
' Revision 1.4  2009/01/23 03:08:55  jeroens
' Removed unused imports
'
' Revision 1.3  2008/12/02 18:22:14  jeroens
' Added standard colour ramp offsets to prevent groups colours getting too light to see
'
' Revision 1.2  2008/11/27 03:10:43  jeroens
' Group visible flags maintained by style guide, no longer by AppLauncher
'
' Revision 1.1  2008/09/26 07:31:23  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports EwECore
Imports System.Drawing
Imports System.Text
Imports SAUPUtil.Misc.Colours
Imports EwEUtils.Core
Imports EwEUtils.Drawing

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

        ''' <summary>Singleton instance</summary>
        Private Shared _inst_ As cStyleGuide = New cStyleGuide()
        ''' <summary>Admin: Monetary unit name lookup table.</summary>
        Private m_dtMonetaryUnitNames As New Dictionary(Of eUnitMonetaryType, String)

        ''' <summary>States the number of decimal digits to be displayed</summary>
        Private m_iNumDigits As Integer = 3

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
        ''' <summary>Color ramp for obtaining the standard EwE5 colors</summary>
        Private m_clrrmpEwE5 As New SAUPColorRamp()
        ''' <summary>Start offset for colour ramp.</summary>
        Private Const c_sRampOffsetStart As Single = 0.15!
        ''' <summary>End offset for colour ramp.</summary>
        Private Const c_sRampOffsetEnd As Single = 1.0!

        ' -- graphs --
        ''' <summary></summary>
        Private m_strGraphFontFamilyName As String = "Microsoft Sans serif"
        ''' <summary></summary>
        Private m_sGraphCaptionFontSize As Single = 12
        ''' <summary></summary>
        Private m_fsGraphCaptionFontStye As FontStyle = FontStyle.Regular
        ''' <summary></summary>
        Private m_sGraphAxisLabelFontSize As Single = 10
        ''' <summary></summary>
        Private m_fsGraphAxisLabelFontStye As FontStyle = FontStyle.Regular
        ''' <summary></summary>
        Private m_sGraphAxisScaleFontSize As Single = 8.25
        ''' <summary></summary>
        Private m_fsGraphAxisScaleFontStye As FontStyle = FontStyle.Regular
        ''' <summary></summary>
        Private m_sGraphLegendFontSize As Single = 8.25
        ''' <summary></summary>
        Private m_fsGraphLegendFontStye As FontStyle = FontStyle.Regular

        ' -- group visibility --
        ''' <summary>List of indexes of groups to hide.</summary>
        Private m_lHiddenGroups As New List(Of Integer)
        ''' <summary>List of indexes of fleets to hide.</summary>
        Private m_lHiddenFleets As New List(Of Integer)
        Private m_bHideTotalCatch As Boolean = False
        Private m_bHideTotalValue As Boolean = False

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
        Private Sub New()

            ' Register one and only instance
            cStyleGuide._inst_ = Me

            ' Control how colour ramp delivers its colours
            Me.m_clrrmpEwE5.ColorOffsetStart = c_sRampOffsetStart
            Me.m_clrrmpEwE5.ColorOffsetEnd = c_sRampOffsetEnd

            ' Load up
            Me.LoadDefaultApplicationColors()
            Me.LoadMonetaryUnitNames()

        End Sub

#End Region ' Private bits

#Region " Public Methods "

        ''' <summary>
        ''' This method loads the default application color mainly used in rendering grid color scheme. 
        ''' </summary>
        Public Sub LoadDefaultApplicationColors()
            'Default colors
            m_dtApplicationColors.Clear()
        End Sub

#End Region ' Public interfaces

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
                Case eApplicationColorType.CHECKED_BACKGROUND : Return Color.FromArgb(255, 176, 233, 173)
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
                If unit <> eUnitMonetaryType.Custom Then
                    Me.m_dtMonetaryUnitNames(unit) = dtNames(unit.ToString())
                End If
            Next

            dtNames.Clear()

        End Sub

#End Region ' Internal implementation

#Region " Public access "

#Region " Styles "

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

        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Singleton: Retrieves the one and only instance of the StyleGuide
        ''' </summary>
        ''' <remarks>Use this method to obtain a reference to the StyleGuide</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetInstance() As cStyleGuide
            Return cStyleGuide._inst_
        End Function

#End Region ' Styles

#Region " NumDigits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sets the number of decimal digits to display.
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
                Me.FireChangeEvent(eChangeType.NumDigits)

            End Set
        End Property

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
        Public Function FormatNumber(ByVal sValue As Single, Optional ByVal style As eStyleFlags = eStyleFlags.OK, _
                Optional ByVal iNumDigits As Integer = -1) As String
            Return Me.FormatNumber(CDbl(sValue), style, iNumDigits)
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
        Public Function FormatNumber(ByVal dValue As Double, Optional ByVal style As eStyleFlags = eStyleFlags.OK, _
                Optional ByVal iNumDigits As Integer = -1) As String

            ' Use styleguide numdigits setting if value not provided
            If iNumDigits < 0 Then iNumDigits = Me.m_iNumDigits

            Dim dTest As Double = CDbl(Math.Abs(dValue))
            Dim iMinPrecision As Integer = 0
            Dim iMaxPrecision As Integer = Math.Min(iNumDigits * 2, 10)

            If (style And eStyleFlags.Null) > 0 Then
                Return ""
            End If

            ' Calculated values must be formatted with a hard number of digits
            If (style And (eStyleFlags.ValueComputed Or eStyleFlags.Sum)) > 0 Then
                Return Microsoft.VisualBasic.FormatNumber(dValue, iNumDigits)
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
            Return Microsoft.VisualBasic.FormatNumber(dValue, Math.Min(Math.Max(iNumDigits, iMinPrecision), iMaxPrecision))

        End Function

#End Region ' NumDigits

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

#Region " ChangeEvent "

        ''' <summary>
        ''' Types of changes that can occur in the StyleGuide.
        ''' </summary>
        Public Enum eChangeType As Integer
            None = 0
            Colours = &H1
            NumDigits = &H2
            Units = &H4
            Fonts = &H8
            GroupVisibility = &H10
            FleetVisibility = &H20
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

#End Region ' ChangeEvent

#Region " Color access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Color IDs supported by the StyleGuide.
        ''' </summary>
        ''' <remarks>
        ''' As may be apparent, not all styles require a foreground and a background.
        ''' colour.
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' To be written
        ''' </summary>
        ''' <param name="eStatus">The status to check</param>
        ''' <param name="colorText">A text color for this status</param>
        ''' <param name="colorBackground">A background color for this status</param>
        ''' <remarks>
        ''' Here, a core variable status will be analyzed to return both a 
        ''' text color and a background color.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Sub GetStyleColors(ByVal eStatus As cStyleGuide.eStyleFlags, _
            ByRef colorText As Color, ByRef colorBackground As Color)

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
            ' This code can probably do with some serious optimizing, but I'm not in the
            ' most brilliant mindset today. 

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

        ''' <summary>
        ''' Public property to get and set GroupColor
        ''' </summary>
        Public Property GroupColor(ByVal core As cCore, ByVal iGroup As Integer) As Color
            Get
                Dim clr As Color = Color.Transparent
                If (0 < iGroup) And (iGroup <= core.nGroups) Then
                    Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(iGroup)
                    clr = cStyleGuide.IntToColor(grp.PoolColor)
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
                    If grp.PoolColor = cStyleGuide.ColorToInt(value) Then Return
                    ' Apply
                    grp.PoolColor = cStyleGuide.ColorToInt(value)
                    ' Notify the world
                    Me.ColorsChanged()
                End If
            End Set
        End Property

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

        Public ReadOnly Property GroupColorDefault(ByVal core As cCore, ByVal iGroup As Integer, Optional ByVal nGroups As Integer = -1) As Color
            Get
                If nGroups = -1 Then nGroups = core.nGroups
                Return Me.m_clrrmpEwE5.GetColor(iGroup, nGroups)
            End Get
        End Property

        Public Function GetGroupColorIndex(ByVal core As cCore, ByVal clr As Color, Optional ByVal nGroups As Integer = -1) As Integer
            For iGroupIndex As Integer = 1 To core.nGroups
                If (Me.GroupColor(core, iGroupIndex) = clr) Then Return iGroupIndex
            Next
            Return -1
        End Function

        Public Shared Function IntToColor(ByVal iColor As Integer) As Color
            Return Drawing.Color.FromArgb((iColor >> 24) And &HFF, (iColor >> 16) And &HFF, (iColor >> 8) And &HFF, iColor And &HFF)
        End Function

        Public Shared Function ColorToInt(ByVal clr As Color) As Integer
            Return ((clr.A And &HFF) << 24) + ((clr.R And &HFF) << 16) + ((clr.G And &HFF) << 8) + (clr.B And &HFF)
        End Function

        ''' <summary>
        ''' Helper method to init the color ramp, one use is Ecospace output
        ''' </summary>
        ''' <remarks>The get one color algorithm was called from SAUPUtil getColor method
        ''' rewritten from EwE5, the agorithm is the same but use Double data type
        ''' for computation to avoid rounding error.</remarks>
        Public Function GetColorRamp(ByVal iNumLevels As Integer) As List(Of Color)
            Dim lColors As New List(Of Color)
            For i As Integer = 0 To iNumLevels
                Dim clr As Color = Me.m_clrrmpEwE5.GetColor(i, iNumLevels)
                lColors.Add(clr)
            Next
            Return lColors
        End Function

        Public Sub ColorsChanged()
            Me.FireChangeEvent(eChangeType.Colours)
        End Sub

        Public Shared Function CalculateAlternatingGroupColor(ByVal i As Integer, ByVal iLen As Integer, _
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

#Region " Graphs and figures "

        Public Property GraphFontFamilyName() As String
            Get
                Return Me.m_strGraphFontFamilyName
            End Get
            Set(ByVal value As String)
                If (String.Compare(Me.m_strGraphFontFamilyName, value, True) = 0) Then Return
                Me.m_strGraphFontFamilyName = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphCaptionFontSize() As Single
            Get
                Return Me.m_sGraphCaptionFontSize
            End Get
            Set(ByVal value As Single)
                If (value = Me.m_sGraphCaptionFontSize) Then Return
                Me.m_sGraphCaptionFontSize = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphCaptionFontStyle() As FontStyle
            Get
                Return Me.m_fsGraphCaptionFontStye
            End Get
            Set(ByVal value As FontStyle)
                If (Me.m_fsGraphCaptionFontStye = value) Then Return
                Me.m_fsGraphCaptionFontStye = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphAxisLabelFontSize() As Single
            Get
                Return Me.m_sGraphAxisLabelFontSize
            End Get
            Set(ByVal value As Single)
                If (Me.m_sGraphAxisLabelFontSize = value) Then Return
                Me.m_sGraphAxisLabelFontSize = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphAxisLabelFontStyle() As FontStyle
            Get
                Return m_fsGraphAxisLabelFontStye
            End Get
            Set(ByVal value As FontStyle)
                If (Me.m_fsGraphAxisLabelFontStye = value) Then Return
                Me.m_fsGraphAxisLabelFontStye = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphAxisScaleFontSize() As Single
            Get
                Return Me.m_sGraphAxisScaleFontSize
            End Get
            Set(ByVal value As Single)
                If (Me.m_sGraphAxisScaleFontSize = value) Then Return
                Me.m_sGraphAxisScaleFontSize = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphAxisScaleFontStyle() As FontStyle
            Get
                Return m_fsGraphAxisScaleFontStye
            End Get
            Set(ByVal value As FontStyle)
                If (Me.m_fsGraphAxisScaleFontStye = value) Then Return
                Me.m_fsGraphAxisScaleFontStye = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphLegendFontSize() As Single
            Get
                Return Me.m_sGraphLegendFontSize
            End Get
            Set(ByVal value As Single)
                If (Me.m_sGraphLegendFontSize = value) Then Return
                Me.m_sGraphLegendFontSize = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Property GraphLegendFontStyle() As FontStyle
            Get
                Return m_fsGraphLegendFontStye
            End Get
            Set(ByVal value As FontStyle)
                If (Me.m_fsGraphLegendFontStye = value) Then Return
                Me.m_fsGraphLegendFontStye = value
                Me.GraphsChanged()
            End Set
        End Property

        Public Sub GraphsChanged()
            Me.FireChangeEvent(eChangeType.Fonts)
        End Sub

#End Region ' Graphs and figures

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

#End Region ' Item visibility

#End Region ' Public access

    End Class

End Namespace
