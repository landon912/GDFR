using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class FSM_Event{
	public delegate IEnumerator FSMEventFuntion(params object[] data);
	public FSMEventFuntion stateFunction = null;
	public string eventName;
	public delegate void EventTransitionFunction(FSM_Event _event);
	public EventTransitionFunction transitionFunction = null;
	public bool isGlobal = false;
	
	public FSM_Event(string _eventName,FSMEventFuntion _stateFunction,bool _isGlobal = false)
	{
		stateFunction = _stateFunction;
		eventName = _eventName;
		transitionFunction = defaultTransitionFunction;
		isGlobal = _isGlobal;
        
        // Before add, check if it already exists and then remove it.
        if (RxFx_FSM.GlobalEventList.ContainsKey(_eventName))
        {
            RxFx_FSM.GlobalEventList.Remove(_eventName);
        }

        RxFx_FSM.GlobalEventList.Add(_eventName, this);
	}

	~FSM_Event()
	{
        // This should only be used if user forgot to remove it manually
        /*
        if (RxFx_FSM.GlobalEventList.ContainsKey(eventName))
        {
            RxFx_FSM.GlobalEventList.Remove(eventName);
        }
        */
    }
	
	void defaultTransitionFunction(FSM_Event _event)
	{
		//This is called when the event is activated.  You can replace this with an external function by assigning the
		//public variable transitionFunction to a function that fits the delegate format.

		//Example use
		//Debug.Log("Transition: " + _event.eventName + " was activated");
	}	
}

public class RxFx_FSM : NetworkBehaviour
{
    public FSM_Event startEvent;
	public static Dictionary<string,FSM_Event> GlobalEventList = new Dictionary<string,FSM_Event>();

	public void callEvent(FSM_Event _event,params object[] data)
	{
		StopAllCoroutines();
		if(_event.transitionFunction!=null)
			_event.transitionFunction(_event);
		StartCoroutine(_event.stateFunction(data));
	}

	public void callEvent(string _eventName,params object[] data)
	{
		callEvent(GlobalEventList[_eventName],data);
	}
}