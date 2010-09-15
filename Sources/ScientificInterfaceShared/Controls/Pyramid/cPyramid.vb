#Region " Imports "

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Globalization
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cPyramid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Pyramid types, enum values based on Network Analysis pyramid file formats
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum ePyramidTypes As Byte
        [Catch] = 0
        Flow = 1
        Biomass = 2
    End Enum

    Public Enum eRenderModeTypes As Byte
        ''' <summary>Render two dimensional</summary>
        Render2D
        ''' <summary>Render three dimensional</summary>
        Render3D
    End Enum

#Region " Private vars "

    Private m_strModel As String = ""
    Private m_bValid As Boolean = False

    ''' <summary>Number of TL read from data file.</summary>
    Private m_iNumTLMax As Integer = 0
    ''' <summary>Number of TL the user wants to see.</summary>
    Private m_iNumTL As Integer = 0
    ''' <summary>Type of pyramid.</summary>
    Private m_pyramidtype As ePyramidTypes = ePyramidTypes.Catch
    ''' <summary>Biomass unit.</summary>
    Private m_strUnit As String = ""
    ''' <summary>Total biomass.</summary>
    Private m_sTotal As Single = 0
    ''' <summary>Biomass per trophic level.</summary>
    Private m_asTRB() As Single = Nothing
    ''' <summary>TE per trophic level.</summary>
    Private m_asValue() As Single = Nothing

    ''' <summary>Calculated height of the pyramid.</summary>
    Private m_sHeight As Single = 0
    ''' <summary>Calculated width of the pyramid.</summary>
    Private m_sWidth As Single = 0
    ''' <summary>Positions of trophic levels, expressed as [0, 1].</summary>
    Private m_asLevels() As Single
    ''' <summary>Scale for drawing the pyramid.</summary>
    Private sScale As Single = 1.0

    ' Make it look pretty - experimental

    Private m_sTEpowNumTL As Single = 0.0!
    Private m_sMidCutScaleFactor As Single = 23.0!
    Private m_sMidCutLogCeiling As Single = 200.0!
    Private m_sHighCutoff As Single = 100.0!
    Private m_sHighCutoffAngle As Single = 19.0!
    Private m_sLowCutoff As Single = 4.0!
    Private m_sLowCutoffAngle As Single = 90.0!

#End Region ' Private vars

    Public Sub New()
        Me.CalculatePyramid()
    End Sub

    Public Sub New(ByVal strModel As String, _
                   ByVal pyramidtype As cPyramid.ePyramidTypes, _
                   ByVal strUnit As String, ByVal iNumTL As Integer, _
                   ByVal sTotalB As Single, ByVal asBiomass() As Single, ByVal asValue() As Single)

        Me.m_strModel = strModel
        Me.m_pyramidtype = pyramidtype
        Me.m_strUnit = strUnit
        Me.m_iNumTLMax = iNumTL
        Me.m_iNumTL = CInt(Math.Ceiling(iNumTL / 2))
        Me.m_sTotal = sTotalB
        Me.m_asTRB = asBiomass
        Me.m_asValue = asValue
        Me.CalculatePyramid()

    End Sub

    Public Sub Reset()
        Me.m_bValid = False
        Me.m_strModel = ""
        Me.m_pyramidtype = 0
        Me.m_iNumTLMax = 0
        Me.m_strUnit = ""
        Me.m_sTotal = 0.0!
    End Sub

#Region " Public properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the model that reflects the pyramid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Model() As String
        Get
            Return Me.m_strModel
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether the data few to the pyramid is valid.
    ''' </summary>
    ''' <remarks>
    ''' Simple validation is performed when reading pyramid data. For now, a
    ''' pyramid is valid if more than one TL of data was provided, and all
    ''' provided TL were successfully read.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsValid() As Boolean
        Get
            Return Me.m_bValid
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the actual number of trophic levels with biomass.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property FunctionalNumTL() As Integer
        Get
            Dim iTL As Integer = 0
            'For iTL As Integer = 1 To Me.m_iNumTLMax
            '    If Me.m_asTRB(iTL) > 0 Then
            '        iNumTLwithBiomass += 1
            '    End If
            'Next iTL
            While (Me.m_asTRB(iTL) > 0) And (iTL < Me.m_iNumTLMax)
                iTL += 1
            End While
            Return iTL
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Number of TL to display.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property NumTL() As Integer
        Get
            Return Math.Min(Me.m_iNumTL, Me.m_iNumTLMax)
        End Get
        Set(ByVal iNumTL As Integer)
            Me.m_iNumTL = iNumTL
            Me.CalculatePyramid()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Max number of TL as read from the data file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxNumTL() As Integer
        Get
            Return Me.m_iNumTLMax
        End Get
    End Property

    Public ReadOnly Property Width() As Single
        Get
            Return Me.m_sWidth
        End Get
    End Property

    Public ReadOnly Property Height() As Single
        Get
            Return Me.m_sHeight
        End Get
    End Property

    Public ReadOnly Property PyramidType() As ePyramidTypes
        Get
            Return Me.m_pyramidtype
        End Get
    End Property

    Public ReadOnly Property FitScale(ByVal rc As Rectangle) As Single
        Get
            Return CSng(Math.Min(rc.Width / Me.m_sWidth, rc.Height / Me.m_sHeight))
        End Get
    End Property

