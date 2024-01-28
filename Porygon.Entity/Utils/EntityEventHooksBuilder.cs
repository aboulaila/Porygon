namespace Porygon.Entity.Utils
{
    public class EntityEventHooksBuilder
    {
        private Func<object, Task>? preValidation;
        private Func<object, Task>? pre;
        private Func<object, Task>? post;

        public EntityEventHooksBuilder PreValidation(Func<object, Task> preValidation)
        {
            this.preValidation = preValidation;
            return this;
        }

        public EntityEventHooksBuilder Pre(Func<object, Task> pre)
        {
            this.pre = pre;
            return this;
        }

        public EntityEventHooksBuilder Post(Func<object, Task> post)
        {
            this.post = post;
            return this;
        }

        public EntityEventsHook Build()
        {
            return new EntityEventsHook(preValidation, pre, post);
        }
    }
}

