﻿
using System;

namespace Intelligence {

    ////only here for type queries and storing lists of actions.
    /// I would much prefer to use generics exclusivly but have
    /// trouble getting covariance to cooperate 
    public abstract class CharacterAction {

        protected Entity entity;

        public abstract void Setup(Context context);

        public virtual void OnStart() { }

        public virtual bool OnUpdate() {
            return true;
        }

        public virtual void OnInterrupt() {}

        public virtual void OnCancel() {}

        public virtual void OnComplete() {}

        public abstract Type ContextType { get; }

    }

    public class CharacterAction<T> : CharacterAction where T : Context {
		
		protected T context;

        public override void Setup(Context context) {
            this.context = context as T;
            entity = context.entity;
        }

        public override Type ContextType {
            get { return typeof(T); }
        }

    }
		
}