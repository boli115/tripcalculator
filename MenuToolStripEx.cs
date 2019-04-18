using System;
using System.Windows.Forms;

namespace TripCalculator
{
	namespace ToolStripExtensions
	{
		
		// The ToolStrip and MenuStrip classes are extended to allow customization of the user interface
		// The following new boolean properties are exposed in the designer:
		
		// ClickThrough - Allow the first click to activate the control, even when the containing form is not active
		// SupressHighlighting - Suppress the mouseover highlighting of the control when the containing form is not active
		
		// The ideas behind this were borrowed from two items found on the Internet:
		// Rick Brewster shows how to implement ClickThrough on his blog at:
		//   http://blogs.msdn.com/rickbrew/
		// JasonD suggests the method to suppress the highlighting on at forum at:
		//   http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=118385&SiteID=1
		
		public class WinConst
		{
			
			public const int WM_MOUSEMOVE = 0x200;
			public const int WM_MOUSEACTIVATE = 0x21;
			public const int MA_ACTIVATE = 1;
			public const int MA_ACTIVATEANDEAT = 2;
			public const int MA_NOACTIVATE = 3;
			public const int MA_NOACTIVATEANDEAT = 4;
		}
		
		
		// This class adds to the functionality provided in System.Windows.Forms.MenuStrip.
		
		// It allows you to "ClickThrough" to the MenuStrip so that you don't have to click once to
		// bring the form into focus and once more to take the desired action
		
		// It also implements a SuppressHighlighting property to turn off the highlighting
		// that occures on mouseover when the form is not active
		
		public class ToolStripEx : ToolStrip
		{
			private bool m_ClickThrough = true;
			//Private m_SuppressHighlighting As Boolean = True
			
			public bool ClickThrough
			{
				get
				{
					return m_ClickThrough;
				}
				set
				{
					m_ClickThrough = value;
				}
			}
			
			public ToolStripEx()
			{
			}
			
			public ToolStripEx(params System.Windows.Forms.ToolStripItem[] items) : base(items)
			{
			}
			
			// This method overrides the procedure that responds to Windows messages.
			
			// It intercepts the WM_MOUSEMOVE message
			// and ignores it if SuppressHighlighting is on and the TopLevelControl does not contain the focus.
			// Otherwise, it calls the base class procedure to handle the message.
			
			// It also intercepts the WM_MOUSEACTIVATE message and replaces an "Activate and Eat" result with
			// an "Activate" result if ClickThrough is enabled.
			
			protected override void WndProc(ref System.Windows.Forms.Message m)
			{
				//Try
				//If m.Msg = WinConst.WM_MOUSEMOVE And Not m_ClickThrough And Not Me.TopLevelControl.ContainsFocus Then
				//    Exit Sub
				//Else
				base.WndProc(ref m);
				//End If
				
				if (m.Msg == WinConst.WM_MOUSEACTIVATE && m_ClickThrough && m.Result == ((IntPtr) WinConst.MA_ACTIVATEANDEAT))
				{
					m.Result = (IntPtr) WinConst.MA_ACTIVATE;
				}
				//Catch ex As Exception
				
				//End Try
			}
			
		}
		
		
		public class MenuStripEx : MenuStrip
		{
			private bool m_ClickThrough = true;
			//Private m_SuppressHighlighting As Boolean = True
			
			public bool ClickThrough
			{
				get
				{
					return m_ClickThrough;
				}
				set
				{
					m_ClickThrough = value;
				}
			}
			
			protected override void WndProc(ref System.Windows.Forms.Message m)
			{
				
				//Try
				//If m.Msg = WinConst.WM_MOUSEMOVE And Not m_ClickThrough And Not Me.TopLevelControl.ContainsFocus Then
				//    Exit Sub
				//Else
				base.WndProc(ref m);
				//End If
				
				if (m.Msg == WinConst.WM_MOUSEACTIVATE && m_ClickThrough && m.Result == ((IntPtr) WinConst.MA_ACTIVATEANDEAT))
				{
					m.Result = (IntPtr) WinConst.MA_ACTIVATE;
				}
				//Catch ex As Exception
				
				//End Try
			}
		}
		
		
	}
	
}
