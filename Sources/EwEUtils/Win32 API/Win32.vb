#Region " Imports "

Option Strict On
Imports System
Imports System.text
Imports System.Runtime.InteropServices

#End Region ' Imports

Namespace Win32Api

    <CLSCompliant(False)> _
    Public Class Win32

#Region " Enums "

        ' Button control messages
        Enum BM As Integer
            BM_CLICK = &HF5
            BM_GETCHECK = &HF0
            BM_GETIMAGE = &HF6
            BM_GETSTATE = &HF2
            BM_SETCHECK = &HF1
            BM_SETIMAGE = &HF7
            BM_SETSTATE = &HF3
            BM_SETSTYLE = &HF4
        End Enum

        Enum BST As Integer
            BST_UNCHECKED = &H0
            BST_CHECKED = &H1
            BST_INDETERMINATE = &H2
            BST_PUSHED = &H4
            BST_FOCUS = &H8
        End Enum

        ' Combo box messages
        Enum CB As Integer
            CB_ADDSTRING = &H143
            CB_DELETESTRING = &H144
            CB_DIR = &H145
            CB_FINDSTRING = &H14C
            CB_FINDSTRINGEXACT = &H158
            CB_GETCOUNT = &H146
            CB_GETCURSEL = &H147
            CB_GETDROPPEDCONTROLRECT = &H152
            CB_GETDROPPEDSTATE = &H157
            CB_GETDROPPEDWIDTH = &H15F
            CB_GETEDITSEL = &H140
            CB_GETEXTENDEDUI = &H156
            CB_GETHORIZONTALEXTENT = &H15D
            CB_GETITEMDATA = &H150
            CB_GETITEMHEIGHT = &H154
            CB_GETLBTEXT = &H148
            CB_GETLBTEXTLEN = &H149
            CB_GETLOCALE = &H15A
            CB_GETTOPINDEX = &H15B
            CB_INITSTORAGE = &H161
            CB_INSERTSTRING = &H14A
            CB_LIMITTEXT = &H141
            CB_MSGMAX = &H15B  ' &h0162, &h0163
            CB_MULTIPLEADDSTRING = &H163
            CB_RESETCONTENT = &H14B
            CB_SELECTSTRING = &H14D
            CB_SETCURSEL = &H14E
            CB_SETDROPPEDWIDTH = &H160
            CB_SETEDITSEL = &H142
            CB_SETEXTENDEDUI = &H155
            CB_SETHORIZONTALEXTENT = &H15E
            CB_SETITEMDATA = &H151
            CB_SETITEMHEIGHT = &H153
            CB_SETLOCALE = &H159
            CB_SETTOPINDEX = &H15C
            CB_SHOWDROPDOWN = &H14F

            CB_ERR = -1
        End Enum

        Enum EM As Integer
            ' Edit box messages
            EM_CANUNDO = &HC6
            EM_CHARFROMPOS = &HD7
            EM_EMPTYUNDOBUFFER = &HCD
            EM_FMTLINES = &HC8
            EM_GETFIRSTVISIBLELINE = &HCE
            EM_GETHANDLE = &HBD
            EM_GETIMESTATUS = &HD9
            EM_GETLIMITTEXT = &HD5
            EM_GETLINE = &HC4
            EM_GETLINECOUNT = &HBA
            EM_GETMARGINS = &HD4
            EM_GETMODIFY = &HB8
            EM_GETPASSWORDCHAR = &HD2
            EM_GETRECT = &HB2
            EM_GETSEL = &HB0
            EM_GETTHUMB = &HBE
            EM_GETWORDBREAKPROC = &HD1
            EM_LIMITTEXT = &HC5
            EM_LINEFROMCHAR = &HC9
            EM_LINEINDEX = &HBB
            EM_LINELENGTH = &HC1
            EM_LINESCROLL = &HB6
            EM_POSFROMCHAR = &HD6
            EM_REPLACESEL = &HC2
            EM_SCROLL = &HB5
            EM_SCROLLCARET = &HB7
            EM_SETHANDLE = &HBC
            EM_SETIMESTATUS = &HD8
            EM_SETLIMITTEXT = &HC5  ' Same as EM_LIMITTEXT
            EM_SETMARGINS = &HD3
            EM_SETMODIFY = &HB9
            EM_SETPASSWORDCHAR = &HCC
            EM_SETREADONLY = &HCF
            EM_SETRECT = &HB3
            EM_SETRECTNP = &HB4
            EM_SETSEL = &HB1
            EM_SETTABSTOPS = &HCB
            EM_SETWORDBREAKPROC = &HD0
            EM_UNDO = &HC7
        End Enum

        ' Listbox messages
        Enum LB As Integer
            LB_ADDFILE = &H196
            LB_ADDSTRING = &H180
            LB_DELETESTRING = &H182
            LB_DIR = &H18D
            LB_FINDSTRING = &H18F
            LB_FINDSTRINGEXACT = &H1A2
            LB_GETANCHORINDEX = &H19D
            LB_GETCARETINDEX = &H19F
            LB_GETCOUNT = &H18B
            LB_GETCURSEL = &H188
            LB_GETHORIZONTALEXTENT = &H193
            LB_GETITEMDATA = &H199
            LB_GETITEMHEIGHT = &H1A1
            LB_GETITEMRECT = &H198
            LB_GETLOCALE = &H1A6
            LB_GETSEL = &H187
            LB_GETSELCOUNT = &H190
            LB_GETSELITEMS = &H191
            LB_GETTEXT = &H189
            LB_GETTEXTLEN = &H18A
            LB_GETTOPINDEX = &H18E
            LB_INITSTORAGE = &H1A8
            LB_INSERTSTRING = &H181
            LB_ITEMFROMPOINT = &H1A9
            LB_MSGMAX = &H1A8
            LB_MULTIPLEADDSTRING = &H1B1
            LB_RESETCONTENT = &H184
            LB_SELECTSTRING = &H18C
            LB_SELITEMRANGE = &H19B
            LB_SELITEMRANGEEX = &H183
            LB_SETANCHORINDEX = &H19C
            LB_SETCARETINDEX = &H19E
            LB_SETCOLUMNWIDTH = &H195
            LB_SETCOUNT = &H1A7
            LB_SETCURSEL = &H186
            LB_SETHORIZONTALEXTENT = &H194
            LB_SETITEMDATA = &H19A
            LB_SETITEMHEIGHT = &H1A0
            LB_SETLOCALE = &H1A5
            LB_SETSEL = &H185
            LB_SETTABSTOPS = &H192
            LB_SETTOPINDEX = &H197
        End Enum

        ' Windows messages
        Enum WM As Integer
            WM_ACTIVATE = &H6
            WM_ACTIVATEAPP = &H1C
            WM_AFXFIRST = &H360
            WM_AFXLAST = &H37F
            WM_APP = &H8000
            WM_APPCOMMAND = &H319
            WM_ASKCBFORMATNAME = &H30C
            WM_CANCELJOURNAL = &H4B
            WM_CANCELMODE = &H1F
            WM_CAPTURECHANGED = &H215
            WM_CHANGECBCHAIN = &H30D
            WM_CHANGEUISTATE = &H127
            WM_CHAR = &H102
            WM_CHARTOITEM = &H2F
            WM_CHILDACTIVATE = &H22
            WM_CLEAR = &H303
            WM_CLOSE = &H10
            WM_COMMAND = &H111
            WM_COMMNOTIFY = &H44  ' no longer suported
            WM_COMPACTING = &H41
            WM_COMPAREITEM = &H39
            WM_CONTEXTMENU = &H7B
            WM_CONVERTREQUESTEX = &H108
            WM_COPY = &H301
            WM_COPYDATA = &H4A
            WM_CREATE = &H1
            WM_CTLCOLOR = &H19
            WM_CTLCOLORBTN = &H135
            WM_CTLCOLORDLG = &H136
            WM_CTLCOLOREDIT = &H133
            WM_CTLCOLORLISTBOX = &H134
            WM_CTLCOLORMSGBOX = &H132
            WM_CTLCOLORSCROLLBAR = &H137
            WM_CTLCOLORSTATIC = &H138
            WM_CUT = &H300
            WM_DDE_FIRST = &H3E0
            WM_DEADCHAR = &H103
            WM_DELETEITEM = &H2D
            WM_DESTROY = &H2
            WM_DESTROYCLIPBOARD = &H307
            WM_DEVICECHANGE = &H219
            WM_DEVMODECHANGE = &H1B
            WM_DISPLAYCHANGE = &H7E
            WM_DRAWCLIPBOARD = &H308
            WM_DRAWITEM = &H2B
            WM_DROPFILES = &H233
            WM_ENABLE = &HA
            WM_ENDSESSION = &H16
            WM_ENTERIDLE = &H121
            WM_ENTERMENULOOP = &H211
            WM_ENTERSIZEMOVE = &H231
            WM_ERASEBKGND = &H14
            WM_EXITMENULOOP = &H212
            WM_EXITSIZEMOVE = &H232
            WM_FONTCHANGE = &H1D
            WM_GETDLGCODE = &H87
            WM_GETFONT = &H31
            WM_GETHOTKEY = &H33
            WM_GETICON = &H7F
            WM_GETMINMAXINFO = &H24
            WM_GETOBJECT = &H3D
            WM_GETTEXT = &HD
            WM_GETTEXTLENGTH = &HE
            WM_HANDHELDFIRST = &H358
            WM_HANDHELDLAST = &H35F
            WM_HELP = &H53
            WM_HOTKEY = &H312
            WM_HSCROLL = &H114
            WM_HSCROLLCLIPBOARD = &H30E
            WM_ICONERASEBKGND = &H27
            WM_IME_CHAR = &H286
            WM_IME_COMPOSITION = &H10F
            WM_IME_COMPOSITIONFULL = &H284
            WM_IME_CONTROL = &H283
            WM_IME_ENDCOMPOSITION = &H10E
            WM_IME_KEYDOWN = &H290
            WM_IME_KEYLAST = &H10F
            WM_IME_KEYUP = &H291
            WM_IME_NOTIFY = &H282
            WM_IME_REQUEST = &H288
            WM_IME_SELECT = &H285
            WM_IME_SETCONTEXT = &H281
            WM_IME_STARTCOMPOSITION = &H10D
            WM_INITDIALOG = &H110
            WM_INITMENU = &H116
            WM_INITMENUPOPUP = &H117
            WM_INPUT = &HFF
            WM_INPUTLANGCHANGE = &H51
            WM_INPUTLANGCHANGEREQUEST = &H50
            WM_KEYDOWN = &H100
            WM_KEYFIRST = &H100
            WM_KEYLAST = &H108
            WM_KEYUP = &H101
            WM_KILLFOCUS = &H8
            WM_LBUTTONDBLCLK = &H203
            WM_LBUTTONDOWN = &H201
            WM_LBUTTONUP = &H202
            WM_MBUTTONDBLCLK = &H209
            WM_MBUTTONDOWN = &H207
            WM_MBUTTONUP = &H208
            WM_MDIACTIVATE = &H222
            WM_MDICASCADE = &H227
            WM_MDICREATE = &H220
            WM_MDIDESTROY = &H221
            WM_MDIGETACTIVE = &H229
            WM_MDIICONARRANGE = &H228
            WM_MDIMAXIMIZE = &H225
            WM_MDINEXT = &H224
            WM_MDIREFRESHMENU = &H234
            WM_MDIRESTORE = &H223
            WM_MDISETMENU = &H230
            WM_MDITILE = &H226
            WM_MEASUREITEM = &H2C
            WM_MENUCHAR = &H120
            WM_MENUCOMMAND = &H126
            WM_MENUDRAG = &H123
            WM_MENUGETOBJECT = &H124
            WM_MENURBUTTONUP = &H122
            WM_MENUSELECT = &H11F
            WM_MOUSEACTIVATE = &H21
            WM_MOUSEFIRST = &H200
            WM_MOUSEHOVER = &H2A1
            WM_MOUSELAST = &H209  ' &h020A, &h020D
            WM_MOUSELEAVE = &H2A3
            WM_MOUSEMOVE = &H200
            WM_MOUSEWHEEL = &H20A
            WM_MOVE = &H3
            WM_MOVING = &H216
            WM_NCACTIVATE = &H86
            WM_NCCALCSIZE = &H83
            WM_NCCREATE = &H81
            WM_NCDESTROY = &H82
            WM_NCHITTEST = &H84
            WM_NCLBUTTONDBLCLK = &HA3
            WM_NCLBUTTONDOWN = &HA1
            WM_NCLBUTTONUP = &HA2
            WM_NCMBUTTONDBLCLK = &HA9
            WM_NCMBUTTONDOWN = &HA7
            WM_NCMBUTTONUP = &HA8
            WM_NCMOUSEHOVER = &H2A0
            WM_NCMOUSELEAVE = &H2A2
            WM_NCMOUSEMOVE = &HA0
            WM_NCPAINT = &H85
            WM_NCRBUTTONDBLCLK = &HA6
            WM_NCRBUTTONDOWN = &HA4
            WM_NCRBUTTONUP = &HA5
            WM_NCXBUTTONDBLCLK = &HAD
            WM_NCXBUTTONDOWN = &HAB
            WM_NCXBUTTONUP = &HAC
            WM_NEXTDLGCTL = &H28
            WM_NEXTMENU = &H213
            WM_NOTIFY = &H4E
            WM_NOTIFYFORMAT = &H55
            WM_NULL = &H0
            WM_PAINT = &HF
            WM_PAINTCLIPBOARD = &H309
            WM_PAINTICON = &H26
            WM_PALETTECHANGED = &H311
            WM_PALETTEISCHANGING = &H310
            WM_PARENTNOTIFY = &H210
            WM_PASTE = &H302
            WM_PENWINFIRST = &H380
            WM_PENWINLAST = &H38F
            WM_POWER = &H48
            WM_POWERBROADCAST = &H218
            WM_PRINT = &H317
            WM_PRINTCLIENT = &H318
            WM_QUERYDRAGICON = &H37
            WM_QUERYENDSESSION = &H11
            WM_QUERYNEWPALETTE = &H30F
            WM_QUERYOPEN = &H13
            WM_QUERYUISTATE = &H129
            WM_QUEUESYNC = &H23
            WM_QUIT = &H12
            WM_RBUTTONDBLCLK = &H206
            WM_RBUTTONDOWN = &H204
            WM_RBUTTONUP = &H205
            WM_RASDIALEVENT = &HCCCD
            WM_RENDERALLFORMATS = &H306
            WM_RENDERFORMAT = &H305
            WM_SETCURSOR = &H20
            WM_SETFOCUS = &H7
            WM_SETFONT = &H30
            WM_SETHOTKEY = &H32
            WM_SETICON = &H80
            WM_SETREDRAW = &HB
            WM_SETTEXT = &HC
            WM_SETTINGCHANGE = &H1A  ' Same as WM_WININICHANGE
            WM_SHOWWINDOW = &H18
            WM_SIZE = &H5
            WM_SIZECLIPBOARD = &H30B
            WM_SIZING = &H214
            WM_SPOOLERSTATUS = &H2A
            WM_STYLECHANGED = &H7D
            WM_STYLECHANGING = &H7C
            WM_SYNCPAINT = &H88
            WM_SYSCHAR = &H106
            WM_SYSCOLORCHANGE = &H15
            WM_SYSCOMMAND = &H112
            WM_SYSDEADCHAR = &H107
            WM_SYSKEYDOWN = &H104
            WM_SYSKEYUP = &H105
            WM_TABLET_FIRST = &H2C0
            WM_TABLET_LAST = &H2DF
            WM_THEMECHANGED = &H31A
            WM_TCARD = &H52
            WM_TIMECHANGE = &H1E
            WM_TIMER = &H113
            WM_UNDO = &H304
            WM_UNICHAR = &H109
            WM_UNINITMENUPOPUP = &H125
            WM_UPDATEUISTATE = &H128
            WM_USER = &H400
            WM_USERCHANGED = &H54
            WM_VKEYTOITEM = &H2E
            WM_VSCROLL = &H115
            WM_VSCROLLCLIPBOARD = &H30A
            WM_WINDOWPOSCHANGED = &H47
            WM_WINDOWPOSCHANGING = &H46
            WM_WININICHANGE = &H1A
            WM_WTSSESSION_CHANGE = &H2B1
            WM_XBUTTONDBLCLK = &H20D
            WM_XBUTTONDOWN = &H20B
            WM_XBUTTONUP = &H20C
        End Enum

        Enum WH As Integer
            WH_CBT = &H5
        End Enum

        Enum HCBT As Integer
            HCBT_MOVESIZE = &H0
            HCBT_MINMAX = &H1
            HCBT_QS = &H2
            HCBT_CREATEWND = &H3
            HCBT_DESTROYWND = &H4
            HCBT_ACTIVATE = &H5
            HCBT_CLICKSKIPPED = &H6
            HCBT_KEYSKIPPED = &H7
            HCBT_SYSCOMMAND = &H8
            HCBT_SETFOCUS = &H9
        End Enum

        ' Application desktop
        Enum ABM As Integer
            ABM_ACTIVATE = &H6  ' lParam = TRUE/FALSE means activate/deactivate
            ABM_GETAUTOHIDEBAR = &H7
            ABM_GETSTATE = &H4
            ABM_GETTASKBARPOS = &H5
            ABM_NEW = &H0
            ABM_QUERYPOS = &H2
            ABM_REMOVE = &H1
            ABM_SETAUTOHIDEBAR = &H8  ' This can fail, you MUST check the result
            ABM_SETPOS = &H3
            ABM_WINDOWPOSCHANGED = &H9
        End Enum

        'Default push button control 
        Enum DM As Integer
            DM_BITSPERPEL = &H40000
            DM_COLLATE = &H8000
            DM_COLOR = &H800
            DM_COPIES = &H100
            DM_DEFAULTSOURCE = &H200
            DM_DISPLAYFLAGS = &H200000
            DM_DISPLAYFREQUENCY = &H400000
            DM_DITHERTYPE = &H4000000
            DM_DUPLEX = &H1000
            DM_FORMNAME = &H10000
            DM_GRAYSCALE = &H1  ' This flag is no longer valid
            DM_ICMINTENT = &H1000000
            DM_ICMMETHOD = &H800000
            DM_INTERLACED = &H2  ' This flag is no longer valid
            DM_LOGPIXELS = &H20000
            DM_MEDIATYPE = &H2000000
            DM_NUP = &H40
            DM_ORIENTATION = &H1
            DM_PANNINGHEIGHT = &H10000000
            DM_PANNINGWIDTH = &H8000000
            DM_PAPERLENGTH = &H4
            DM_PAPERSIZE = &H2
            DM_PAPERWIDTH = &H8
            DM_PELSHEIGHT = &H100000
            DM_PELSWIDTH = &H80000
            DM_POSITION = &H20
            DM_PRINTQUALITY = &H400
            DM_SCALE = &H10
            DM_SPECVERSION = &H320       ' &h0400 &h0401
            DM_TTOPTION = &H4000
            DM_YRESOLUTION = &H2000
        End Enum

        ' Header control
        Enum HDM As Integer
            HDM_FIRST = &H1200
        End Enum

        ' List view control
        Enum LVM As Integer
            LVM_FIRST = &H1000
            LVN_ITEMCHANGED = -101
            LVM_GETSUBITEMRECT = LVM_FIRST + 56
            LVM_FINDITEM = LVM_FIRST + 83
        End Enum

        Enum LVIR As Integer
            LVIR_BOUNDS = 0
            LVIR_ICON = 1
            LVIR_LABEL = 2
            LVIR_SELECTBOUNDS = 3
        End Enum

        Enum LVFI As Integer
            LVFI_PARAM = 1
            LVFI_STRING = 2
            LVFI_PARTIAL = 8
            LVFI_WRAP = 32
            LVFI_NEARESTXY = 64
        End Enum

        ' Status bar window
        Enum SB As Integer
            SB_CONST_ALPHA = &H1
            SB_GRAD_RECT = &H10
            SB_GRAD_TRI = &H20
            SB_NONE = &H0
            SB_PIXEL_ALPHA = &H2
            SB_PREMULT_ALPHA = &H4
            SB_SIMPLEID = &HFF
        End Enum

        ' Scroll bar control
        Enum SBM As Integer
            SBM_ENABLE_ARROWS = &HE4  ' Not in win3.1
            SBM_GETPOS = &HE1  ' Not in win3.1
            SBM_GETRANGE = &HE3  ' Not in win3.1
            SBM_GETSCROLLINFO = &HEA
            SBM_SETPOS = &HE0  ' Not in win3.1
            SBM_SETRANGE = &HE2  ' Not in win3.1
            SBM_SETRANGEREDRAW = &HE6  ' Not in win3.1
            SBM_SETSCROLLINFO = &HE9
        End Enum

        ' Static control
        Enum STM As Integer
            STM_GETICON = &H171
            STM_GETIMAGE = &H173
            STM_MSGMAX = &H174
            STM_ONLY_THIS_INTERFACE = &H1
            STM_ONLY_THIS_NAME = &H8
            STM_ONLY_THIS_PROTOCOL = &H2
            STM_ONLY_THIS_TYPE = &H4
            STM_SETICON = &H170
            STM_SETIMAGE = &H172
        End Enum

        ' Tab control
        Enum TCM As Integer
            TCM_FIRST = &H1300
        End Enum

        ' Window styles
        Enum WS As Integer
            WS_OVERLAPPED = &H0L
            WS_POPUP = &H80000000
            WS_CHILD = &H40000000
            WS_MINIMIZE = &H20000000
            WS_VISIBLE = &H10000000
            WS_DISABLED = &H8000000
            WS_CLIPSIBLINGS = &H4000000
            WS_CLIPCHILDREN = &H2000000
            WS_MAXIMIZE = &H1000000
            WS_CAPTION = &HC00000     ' WS_BORDER or WS_DLGFRAME 
            WS_BORDER = &H800000
            WS_DLGFRAME = &H400000
            WS_VSCROLL = &H200000
            WS_HSCROLL = &H100000
            WS_SYSMENU = &H80000
            WS_THICKFRAME = &H40000
            WS_GROUP = &H20000
            WS_TABSTOP = &H10000

            WS_MINIMIZEBOX = &H20000
            WS_MAXIMIZEBOX = &H10000

            WS_TILED = WS_OVERLAPPED
            WS_ICONIC = WS_MINIMIZE
            WS_SIZEBOX = WS_THICKFRAME
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW
            WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or WS_THICKFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX)
            WS_POPUPWINDOW = (WS_POPUP Or WS_BORDER Or WS_SYSMENU)
            WS_CHILDWINDOW = (WS_CHILD)

            WS_EX_CLIENTEDGE = &H200L
        End Enum

        Enum SC As Integer
            SC_SIZE = &HF000
            SC_MOVE = &HF010
            SC_MINIMIZE = &HF020
            SC_MAXIMIZE = &HF030
            SC_NEXTWINDOW = &HF040
            SC_PREVWINDOW = &HF050
            SC_CLOSE = &HF060
            SC_VSCROLL = &HF070
            SC_HSCROLL = &HF080
            SC_MOUSEMENU = &HF090
            SC_KEYMENU = &HF100
            SC_ARRANGE = &HF110
            SC_RESTORE = &HF120
            SC_TASKLIST = &HF130
            SC_SCREENSAVE = &HF140
            SC_HOTKEY = &HF150
        End Enum

        Enum DS As Integer
            DS_ABSALIGN = &H1L
            DS_SYSMODAL = &H2L
            DS_LOCALEDIT = &H20L
            DS_SETFONT = &H40L
            DS_MODALFRAME = &H80L
            DS_NOIDLEMSG = &H100L
        End Enum

        Enum SS As Integer
            SS_LEFT = &H0L
            SS_CENTER = &H1L
            SS_RIGHT = &H2L
            SS_ICON = &H3L
            SS_BLACKRECT = &H4L
            SS_GRAYRECT = &H5L
            SS_WHITERECT = &H6L
            SS_BLACKFRAME = &H7L
            SS_GRAYFRAME = &H8L
            SS_WHITEFRAME = &H9L
            SS_SIMPLE = &HBL
            SS_LEFTNOWORDWRAP = &HCL
            SS_NOPREFIX = &H80L
        End Enum

        Enum BS As Integer
            BS_PUSHBUTTON = &H0L
            BS_DEFPUSHBUTTON = &H1L
            BS_CHECKBOX = &H2L
            BS_AUTOCHECKBOX = &H3L
            BS_RADIOBUTTON = &H4L
            BS_3STATE = &H5L
            BS_AUTO3STATE = &H6L
            BS_GROUPBOX = &H7L
            BS_USERBUTTON = &H8L
            BS_AUTORADIOBUTTON = &H9L
            BS_OWNERDRAW = &HBL
            BS_LEFTTEXT = &H20L
        End Enum

        Enum ES As Integer
            ES_LEFT = &H0L
            ES_CENTER = &H1L
            ES_RIGHT = &H2L
            ES_MULTILINE = &H4L
            ES_UPPERCASE = &H8L
            ES_LOWERCASE = &H10L
            ES_PASSWORD = &H20L
            ES_AUTOVSCROLL = &H40L
            ES_AUTOHSCROLL = &H80L
            ES_NOHIDESEL = &H100L
            ES_OEMCONVERT = &H400L
            ES_READONLY = &H800L
            ES_WANTRETURN = &H1000L
        End Enum

        Enum SBS As Integer
            SBS_HORZ = &H0L
            SBS_VERT = &H1L
            SBS_TOPALIGN = &H2L
            SBS_LEFTALIGN = &H2L
            SBS_BOTTOMALIGN = &H4L
            SBS_RIGHTALIGN = &H4L
            SBS_SIZEBOXTOPLEFTALIGN = &H2L
            SBS_SIZEBOXBOTTOMRIGHTALIGN = &H4L
            SBS_SIZEBOX = &H8L
        End Enum

        Enum LBS As Integer
            LBS_NOTIFY = &H1L
            LBS_SORT = &H2L
            LBS_NOREDRAW = &H4L
            LBS_MULTIPLESEL = &H8L
            LBS_OWNERDRAWFIXED = &H10L
            LBS_OWNERDRAWVARIABLE = &H20L
            LBS_HASSTRINGS = &H40L
            LBS_USETABSTOPS = &H80L
            LBS_NOINTEGRALHEIGHT = &H100L
            LBS_MULTICOLUMN = &H200L
            LBS_WANTKEYBOARDINPUT = &H400L
            LBS_EXTENDEDSEL = &H800L
            LBS_DISABLENOSCROLL = &H1000L
            LBS_STANDARD = (LBS_NOTIFY Or LBS_SORT Or WS.WS_VSCROLL Or WS.WS_BORDER)
        End Enum

        Enum CBS As Integer
            CBS_SIMPLE = &H1L
            CBS_DROPDOWN = &H2L
            CBS_DROPDOWNLIST = &H3L
            CBS_OWNERDRAWFIXED = &H10L
            CBS_OWNERDRAWVARIABLE = &H20L
            CBS_AUTOHSCROLL = &H40L
            CBS_OEMCONVERT = &H80L
            CBS_SORT = &H100L
            CBS_HASSTRINGS = &H200L
            CBS_NOINTEGRALHEIGHT = &H400L
            CBS_DISABLENOSCROLL = &H800L
        End Enum

        Enum ID As Integer
            IDOK = 1
            IDCANCEL = 2
            IDABORT = 3
            IDRETRY = 4
            IDIGNORE = 5
            IDYES = 6
            IDNO = 7
        End Enum

        Enum SWP As Integer
            SWP_NOSIZE = &H1
            SWP_NOMOVE = &H2
            SWP_NOZORDER = &H4
            SWP_NOREDRAW = &H8
            SWP_NOACTIVATE = &H10
            SWP_FRAMECHANGED = &H20
            SWP_SHOWWINDOW = &H40
            SWP_HIDEWINDOW = &H80
            SWP_NOCOPYBITS = &H100
            SWP_NOOWNERZORDER = &H200
            SWP_DRAWFRAME = SWP_FRAMECHANGED
            SWP_NOREPOSITION = SWP_NOOWNERZORDER
            SWP_NOSENDCHANGING = &H400
            SWP_DEFERERASE = &H2000
        End Enum

        ' Hmmmm
        Enum OCM As Integer
            OCM_BASE = &H2000
            OCM_NOTIFY = OCM_BASE + WM.WM_NOTIFY
        End Enum

        Enum NM As Integer
            NM_CUSTOMDRAW = -12
            NM_SETFOCUS = -7
        End Enum

        ' custom draw return flags
        Enum CDRF As Integer
            CDRF_DODEFAULT = &H0
            CDRF_SKIPDEFAULT = &H4
            CDRF_NOTIFYITEMDRAW = &H20
        End Enum

        ' custom draw state flags
        Enum CDDS As Integer
            CDDS_PREPAINT = &H1
            CDDS_ITEM = &H10000
            CDDS_ITEMPREPAINT = CDDS_ITEM Or CDDS_PREPAINT
        End Enum

