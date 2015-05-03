using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurkenplayer
{
    public delegate void ConditionChangingEventHandler(object sender, ConditionChangedEventArgs e);

    /// <summary>
    /// Holds a boolean value for sharing/accessing one boolean in different classes, if both classes 
    /// exchange their MPSharedCondition object.
    /// </summary>
    public class MPSharedCondition
    {
        // Fields
        public event ConditionChangingEventHandler conditionChangingEvent;
        private bool condition;

        // Properties
        public bool Condition
        {
            get { return condition; }
            set 
            {
                OnConditionChanging(new ConditionChangedEventArgs(value));
                condition = value; 
            }
        }

        // Constructor
        public MPSharedCondition(bool condition)
        {
            this.condition = condition;
        }

        // Methods
        public virtual void OnConditionChanging(ConditionChangedEventArgs e)
        {
            if (conditionChangingEvent != null)
                conditionChangingEvent(this, e);
        }
    }

    /// <summary>
    /// Provides information about the new condition.
    /// </summary>
    public class ConditionChangedEventArgs : EventArgs
    {
        // Fields
        private bool newCondition;

        // Properties
        public bool NewCondition
        {
            get { return newCondition; }
        }

        // Constructor
        public ConditionChangedEventArgs(bool newCondition)
        {
            this.newCondition = newCondition;
        }
    }
}
