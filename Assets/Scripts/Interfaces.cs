using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Multiple inheritance is impossible
// However, a class can inherit from a superclass, plus extend many interfaces
namespace Interfaces{

	public interface ISubject
	{
	    // Attach an observer to the subject.
	    void Attach(IObserver observer);

	    // Detach an observer from the subject.
	    void Detach(IObserver observer);

	    // Notify all observers about an event.
	    void Notify();
	}

	public interface IObserver
	{
	    // Receive update from subject
	    void UpdateOnChange(ISubject subject);
	}

}