#End Region ' Public properties

#Region " File I/O "

    Public Function ToXML(ByVal strFilename As String) As Boolean

        Dim doc As XmlDocument = New XmlDocument()
        Dim nodePyramid As XmlNode = Nothing
        Dim attrib As XmlAttribute = Nothing
        Dim nodeTL As XmlNode = Nothing
        Dim ciEnUSLocale As New CultureInfo("en-US")

        Try

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", ""))
            nodePyramid = doc.CreateElement("Pyramid")
            doc.AppendChild(nodePyramid)

            attrib = doc.CreateAttribute("Model")
            attrib.Value = Me.m_strModel
            nodePyramid.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("Type")
            attrib.Value = Me.m_pyramidtype.ToString()
            nodePyramid.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("Unit")
            attrib.Value = Me.m_strUnit
            nodePyramid.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("TotalBiomass")
            attrib.Value = Me.m_sTotal.ToString(ciEnUSLocale)
            nodePyramid.Attributes.Append(attrib)

            attrib = doc.CreateAttribute("NumTL")
            attrib.Value = Me.m_iNumTLMax.ToString(ciEnUSLocale)
            nodePyramid.Attributes.Append(attrib)

            For iTL As Integer = 1 To Me.m_iNumTLMax
                nodeTL = doc.CreateElement("TrophicLevel")

                attrib = doc.CreateAttribute("Seq")
                attrib.Value = iTL.ToString(ciEnUSLocale)
                nodeTL.Attributes.Append(attrib)

                Select Case Me.PyramidType
                    Case ePyramidTypes.Biomass : attrib = doc.CreateAttribute("Biomass")
                    Case ePyramidTypes.Catch : attrib = doc.CreateAttribute("Catch")
                    Case ePyramidTypes.Flow : attrib = doc.CreateAttribute("Throughput")
                End Select
                attrib.Value = Me.m_asTRB(iTL).ToString(ciEnUSLocale)
                nodeTL.Attributes.Append(attrib)

                Select Case Me.PyramidType
                    Case ePyramidTypes.Biomass : attrib = doc.CreateAttribute("RelativeBiomass")
                    Case ePyramidTypes.Catch : attrib = doc.CreateAttribute("RelativeCatch")
                    Case ePyramidTypes.Flow : attrib = doc.CreateAttribute("RelativeThroughput")
                End Select
                attrib.Value = Me.m_asValue(iTL).ToString(ciEnUSLocale)
                nodeTL.Attributes.Append(attrib)

                nodePyramid.AppendChild(nodeTL)
            Next iTL

            doc.Save(strFilename)

        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function

    Public Function FromXML(ByVal strFilename As String) As Boolean

        Dim doc As XmlDocument = New XmlDocument()
        Dim iTL As Integer = 0
        Dim sVal1 As Single = 0.0!
        Dim sVal2 As Single = 0.0!

        Try
            Me.Reset()
            doc.Load(strFilename)
        Catch ex As Exception
            Return False
        End Try

        Try

            For Each nodePyramid As XmlNode In doc.ChildNodes
                If String.Compare(nodePyramid.Name, "pyramid", True) = 0 Then
                    ' Found 'pyramid' node, scam attributes
                    For Each attrib As XmlAttribute In nodePyramid.Attributes
                        Select Case attrib.Name.ToLower()
                            Case "model" : Me.m_strModel = attrib.Value
                            Case "type" : Me.m_pyramidtype = DirectCast([Enum].Parse(GetType(ePyramidTypes), attrib.Value), ePyramidTypes)
                            Case "unit" : Me.m_strUnit = attrib.Value
                            Case "totalbiomass" : Me.m_sTotal = cStringUtils.ConvertToSingle(attrib.Value, 0.0!)
                            Case "numtl" : Me.m_iNumTLMax = cStringUtils.ConvertToInteger(attrib.Value, 0)
                            Case Else ' NOP
                        End Select
                    Next

                    ReDim Me.m_asTRB(Me.m_iNumTLMax)
                    ReDim Me.m_asValue(Me.m_iNumTLMax)

                    For Each nodeTL As XmlNode In nodePyramid.ChildNodes
                        If String.Compare(nodeTL.Name, "trophiclevel", True) = 0 Then
                            ' Found TL node
                            iTL = 0 : sVal1 = 0.0! : sVal2 = 0.0!
                            For Each attrib As XmlAttribute In nodeTL.Attributes
                                Select Case attrib.Name.ToLower()
                                    Case "seq" : iTL = cStringUtils.ConvertToInteger(attrib.Value, 0)
                                    Case "biomass", "catch", "throughput" : sVal1 = cStringUtils.ConvertToSingle(attrib.Value, 0.0!)
                                    Case "relativebiomass", "relativecatch", "relativethroughput" : sVal2 = cStringUtils.ConvertToSingle(attrib.Value, 0.0!)
                                    Case Else ' NOP
                                End Select
                            Next
                            Me.m_asTRB(iTL) = sVal1
                            Me.m_asValue(iTL) = sVal2
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            Me.Reset()
            Return False
        End Try

        Me.CalculatePyramid()

        Return True

    End Function

    'Public Function Read(ByVal strFileName As String) As Boolean

    '    Dim tr As TextReader = Nothing
    '    Dim strLine As String = ""
    '    Dim bSucces As Boolean = True

    '    Me.m_bValid = False

    '    If (Not File.Exists(strFileName)) Then Return False

    '    tr = New StreamReader(strFileName, New System.Text.UTF8Encoding())
    '    Try
    '        strLine = tr.ReadLine()
    '        Me.m_pyramidtype = DirectCast(Convert.ToByte(strLine.Substring(0, 1)), ePyramidTypes)
    '        Me.m_iNumTLMax = Convert.ToInt16(strLine.Substring(1))
    '        ReDim Me.m_asTRB(Me.m_iNumTLMax)
    '        ReDim Me.m_asValue(Me.m_iNumTLMax)

    '        Me.m_strUnit = tr.ReadLine()

    '        Me.m_sTotalB = Convert.ToSingle(tr.ReadLine())
    '        For iTL As Integer = 0 To Me.m_iNumTLMax - 1
    '            strLine = tr.ReadLine()
    '            Me.m_asTRB(iTL) = Convert.ToSingle(strLine.Substring(0, 12))
    '            Me.m_asValue(iTL) = Convert.ToSingle(strLine.Substring(12))
    '        Next iTL

    '        ' Update
    '        Me.NumTL = Me.FunctionalNumTL
    '        Me.m_bValid = True

    '    Catch e As Exception
    '        bSucces = False
    '    Finally
    '        tr.Close()
    '    End Try

    '    Return bSucces
    'End Function