#End Region ' Enums

#Region " Structures "

        <StructLayout(LayoutKind.Sequential)> _
        Public Structure RECT
            Public Left As Integer
            Public Top As Integer
            Public Right As Integer
            Public Bottom As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)> _
        Public Structure POINT
            Public X As Integer
            Public Y As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)> _
        Public Structure NMHDR
            Public hwndFrom As IntPtr
            Public idFrom As Integer
            Public code As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)> _
        Public Structure NMCUSTOMDRAW
            Public hdr As NMHDR
            Public dwDrawStage As Integer
            Public hdc As IntPtr
            Public rc As RECT
            Public dwItemSpec As Integer
            Public uItemState As Integer
            Public lItemlParam As IntPtr
        End Structure

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
        Public Structure LVFINDINFO
            Dim flags As System.UInt32
            <MarshalAs(UnmanagedType.LPTStr)> _
            Dim psz As String
            Dim lParam As IntPtr
            Dim pt As POINT
            Dim vkDirection As System.UInt32
        End Structure

        <StructLayout(LayoutKind.Sequential)> _
        Public Class MSLLHOOKSTRUCT
            Public pt As POINT
            Public mouseData As Integer
            Public flags As Integer
            Public time As Integer
            Public dwExtraInfo As Integer
        End Class

        <StructLayout(LayoutKind.Sequential)> _
        Public Class KeyboardHookStruct
            Public vkCode As Integer  'Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            Public scanCode As Integer ' Specifies a hardware scan code for the key. 
            Public flags As Integer ' Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            Public time As Integer ' Specifies the time stamp for this message.
            Public dwExtraInfo As Integer ' Specifies extra information associated with the message. 
        End Class

#End Region ' Structures

#Region " Delegates "

        Public Delegate Function HookProc( _
            ByVal nCode As Integer, _
            ByVal wParam As IntPtr, _
            ByVal lParam As IntPtr) As Integer

#End Region ' Delegates

    End Class

End Namespace
