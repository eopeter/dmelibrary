using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace DME.Wimdows.Win32API
{
    #region Win32 API 结构体声明

    /// <summary>
    /// Win32 API 结构体声明
    /// </summary>
    #region SIZE
    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;
    }
    #endregion

    #region RECT
    /// <summary>
    /// 定义了一个矩形框左上角以及右下角的坐标
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// 指定矩形框左上角的x坐标
        /// </summary>
        public int left;
        /// <summary>
        /// 指定矩形框左上角的y坐标
        /// </summary>
        public int top;
        /// <summary>
        /// 指定矩形框右下角的x坐标
        /// </summary>
        public int right;
        /// <summary>
        /// 指定矩形框右下角的y坐标
        /// </summary>
        public int bottom;
    }
    #endregion

    #region INITCOMMONCONTROLSEX
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class INITCOMMONCONTROLSEX
    {
        public int dwSize;
        public int dwICC;
    }
    #endregion

    #region TBBUTTON
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TBBUTTON
    {
        /// <summary>
        /// iBitmap 按钮图像的从零开始的索引。如果这个按钮没有图像，则这个成员是NULL。 
        /// </summary>
        public int iBitmap;
        /// <summary>
        /// idCommand 与此按钮相关联的命令标识符。当按钮被选择时，这个标识符在一个WM_COMMAND消息中被发送。如果fsStyle成员的值为TBSTYLE_SEP，则这个成员必须是零。 
        /// </summary>
        public int idCommand;
        /// <summary>
        /// fsState 按钮的状态标志。它可以是下面列出的值的一个组合： 
        ///· TBSTATE_CHECKED 该按钮具有TBSTYLE_CHECKED风格并且被按下。  
        ///· TBSTATE_ENABLED 按钮接收用户输入。一个不具有这个状态的按钮是不接收用户输入的，并且变灰。  
        ///· TBSTATE_HIDDEN 按钮不可见，并且不能接收用户输入。  
        ///· TBSTATE_INDETERMINATE 按钮是变灰的。  
        ///· TBSTATE_PRESSED 按钮被按下。  
        ///· TBSTATE_WRAP 按钮之后是一个分隔线。此按钮还必须具有TBSTATE_ENABLED状态。 
        /// </summary>
        public byte fsState;
        /// <summary>
        /// fsStyle 按钮风格。它可以是下列值的一个组合： 
        /// · TBSTYLE_BUTTON 创建一个标准的按钮。  
        /// · TBSTYLE_CHECK  创建一个每次用户点击时可以在按下和弹起状态间切换的按钮。该按钮则处于按下状态时有一种不同的背景颜色。
        /// · TBSTYLE_CHECKGROUP 创建一个核选按钮，它被选择后一直处于按下状态，直到同组中的另一个按钮被按下时它才弹起。 
        /// · TBSTYLE_GROUP 创建一个被选择后一直处于按下状态，直到同组中的另一个按钮被按下时它才弹起的按钮。
        /// · TBSTYLE_SEP 创建一个分隔线，为按钮组之间提供一个小的间距。具有这个风格的按钮是不接收用户输入的。 
        /// </summary>
        public byte fsStyle;
        public byte bReserved0;
        public byte bReserved1;
        /// <summary>
        /// 用户定义的数据。
        /// </summary>
        public int dwData;
        /// <summary>
        /// iString 要用来作为按钮的标签的字符串的从零开始的索引。如果这个按钮没有字符串则这个值为NULL  
        /// </summary>
        public int iString;
    }
    #endregion

    #region POINT
    /// <summary>
    /// 定义了x和y的坐标。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }
    #endregion

    #region NMHDR
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        /// <summary>
        /// hwndFrom是发送通知消息的窗口句柄。
        /// </summary>
        public IntPtr hwndFrom;
        /// <summary>
        /// idFrom就是控件ID。
        /// </summary>
        public int idFrom;
        /// <summary>
        /// code域包含的是通知码
        /// </summary>
        public int code;
    }
    #endregion

    #region TOOLTIPTEXTA
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TOOLTIPTEXTA
    {
        public NMHDR hdr;
        public IntPtr lpszText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szText;
        public IntPtr hinst;
        public int uFlags;
    }
    #endregion

    #region TOOLTIPTEXT
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TOOLTIPTEXT
    {
        public NMHDR hdr;
        public IntPtr lpszText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szText;
        public IntPtr hinst;
        public int uFlags;
    }
    #endregion

    #region NMCUSTOMDRAW
    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        public NMHDR hdr;
        public int dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public int dwItemSpec;
        public int uItemState;
        public int lItemlParam;
    }
    #endregion

    #region NMTBCUSTOMDRAW
    [StructLayout(LayoutKind.Sequential)]
    public struct NMTBCUSTOMDRAW
    {
        public NMCUSTOMDRAW nmcd;
        public IntPtr hbrMonoDither;
        public IntPtr hbrLines;
        public IntPtr hpenLines;
        public int clrText;
        public int clrMark;
        public int clrTextHighlight;
        public int clrBtnFace;
        public int clrBtnHighlight;
        public int clrHighlightHotTrack;
        public RECT rcText;
        public int nStringBkMode;
        public int nHLStringBkMode;
    }
    #endregion

    #region NMLVCUSTOMDRAW
    [StructLayout(LayoutKind.Sequential)]
    public struct NMLVCUSTOMDRAW
    {
        public NMCUSTOMDRAW nmcd;
        public uint clrText;
        public uint clrTextBk;
        public int iSubItem;
    }
    #endregion

    #region TBBUTTONINFO
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TBBUTTONINFO
    {
        public int cbSize;
        public int dwMask;
        public int idCommand;
        public int iImage;
        public byte fsState;
        public byte fsStyle;
        public short cx;
        public IntPtr lParam;
        public IntPtr pszText;
        public int cchText;
    }
    #endregion

    #region REBARBANDINFO
    [StructLayout(LayoutKind.Sequential)]
    public struct REBARBANDINFO
    {
        public int cbSize;
        public int fMask;
        public int fStyle;
        public int clrFore;
        public int clrBack;
        public IntPtr lpText;
        public int cch;
        public int iImage;
        public IntPtr hwndChild;
        public int cxMinChild;
        public int cyMinChild;
        public int cx;
        public IntPtr hbmBack;
        public int wID;
        public int cyChild;
        public int cyMaxChild;
        public int cyIntegral;
        public int cxIdeal;
        public int lParam;
        public int cxHeader;
    }
    #endregion

    #region MOUSEHOOKSTRUCT
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEHOOKSTRUCT
    {
        public POINT pt;
        public IntPtr hwnd;
        public int wHitTestCode;
        public IntPtr dwExtraInfo;
    }
    #endregion

    #region NMTOOLBAR
    [StructLayout(LayoutKind.Sequential)]
    public struct NMTOOLBAR
    {
        public NMHDR hdr;
        public int iItem;
        public TBBUTTON tbButton;
        public int cchText;
        public IntPtr pszText;
        public RECT rcButton;
    }
    #endregion

    #region NMREBARCHEVRON
    [StructLayout(LayoutKind.Sequential)]
    public struct NMREBARCHEVRON
    {
        public NMHDR hdr;
        public int uBand;
        public int wID;
        public int lParam;
        public RECT rc;
        public int lParamNM;
    }
    #endregion

    #region BITMAP
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAP
    {
        public long bmType;
        public long bmWidth;
        public long bmHeight;
        public long bmWidthBytes;
        public short bmPlanes;
        public short bmBitsPixel;
        public IntPtr bmBits;
    }
    #endregion

    #region BITMAPINFO_FLAT
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO_FLAT
    {
        public int bmiHeader_biSize;
        public int bmiHeader_biWidth;
        public int bmiHeader_biHeight;
        public short bmiHeader_biPlanes;
        public short bmiHeader_biBitCount;
        public int bmiHeader_biCompression;
        public int bmiHeader_biSizeImage;
        public int bmiHeader_biXPelsPerMeter;
        public int bmiHeader_biYPelsPerMeter;
        public int bmiHeader_biClrUsed;
        public int bmiHeader_biClrImportant;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] bmiColors;
    }
    #endregion

    #region RGBQUAD
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }
    #endregion

    #region BITMAPINFOHEADER
    [StructLayout(LayoutKind.Sequential)]
    public class BITMAPINFOHEADER
    {
        public int biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }
    #endregion

    #region BITMAPINFO
    [StructLayout(LayoutKind.Sequential)]
    public class BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader = new BITMAPINFOHEADER();
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] bmiColors;
    }
    #endregion

    #region PALETTEENTRY
    [StructLayout(LayoutKind.Sequential)]
    public struct PALETTEENTRY
    {
        public byte peRed;
        public byte peGreen;
        public byte peBlue;
        public byte peFlags;
    }
    #endregion

    #region MSG
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }
    #endregion

    #region HD_HITTESTINFO
    [StructLayout(LayoutKind.Sequential)]
    public struct HD_HITTESTINFO
    {
        public POINT pt;
        public uint flags;
        public int iItem;
    }
    #endregion

    #region DLLVERSIONINFO
    [StructLayout(LayoutKind.Sequential)]
    public struct DLLVERSIONINFO
    {
        public int cbSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformID;
    }
    #endregion

    #region PAINTSTRUCT
    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public Rectangle rcPaint;
        public int fRestore;
        public int fIncUpdate;
        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int Reserved5;
        public int Reserved6;
        public int Reserved7;
        public int Reserved8;
    }
    #endregion

    #region BLENDFUNCTION
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }
    #endregion

    #region TRACKMOUSEEVENTS
    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENTS
    {
        public uint cbSize;
        public uint dwFlags;
        public IntPtr hWnd;
        public uint dwHoverTime;
    }
    #endregion

    #region STRINGBUFFER
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct STRINGBUFFER
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string szText;
    }
    #endregion

    #region NMTVCUSTOMDRAW
    [StructLayout(LayoutKind.Sequential)]
    public struct NMTVCUSTOMDRAW
    {
        public NMCUSTOMDRAW nmcd;
        public uint clrText;
        public uint clrTextBk;
        public int iLevel;
    }
    #endregion

    #region TVITEM
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TVITEM
    {
        public uint mask;
        public IntPtr hItem;
        public uint state;
        public uint stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public int iSelectedImage;
        public int cChildren;
        public int lParam;
    }
    #endregion

    #region LVITEM
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct LVITEM
    {
        public uint mask;
        public int iItem;
        public int iSubItem;
        public uint state;
        public uint stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public int lParam;
        public int iIndent;
    }
    #endregion

    #region HDITEM
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct HDITEM
    {
        public uint mask;
        public int cxy;
        public IntPtr pszText;
        public IntPtr hbm;
        public int cchTextMax;
        public int fmt;
        public int lParam;
        public int iImage;
        public int iOrder;
    }
    #endregion

    #region WINDOWPLACEMENT
    /// <summary>
    /// 包含了有关窗口在屏幕上位置的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WINDOWPLACEMENT
    {
        /// <summary>
        /// 指定了结构的长度，以字节为单位。
        /// </summary>
        public uint length;
        /// <summary>
        /// flags 指定了控制最小化窗口的位置的标志以及复原窗口的方法。这个成员可以是下面列出的标志之一，或都是：
        /// · WPF_SETMINPOSITION 表明可以指定最小化窗口的x和y坐标。如果是在ptMinPosition成员中设置坐标，则必须指定这个标志。
        /// · WPF_RESTORETOMAXIMIZED 表明复原后的窗口将会被最大化，而不管它在最小化之前是否是最大化的。这个设置仅在下一次复原窗口时有效。它不改变缺省的复原操作。这个标志仅当showCmd成员中指定了SW_SHOWMINIMIZED时才有效。
        /// </summary>
        public uint flags;
        /// <summary>
        /// showCmd 指定了窗口的当前显示状态。这个成员可以是下列值之一： 
        /// · SW_HIDE 隐藏窗口，使其它窗口变为激活的。
        /// · SW_MINIMIZE 最小化指定的窗口，并激活系统列表中的顶层窗口。
        /// · SW_RESTORE 激活并显示窗口。如果窗口是最小化或最大化的，Windows将把它恢复到原来的大小和位置（与SW_SHOWNORMAL相同）。 
        /// · SW_SHOW 激活窗口并按照当前的位置和大小显示窗口。
        /// · SW_SHOWMAXIMIZED 激活窗口并将其显示为最大化的。
        /// · SW_SHOWMINIMIZED 激活窗口并将其显示为图标。 
        /// · SW_SHOWMINNOACTIVE 将窗口显示为图标。当前激活的窗口仍保持激活状态。
        /// · SW_SHOWNA 按当前状态显示窗口。当前激活的窗口仍保持激活状态。
        /// · SW_SHOWNOACTIVATE 按最近的位置和大小显示窗口。当前激活的窗口仍保持激活状态。 
        /// · SW_SHOWNORMAL 激活并显示窗口。如果窗口是最小化或最大化的，Windows将它恢复到原来的大小和位置（与SW_RESTORE相同）。
        /// </summary>
        public uint showCmd;
        /// <summary>
        /// ptMinPosition 指定了窗口被最小化时左上角的位置。
        /// </summary>
        public POINT ptMinPosition;
        /// <summary>
        /// ptMaxPosition 指定了窗口被最大化时左上角的位置。
        /// </summary>
        public POINT ptMaxPosition;
        /// <summary>
        /// rcNormalPosition 指定了窗口处于正常状态（复原）时的坐标。
        /// </summary>
        public RECT rcNormalPosition;
    }
    #endregion

    #region SCROLLINFO
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SCROLLINFO
    {
        public uint cbSize;
        public uint fMask;
        public int nMin;
        public int nMax;
        public uint nPage;
        public int nPos;
        public int nTrackPos;
    }
    #endregion

    #region MouseHookStruct
    /// <summary>
    /// 当WH_MOUSE钩子处理的鼠标事件时，该结构包含鼠标信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        /// <summary>
        /// POINT结构对象，保存鼠标在屏幕上的x,y坐标
        /// </summary>
        public POINT pt;
        /// <summary>
        /// 接收到鼠标消息的窗口的句柄 
        /// </summary>
        public int hwnd;
        /// <summary>
        /// hit-test值，详细描述参见WM_NCHITTEST消息
        /// </summary>
        public int wHitTestCode;
        /// <summary>
        /// 指定与本消息联系的额外消息 
        /// </summary>
        public int dwExtraInfo;
    }
    #endregion

    #region KeyBoardHook
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        /// <summary>
        /// // Specifies a virtual-key code. The code must be a value in the range 1 to 254.
        /// </summary>
        public int vkCode;
        /// <summary>
        /// // Specifies a hardware scan code for the key.
        /// </summary>
        public int scanCode;
        /// <summary>
        /// // Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
        /// </summary>
        public int flags;
        /// <summary>
        /// // Specifies the time stamp for this message.
        /// </summary>
        public int time;
        /// <summary>
        /// // Specifies extra information associated with the message.
        /// </summary>
        public int dwExtraInfo;
    }
    #endregion

    #region MouseLLHookStruct
    /// <summary>
    /// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MouseLLHookStruct
    {
        /// <summary>
        /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
        /// </summary>
        public POINT pt;
        /// <summary>
        /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. 
        /// The low-order word is reserved. A positive value indicates that the wheel was rotated forward, 
        /// away from the user; a negative value indicates that the wheel was rotated backward, toward the user. 
        /// One wheel click is defined as WHEEL_DELTA, which is 120. 
        ///If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
        /// or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
        /// and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is not used. 
        ///XBUTTON1
        ///The first X button was pressed or released.
        ///XBUTTON2
        ///The second X button was pressed or released.
        /// </summary>
        public int mouseData;
        /// <summary>
        /// Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value Purpose 
        ///LLMHF_INJECTED Test the event-injected flag.  
        ///0
        ///Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
        ///1-15
        ///Reserved.
        /// </summary>
        public int flags;
        /// <summary>
        /// Specifies the time stamp for this message.
        /// </summary>
        public int time;
        /// <summary>
        /// Specifies extra information associated with the message. 
        /// </summary>
        public int dwExtraInfo;
    }
    #endregion

    #endregion Win32 API 结构体声明
}