#End Region ' File IO

#Region " Plotting "

    Public Sub Plot(ByVal g As Graphics, ByVal rc As Rectangle, ByVal sScale As Single)

        If Not Me.IsValid Then Return

        'Dim sScale As Single = Math.Min(rc.Width / Me.m_sWidth, rc.Height / Me.m_sHeight)

        Dim ptTop As New Point(rc.Left + CInt(rc.Width / 2), rc.Top + CInt(rc.Height / 2 - (sScale * Me.m_sHeight / 2)))
        Dim ptBL As New Point(rc.Left + CInt((rc.Width / 2) - (sScale * Me.m_sWidth / 2)), rc.Top + CInt(rc.Height / 2 + (sScale * Me.m_sHeight / 2)))
        Dim ptBR As New Point(rc.Left + CInt((rc.Width / 2) + (sScale * Me.m_sWidth / 2)), rc.Top + CInt(rc.Height / 2 + (sScale * Me.m_sHeight / 2)))

        ' Plot pyramid
        g.DrawLine(Pens.Black, ptTop, ptBR)
        g.DrawLine(Pens.Black, ptBR, ptBL)
        g.DrawLine(Pens.Black, ptBL, ptTop)

        ' Plot TLs
        For iTL As Integer = 1 To Me.NumTL - 1
            Dim sYLevel As Single = Me.m_sHeight * Me.m_asLevels(iTL)
            Dim ptL As New Point(ptBL.X, ptTop.Y + CInt(sScale * sYLevel))
            Dim ptR As New Point(ptBR.X, ptL.Y)
            g.DrawLine(Pens.Red, ptL, ptR)
        Next

        ' Plot scale box
        Dim sVal As Single = CSng(Math.Pow(10, Math.Floor(Math.Log10(Me.SumB))))
        Dim sScaleBoxSize As Single = CSng(Math.Sqrt(sVal) * sScale)

        While sScaleBoxSize > CSng(Math.Max(rc.Width, rc.Height) / 4)
            sVal /= 10.0!
            sScaleBoxSize /= 10.0!
        End While

        g.DrawRectangle(Pens.Black, rc.Left, rc.Top, sScaleBoxSize, sScaleBoxSize)
        g.DrawString(String.Format("{0} {1}", sVal, Me.m_strUnit), SystemFonts.DialogFont, Brushes.Black, rc.Left, rc.Top + sScaleBoxSize)

    End Sub

