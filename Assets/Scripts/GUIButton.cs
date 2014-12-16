/*
 *
 * **** GUIButton CLASS ****
 *
 * this versions sends only events to the topmost button ...
 *
 *
 * Fixes the bugs from the original GUI.Button function
 * Based on the script from Joe Strout:
 * http://forum.unity3d.com/threads/96563-corrected-GUI.Button-code-%28works-properly-with-layered-controls%29?p=629284#post629284
 *
 *
 * The difference in this script is that it will only fire events (click and rollover!)
 * for the topmost button when using overlapping buttons inside the same GUI.depth!
 * Therefore the script finds the topmost button during the layout process, so it
 * can decide which button REALLY has been clicked.
 *
 * Benefits:
 * 1. The script will only hover the topmost button!
 *    (doesn't matter wheter the topmost button is defined via GUI.depth or via drawing order!)
 * 2. The script will only send events to the topmost button (as opposed to Joe's original script)
 * 3. The script works for overlapping buttons inside same GUI.depth levels,
 *    as well as for overlapping buttons using different GUI.depth values
 * 4. The script also works when overlapping buttons over buttons inside scrollviews, etc.
 *
 * Usage:  just like GUI.Button() ... for example:
 *
 *  if ( GUIButton.Button(new Rect(0,0,100,100), "button_action", GUI.skin.customStyles[0]) )
 *  {
 *      Debug.Log( "Button clicked ..." );
 *  }
 *
 *
 *
 * Original script (c) by Joe Strout!
 *
 * Code changes:
 * Copyright (c) 2012 by Frank Baumgartner, Baumgartner New Media GmbH, fb@b-nm.at
 *
 *
 * */


using UnityEngine;
using System.Collections;

public class GUIButton : MonoBehaviour 
{
	private static int highestDepthID = 0;
	private static Vector2 touchBeganPosition = Vector2.zero;
	private static EventType lastEventType = EventType.Layout;
	
	private static bool wasDragging = false;
	
	private static int frame = 0;
	private static int lastEventFrame = 0;
	
	
	public static bool Button(Rect bounds, string caption, GUIStyle btnStyle = null )
	{
		int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);
		bool isMouseOver = bounds.Contains(Event.current.mousePosition);
		int depth = (1000 - GUI.depth) * 1000 + controlID;
		if ( isMouseOver && depth > highestDepthID ) highestDepthID = depth;
		bool isTopmostMouseOver = (highestDepthID == depth);
		bool paintMouseOver = isTopmostMouseOver;
		
		if ( btnStyle == null )
		{
			btnStyle = GUI.skin.FindStyle("button");
		}
		
		if ( Event.current.type == EventType.Layout && lastEventType != EventType.Layout )
		{
			highestDepthID = 0;
			frame++;
		}
		lastEventType = Event.current.type;
		
		if ( Event.current.type == EventType.Repaint )
		{
			bool isDown = (GUIUtility.hotControl == controlID);
			btnStyle.Draw(bounds, new GUIContent(caption), paintMouseOver, isDown, false, false);          
			
		}
		
		// Workaround:
		// ignore duplicate mouseUp events. These can occur when running
		// unity editor with unity remote on iOS ... (anybody knows WHY?)
		if ( frame <= (1+lastEventFrame) ) return false;
		
		switch ( Event.current.GetTypeForControl(controlID) )
		{
		case EventType.mouseDown:
		{
			if ( isTopmostMouseOver && !wasDragging )
			{
				GUIUtility.hotControl = controlID;
			}
			break;
		}
			
		case EventType.mouseUp:
		{
			if ( isTopmostMouseOver && !wasDragging )
			{
				GUIUtility.hotControl = 0;
				lastEventFrame = frame;
				return true;
			}
			break;
		}
		}
		return false;
	}
}

