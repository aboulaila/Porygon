using Porygon.Entity.Interfaces;

namespace Porygon.Entity.Utils
{
    public class EntityEventsHook : IEntityEventsHook
	{
        private readonly Func<object, Task>? preValidation;
        private readonly Func<object, Task>? pre;
        private readonly Func<object, Task>? post;

        public EntityEventsHook(Func<object, Task>? preValidation, Func<object, Task>? pre, Func<object, Task>? post)
		{
            this.preValidation = preValidation;
            this.pre = pre;
            this.post = post;
        }

        public async Task InvokePreValidation(object entity)
        {
            if (preValidation != null)
                await preValidation(entity);
        }

        public async Task InvokePre(object entity)
        {
            if (pre != null)
                await pre(entity);
        }

        public async Task InvokePost(object entity)
        {
            if (post != null)
                await post(entity);
        }
    }
}