#End Region ' Plotting

#Region " Dev time play variables to see what looks great "

    Public Property MidCutScaleFactor() As Single
        Get
            Return Me.m_sMidCutScaleFactor
        End Get
        Set(ByVal value As Single)
            Me.m_sMidCutScaleFactor = value
            Me.CalculatePyramid()
        End Set
    End Property

    Public Property MidCutLogCeiling() As Single
        Get
            Return Me.m_sMidCutLogCeiling
        End Get
        Set(ByVal value As Single)
            Me.m_sMidCutLogCeiling = value
            Me.CalculatePyramid()
        End Set
    End Property

    Public Property HighCutOff() As Single
        Get
            Return Me.m_sHighCutoff
        End Get
        Set(ByVal value As Single)
            Me.m_sHighCutoff = value
            Me.CalculatePyramid()
        End Set
    End Property

    Public Property HighCutOffAngle() As Single
        Get
            Return Me.m_sHighCutoffAngle
        End Get
        Set(ByVal value As Single)
            Me.m_sHighCutoffAngle = value
            Me.CalculatePyramid()
        End Set
    End Property

    Public Property LowCutOff() As Single
        Get
            Return Me.m_sLowCutoff
        End Get
        Set(ByVal value As Single)
            Me.m_sLowCutoff = value
            Me.CalculatePyramid()
        End Set
    End Property

    Public Property LowCutoffAngle() As Single
        Get
            Return Me.m_sLowCutoffAngle
        End Get
        Set(ByVal value As Single)
            Me.m_sLowCutoffAngle = value
            Me.CalculatePyramid()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the sum of biomasses 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SumB() As Single
        Get
            Dim sSumB As Single = 0.0
            For iTL As Integer = 0 To Me.NumTL
                sSumB += Me.m_asTRB(iTL)
            Next iTL
            Return sSumB
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Internal calculated value, serving as the base to determine the top 
    ''' angle of the pyramid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property TEpowNumTE() As Single
        Get
            Return Me.m_sTEpowNumTL
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the top angle of pyramid for the current number of trophic levels.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property TopAngle() As Single
        Get
            Dim dAverageTE As Double = 0.0
            Dim dY As Double = 0.0
            Dim iNumTE As Integer = 1
            Dim sTopAngle As Single = 90.0!

            ' JS: Revised the number of trophic levels calculation. Previously, only TLs 1-4 were used. 
            '     Changed this logic to loop up to configurable value NumTL
            dAverageTE = 1.0
            iNumTE = 1
            While (Me.m_asValue(iNumTE) > 0 And iNumTE <= Me.NumTL)
                dAverageTE *= Me.m_asValue(iNumTE)
                iNumTE += 1
            End While
            dY = CDbl(1 / iNumTE)

            'if(pDoc->TrEf[3] != 0) 		//tro_levels = 3; level 4
            '{
            '		y=(double)(.333);
            '		AveTrEf = pDoc->TrEf[1]*pDoc->TrEf[2]*pDoc->TrEf[3];
            '}
            'else if(pDoc->TrEf[2] != 0) 	//tro_levels = 2; level 3
            '{
            '		y=(double)(.5);
            '		AveTrEf = pDoc->TrEf[1]*pDoc->TrEf[2];
            '}
            'else 						//tro_levels = 1; level 2
            '{
            '		y=(double)(1.0);
            '		AveTrEf = pDoc->TrEf[1];
            '}
            Me.m_sTEpowNumTL = CSng(Math.Pow(dAverageTE, dY))

            ' VC used a series of hard constants to 'make it look good'
            If (Me.m_sTEpowNumTL > Me.m_sHighCutoff) Then
                sTopAngle = Me.m_sHighCutoffAngle
            ElseIf (Me.m_sTEpowNumTL > Me.m_sLowCutoff) Then
                sTopAngle = CSng(Me.m_sMidCutScaleFactor! * Math.Log10(Me.m_sMidCutLogCeiling / Me.m_sTEpowNumTL))
            Else
                sTopAngle = Me.m_sLowCutoffAngle
            End If

            Return sTopAngle
        End Get
    End Property

#End Region ' Dev time play variables to see what looks great

#Region " Internals "

    Private Sub CalculatePyramid()

        Me.m_bValid = (Me.NumTL > 0)

        If Not Me.IsValid Then Return

        ' Use surface calcs to position levels
        '     .
        '    /|\ α = half top angle
        '   / | \
        '  /  |  \  h = height
        ' /___|___\
        '       w = half base width
        '
        ' tan(α) = w/h : w = h*tan(α)
        ' where α = half top angle
        '       h = pyramid height
        '       w = pyramid half base width
        '
        ' Half area of pyramid R = Σ{num}/2 = SumB/2
        ' Half area R = w*h/2 = h^2*tan(α)/2 => h = √(2*R/tan(α)) = √(SumB/tan(α))

        Dim sAngleDeg As Single = Me.TopAngle
        Dim sHalfAngleRad As Single = CSng(sAngleDeg * Math.PI / 360.0!)
        Dim sTanHalfAngleRad As Single = CSng(Math.Tan(sHalfAngleRad))
        Dim sSumTL As Single = 0

        Me.m_sHeight = CSng(Math.Sqrt(Me.SumB / sTanHalfAngleRad))
        Me.m_sWidth = CSng(Me.m_sHeight * sTanHalfAngleRad) * 2
        ReDim Me.m_asLevels(Me.NumTL)

        '' Diagnostics
        'Console.WriteLine("Pyramid surface {0}, height {1}, width {2} (top angle {3})", SumB, Me.m_sHeight, Me.m_sWidth, sAngleDeg)

        ' Level positions are calculated via the total value of each TL
        For iTL As Integer = Me.NumTL - 1 To 0 Step -1
            sSumTL += Me.m_asTRB(iTL)
            ' TL floor calculated relative to pyramid height
            Me.m_asLevels(iTL) = CSng(Math.Sqrt(sSumTL / sTanHalfAngleRad) / Me.m_sHeight)
            '' Diagnostics
            'Console.WriteLine("  TL {0} at {1}", iTL, Me.m_asLevels(iTL))
        Next

    End Sub

#End Region ' Internals

End Class

