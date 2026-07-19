using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Reflection.Metadata;

/// <summary>

/// ''' The rich text box print control class was developed by Microsoft, information about this control can be found in your help files at:  

/// ''' ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.KB.v10.en/enu_kbvbnetkb/vbnetkb/811401.htm

/// ''' In general, their intent was to create a rich text box control with print capability embedded into the control.

/// ''' </summary>

/// ''' <remarks>This control class replaces the use of the regular RichTextBox control; 

/// ''' the purpose of this extension was specifically to facilitate printing the contents of a rich text box control.</remarks>

public class CustomRichTextBox : RichTextBox
{

    // Convert the unit that is used by the .NET framework (1/100 inch) 
    // and the unit that is used by Win32 API calls (twips 1/1440 inch)
    private const double AnInch = 14.4;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CHARRANGE
    {
        public int cpMin;          // First character of range (0 for start of doc)
        public int cpMax;          // Last character of range (-1 for end of doc)
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FORMATRANGE
    {
        public IntPtr hdc;             // Actual DC to draw on
        public IntPtr hdcTarget;       // Target DC for determining text formatting
        public RECT rc;                // Region of the DC to draw to (in twips)
        public RECT rcPage;            // Region of the whole DC (page size) (in twips)
        public CHARRANGE chrg;         // Range of text to draw (see above declaration)
    }

    private const int WM_USER = 0x400;
    private const int EM_FORMATRANGE = WM_USER + 57;

    public CustomRichTextBox()
    {
        base.AllowDrop = true;
    }

    [System.Runtime.InteropServices.DllImport("USER32")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    // Render the contents of the RichTextBox for printing
    // Return the last character printed + 1 (printing start from this point for next page)
    public int Print(int charFrom, int charTo, PrintPageEventArgs e)
    {

        // Mark starting and ending character 
        CHARRANGE cRange;
        cRange.cpMin = charFrom;
        cRange.cpMax = charTo;

        // Calculate the area to render and print
        RECT rectToPrint;
        rectToPrint.Top = e.MarginBounds.Top * AnInch;
        rectToPrint.Bottom = e.MarginBounds.Bottom * AnInch;
        rectToPrint.Left = e.MarginBounds.Left * AnInch;
        rectToPrint.Right = e.MarginBounds.Right * AnInch;

        // Calculate the size of the page
        RECT rectPage;
        rectPage.Top = e.PageBounds.Top * AnInch;
        rectPage.Bottom = e.PageBounds.Bottom * AnInch;
        rectPage.Left = e.PageBounds.Left * AnInch;
        rectPage.Right = e.PageBounds.Right * AnInch;

        IntPtr hdc = e.Graphics.GetHdc();

        FORMATRANGE fmtRange;
        fmtRange.chrg = cRange;                 // Indicate character from to character to 
        fmtRange.hdc = hdc;                     // Use the same DC for measuring and rendering
        fmtRange.hdcTarget = hdc;               // Point at printer hDC
        fmtRange.rc = rectToPrint;              // Indicate the area on page to print
        fmtRange.rcPage = rectPage;             // Indicate whole size of page

        IntPtr res = IntPtr.Zero;

        IntPtr wparam = IntPtr.Zero;
        wparam = new IntPtr(1);

        // Move the pointer to the FORMATRANGE structure in memory
        IntPtr lparam = IntPtr.Zero;
        lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
        Marshal.StructureToPtr(fmtRange, lparam, false);

        // Send the rendered data for printing 
        res = SendMessage(Handle, EM_FORMATRANGE, wparam, lparam);

        // Free the block of memory allocated
        Marshal.FreeCoTaskMem(lparam);

        // Release the device context handle obtained by a previous call
        e.Graphics.ReleaseHdc(hdc);

        // Return last + 1 character printer
        return res.ToInt32();
    }

    private void CustomRichTextBoxl_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
    {
        if ((e.Data.GetDataPresent(DataFormats.Text)))
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void CustomRichTextBox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
    {
        Int16 i;
        string s;

        // Get start position to drop the text.
        i = base.SelectionStart;
        s = base.Text.Substring(i);
        base.Text = base.Text.Substring(0, i);

        // Drop the text on to the RichTextBox.
        base.Text = base.Text + e.Data.GetData(DataFormats.Text).ToString();
        base.Text = base.Text + s;
    }
}
